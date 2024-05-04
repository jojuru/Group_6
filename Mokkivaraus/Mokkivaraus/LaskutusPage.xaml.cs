using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Text;
namespace Mokkivaraus;

public partial class LaskutusPage : TabbedPage
{	
	public ObservableCollection<Laskutus> LaskutusCollection { get; set; }
    public ObservableCollection<Alue> Alueet { get; set; }
    public ObservableCollection<Mokki> Mokit { get; set; }
    public ObservableCollection<PalveluRaportti> Palvelut { get; set; }


    static private String connstring = "server=localhost;uid=root;port=3306;pwd=root;database=vn";
    public LaskutusPage()
	{
		InitializeComponent();

        LaskutusCollection = new ObservableCollection<Laskutus>();
        LaskuListaLv.BindingContext = LaskutusCollection;
        Mokit = new ObservableCollection<Mokki>();
        AlueListaLv.ItemsSource = Mokit;

        Palvelut = new ObservableCollection<PalveluRaportti>();

        HaeLaskutAsiakkaalle();
        LataaAlueet();
	}

	private void SqlHaeLaskut()
	{
        MySqlConnection con = new MySqlConnection();
        con.ConnectionString = connstring;
        con.Open();
        string sql = "SELECT * FROM lasku";
        MySqlCommand cmd = new MySqlCommand(sql, con);
        MySqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Laskutus LASKUTUS = new Laskutus();
            LASKUTUS.lasku_id = reader["lasku_id"].ToString();
            LASKUTUS.varaus_id = reader["varaus_id"].ToString();
            LASKUTUS.summa = reader["summa"].ToString();
            LASKUTUS.alv = reader["alv"].ToString();
            LASKUTUS.maksettu = reader["maksettu"].ToString();
            LaskutusCollection.Add(LASKUTUS);
        }
        LaskuListaLv.ItemsSource = LaskutusCollection;
    }
    // Haetaan kaikki laskut kun sivu avataan
    private void HaeLaskutAsiakkaalle()
    {
        var etunimi = etunimiEntry.Text;
        var sukunimi = sukunimiEntry.Text;
        var varausNumero = varausNumeroEntry.Text;
        var sahkoposti = sahkopostiEntry.Text;
        var puhelinnro = puhelinNumeroEntry.Text;

        var query = new StringBuilder("SELECT l.lasku_id, l.varaus_id, a.etunimi, a.sukunimi, a.puhelinnro, l.summa, l.alv, l.maksettu FROM lasku l JOIN varaus v ON l.varaus_id = v.varaus_id JOIN asiakas a ON v.asiakas_id = a.asiakas_id");
        var conditions = new List<string>();

        if (!string.IsNullOrEmpty(etunimi))
            conditions.Add("a.etunimi LIKE @etunimi");
        if (!string.IsNullOrEmpty(sukunimi))
            conditions.Add("a.sukunimi LIKE @sukunimi");
        if (!string.IsNullOrEmpty(varausNumero))
            conditions.Add("v.varaus_id LIKE @varausId");
        if (!string.IsNullOrEmpty(sahkoposti))
            conditions.Add("a.email LIKE @email");
        if (!string.IsNullOrEmpty(puhelinnro))
            conditions.Add("a.puhelinnro LIKE @puhelinnro");


        if (conditions.Count > 0)
        {
            query.Append(" WHERE " + string.Join(" AND ", conditions));
        }

        MySqlConnection con = new MySqlConnection(connstring);
        con.Open();
        MySqlCommand cmd = new MySqlCommand(query.ToString(), con);

        if (!string.IsNullOrEmpty(etunimi))
            cmd.Parameters.AddWithValue("@etunimi", $"%{etunimi}%");
        if (!string.IsNullOrEmpty(sukunimi))
            cmd.Parameters.AddWithValue("@sukunimi", $"%{sukunimi}%");
        if (!string.IsNullOrEmpty(varausNumero))
            cmd.Parameters.AddWithValue("@varausId", $"%{varausNumero}%");
        if (!string.IsNullOrEmpty(sahkoposti))
            cmd.Parameters.AddWithValue("@email", $"%{sahkoposti}%");
        if (!string.IsNullOrEmpty(puhelinnro))
            cmd.Parameters.AddWithValue("@puhelinnro", $"%{puhelinnro}%");


        var reader = cmd.ExecuteReader();
        LaskutusCollection.Clear();

        while (reader.Read())
        {
            var laskutus = new Laskutus
            {
                lasku_id = reader["lasku_id"].ToString(),
                etunimi = reader["etunimi"].ToString(),
                sukunimi = reader["sukunimi"].ToString(),
                varaus_id = reader["varaus_id"].ToString(),
                summa = reader["summa"].ToString(),
                alv = reader["alv"].ToString(),
                maksettu = reader["maksettu"].ToString(),
                puhelinnro = reader["puhelinnro"].ToString()
            };
            LaskutusCollection.Add(laskutus);
        }
        LaskuListaLv.ItemsSource = LaskutusCollection;
        con.Close();
    }

    // Haetaan laskut, jotka t‰ytt‰‰ hakuehdot, tyhj‰t kent‰t hakee kaikki laskut
    private void LaskuHaeBtn_Clicked(object sender, EventArgs e)
    {
        var etunimi = etunimiEntry.Text;
        var sukunimi = sukunimiEntry.Text;
        var varausNumero = varausNumeroEntry.Text;
        var sahkoposti = sahkopostiEntry.Text;
        var puhelinnro = puhelinNumeroEntry.Text;

        var query = new StringBuilder("SELECT l.lasku_id, l.varaus_id, a.etunimi, a.sukunimi, a.puhelinnro, l.summa, l.alv, l.maksettu FROM lasku l JOIN varaus v ON l.varaus_id = v.varaus_id JOIN asiakas a ON v.asiakas_id = a.asiakas_id ");
        var conditions = new List<string>();

        if (!string.IsNullOrEmpty(etunimi))
            conditions.Add("a.etunimi LIKE @etunimi");
        if (!string.IsNullOrEmpty(sukunimi))
            conditions.Add("a.sukunimi LIKE @sukunimi");
        if (!string.IsNullOrEmpty(varausNumero))
            conditions.Add("v.varaus_id LIKE @varausId");
        if (!string.IsNullOrEmpty(sahkoposti))
            conditions.Add("a.email LIKE @email");
        if (!string.IsNullOrEmpty(puhelinnro))
            conditions.Add("a.puhelinnro LIKE @puhelinnro");

        if (conditions.Count > 0)
        {
            query.Append(" WHERE " + string.Join(" AND ", conditions));
        }

        MySqlConnection con = new MySqlConnection(connstring);
        con.Open();
        MySqlCommand cmd = new MySqlCommand(query.ToString(), con);

        if (!string.IsNullOrEmpty(etunimi))
            cmd.Parameters.AddWithValue("@etunimi", $"%{etunimi}%");
        if (!string.IsNullOrEmpty(sukunimi))
            cmd.Parameters.AddWithValue("@sukunimi", $"%{sukunimi}%");
        if (!string.IsNullOrEmpty(varausNumero))
            cmd.Parameters.AddWithValue("@varausId", $"%{varausNumero}%");
        if (!string.IsNullOrEmpty(sahkoposti))
            cmd.Parameters.AddWithValue("@email", $"%{sahkoposti}%");
        if (!string.IsNullOrEmpty(puhelinnro))
            cmd.Parameters.AddWithValue("@puhelinnro", $"%{puhelinnro}%");

        var reader = cmd.ExecuteReader();
        LaskutusCollection.Clear();

        while (reader.Read())
        {
            var laskutus = new Laskutus
            {
                lasku_id = reader["lasku_id"].ToString(),
                etunimi = reader["etunimi"].ToString(),
                sukunimi = reader["sukunimi"].ToString(),
                varaus_id = reader["varaus_id"].ToString(),
                summa = reader["summa"].ToString(),
                alv = reader["alv"].ToString(),
                maksettu = reader["maksettu"].ToString(),
                puhelinnro = reader["puhelinnro"].ToString()
            };
            LaskutusCollection.Add(laskutus);
        }
        LaskuListaLv.ItemsSource = LaskutusCollection;
        con.Close();
    }

    // T‰st‰ alkaa raportointipuolen toiminnat

    // ladataan alueiden nimet raportoinnin aluevalintalistaan
    private void LataaAlueet()
    {

        using (MySqlConnection con = new MySqlConnection(connstring))
        {
            con.Open();
            string query = "SELECT alue_id, nimi FROM alue";
            MySqlCommand cmd = new MySqlCommand(query, con);
            var reader = cmd.ExecuteReader();
            Alueet = new ObservableCollection<Alue>();
            while (reader.Read())
            {
                Alueet.Add(new Alue
                {
                    alue_id = reader["alue_id"].ToString(),
                    nimi = reader["nimi"].ToString()
                });
            }
            con.Close();
        }

        // Aseta Alueet AluePickerin ItemsSourceksi
        this.Dispatcher.Dispatch(() =>
        {
            AluePicker.ItemsSource = Alueet;
        });

    }

    // Luo raportti napin painallus
    private void LuoRaportti_Clicked(object sender, EventArgs e)
    {
        DateTime startDate = startDatePicker.Date;
        DateTime endDate = endDatePicker.Date;
        if (startDate <= endDate)
        {
            varatutMokitLabel.Text = $"Varatut mˆkit aikav‰lill‰ {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}";
            varatutPalvelutLabel.Text = $"Varatut palvelut aikav‰lill‰ {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}";
            HaeVaratutMokit(startDate, endDate);
            HaeVaratutPalvelut(startDate, endDate);
        }
        else
        {
            DisplayAlert("Virhe", "Alkup‰iv‰m‰‰r‰ ei saa olla myˆhemmin kuin loppup‰iv‰m‰‰r‰.", "OK");
        }
    }

    // Raportille tietojen haku
    private void HaeVaratutMokit(DateTime alkuPvm, DateTime loppuPvm)
    {
        if (AluePicker.SelectedItem == null)
        {
            DisplayAlert("Valinta puuttuu", "Valitse alue ennen hakua.", "OK");
            return;
        }

        Alue valittuAlue = AluePicker.SelectedItem as Alue;
        if (valittuAlue == null)
        {
            DisplayAlert("Virhe", "Valittu alue on virheellinen.", "OK");
            return;
        }

        using (MySqlConnection con = new MySqlConnection(connstring))
        {
            try
            {
                con.Open();
                string query = @"
                SELECT m.mokkinimi, m.katuosoite, m.postinro, a.nimi AS alue_nimi
                FROM mokki m
                JOIN varaus v ON m.mokki_id = v.mokki_id
                JOIN alue a ON m.alue_id = a.alue_id
                WHERE a.nimi = @ValittuAlue AND 
                      v.varattu_alkupvm <= @LoppuPvm AND 
                      v.varattu_loppupvm >= @AlkuPvm";
                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ValittuAlue", valittuAlue.nimi);
                cmd.Parameters.AddWithValue("@AlkuPvm", alkuPvm.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@LoppuPvm", loppuPvm.ToString("yyyy-MM-dd"));

                var reader = cmd.ExecuteReader();
                Mokit.Clear();
                while (reader.Read())
                {
                    Mokit.Add(new Mokki
                    {
                        mokkinimi = reader["mokkinimi"].ToString(),
                        katuosoite = reader["katuosoite"].ToString(),
                        postinro = reader["postinro"].ToString(),
                        alue = reader["alue_nimi"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Virhe", "Tietokantavirhe: " + ex.Message, "OK");
            }
            finally
            {
                con.Close();
            }
        }
    }
    // Haetaan varatut palvelut
    private void HaeVaratutPalvelut(DateTime alkuPvm, DateTime loppuPvm)
    {
        if (AluePicker.SelectedItem == null)
        {
            DisplayAlert("Valinta puuttuu", "Valitse alue ennen hakua.", "OK");
            return;
        }

        Alue valittuAlue = AluePicker.SelectedItem as Alue;
        if (valittuAlue == null)
        {
            DisplayAlert("Virhe", "Valittu alue on virheellinen.", "OK");
            return;
        }

        using (MySqlConnection con = new MySqlConnection(connstring))
        {
            try
            {
                con.Open();
                string query = @"
                    SELECT p.nimi AS palvelun_nimi, COUNT(vp.varaus_id) AS varaus_maara
                    FROM varauksen_palvelut vp
                    JOIN varaus v ON vp.varaus_id = v.varaus_id
                    JOIN palvelu p ON vp.palvelu_id = p.palvelu_id
                    JOIN alue a ON p.alue_id = a.alue_id
                    WHERE a.nimi = @ValittuAlue AND 
                          v.varattu_alkupvm <= @LoppuPvm AND 
                          v.varattu_loppupvm >= @AlkuPvm
                    GROUP BY p.nimi";
                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ValittuAlue", valittuAlue.nimi);
                cmd.Parameters.AddWithValue("@AlkuPvm", alkuPvm.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@LoppuPvm", loppuPvm.ToString("yyyy-MM-dd"));


                var reader = cmd.ExecuteReader();
                Palvelut.Clear();
                while (reader.Read())
                {
                    Palvelut.Add(new PalveluRaportti
                    {
                        palvelun_nimi = reader["palvelun_nimi"].ToString(),
                        varaus_maara = Convert.ToInt32(reader["varaus_maara"])
                    });
                }
                PalvelutListaLv.ItemsSource = Palvelut;
            }
            catch (Exception ex)
            {
                DisplayAlert("Virhe", "Tietokantavirhe: " + ex.Message, "OK");
            }
            finally
            {
                con.Close();
            }
        }
    }
}