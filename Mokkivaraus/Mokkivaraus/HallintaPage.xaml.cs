using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;

namespace Mokkivaraus;

public partial class HallintaPage : TabbedPage
{
    public ObservableCollection<Alue> AlueCollection { get; set; }
    public ObservableCollection<Asiakas> AsiakasCollection { get; set; }
    public ObservableCollection<Mokki> MokkiCollection { get; set; }
    public ObservableCollection<Palvelu> PalveluCollection { get; set; }

    static private String connstring = "server=localhost;uid=root;port=3306;pwd=root;database=vn";
    public HallintaPage()
	{
		InitializeComponent();

        AlueCollection = new ObservableCollection<Alue>();
        AsiakasCollection = new ObservableCollection<Asiakas>();
        MokkiCollection = new ObservableCollection<Mokki>();
        PalveluCollection = new ObservableCollection<Palvelu>();
        BindingContext = this;
		AlueListaLv.BindingContext = AlueCollection;
        AsiakasListaLv.BindingContext = AsiakasCollection;
        MokkiListaLv.BindingContext = MokkiCollection;
        PalveluListaLv.BindingContext = PalveluCollection;



        SqlHaeKaikki();
    }

    //Tietokanta kaikkien haut---------------------------------------------------------------------------------------------------------------------

    //hakee tietokannasta kaikki
    private void SqlHaeKaikki()
	{
        SqlHaeAsiakkaat();
        SqlHaeAlueet(); //Mokkit ja Palvelut käyttää alueita.
        SqlHaeMokit();
        SqlHaePalvelut();
    }
	
	//hakee tietokannastta Alueet
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
            Alue ALUE = new Alue();
			ALUE.alue_id = reader["alue_id"].ToString();
            ALUE.nimi = reader["nimi"].ToString();
			AlueCollection.Add(ALUE);
        }
		AlueListaLv.ItemsSource = AlueCollection;
	}

    //hakee tietokannastta Asiakkaat
    private void SqlHaeAsiakkaat()
	{
        AsiakasCollection.Clear();
        MySqlConnection con = new MySqlConnection();
        con.ConnectionString = connstring;
        con.Open();
        string sql = "SELECT * FROM asiakas";
        MySqlCommand cmd = new MySqlCommand(sql, con);
        MySqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Asiakas ASIAKAS  = new Asiakas();
            ASIAKAS.asiakas_id = reader["asiakas_id"].ToString();
            ASIAKAS.etunimi = reader["etunimi"].ToString();
            ASIAKAS.sukunimi = reader["sukunimi"].ToString();
            ASIAKAS.postinro = reader["postinro"].ToString();
            ASIAKAS.lahiosoite = reader["lahiosoite"].ToString();
            ASIAKAS.email = reader["email"].ToString();
            ASIAKAS.puhelinnro = reader["puhelinnro"].ToString();
            AsiakasCollection.Add(ASIAKAS);
        }
        AsiakasListaLv.ItemsSource = AsiakasCollection;
    }

    //hakee tietokannastta Mokit
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
            MOKKI.hinta = reader["hinta"].ToString() + "€";
            MOKKI.kuvaus = reader["kuvaus"].ToString();
            MOKKI.hinta = reader["henkilomaara"].ToString();
            MOKKI.kuvaus = reader["varustelu"].ToString();
            //etsii alue collectionista oikean alueen id perusteella
            var mok = AlueCollection.FirstOrDefault(m => m.alue_id == reader["alue_id"].ToString());
            MOKKI.alue = mok.nimi;

            MokkiCollection.Add(MOKKI);
        }
        MokkiListaLv.ItemsSource = MokkiCollection;
    }

    //hakee tietokannastta Palvelut
    private void SqlHaePalvelut()
    {
        PalveluCollection.Clear();
        MySqlConnection con = new MySqlConnection();
        con.ConnectionString = connstring;
        con.Open();
        string sql = "SELECT * FROM palvelu";
        MySqlCommand cmd = new MySqlCommand(sql, con);
        MySqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Palvelu PALVELU = new Palvelu();
            PALVELU.palvelu_id = reader["palvelu_id"].ToString();
            PALVELU.alue_id = reader["alue_id"].ToString();
            PALVELU.nimi = reader["nimi"].ToString();
            PALVELU.kuvaus = reader["kuvaus"].ToString();
            PALVELU.hinta = reader["hinta"].ToString() + "€";
            PALVELU.alv = reader["alv"].ToString() + "%";
            //etsii alue collectionista oikean alueen id perusteella
            var pal = AlueCollection.FirstOrDefault(p => p.alue_id == reader["alue_id"].ToString());
            PALVELU.alue = pal.nimi;

            PalveluCollection.Add(PALVELU);
        }
        PalveluListaLv.ItemsSource = PalveluCollection;
    }

    //Napit ----------------------------------------------------------------------------------------------------------------------------------------------

    //Hakee alueita alkean ensimmäisestä kirjaimesta
    private void AlueHaeBtn_Clicked(object sender, EventArgs e)
    {
        AlueCollection.Clear();
        AlueListaLv.ItemsSource = AlueCollection;

        MySqlConnection con = new MySqlConnection();
        con.ConnectionString = connstring;
        con.Open();
        string sql = "SELECT * FROM alue WHERE nimi LIKE '" 
            + AlueNimiEnt.Text + "%'";
        MySqlCommand cmd = new MySqlCommand(sql, con);
        MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Alue ALUE = new Alue();
            ALUE.alue_id = reader["alue_id"].ToString();
            ALUE.nimi = reader["nimi"].ToString();
            AlueCollection.Add(ALUE);
        }
        AlueListaLv.ItemsSource = AlueCollection;
    }

    //Mökkien haku
    private void MokkiHaeBtn_Clicked(object sender, EventArgs e)
    {
        MokkiCollection.Clear();
        MokkiListaLv.ItemsSource = MokkiCollection;

        MySqlConnection con = new MySqlConnection();
        con.ConnectionString = connstring;
        con.Open();

        string sql = "SELECT * FROM mokki WHERE ";
        sql = sql + "(mokki_id LIKE '" + MokkiMokkiidEnt.Text + "%') AND ";
        sql = sql + "(mokkinimi LIKE '" + MokkiMokkinimiEnt.Text + "%') AND ";
        sql = sql + "(katuosoite LIKE '" + MokkiKatuosoiteEnt.Text + "%') AND ";
        sql = sql + "(postinro LIKE '" + MokkiPostinroEnt.Text + "%') AND ";
        sql = sql + "(hinta LIKE '" + MokkiHintaEnt.Text + "%') AND ";
        sql = sql + "(henkilomaara LIKE '" + MokkiHenkilomaaraEnt.Text + "%') AND ";
        sql = sql + "(kuvaus LIKE '" + MokkiKuvausEnt.Text + "%')";

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
            MOKKI.hinta = reader["hinta"].ToString() + "€";
            MOKKI.kuvaus = reader["kuvaus"].ToString();
            MOKKI.hinta = reader["henkilomaara"].ToString();
            MOKKI.kuvaus = reader["varustelu"].ToString();
            //etsii alue collectionista oikean alueen id perusteella
            var mok = AlueCollection.FirstOrDefault(m => m.alue_id == reader["alue_id"].ToString());
            MOKKI.alue = mok.nimi;

            MokkiCollection.Add(MOKKI);
        }

        MokkiListaLv.ItemsSource = MokkiCollection;
    }

    //luonti napit***************

    //Uude alueen luonti
    private void AlueLuoBtn_Clicked(object sender, EventArgs e)
    {
        MySqlConnection con = new MySqlConnection();
        con.ConnectionString = connstring;
        con.Open();
        string sql = $"INSERT INTO alue (nimi) VALUES ('{AlueNimiEnt.Text}')";
        MySqlCommand insertCmd = new MySqlCommand(sql, con);
        try
        {
            insertCmd.ExecuteNonQuery();
        }
        catch (Exception error)
        {
            DisplayAlert("Alert", error.Message, "OK");
        }

        SqlHaeAlueet();
    }
    //Uuden asiakkuuden luonti
    private void AsiakkuusLuoBtn_Clicked(object sender, EventArgs e)
    {
        MySqlConnection con = new MySqlConnection();
        con.ConnectionString = connstring;
        con.Open();
        string sql = $"INSERT INTO asiakas (etunimi, sukunimi, lahiosoite, postinro, email, puhelinnro) " +
            $"VALUES ('{AsiakkuusEtunimiEnt.Text}', '{AsiakkuusSukunimiEnt.Text}', " +
            $"'{AsiakkuusLahiosoiteEnt.Text}', '{AsiakkuusPostinroEnt.Text}', " +
            $"'{AsiakkuusEmailEnt.Text}', '{AsiakkuusPuhelinnroEnt.Text}')";
        MySqlCommand insertCmd = new MySqlCommand(sql, con);
        try
        {
            insertCmd.ExecuteNonQuery();
        }
        catch (Exception error)
        {
            DisplayAlert("Alert", error.Message, "OK");
        }

        SqlHaeAsiakkaat();
    }

    //Uude Mökin luonti
    private void MokkiLuoBtn_Clicked(object sender, EventArgs e)
    {
        //checkboksit
        List<string> selectedItems = new List<string>();
        if (VarusteluTvCb.IsChecked)
            selectedItems.Add("tv");
        if (VarusteluAstianpesukoneCb.IsChecked)
            selectedItems.Add("astianpesukone");
        if (VarusteluPyykinpesukoneCb.IsChecked)
            selectedItems.Add("pyykinpesukone");
        if (VarusteluSaunaCb.IsChecked)
            selectedItems.Add("sauna");
        if (VarusteluTakkaCb.IsChecked)
            selectedItems.Add("takka");
        string VARUSTELU = string.Join(",", selectedItems);

        //muuttaa alueen nimen Idksi tietokantaan.
        String ALUE_ID = null;
        foreach (var item in AlueCollection)
        {
            if (item.nimi == MokkiAlueEnt.Text)
            {
                ALUE_ID = item.alue_id;
            }
        }

        if (ALUE_ID == null)
        {
            DisplayAlert("Alert", "Aluetta ei löydetty tietokannasta", "OK");
        }
        else
        {
            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = connstring;
            con.Open();
            string sql = $"INSERT INTO mokki (mokkinimi, alue_id, " +
                $"katuosoite, postinro, hinta, henkilomaara, kuvaus, varustelu) " +
                $"VALUES ('{MokkiMokkinimiEnt.Text}',{ALUE_ID}, '{MokkiKatuosoiteEnt.Text}', " +
                $"'{MokkiPostinroEnt.Text}', {MokkiHintaEnt.Text}, {MokkiHenkilomaaraEnt.Text}, " +
                $"'{MokkiKuvausEnt.Text}', '{VARUSTELU}')";
            MySqlCommand insertCmd = new MySqlCommand(sql, con);
            try
            {
                insertCmd.ExecuteNonQuery();
            }
            catch (Exception error)
            {
                DisplayAlert("Alert", error.Message, "OK");
            }

            SqlHaeMokit();
        }
    }
    //Uude palvelun luonti
    private void PalveluLuoBtn_Clicked(object sender, EventArgs e)
    {
        //muuttaa alueen nimen Idksi tietokantaan.
        String ALUE_ID = null;
        foreach (var item in AlueCollection)
        {
            if(item.nimi == PalveluAlueEnt.Text)
            {
                ALUE_ID = item.alue_id;
            }
        }

        if ( ALUE_ID == null)
        {
            DisplayAlert("Alert", "Aluetta ei löydetty tietokannasta", "OK");
        }
        else { 
            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = connstring;
            con.Open();
            string sql = $"INSERT INTO palvelu (palvelu_id, nimi, alue_id, hinta, alv, kuvaus) " +
                $"VALUES ({PalveluIdEnt.Text},'{PalveluNimiEnt.Text}', {ALUE_ID}, " +
                $"{PalveluHintaEnt.Text}, {PalveluAlvEnt.Text}, " +
                $"'{PalveluKuvausEnt.Text}')";
            MySqlCommand insertCmd = new MySqlCommand(sql, con);
            try
            {
                insertCmd.ExecuteNonQuery();
            }
            catch (Exception error)
            {
                DisplayAlert("Alert", error.Message, "OK");
            }

            SqlHaePalvelut();
        }
    }

}