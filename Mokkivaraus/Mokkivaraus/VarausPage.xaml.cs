using Google.Protobuf.Reflection;
using Microsoft.Maui.Controls;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Mokkivaraus;

public partial class VarausPage : ContentPage
{
    public ObservableCollection<Alue> AlueCollection { get; set; }
    public ObservableCollection<Mokki> MokkiCollection { get; set; }
    public ObservableCollection<Palvelu> PalveluCollection { get; set; }
    public ObservableCollection<ServiceOption> ServiceOptions { get; set; } = new ObservableCollection<ServiceOption>();
    public ObservableCollection<ServiceOption> VarusteluOptions { get; set; } = new ObservableCollection<ServiceOption>();

    public HashSet<string> addedServices = new HashSet<string>();
    public HashSet<string> addedVarustelu = new HashSet<string>();
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

        SqlHaeAlueet();
        SqlHaePalvelut();
        SqlHaeMokit();
    }
    private void SqlHaeMokit()
    {
        MokkiCollection.Clear();
        addedVarustelu.Clear();

        MySqlConnection con = new MySqlConnection();
        con.ConnectionString = connstring;
        con.Open();

        string sql = "SELECT m.*, a.nimi AS alue, p.nimi AS palvelut " +
                    "FROM mokki m " +
                    "JOIN alue a ON m.alue_id = a.alue_id " +
                    "LEFT JOIN varauksen_palvelut vp ON m.mokki_id = vp.varaus_id " +
                    "LEFT JOIN palvelu p ON vp.palvelu_id = p.palvelu_id " +
                    "GROUP BY m.mokki_id";

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
            MOKKI.varustelu = reader["varustelu"].ToString();

            // Split the varustelu string and populate VarusteluOptions
            string varusteluString = reader["varustelu"].ToString();
            string varusteluString2 = varusteluString.Replace(" ", "");
            string[] varusteluItems = varusteluString2.Split(',');
            foreach (var item in varusteluItems)
            {
                if (!addedVarustelu.Contains(item))
                {
                    // Create a new ServiceOption and add it to the collection
                    VarusteluOptions.Add(new ServiceOption { Name = item, IsSelected = false });
                    addedVarustelu.Add(item); // Keep track of added services
                }
            }

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
        con.Close();
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
        con.Close();

    }
    private void SqlHaePalvelut()
    {
        PalveluCollection.Clear();
        addedServices.Clear();

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

            // tehd‰‰n uusi lista alueelle jos sit‰ ei ole
            if (!ServicesByArea.ContainsKey(alueNimi))
            {
                ServicesByArea[alueNimi] = new List<string>();
            }
            ServicesByArea[alueNimi].Add(palveluNimi);
            // Check if the service name has already been added to avoid duplicates

            if (!addedServices.Contains(palveluNimi))
            {
                // Create a new ServiceOption and add it to the collection
                ServiceOptions.Add(new ServiceOption { Name = palveluNimi, IsSelected = false });
                addedServices.Add(palveluNimi); // Keep track of added services
            }
        }

        con.Close();

    }

    private void ResetButton_Clicked(object sender, EventArgs e)
    {
        //clear all lists
        MokkiCollection.Clear();
        AlueCollection.Clear();
        PalveluCollection.Clear();
        ServiceOptions.Clear();
        VarusteluOptions.Clear();
        ServicesByArea.Clear();
        addedServices.Clear();
        addedVarustelu.Clear();

        // Reset the UI elements to their default state
        MokinNimiEntry.Text = "";
        MinHintaEntry.Text = "";
        MaxHintaEntry.Text = "";
        AlueListaPicker.SelectedIndex = -1;

        //reset checkboxes
        VarusteluOptions.Clear();
        ServiceOptions.Clear();

        //alkuper n‰kym‰
        SqlHaeAlueet();
        SqlHaePalvelut();
        SqlHaeMokit();
    }
    private void HaeButton_Clicked(object sender, EventArgs e)
    {
        // HERE WE ADD FILTERS FOR SEARCH
        MokkiCollection.Clear();

        using (MySqlConnection con = new MySqlConnection(connstring))
        {
            con.Open();

            string sql = "SELECT * FROM mokki WHERE ";


            List<string> conditions = new List<string>();

            // M÷KKINIMI
            if (!string.IsNullOrEmpty(MokinNimiEntry.Text))
            {
                conditions.Add("mokkinimi LIKE CONCAT('%', @mokkinimi, '%')");
            }

            // VARUSTELU
            List<string> selectedVarustelu = new List<string>();
            foreach (ServiceOption option in VarusteluOptions)
            {
                if (option.IsSelected)
                {
                    selectedVarustelu.Add(option.Name);
                }
            }
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


            // Yhdist‰ kaikki ehdot AND-operaattorilla
            if (conditions.Count > 0)
            {
                sql += string.Join(" AND ", conditions);
            }
            else
            {
                sql += "1=1"; // Jos mit‰‰n ehtoja ei ole, palauta kaikki tulokset
            }
            MySqlCommand cmd = new MySqlCommand(sql, con);

            // Lis‰t‰‰n parametri, jos hakuteksti on annettu
            if (!string.IsNullOrEmpty(MokinNimiEntry.Text))
            {
                cmd.Parameters.AddWithValue("@mokkinimi", MokinNimiEntry.Text);
            }
            // Lis‰t‰‰n parametrit valituille varusteluille
            for (int i = 0; i < selectedVarustelu.Count; i++)
            {
                cmd.Parameters.AddWithValue("@varustelu" + i, selectedVarustelu[i]);
            }
            // Lis‰‰ parametri valitulle alueelle
            if (AlueListaPicker.SelectedItem != null)
            {
                var selectedAlue = AlueListaPicker.SelectedItem as Alue;
                if (selectedAlue != null)
                {
                    cmd.Parameters.AddWithValue("@alue_id", selectedAlue.alue_id);
                }
            }
            // Lis‰‰ parametrit minimi- ja maksimihinnalle
            cmd.Parameters.AddWithValue("@minHinta", minHinta);
            cmd.Parameters.AddWithValue("@maxHinta", maxHinta);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Luetaan tietokannasta ja lis‰t‰‰n tulokset MokkiCollectioniin
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
            con.Close();
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