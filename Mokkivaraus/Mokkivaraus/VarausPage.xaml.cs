using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Mokkivaraus;

public partial class VarausPage : TabbedPage
{
    public ObservableCollection<Alue> AlueCollection { get; set; }
    public ObservableCollection<Mokki> MokkiCollection { get; set; }
    public ObservableCollection<Palvelu> PalveluCollection { get; set; }

    // grouppaa aktiviteetit alueen mukaan
    public Dictionary<string, List<string>> ServicesByArea = new Dictionary<string, List<string>>();

    static private String connstring = "server=localhost;uid=root;port=3306;pwd=root;database=vn";

    public VarausPage()
    {
        InitializeComponent();

        AlueCollection = new ObservableCollection<Alue>();
        MokkiCollection = new ObservableCollection<Mokki>();
        PalveluCollection = new ObservableCollection<Palvelu>();

        BindingContext = this;
        MokkiListaLv.BindingContext = MokkiCollection;

        SqlHaeAlueet();
        SqlHaePalvelut();
        SqlHaeMokit();
    }
    private void SqlHaeMokit()
    {
        MokkiCollection.Clear();

        MySqlConnection con = new MySqlConnection();
        con.ConnectionString = connstring;
        con.Open();

        string sql = "SELECT * FROM mokki";
        MySqlCommand cmd = new MySqlCommand(sql, con);
        MySqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Mokki MOKKI = new Mokki();
            MOKKI.mokki_id = reader["mokki_id"].ToString();
            MOKKI.alue_id = reader["alue_id"].ToString();
            MOKKI.postinro = reader["postinro"].ToString();
            MOKKI.mokkinimi = reader["mokkinimi"].ToString();
            MOKKI.katuosoite = reader["katuosoite"].ToString();
            MOKKI.hinta = reader["hinta"].ToString();
            MOKKI.kuva = reader["kuva"].ToString();
            MOKKI.kuvaus = reader["kuvaus"].ToString();
            MOKKI.henkilomaara = reader["henkilomaara"].ToString();
            // Split the varustelu string into individual items and join them with ", "
            MOKKI.varustelu = reader["varustelu"].ToString();
            string[] varusteluItems = MOKKI.varustelu.Split(',');
            for (int i = 0; i < varusteluItems.Length; i++)
            {
                varusteluItems[i] = char.ToUpper(varusteluItems[i][0]) + varusteluItems[i].Substring(1);
            }
            MOKKI.varustelu = string.Join(", ", varusteluItems);

            //etsii alue collectionista oikean alueen id perusteella
            var mok = AlueCollection.FirstOrDefault(m => m.alue_id == reader["alue_id"].ToString());
            MOKKI.alue = mok.nimi;

            // Fetch the list of services for this mokki from servicesByArea
            if (ServicesByArea.ContainsKey(MOKKI.alue))
            {
                string servicesString = string.Join(", ", ServicesByArea[MOKKI.alue]);
                MOKKI.palvelut = servicesString;
            }

            MokkiCollection.Add(MOKKI);
        }

        MokkiListaLv.ItemsSource = MokkiCollection;
    }
    private void SqlHaeAlueet()
    {
        AlueCollection.Clear();

        MySqlConnection con = new MySqlConnection();
        con.ConnectionString = connstring;
        con.Open();
        string sql = "SELECT * FROM alue";
        MySqlCommand cmd = new MySqlCommand(sql, con);
        MySqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Alue alue = new Alue
            {
                alue_id = reader["alue_id"].ToString(),
                nimi = reader["nimi"].ToString()
            };

            AlueCollection.Add(alue);
        }
        //pickeriin alueet
        AlueListaPicker.ItemsSource = AlueCollection;
    }
    private void SqlHaePalvelut()
    {
        PalveluCollection.Clear();
        MySqlConnection con = new MySqlConnection();
        con.ConnectionString = connstring;
        con.Open();

        // SQL query to select services associated with each area
        string sql = "SELECT palvelu.*, alue.nimi AS alue_nimi FROM palvelu JOIN alue ON palvelu.alue_id = alue.alue_id";
        MySqlCommand cmd = new MySqlCommand(sql, con);
        MySqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            // Retrieve the area name and service name
            string alueNimi = reader["alue_nimi"].ToString();
            string palveluNimi = reader["nimi"].ToString();

            // tehdään uusi lista alueelle jos sitä ei ole
            if (!ServicesByArea.ContainsKey(alueNimi))
            {
                ServicesByArea[alueNimi] = new List<string>();
            }
            ServicesByArea[alueNimi].Add(palveluNimi);
        }

    }

    private void valitseBtn_Clicked(object sender, EventArgs e)
    {

    }
    private void HaeButton_Clicked(object sender, EventArgs e)
    {
        // Tähän lisätään hakutoiminnallisuus: TÄMÄ LISÄTTY!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        MokkiCollection.Clear();

        using (MySqlConnection con = new MySqlConnection(connstring))
        {
            con.Open();

            string sql = "SELECT * FROM mokki WHERE ";

            List<string> conditions = new List<string>();

            // MÖKKINIMI
            if (!string.IsNullOrEmpty(MokinNimiEntry.Text))
            {
                conditions.Add("mokkinimi LIKE CONCAT('%', @mokkinimi, '%')");
            }

            // VARUSTELU
            List<string> selectedVarustelu = new List<string>();
            if (TVCheckBox.IsChecked)
                selectedVarustelu.Add("tv");
            if (AstianpesukoneCheckBox.IsChecked)
                selectedVarustelu.Add("astianpesukone");
            if (SaunaCheckBox.IsChecked)
                selectedVarustelu.Add("sauna");
            if (TakkaCheckBox.IsChecked)
                selectedVarustelu.Add("takka");
            if (PyykinpesukoneCheckBox.IsChecked)
                selectedVarustelu.Add("pyykinpesukone");

            if (selectedVarustelu.Count > 0)
            {
                List<string> varusteluConditions = new List<string>();
                for (int i = 0; i < selectedVarustelu.Count; i++)
                {
                    varusteluConditions.Add("varustelu LIKE CONCAT('%', @varustelu" + i + ", '%')");
                }
                conditions.Add("(" + string.Join(" AND ", varusteluConditions) + ")");
            }

            // ALUEET
            if (AlueListaPicker.SelectedItem != null)
            {
                var selectedAlue = AlueListaPicker.SelectedItem as Alue;
                if (selectedAlue != null)
                {
                    conditions.Add("alue_id = @alue_id");
                }
            }

            // HINTA
            double minHinta, maxHinta;
            if (double.TryParse(MinHintaEntry.Text, out minHinta))
            {
                conditions.Add("hinta >= @minHinta");
            }
            if (double.TryParse(MaxHintaEntry.Text, out maxHinta))
            {
                conditions.Add("hinta <= @maxHinta");
            }


            // Yhdistä kaikki ehdot AND-operaattorilla
            if (conditions.Count > 0)
            {
                sql += string.Join(" AND ", conditions);
            }
            else
            {
                sql += "1=1"; // Jos mitään ehtoja ei ole, palauta kaikki tulokset
            }

            MySqlCommand cmd = new MySqlCommand(sql, con);

            // Lisätään parametri, jos hakuteksti on annettu
            if (!string.IsNullOrEmpty(MokinNimiEntry.Text))
            {
                cmd.Parameters.AddWithValue("@mokkinimi", MokinNimiEntry.Text);
            }
            // Lisätään parametrit valituille varusteluille
            for (int i = 0; i < selectedVarustelu.Count; i++)
            {
                cmd.Parameters.AddWithValue("@varustelu" + i, selectedVarustelu[i]);
            }
            // Lisää parametri valitulle alueelle
            if (AlueListaPicker.SelectedItem != null)
            {
                var selectedAlue = AlueListaPicker.SelectedItem as Alue;
                if (selectedAlue != null)
                {
                    cmd.Parameters.AddWithValue("@alue_id", selectedAlue.alue_id);
                }
            }
            // Lisää parametrit minimi- ja maksimihinnalle
            cmd.Parameters.AddWithValue("@minHinta", minHinta);
            cmd.Parameters.AddWithValue("@maxHinta", maxHinta);

            // PALVELUT


            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Luetaan tietokannasta ja lisätään tulokset MokkiCollectioniin
                    Mokki MOKKI = new Mokki();
                    MOKKI.mokki_id = reader["mokki_id"].ToString();
                    MOKKI.alue_id = reader["alue_id"].ToString();
                    MOKKI.postinro = reader["postinro"].ToString();
                    MOKKI.mokkinimi = reader["mokkinimi"].ToString();
                    MOKKI.katuosoite = reader["katuosoite"].ToString();
                    MOKKI.hinta = reader["hinta"].ToString();
                    MOKKI.kuvaus = reader["kuvaus"].ToString();
                    MOKKI.henkilomaara = reader["henkilomaara"].ToString();
                    MOKKI.varustelu = reader["varustelu"].ToString();
                    string[] varusteluItems = MOKKI.varustelu.Split(',');
                    for (int i = 0; i < varusteluItems.Length; i++)
                    {
                        varusteluItems[i] = char.ToUpper(varusteluItems[i][0]) + varusteluItems[i].Substring(1);
                    }
                    MOKKI.varustelu = string.Join(", ", varusteluItems);

                    //etsii alue collectionista oikean alueen id perusteella
                    var mok = AlueCollection.FirstOrDefault(m => m.alue_id == reader["alue_id"].ToString());
                    if (mok != null)
                    {
                        MOKKI.alue = mok.nimi;
                    }

                    // Fetch the list of services for this mokki from servicesByArea
                    if (ServicesByArea.ContainsKey(MOKKI.alue))
                    {
                        string servicesString = string.Join(", ", ServicesByArea[MOKKI.alue]);
                        MOKKI.palvelut = servicesString;
                    }

                    MokkiCollection.Add(MOKKI);
                }
            }
        }

        MokkiListaLv.ItemsSource = MokkiCollection;
    }
    private async void TutustuButton_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var clickedMokki = button?.CommandParameter as Mokki; // Assuming Mokki is the data type of your item

        if (clickedMokki != null)
        {
            // Pass the clicked Mokki object as a parameter to the MokkiPage
            await Navigation.PushAsync(new MokkiPage(clickedMokki));
        }
    }
}
