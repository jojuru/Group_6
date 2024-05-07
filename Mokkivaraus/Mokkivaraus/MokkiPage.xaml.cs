using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Mokkivaraus;

public partial class MokkiPage : ContentPage
{
    public ObservableCollection<Mokki> ClickedMokkiList { get; }
    static private String connstring = "server=localhost;uid=root;port=3306;pwd=root;database=vn";

    public MokkiPage(Mokki clickedMokki)
    {
        InitializeComponent();
        ClickedMokkiList = new ObservableCollection<Mokki> { clickedMokki };
        BindingContext = this;
        // Print all properties of the clickedMokki object
        foreach (var property in typeof(Mokki).GetProperties())
        {
            var value = property.GetValue(clickedMokki);
            Debug.WriteLine($"{property.Name}: {value}");
        }

    }
    private async void VarausBtnClicked(object sender, EventArgs e)
    {
        //ALKU
        bool asiakasSuccess = false;
        bool varausSuccess = false;
        bool laskuSuccess = false;

        try
        {
            // 1.Ensin luodaan uusi asiakas tietokantaan
            // Get user input values from the Entry elements
            string etunimi = etunimi_Ent.Text;
            string sukunimi = sukunimi_Ent.Text;
            string postinro = postinro_Ent.Text;
            string lahiosoite = lahiosoite_Ent.Text;
            string email = email_Ent.Text;
            string puhelinnro = puhelinnro_Ent.Text;

            // Check if any of the input fields are empty
            if (string.IsNullOrEmpty(etunimi) || string.IsNullOrEmpty(sukunimi) || string.IsNullOrEmpty(postinro) || string.IsNullOrEmpty(lahiosoite) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(puhelinnro))
            {
                DisplayAlert("Virhe", "Tietoja puuttuu", "ok");
                return;
            }

            int asiakasId = 0;

            string asiakas_sql = "INSERT INTO asiakas (postinro, etunimi, sukunimi, lahiosoite, email, puhelinnro) " +
                                "VALUES (@postinro, @etunimi, @sukunimi, @lahiosoite, @email, @puhelinnro); SELECT LAST_INSERT_ID();";

            // Sy�tet��n tiedot sql
            using (MySqlConnection connection = new MySqlConnection(connstring))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(asiakas_sql, connection))
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

            Debug.WriteLine($"luotu asiakas_id: {asiakasId}");
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

            // 3. Luodaan lasku tietokantaan
            string lasku_sql = "INSERT INTO vn.lasku (varaus_id, summa, alv, maksettu) " +
                                "VALUES (@varaus_id, @summa, @alv, @maksettu)";


            using (MySqlConnection connection = new MySqlConnection(connstring))
            {
                connection.Open();

                using (MySqlCommand cmd = new MySqlCommand(lasku_sql, connection))
                {
                    cmd.Parameters.AddWithValue("@varaus_id", varausId);
                    cmd.Parameters.AddWithValue("@summa", 804.00);
                    cmd.Parameters.AddWithValue("@alv", 155.61);
                    cmd.Parameters.AddWithValue("@maksettu", 1);

                    cmd.ExecuteNonQuery();
                }
            }
            Debug.WriteLine($"Lasku onnuistui");

            laskuSuccess = true;
        }
        catch (Exception ex)
        {
            // Handle exceptions for each operation separately
            Debug.WriteLine("Error: " + ex.Message);
            DisplayAlert("SQL virhe", ex.Message, "OK");
        }

        // Display "Onnistui" only if all operations were successful
        if (asiakasSuccess && varausSuccess && laskuSuccess)
        {
            DisplayAlert("Onnistui", "Varaus tehty!", "OK");
            // Clear input fields 
            etunimi_Ent.Text = "";
            sukunimi_Ent.Text = "";
            postinro_Ent.Text = "";
            lahiosoite_Ent.Text = "";
            email_Ent.Text = "";
            puhelinnro_Ent.Text = "";
        }
    }

}