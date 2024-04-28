using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;

namespace Mokkivaraus;

public partial class VarausPage
{
    public ObservableCollection<Alue> AlueCollection { get; set; }
    public ObservableCollection<Mokki> MokkiCollection { get; set; }
    static private String connstring = "server=localhost;uid=root;port=3306;pwd=root;database=vn";

    public VarausPage()
	{
		InitializeComponent();

        AlueCollection = new ObservableCollection<Alue>();
        MokkiCollection = new ObservableCollection<Mokki>();
        BindingContext = this;
        MokkiListaLv.BindingContext = MokkiCollection;

        //t‰m‰n avulla saadaan aluenimi labeliin 
        // Create the converter and pass the AlueCollection
        var converter = new AlueIdToNimiConverter(AlueCollection);
        // Add the converter to the page resources
        Resources = new ResourceDictionary();
        Resources.Add("AlueIdToNimiConverter", converter);


        SqlHaeMokit();
        //lis‰‰n alue pickeriin vaihtoehdoksi
        SqlHaeAlueet();
    }
    //hakee tietokannastta Mokit
    private void SqlHaeMokit()
    {
        MokkiCollection.Clear(); // Clear the existing collection

        MySqlConnection con = new MySqlConnection();
        con.ConnectionString = connstring;
        con.Open();

        string sql = "SELECT * FROM mokki";
        MySqlCommand cmd = new MySqlCommand(sql, con);
        MySqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Mokki mokki = new Mokki
            {
                mokki_id = reader["mokki_id"].ToString(),
                alue_id = reader["alue_id"].ToString(),
                postinro = reader["postinro"].ToString(),
                mokkinimi = reader["mokkinimi"].ToString(),
                katuosoite = reader["katuosoite"].ToString(),
                hinta = reader["hinta"].ToString(),
                kuvaus = reader["kuvaus"].ToString(),
                henkilomaara = reader["henkilomaara"].ToString(),
                varustelu = reader["varustelu"].ToString()
            };

            MokkiCollection.Add(mokki);
        }

        MokkiListaLv.ItemsSource = MokkiCollection;
    }
    private void SqlHaeAlueet()
    {
        AlueCollection.Clear(); // Clear the existing collection

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
        
        AlueListaLv.ItemsSource = AlueCollection;
    }
    private void valitseBtn_Clicked(object sender, EventArgs e)
    {

    }
}
