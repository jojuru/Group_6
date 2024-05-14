using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace Mokkivaraus;

public partial class MokkiPage : ContentPage
{
    public ObservableCollection<Mokki> ClickedMokkiList { get; }
    public ObservableCollection<Palvelu> PalveluCollection { get; set; }
    public ObservableCollection<ServiceOption> ServiceOptions { get; set; } = new ObservableCollection<ServiceOption>();
    static private String connstring = "server=localhost;uid=root;port=3306;pwd=root;database=vn";
    public double hinta = 0;
    public string SelectedMokkiAlueId { get; set; }

    public MokkiPage(Mokki clickedMokki)
    {
        InitializeComponent();

        PalveluCollection = new ObservableCollection<Palvelu>();
        ClickedMokkiList = new ObservableCollection<Mokki> { clickedMokki };
        BindingContext = this;
        Debug.WriteLine(ClickedMokkiList[0].varustelu);

        double.Parse(ClickedMokkiList[0].hinta);
        SelectedMokkiAlueId = ClickedMokkiList[0].alue_id;

        foreach (var property in typeof(Mokki).GetProperties())
        {
            var value = property.GetValue(clickedMokki);
            Debug.WriteLine($"{property.Name}: {value}");
        }

        SqlHaePalvelut();
    }
    private void OnDateChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "Date")
        {
            UpdateHintaLabel();
        }
    }
    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        UpdateHintaLabel();
    }
    private void UpdateHintaLabel()
    {
        DateTime alkupvm = saapuminenDate.Date;
        DateTime loppupvm = lahtoDate.Date;
        int daysDifference = (loppupvm - alkupvm).Days;

        double mokkihinta;
        if (double.TryParse(ClickedMokkiList[0].hinta, out double hintaDouble))
        {
            mokkihinta = hintaDouble * daysDifference;
            mokkihinta = Math.Round(mokkihinta, 2);
        }
        else
        {
            mokkihinta = 0;
        }

        double palveluHinta = 0;
        foreach (var service in ServiceOptions.Where(s => s.IsSelected))
        {
            palveluHinta += service.Price;
        }
        double kokonaishinta = mokkihinta + palveluHinta;

        hintaLabel.Text = $"Tämänhetkinen hinta: {kokonaishinta.ToString("C")}";
    }
    private void SqlHaePalvelut()
    {
        PalveluCollection.Clear();
        MySqlConnection con = new MySqlConnection();
        con.ConnectionString = connstring;
        con.Open();

        // SQL query to select services associated with the selected area
        string sql = "SELECT palvelu.*, alue.nimi AS alue_nimi FROM palvelu JOIN alue ON palvelu.alue_id = alue.alue_id WHERE palvelu.alue_id = @alue_id";
        MySqlCommand cmd = new MySqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@alue_id", SelectedMokkiAlueId);
        MySqlDataReader reader = cmd.ExecuteReader();

        HashSet<string> addedServices = new HashSet<string>();

        while (reader.Read())
        {
            Palvelu PALVELU = new Palvelu();
            string palveluNimi = reader["nimi"].ToString();
            PALVELU.palvelu_id = reader["palvelu_id"].ToString();
            PALVELU.alue_id = reader["alue_id"].ToString();
            PALVELU.nimi = reader["nimi"].ToString();
            PALVELU.kuvaus = reader["kuvaus"].ToString();
            PALVELU.hinta = reader["hinta"].ToString();
            double hinta = double.Parse(reader["hinta"].ToString());
            PALVELU.alv = reader["alv"].ToString();

            if (!addedServices.Contains(palveluNimi))
            {

                // Create a new ServiceOption and add it to the collection
                ServiceOptions.Add(new ServiceOption { Name = palveluNimi, IsSelected = false, Price = hinta });
                addedServices.Add(palveluNimi); // Keep track of added services
            }

            PalveluCollection.Add(PALVELU);
        }
        con.Close();
    }
    private async void VarausBtnClicked(object sender, EventArgs e)
    {
        //ALKU
        bool asiakasSuccess = false;
        bool varausSuccess = false;
        bool laskuSuccess = false;
        bool varPalvelutSuccess = false;

        try
        {

            // 1. Check if the phone number exists in the database
            string puhelinnro = puhelinnro_Ent.Text;
            int asiakasId = await CheckPhoneNumberExists(puhelinnro);

            if (asiakasId == 0)
            {
                // If the phone number does not exist, create a new asiakas
                // Get user input values from the Entry elements
                string etunimi = etunimi_Ent.Text;
                string sukunimi = sukunimi_Ent.Text;
                string postinro = postinro_Ent.Text;
                string lahiosoite = lahiosoite_Ent.Text;
                string email = email_Ent.Text;

                string asiakas_sql = "INSERT INTO asiakas (postinro, etunimi, sukunimi, lahiosoite, email, puhelinnro) " +
                              "VALUES (@postinro, @etunimi, @sukunimi, @lahiosoite, @email, @puhelinnro); SELECT LAST_INSERT_ID();";

                // Check if any of the input fields are empty
                if (string.IsNullOrEmpty(etunimi) || string.IsNullOrEmpty(sukunimi) || string.IsNullOrEmpty(postinro)
                    || string.IsNullOrEmpty(lahiosoite) || string.IsNullOrEmpty(email))
                {
                    await DisplayAlert("Virhe", "Tietoja puuttuu", "ok");
                    return;
                }

                using (MySqlConnection con = new MySqlConnection(connstring))
                {
                    con.Open();

                    using (MySqlCommand command = new MySqlCommand(asiakas_sql, con))
                    {
                        command.Parameters.AddWithValue("@postinro", postinro);
                        command.Parameters.AddWithValue("@etunimi", etunimi);
                        command.Parameters.AddWithValue("@sukunimi", sukunimi);
                        command.Parameters.AddWithValue("@lahiosoite", lahiosoite);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@puhelinnro", puhelinnro);

                        asiakasId = Convert.ToInt32(await command.ExecuteScalarAsync());
                    }
                }

                Debug.WriteLine($"Luotu uudella asiakas_id:llä: {asiakasId}");
            }
            else
            {
                Debug.WriteLine($"Luotu vanhalla asiakas_id:llä: {asiakasId}");
            }

            asiakasSuccess = true;

            // 2. Sitten luodaan uusi varaus tietokantaan
            DateTime alkupvm = saapuminenDate.Date;
            DateTime loppupvm = lahtoDate.Date;
            DateTime varattupvm = DateTime.Now.AddMinutes(-5);

            int varausId = 0;
            string varaus_sql = "INSERT INTO vn.varaus (asiakas_id, mokki_id, varattu_pvm, vahvistus_pvm, varattu_alkupvm, varattu_loppupvm) " +
                                 "VALUES (@asiakas_id, @mokki_id, @varattu_pvm, @vahvistus_pvm, @varattu_alkupvm, @varattu_loppupvm); SELECT LAST_INSERT_ID();";

            using (MySqlConnection connection = new MySqlConnection(connstring))
            {
                connection.Open();

                using (MySqlCommand cmd = new MySqlCommand(varaus_sql, connection))
                {
                    cmd.Parameters.AddWithValue("@asiakas_id", asiakasId);
                    cmd.Parameters.AddWithValue("@mokki_id", ClickedMokkiList[0].mokki_id);
                    cmd.Parameters.AddWithValue("@varattu_pvm", varattupvm);
                    cmd.Parameters.AddWithValue("@vahvistus_pvm", DateTime.Now);
                    cmd.Parameters.AddWithValue("@varattu_alkupvm", alkupvm);
                    cmd.Parameters.AddWithValue("@varattu_loppupvm", loppupvm);

                    varausId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }
            }
            Debug.WriteLine("Varaus onnuistui, id: " + varausId);
            varausSuccess = true;

            // 3. Luodaan lasku 
            // lasketaan hinta valitulle päiville
            int daysDifference = (loppupvm - alkupvm).Days;

            string hintaString = ClickedMokkiList[0].hinta;
            double hintaDouble = double.Parse(hintaString);
            double mokkihinta = hintaDouble * daysDifference;

            double alvhinta = mokkihinta * 0.1935;

            mokkihinta = Math.Round(mokkihinta, 2);
            alvhinta = Math.Round(alvhinta, 2);

            Debug.Write(hintaString + " " + mokkihinta + " " + alvhinta);
            // Check which radio button is selected
            int laskuntyyppi = 1; //default email
            if (paperiBox.IsChecked)
            {
                laskuntyyppi = 0;
            }
            else if (emailBox.IsChecked)
            {
                laskuntyyppi = 1;
            }

            string lasku_sql = "INSERT INTO vn.lasku (varaus_id, summa, alv, maksettu, laskun_tyyppi) " +
                                "VALUES (@varaus_id, @summa, @alv, @maksettu, @laskun_tyyppi)";


            using (MySqlConnection connection = new MySqlConnection(connstring))
            {
                connection.Open();

                using (MySqlCommand cmd = new MySqlCommand(lasku_sql, connection))
                {
                    cmd.Parameters.AddWithValue("@varaus_id", varausId);
                    cmd.Parameters.AddWithValue("@summa", mokkihinta);
                    cmd.Parameters.AddWithValue("@alv", alvhinta);
                    cmd.Parameters.AddWithValue("@maksettu", 0); 
                    cmd.Parameters.AddWithValue("@laskun_tyyppi", laskuntyyppi);

                    cmd.ExecuteNonQuery();
                }
            }
            Debug.WriteLine($"Lasku onnuistui");

            laskuSuccess = true;
            //4. Luodaan varauksen_palvelut tietokantaan KESKEN!!
            List<string> selectedPalveluIds = new List<string>();

            foreach (var service in ServiceOptions.Where(s => s.IsSelected))
            {
                // Find the corresponding Palvelu object in PalveluCollection
                var palvelu = PalveluCollection.FirstOrDefault(p => p.nimi == service.Name);

                if (palvelu != null)
                {
                    selectedPalveluIds.Add(palvelu.palvelu_id);
                    string varPalvelu_sql = "INSERT INTO vn.varauksen_palvelut (varaus_id, palvelu_id, lkm) " +
                        "VALUES (@varaus_id, @palvelu_id, @lkm)";

                    using (MySqlConnection con = new MySqlConnection(connstring))
                    {
                        con.Open();

                        using (MySqlCommand cmd = new MySqlCommand(varPalvelu_sql, con))
                        {
                            cmd.Parameters.AddWithValue("@varaus_id", varausId);
                            cmd.Parameters.AddWithValue("@palvelu_id", palvelu.palvelu_id);
                            cmd.Parameters.AddWithValue("@lkm", 1); // Assuming quantity is always 1

                            cmd.ExecuteNonQuery();
                        }

                        con.Close();
                    }
                }
            }
            foreach (var item in selectedPalveluIds)
            {
                Debug.WriteLine("palveluid: " + item);
            }



            varPalvelutSuccess = true;
        }
        catch (Exception ex)
        {
            // Handle exceptions for each operation separately
            Debug.WriteLine("Error: " + ex.Message);
            await DisplayAlert("SQL virhe", ex.Message, "OK");
        }

        // Display "Onnistui" only if all operations were successful
        if (asiakasSuccess && varausSuccess && laskuSuccess && varPalvelutSuccess)
        {
            await DisplayAlert("Onnistui", "Varaus tehty!", "OK");
            // Clear input fields 
            etunimi_Ent.Text = "";
            sukunimi_Ent.Text = "";
            postinro_Ent.Text = "";
            lahiosoite_Ent.Text = "";
            email_Ent.Text = "";
            puhelinnro_Ent.Text = "";
        }
    }
    private async Task<int> CheckPhoneNumberExists(string puhelinnro)
    {
        int asiakasId = 0;

        string sql = "SELECT asiakas_id FROM asiakas WHERE puhelinnro = @puhelinnro";

        using (MySqlConnection con = new MySqlConnection(connstring))
        {
            con.Open();

            using (MySqlCommand cmd = new MySqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@puhelinnro", puhelinnro);

                object result = await cmd.ExecuteScalarAsync();

                if (result != null)
                {
                    asiakasId = Convert.ToInt32(result);
                }
            }
            con.Close();
        }

        return asiakasId;
    }
}
