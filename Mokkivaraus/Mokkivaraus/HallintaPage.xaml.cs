using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;

namespace Mokkivaraus;

public partial class HallintaPage : TabbedPage
{
    public ObservableCollection<Alue> AlueCollection { get; set; }
    public HallintaPage()
	{
		InitializeComponent();
        AlueCollection = new ObservableCollection<Alue>();
		BindingContext = this;
		AlueListaLv.BindingContext = AlueCollection;


        SqlHaku();
    }
	
	//hakee tietokannastta kaiken
	private void SqlHaku()
	{
		string connstring = "server=localhost;uid=root;pwd=root;database=vn";
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

}