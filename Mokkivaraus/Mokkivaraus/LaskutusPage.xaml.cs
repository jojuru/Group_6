using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
namespace Mokkivaraus;

public partial class LaskutusPage : TabbedPage
{	
	public ObservableCollection<Laskutus> LaskutusCollection { get; set; }

    static private String connstring = "server=localhost;uid=root;port=3306;pwd=Verorakoja123;database=vn";
    public LaskutusPage()
	{
		InitializeComponent();

        LaskutusCollection = new ObservableCollection<Laskutus>();
        LaskuListaLv.BindingContext = LaskutusCollection;
        
        SqlHaeLaskut();
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

}