using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Text;
namespace Mokkivaraus;

public partial class LaskutusPage : TabbedPage
{	
	public ObservableCollection<Laskutus> LaskutusCollection { get; set; }

    static private String connstring = "server=localhost;uid=root;port=3306;pwd=root;database=vn";
    public LaskutusPage()
	{
		InitializeComponent();

        LaskutusCollection = new ObservableCollection<Laskutus>();
        LaskuListaLv.BindingContext = LaskutusCollection;

        HaeLaskutAsiakkaalle(2);
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
    private void HaeLaskutAsiakkaalle(int asiakasId)
    {
        MySqlConnection con = new MySqlConnection(connstring);
        con.Open();
        string sql = @"
            SELECT l.lasku_id, l.varaus_id, l.summa, l.alv, l.maksettu
            FROM lasku l
            JOIN varaus v ON l.varaus_id = v.varaus_id
            JOIN asiakas a ON v.asiakas_id = a.asiakas_id
            WHERE a.asiakas_id = @asiakasId";

        MySqlCommand cmd = new MySqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@asiakasId", asiakasId);

        MySqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Laskutus laskutus = new Laskutus
            {
                lasku_id = reader["lasku_id"].ToString(),
                varaus_id = reader["varaus_id"].ToString(),
                summa = reader["summa"].ToString(),
                alv = reader["alv"].ToString(),
                maksettu = reader["maksettu"].ToString()
            };
            LaskutusCollection.Add(laskutus);
        }
        LaskuListaLv.ItemsSource = LaskutusCollection;
        con.Close();
    }

    private void LaskuHaeBtn_Clicked(object sender, EventArgs e)
    {
        var etunimi = etunimiEntry.Text;
        var sukunimi = sukunimiEntry.Text;
        var varausNumero = varausNumeroEntry.Text;
        var sahkoposti = sahkopostiEntry.Text;
        var puhelinnumero = puhelinNumeroEntry.Text;

        var query = new StringBuilder("SELECT l.lasku_id, l.varaus_id, a.etunimi, a.sukunimi, l.summa, l.alv, l.maksettu FROM lasku l JOIN varaus v ON l.varaus_id = v.varaus_id JOIN asiakas a ON v.asiakas_id = a.asiakas_id ");
        var conditions = new List<string>();

        if (!string.IsNullOrEmpty(etunimi))
            conditions.Add("a.etunimi LIKE @etunimi");
        if (!string.IsNullOrEmpty(sukunimi))
            conditions.Add("a.sukunimi LIKE @sukunimi");
        if (!string.IsNullOrEmpty(varausNumero))
            conditions.Add("v.varaus_id LIKE @varausId");
        if (!string.IsNullOrEmpty(sahkoposti))
            conditions.Add("a.email LIKE @email");

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
                maksettu = reader["maksettu"].ToString()
            };
            LaskutusCollection.Add(laskutus);
        }
        LaskuListaLv.ItemsSource = LaskutusCollection;
        con.Close();
    }
}