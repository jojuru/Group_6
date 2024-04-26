using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;

namespace Mokkivaraus;

public partial class HallintaPage : TabbedPage
{
    public ObservableCollection<Alue> AlueCollection { get; set; }
    public ObservableCollection<Asiakas> AsiakasCollection { get; set; }

    static private String connstring = "server=localhost;uid=root;pwd=root;database=vn";
    public HallintaPage()
	{
		InitializeComponent();
        AlueCollection = new ObservableCollection<Alue>();
        AsiakasCollection = new ObservableCollection<Asiakas>();
        BindingContext = this;
		AlueListaLv.BindingContext = AlueCollection;
        AsiakasListaLv.BindingContext = AsiakasCollection;


        AlueCollection = new ObservableCollection<Alue>();
        BindingContext = this;
        AlueListaLv.BindingContext = AlueCollection;

        SqlHaeKaikki();
    }

    //hakee tietokannasta kaikki
    private void SqlHaeKaikki()
	{
        SqlHaeAsiakkaat();
        SqlHaeAlueet();

	}
	
	//hakee tietokannastta Alueet
	private void SqlHaeAlueet()
	{
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
}