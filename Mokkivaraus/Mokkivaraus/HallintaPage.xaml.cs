using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Mokkivaraus;

public partial class HallintaPage : TabbedPage
{
    public ObservableCollection<Alue> AlueCollection { get; set; }
    public ObservableCollection<Asiakas> AsiakasCollection { get; set; }
    public ObservableCollection<Mokki> MokkiCollection { get; set; }
    public ObservableCollection<Palvelu> PalveluCollection { get; set; }
    public ObservableCollection<Varaus> VarausCollection { get; set; }



    static private String connstring = "server=localhost;uid=root;port=3306;pwd=root;database=vn";
    public HallintaPage()
    {
        InitializeComponent();

        AlueCollection = new ObservableCollection<Alue>();
        AsiakasCollection = new ObservableCollection<Asiakas>();
        MokkiCollection = new ObservableCollection<Mokki>();
        PalveluCollection = new ObservableCollection<Palvelu>();
        VarausCollection = new ObservableCollection<Varaus>();
        BindingContext = this;
        AlueListaLv.BindingContext = AlueCollection;
        AsiakasListaLv.BindingContext = AsiakasCollection;
        MokkiListaLv.BindingContext = MokkiCollection;
        PalveluListaLv.BindingContext = PalveluCollection;
        VarausListaLv.BindingContext = VarausCollection;

        MokkiImagePicker.ItemsSource = MokkiKuvat(8);

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
        SqlHaeVaraus();
    }
    //hakee tietokannastta Varaukset
    private void SqlHaeVaraus()
    {
        VarausCollection.Clear();
        string sql = "SELECT * FROM varaus";

        SqlGet(sql, reader =>
        {
            Varaus VARAUS = new Varaus();
            VARAUS.varaus_id = reader["varaus_id"].ToString();
            VARAUS.asiakas_id = reader["asiakas_id"].ToString();
            VARAUS.mokki_id = reader["mokki_id"].ToString();
            VARAUS.varattupvm = Convert.ToDateTime(reader["varattu_pvm"]).ToString("yyyy-MM-dd");
            VARAUS.vahvistuspvm = Convert.ToDateTime(reader["vahvistus_pvm"]).ToString("yyyy-MM-dd");
            VARAUS.varattualkupvm = Convert.ToDateTime(reader["varattu_alkupvm"]).ToString("yyyy-MM-dd");
            VARAUS.varattuloppupvm = Convert.ToDateTime(reader["varattu_loppupvm"]).ToString("yyyy-MM-dd"); ;

            var asiakas = AsiakasCollection.FirstOrDefault(a => a.asiakas_id == VARAUS.asiakas_id);
            if (asiakas != null)
                VARAUS.asiakas_id = asiakas.etunimi + " " + asiakas.sukunimi;

            var mokki = MokkiCollection.FirstOrDefault(a => a.mokki_id == VARAUS.mokki_id);
            if (mokki != null)
                VARAUS.mokki_id = mokki.mokkinimi;

            VarausCollection.Add(VARAUS);
        });

        VarausListaLv.ItemsSource = VarausCollection;
    }

    //hakee tietokannastta Alueet
    private void SqlHaeAlueet()
    {
        AlueCollection.Clear();
        string sql = "SELECT * FROM alue";

        SqlGet(sql, reader =>
        {
            Alue ALUE = new Alue();
            ALUE.alue_id = reader["alue_id"].ToString();
            ALUE.nimi = reader["nimi"].ToString();
            AlueCollection.Add(ALUE);
        });

        AlueListaLv.ItemsSource = AlueCollection;
    }

    //hakee tietokannastta Asiakkaat
    private void SqlHaeAsiakkaat()
    {
        AsiakasCollection.Clear();

        string sql = "SELECT * FROM asiakas";
        SqlGet(sql, reader =>
        {
            Asiakas ASIAKAS = new Asiakas();
            ASIAKAS.asiakas_id = reader["asiakas_id"].ToString();
            ASIAKAS.etunimi = reader["etunimi"].ToString();
            ASIAKAS.sukunimi = reader["sukunimi"].ToString();
            ASIAKAS.postinro = reader["postinro"].ToString();
            ASIAKAS.lahiosoite = reader["lahiosoite"].ToString();
            ASIAKAS.email = reader["email"].ToString();
            ASIAKAS.puhelinnro = reader["puhelinnro"].ToString();
            AsiakasCollection.Add(ASIAKAS);
        });

        AsiakasListaLv.ItemsSource = AsiakasCollection;
    }

    //hakee tietokannastta Mokit
    private void SqlHaeMokit()
    {
        MokkiCollection.Clear();
        string sql = "SELECT * FROM mokki";
        SqlGet(sql, reader =>
        {
            Mokki MOKKI = new Mokki();
            MOKKI.mokki_id = reader["mokki_id"].ToString();
            MOKKI.alue_id = reader["alue_id"].ToString();
            MOKKI.postinro = reader["postinro"].ToString();
            MOKKI.mokkinimi = reader["mokkinimi"].ToString();
            MOKKI.katuosoite = reader["katuosoite"].ToString();
            MOKKI.hinta = reader["hinta"].ToString();
            MOKKI.kuvaus = reader["kuvaus"].ToString();
            MOKKI.kuva = reader["kuva"].ToString();
            MOKKI.henkilomaara = reader["henkilomaara"].ToString();
            MOKKI.varustelu = reader["varustelu"].ToString();
            //etsii alue collectionista oikean alueen id perusteella
            var mok = AlueCollection.FirstOrDefault(m => m.alue_id == reader["alue_id"].ToString());
            MOKKI.alue = mok.nimi;

            MokkiCollection.Add(MOKKI);
        });

        MokkiListaLv.ItemsSource = MokkiCollection;
    }

    //hakee tietokannastta Palvelut
    private void SqlHaePalvelut()
    {
        PalveluCollection.Clear();
        string sql = "SELECT * FROM palvelu";

        SqlGet(sql, reader =>
        {
            Palvelu PALVELU = new Palvelu();
            PALVELU.palvelu_id = reader["palvelu_id"].ToString();
            PALVELU.alue_id = reader["alue_id"].ToString();
            PALVELU.nimi = reader["nimi"].ToString();
            PALVELU.kuvaus = reader["kuvaus"].ToString();
            PALVELU.hinta = reader["hinta"].ToString();
            PALVELU.alv = reader["alv"].ToString();
            //etsii alue collectionista oikean alueen id perusteella
            var pal = AlueCollection.FirstOrDefault(p => p.alue_id == reader["alue_id"].ToString());
            PALVELU.alue = pal.nimi;

            PalveluCollection.Add(PALVELU);
        });

        PalveluListaLv.ItemsSource = PalveluCollection;
    }

    //Napit ----------------------------------------------------------------------------------------------------------------------------------------------

    //Hakee alueita alkean ensimmäisestä kirjaimesta
    private void AlueHaeBtn_Clicked(object sender, EventArgs e)
    {
        AlueCollection.Clear();

        string sql = "SELECT * FROM alue WHERE nimi LIKE '"
            + AlueNimiEnt.Text + "%'";

        SqlGet(sql, reader =>
        {
            Alue ALUE = new Alue();
            ALUE.alue_id = reader["alue_id"].ToString();
            ALUE.nimi = reader["nimi"].ToString();
            AlueCollection.Add(ALUE);
        });

        AlueListaLv.ItemsSource = AlueCollection;
    }

    //Mökkien haku
    private void MokkiHaeBtn_Clicked(object sender, EventArgs e)
    {
        MokkiCollection.Clear();

        string sql = "SELECT * FROM mokki WHERE ";
        sql = sql + "(mokki_id LIKE '" + MokkiMokkiidEnt.Text + "%') AND ";
        sql = sql + "(mokkinimi LIKE '" + MokkiMokkinimiEnt.Text + "%') AND ";
        sql = sql + "(alue_id LIKE '" + AlueNimiToId(MokkiAlueEnt.Text) + "%') AND ";
        sql = sql + "(katuosoite LIKE '" + MokkiKatuosoiteEnt.Text + "%') AND ";
        sql = sql + "(postinro LIKE '" + MokkiPostinroEnt.Text + "%') AND ";
        sql = sql + "(hinta LIKE '" + MokkiHintaEnt.Text + "%') AND ";
        sql = sql + "(henkilomaara LIKE '" + MokkiHenkilomaaraEnt.Text + "%') AND ";

        sql = sql + "(kuvaus LIKE '" + MokkiKuvausEnt.Text + "%')";

        SqlGet(sql, reader =>
        {
            Mokki MOKKI = new Mokki();
            MOKKI.mokki_id = reader["mokki_id"].ToString();
            MOKKI.alue_id = reader["alue_id"].ToString();
            MOKKI.postinro = reader["postinro"].ToString();
            MOKKI.mokkinimi = reader["mokkinimi"].ToString();
            MOKKI.katuosoite = reader["katuosoite"].ToString();
            MOKKI.hinta = reader["hinta"].ToString();
            MOKKI.kuvaus = reader["kuvaus"].ToString();
            MOKKI.kuva = reader["kuva"].ToString();
            MOKKI.henkilomaara = reader["henkilomaara"].ToString();
            MOKKI.varustelu = reader["varustelu"].ToString();
            //etsii alue collectionista oikean alueen id perusteella
            var mok = AlueCollection.FirstOrDefault(m => m.alue_id == reader["alue_id"].ToString());
            MOKKI.alue = mok.nimi;

            MokkiCollection.Add(MOKKI);
        });

        MokkiListaLv.ItemsSource = MokkiCollection;
    }

    private void AsiakkuusHaeBtn_Clicked(object sender, EventArgs e)
    {
        AsiakasCollection.Clear();

        string sql = "SELECT * FROM asiakas WHERE ";
        sql += "(asiakas_id LIKE '" + AsiakkuusIdEnt.Text + "%') AND ";
        sql += "(etunimi LIKE '" + AsiakkuusEtunimiEnt.Text + "%') AND ";
        sql += "(sukunimi LIKE '" + AsiakkuusSukunimiEnt.Text + "%') AND ";
        sql += "(lahiosoite LIKE '" + AsiakkuusLahiosoiteEnt.Text + "%') AND ";
        sql += "(postinro LIKE '" + AsiakkuusPostinroEnt.Text + "%') AND ";
        sql += "(puhelinnro LIKE '" + AsiakkuusPuhelinnroEnt.Text + "%') AND ";
        sql += "(email LIKE '" + AsiakkuusEmailEnt.Text + "%')";


        SqlGet(sql, reader =>
        {
            Asiakas ASIAKAS = new Asiakas();
            ASIAKAS.asiakas_id = reader["asiakas_id"].ToString();
            ASIAKAS.etunimi = reader["etunimi"].ToString();
            ASIAKAS.sukunimi = reader["sukunimi"].ToString();
            ASIAKAS.postinro = reader["postinro"].ToString();
            ASIAKAS.lahiosoite = reader["lahiosoite"].ToString();
            ASIAKAS.email = reader["email"].ToString();
            ASIAKAS.puhelinnro = reader["puhelinnro"].ToString();
            AsiakasCollection.Add(ASIAKAS);
        });

        AsiakasListaLv.ItemsSource = AsiakasCollection;
    }

    private void PalveluHaeBtn_Clicked(object sender, EventArgs e)
    {
        PalveluCollection.Clear();

        string sql = "SELECT * FROM palvelu WHERE ";
        sql += "(palvelu_id LIKE '" + PalveluIdEnt.Text + "%') AND ";
        sql += "(alue_id LIKE '" + AlueNimiToId(AlueIdEnt.Text) + "%') AND ";
        sql += "(nimi LIKE '" + PalveluNimiEnt.Text + "%') AND ";
        sql += "(kuvaus LIKE '" + PalveluKuvausEnt.Text + "%') AND ";
        sql += "(hinta LIKE '" + PalveluHintaEnt.Text + "%') AND ";
        sql += "(alv LIKE '" + PalveluAlvEnt.Text + "%')";



        SqlGet(sql, reader =>
        {
            Palvelu PALVELU = new Palvelu();
            PALVELU.palvelu_id = reader["palvelu_id"].ToString();
            PALVELU.alue_id = reader["alue_id"].ToString();
            PALVELU.nimi = reader["nimi"].ToString();
            PALVELU.kuvaus = reader["kuvaus"].ToString();
            PALVELU.hinta = reader["hinta"].ToString();
            PALVELU.alv = reader["alv"].ToString();
            //etsii alue collectionista oikean alueen id perusteella
            var pal = AlueCollection.FirstOrDefault(p => p.alue_id == reader["alue_id"].ToString());
            PALVELU.alue = pal.nimi;

            PalveluCollection.Add(PALVELU);
        });

        PalveluListaLv.ItemsSource = PalveluCollection;
    }

    //luonti napit***************

    //Uude alueen luonti
    private void AlueLuoBtn_Clicked(object sender, EventArgs e)
    {
        string sql = $"INSERT INTO alue (nimi) VALUES ('{AlueNimiEnt.Text}')";
        SqlInsert(sql);
        SqlHaeAlueet();
    }

    //Uuden asiakkuuden luonti
    private void AsiakkuusLuoBtn_Clicked(object sender, EventArgs e)
    {
        string sql = $"INSERT INTO asiakas (etunimi, sukunimi, lahiosoite, postinro, email, puhelinnro) " +
            $"VALUES ('{AsiakkuusEtunimiEnt.Text}', '{AsiakkuusSukunimiEnt.Text}', " +
            $"'{AsiakkuusLahiosoiteEnt.Text}', '{AsiakkuusPostinroEnt.Text}', " +
            $"'{AsiakkuusEmailEnt.Text}', '{AsiakkuusPuhelinnroEnt.Text}')";
        SqlInsert(sql);
        SqlHaeAsiakkaat();
    }

    //Uude Mökin luonti
    private void MokkiLuoBtn_Clicked(object sender, EventArgs e)
    {

        //muuttaa alueen nimen Idksi tietokantaan.
        String ALUE_ID = AlueNimiToId(MokkiAlueEnt.Text);

        if (ALUE_ID == null)
        {
            DisplayAlert("Alert", "Aluetta ei löydetty tietokannasta", "OK");
        }
        else
        {
            string sql = $"INSERT INTO mokki (mokkinimi, alue_id, " +
                $"katuosoite, postinro, hinta, henkilomaara, kuvaus, varustelu, kuva) " +
                $"VALUES ('{MokkiMokkinimiEnt.Text}',{ALUE_ID}, '{MokkiKatuosoiteEnt.Text}', " +
                $"'{MokkiPostinroEnt.Text}', {MokkiHintaEnt.Text}, {MokkiHenkilomaaraEnt.Text}, " +
                $"'{MokkiKuvausEnt.Text}', '{VarusteluToString()}', '{MokkiImagePicker.SelectedItem}')";
            SqlInsert(sql);

            SqlHaeMokit();
        }

    }
    //Uude palvelun luonti
    private void PalveluLuoBtn_Clicked(object sender, EventArgs e)
    {
        //muuttaa alueen nimen Idksi tietokantaan.
        String ALUE_ID = AlueNimiToId(PalveluAlueEnt.Text);

        if (ALUE_ID == null)
        {
            DisplayAlert("Alert", "Aluetta ei löydetty tietokannasta", "OK");
        }
        else
        {
            string sql = $"INSERT INTO palvelu ( nimi, alue_id, hinta, alv, kuvaus) " +
                $"VALUES ('{PalveluNimiEnt.Text}', {ALUE_ID}, " +
                $"{PalveluHintaEnt.Text}, {PalveluAlvEnt.Text}, " +
                $"'{PalveluKuvausEnt.Text}')";
            SqlInsert(sql);
            SqlHaePalvelut();
        }
    }

    //Muokaus Napit ja lista funktiot ----------------------------------------------------------------------------------------------------------------------------------------------

    //Varaus muokkaus **********************
    //Laittaa Varaus sivun muokkaus tilaan
    private void VarausOnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
            return;
        Varaus varaus = (Varaus)e.SelectedItem;

        VarausLuoBtn.IsVisible = false;
        VarausHaeBtn.IsVisible = false;
        VarausHyvaksyMuutosBtn.IsVisible = true;
        VarausHylkaaMuutosBtn.IsVisible = true;
        VarausPoistaBtn.IsVisible = true;
        VarausAsiakasIdEnt.IsReadOnly = true;
        VarausIdEnt.IsReadOnly = true;

        VarausIdEnt.Text = varaus.varaus_id;
        VarausMökkiIdEnt.Text = varaus.mokki_id;
        VarausAsiakasIdEnt.Text = varaus.asiakas_id;
        vahvistuDp.Date = DateTime.Parse(varaus.vahvistuspvm);
        varattuDp.Date = DateTime.Parse(varaus.varattupvm);
        varattualkuDp.Date = DateTime.Parse(varaus.varattualkupvm);
        vahvistuloppDp.Date = DateTime.Parse(varaus.varattuloppupvm);

        VarausLbl.Text = "Muokkaa Varaus";
        VarausLbl.TextColor = Colors.Red;

        VarausListaLv.SelectedItem = null;
    }

    private async void VarausHyvaksyMuutosBtn_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Varoitus", "Haluatko varmasti tallentaa muutokset", "Kyllä", "Ei");
        if (answer)
        {
           

            VarausPageReset();
            SqlHaeVaraus();
        }
    }

    private void VarausHylkaaMuutosBtn_Clicked(object sender, EventArgs e)
    {
        VarausPageReset();
    }


    //muuttaa alue sivun perus näkymään
    private void VarausPageReset()
    {
        VarausAsiakasIdEnt.IsReadOnly = false;
        VarausIdEnt.IsReadOnly = false;
        VarausLuoBtn.IsVisible = true;
        VarausHaeBtn.IsVisible = true;
        VarausHyvaksyMuutosBtn.IsVisible = false;
        VarausHylkaaMuutosBtn.IsVisible = false;
        VarausPoistaBtn.IsVisible = false;

        VarausIdEnt.Text = string.Empty;
        VarausMökkiIdEnt.Text = string.Empty;
        VarausAsiakasIdEnt.Text = string.Empty;
        vahvistuDp.Date = DateTime.Now;
        varattuDp.Date = DateTime.Now;
        varattualkuDp.Date = DateTime.Now;
        vahvistuloppDp.Date = DateTime.Now;

        VarausLbl.Text = "Hae/luo Varaus";
        VarausLbl.TextColor = Colors.Black;
    }

    //Alue muokkaus **********************
    //Laittaa alue sivun muokkaus tilaan
    private void AlueOnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
            return;
        Alue alue = (Alue)e.SelectedItem;

        AlueLuoBtn.IsVisible = false;
        AlueHaeBtn.IsVisible = false;
        AlueHyvaksyMuutosBtn.IsVisible = true;
        AlueHylkaaMuutosBtn.IsVisible = true;
        AluePoistaBtn.IsVisible = true;

        AlueIdEnt.IsReadOnly = true;
        AlueNimiEnt.Text = alue.nimi;
        AlueIdEnt.Text = alue.alue_id;
        AlueLbl.Text = "Muokkaa Alue";
        AlueLbl.TextColor = Colors.Red;

        AlueListaLv.SelectedItem = null;
    }

    private async void AlueHyvaksyMuutosBtn_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Varoitus", "Haluatko varmasti tallentaa muutokset", "Kyllä", "Ei");
        if (answer)
        {
            string sql = $"UPDATE alue SET nimi = '{AlueNimiEnt.Text}' WHERE alue_id = {AlueIdEnt.Text}";
            SqlInsert(sql);

            AluePageReset();
            SqlHaeAlueet();
        }
    }

    private void AlueHylkaaMuutosBtn_Clicked(object sender, EventArgs e)
    {
        AluePageReset();
    }


    //muuttaa alue sivun perus näkymään
    private void AluePageReset()
    {
        AlueIdEnt.IsReadOnly = false;
        AlueLuoBtn.IsVisible = true;
        AlueHaeBtn.IsVisible = true;
        AlueHyvaksyMuutosBtn.IsVisible = false;
        AlueHylkaaMuutosBtn.IsVisible = false;
        AluePoistaBtn.IsVisible = false;
        AlueNimiEnt.Text = string.Empty;
        AlueIdEnt.Text = string.Empty;
        AlueLbl.Text = "Hae/luo alue";
        AlueLbl.TextColor = Colors.Black;
    }

    //Mökki muokkaus **********************
    //Laittaa Mökki sivun muokkaus tilaan 
    private void MokkiOnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
            return;
        Mokki mokki = (Mokki)e.SelectedItem;

        MokkiLuoBtn.IsVisible = false;
        MokkiHaeBtn.IsVisible = false;
        MokkiHyvaksyMuutosBtn.IsVisible = true;
        MokkiHylkaaMuutosBtn.IsVisible = true;
        MokkiMokkiidEnt.IsReadOnly = true;
        MokkiPoistaBtn.IsVisible = true;

        MokkiMokkinimiEnt.Text = mokki.mokkinimi;
        MokkiAlueEnt.Text = mokki.alue;
        MokkiKatuosoiteEnt.Text = mokki.katuosoite;
        MokkiPostinroEnt.Text = mokki.postinro;
        MokkiHintaEnt.Text = mokki.hinta;
        MokkiHenkilomaaraEnt.Text = mokki.henkilomaara;
        MokkiKuvausEnt.Text = mokki.kuvaus;
        MokkiImg.Source = mokki.kuva;
        MokkiImagePicker.SelectedItem = mokki.kuva;
        MokkiMokkiidEnt.Text = mokki.mokki_id;

        string Varustelu = mokki.varustelu;
        Varustelu = Varustelu.Replace(" ", "");
        string[] VarusteluSplit = Varustelu.Split(',');

        MokkiCheckboxReset();

        foreach (var item in VarusteluSplit)
        {
            if (item == "tv")
                VarusteluTvCb.IsChecked = true;
            if (item == "astianpesukone")
                VarusteluAstianpesukoneCb.IsChecked = true;
            if (item == "pyykinpesukone")
                VarusteluPyykinpesukoneCb.IsChecked = true;
            if (item == "sauna")
                VarusteluSaunaCb.IsChecked = true;
            if (item == "takka")
                VarusteluTakkaCb.IsChecked = true;
        }


        MokkiLbl.Text = "Muokkaa Mökki";
        MokkiLbl.TextColor = Colors.Red;

        MokkiListaLv.SelectedItem = null;
    }

    private async void MokkiHyvaksyMuutosBtn_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Varoitus", "Haluatko varmasti tallentaa muutokset", "Kyllä", "Ei");
        if (answer)
        {

            string sql = $"UPDATE mokki SET " +
                $"mokkinimi = '{MokkiMokkinimiEnt.Text}', " +
                $"alue_id = {AlueNimiToId(MokkiAlueEnt.Text)}, " +
                $"katuosoite = '{MokkiKatuosoiteEnt.Text}', " +
                $"postinro = '{MokkiPostinroEnt.Text}', " +
                $"hinta = {MokkiHintaEnt.Text}, " +
                $"henkilomaara = {MokkiHenkilomaaraEnt.Text}, " +
                $"kuvaus = '{MokkiKuvausEnt.Text}', " +
                $"varustelu = '{VarusteluToString()}', " +
                $"kuva = '{MokkiImagePicker.SelectedItem}' " +
                $"WHERE mokki_id = {MokkiMokkiidEnt.Text}";

            SqlInsert(sql);
            MokkiPageReset();
            SqlHaeMokit();
        }

    }

    private void MokkiHylkaaMuutosBtn_Clicked(object sender, EventArgs e)
    {
        MokkiPageReset();
    }


    //muuttaa alue sivun perus näkymään
    private void MokkiPageReset()
    {
        MokkiLuoBtn.IsVisible = true;
        MokkiHaeBtn.IsVisible = true;
        MokkiHyvaksyMuutosBtn.IsVisible = false;
        MokkiHylkaaMuutosBtn.IsVisible = false;
        MokkiMokkiidEnt.IsReadOnly = false;
        MokkiPoistaBtn.IsVisible = false;

        MokkiMokkinimiEnt.Text = string.Empty;
        MokkiAlueEnt.Text = string.Empty;
        MokkiKatuosoiteEnt.Text = string.Empty;
        MokkiPostinroEnt.Text = string.Empty;
        MokkiHintaEnt.Text = string.Empty;
        MokkiHenkilomaaraEnt.Text = string.Empty;
        MokkiKuvausEnt.Text = string.Empty;
        MokkiImg.Source = string.Empty;
        MokkiImagePicker.SelectedItem = string.Empty;
        MokkiMokkiidEnt.Text = string.Empty;

        MokkiCheckboxReset();

        MokkiLbl.Text = "Hae/luo Mökki";
        MokkiLbl.TextColor = Colors.Black;
    }

    //Asiakas muokkaus **********************
    //Laittaa Asiakas sivun muokkaus tilaan 
    private void AsiakkuusOnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
            return;
        Asiakas asiakas = (Asiakas)e.SelectedItem;

        AsiakkuusLuoBtn.IsVisible = false;
        AsiakkuusHaeBtn.IsVisible = false;
        AsiakkuusHyvaksyMuutosBtn.IsVisible = true;
        AsiakkuusHylkaaMuutosBtn.IsVisible = true;
        AsiakkuusPoistaBtn.IsVisible = true;

        AsiakkuusIdEnt.IsReadOnly = true;
        AsiakkuusIdEnt.Text = asiakas.asiakas_id;
        AsiakkuusEtunimiEnt.Text = asiakas.etunimi;
        AsiakkuusSukunimiEnt.Text = asiakas.sukunimi;
        AsiakkuusLahiosoiteEnt.Text = asiakas.lahiosoite;
        AsiakkuusPostinroEnt.Text = asiakas.postinro;
        AsiakkuusPuhelinnroEnt.Text = asiakas.puhelinnro;
        AsiakkuusEmailEnt.Text = asiakas.email;

        AsiakkuusLbl.Text = "Muokkaa Asiakkuus";
        AsiakkuusLbl.TextColor = Colors.Red;

        AsiakasListaLv.SelectedItem = null;
    }

    private async void AsiakkuusHyvaksyMuutosBtn_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Varoitus", "Haluatko varmasti tallentaa muutokset", "Kyllä", "Ei");
        if (answer)
        {
            string sql = $"UPDATE asiakas SET " +
            $"etunimi = '{AsiakkuusEtunimiEnt.Text}', " +
            $"sukunimi = '{AsiakkuusSukunimiEnt.Text}', " +
            $"lahiosoite = '{AsiakkuusLahiosoiteEnt.Text}', " +
            $"postinro = '{AsiakkuusPostinroEnt.Text}', " +
            $"email = '{AsiakkuusEmailEnt.Text}', " +
            $"puhelinnro = '{AsiakkuusPuhelinnroEnt.Text}' " +
            $"WHERE asiakas_id = {AsiakkuusIdEnt.Text}";

            SqlInsert(sql);

            AsiakkuusPageReset();
            SqlHaeAsiakkaat();
        }

    }

    private void AsiakkuusHylkaaMuutosBtn_Clicked(object sender, EventArgs e)
    {
        AsiakkuusPageReset();
    }


    //muuttaa alue sivun perus näkymään
    private void AsiakkuusPageReset()
    {
        AsiakkuusLuoBtn.IsVisible = true;
        AsiakkuusHaeBtn.IsVisible = true;
        AsiakkuusHyvaksyMuutosBtn.IsVisible = false;
        AsiakkuusHylkaaMuutosBtn.IsVisible = false;
        AsiakkuusPoistaBtn.IsVisible = false;

        AsiakkuusIdEnt.IsReadOnly = false;
        AsiakkuusIdEnt.Text = string.Empty;
        AsiakkuusEtunimiEnt.Text = string.Empty;
        AsiakkuusSukunimiEnt.Text = string.Empty;
        AsiakkuusLahiosoiteEnt.Text = string.Empty;
        AsiakkuusPostinroEnt.Text = string.Empty;
        AsiakkuusPuhelinnroEnt.Text = string.Empty;
        AsiakkuusEmailEnt.Text = string.Empty;


        AsiakkuusLbl.Text = "Hae/luo Asiakkuus";
        AsiakkuusLbl.TextColor = Colors.Black;
    }

    //Palvelu muokkaus **********************
    //Laittaa Palvelu sivun muokkaus tilaan 
    private void PalveluOnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
            return;
        Palvelu palvelu = (Palvelu)e.SelectedItem;

        PalveluLuoBtn.IsVisible = false;
        PalveluHaeBtn.IsVisible = false;
        PalveluHyvaksyMuutosBtn.IsVisible = true;
        PalveluHylkaaMuutosBtn.IsVisible = true;
        PalveluPoistaBtn.IsVisible = true;

        PalveluIdEnt.IsReadOnly = true;
        PalveluIdEnt.Text = palvelu.palvelu_id;
        PalveluNimiEnt.Text = palvelu.nimi;
        PalveluAlueEnt.Text = palvelu.alue;
        PalveluHintaEnt.Text = palvelu.hinta;
        PalveluAlvEnt.Text = palvelu.alv;
        PalveluKuvausEnt.Text = palvelu.kuvaus;

        PalveluLbl.Text = "Muokkaa Palvelu";
        PalveluLbl.TextColor = Colors.Red;

        PalveluListaLv.SelectedItem = null;
    }

    private async void PalveluHyvaksyMuutosBtn_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Varoitus", "Haluatko varmasti tallentaa muutokset", "Kyllä", "Ei");
        if (answer)
        {
            string sql = $"UPDATE palvelu SET " +
            $"nimi = '{PalveluNimiEnt.Text}', " +
            $"alue_id = {AlueNimiToId(PalveluAlueEnt.Text)}, " +
            $"hinta = {PalveluHintaEnt.Text}, " +
            $"alv = {PalveluAlvEnt.Text}, " +
            $"kuvaus = '{PalveluKuvausEnt.Text}' " +
            $"WHERE palvelu_id = {PalveluIdEnt.Text}";

            SqlInsert(sql);
            PalveluPageReset();
            SqlHaePalvelut();
        }

    }

    private void PalveluHylkaaMuutosBtn_Clicked(object sender, EventArgs e)
    {
        PalveluPageReset();
    }


    //muuttaa alue sivun perus näkymään
    private void PalveluPageReset()
    {
        PalveluLuoBtn.IsVisible = true;
        PalveluHaeBtn.IsVisible = true;
        PalveluHyvaksyMuutosBtn.IsVisible = false;
        PalveluHylkaaMuutosBtn.IsVisible = false;
        PalveluPoistaBtn.IsVisible = false;

        PalveluIdEnt.IsReadOnly = false;
        PalveluIdEnt.Text = string.Empty;
        PalveluNimiEnt.Text = string.Empty;
        PalveluAlueEnt.Text = string.Empty;
        PalveluHintaEnt.Text = string.Empty;
        PalveluAlvEnt.Text = string.Empty;
        PalveluKuvausEnt.Text = string.Empty;


        PalveluLbl.Text = "Hae/luo Palvelu";
        PalveluLbl.TextColor = Colors.Black;
    }


    //Poisto napit -------------------------------------------------------
    private async void AsiakkuusPoistaBtn_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("Varoitus", "Asiakkas poisto toimii vaan jos asiakkaalla ei ole sidottuja tietoja", "ok");
        bool answer = await DisplayAlert("Varoitus", "Haluatko varmasti poistaa asiakkaan", "Kyllä", "Ei");
        if (answer)
        {
            string sql = $"DELETE FROM asiakas WHERE asiakas_id = {AsiakkuusIdEnt.Text}";
            SqlInsert(sql);
            AsiakkuusPageReset();
            SqlHaeAsiakkaat();
        }
    }

    private async void AluePoistaBtn_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Varoitus", "Haluatko varmasti poistaa", "Kyllä", "Ei");
        if (answer)
        {
            string sql = $"DELETE FROM alue WHERE alue_id = {AlueIdEnt.Text}";
            SqlInsert(sql);
            AluePageReset();
            SqlHaeAlueet();
        }
    }

    private async void MokkiPoistaBtn_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Varoitus", "Haluatko varmasti poistaa", "Kyllä", "Ei");
        if (answer)
        {
            string sql = $"DELETE FROM mokki WHERE mokki_id = {MokkiMokkiidEnt.Text}";
            SqlInsert(sql);
            MokkiPageReset();
            SqlHaeMokit();
        }
    }

    private async void PalveluPoistaBtn_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Varoitus", "Haluatko varmasti poistaa", "Kyllä", "Ei");
        if (answer)
        {
            string sql = $"DELETE FROM palvelu WHERE palvelu_id = {PalveluIdEnt.Text}";
            SqlInsert(sql);
            PalveluPageReset();
            SqlHaePalvelut();
        }
    }

    private async void VarausPoistaBtn_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Varoitus", "Oletko varma, Varaukseen liittyvät laskut myös poistetaan!", "Kyllä", "Ei");
        if (answer)
        {
            string sql1 = $"DELETE FROM varauksen_palvelut WHERE varaus_id = {VarausIdEnt.Text}";
            SqlInsert(sql1);
            string sql2 = $"DELETE FROM lasku WHERE varaus_id = {VarausIdEnt.Text}";
            SqlInsert(sql2);
            string sql3 = $"DELETE FROM varaus WHERE varaus_id = {VarausIdEnt.Text}";
            SqlInsert(sql3);
            VarausPageReset();
            SqlHaeVaraus();
        }
    }



    //Muut funktiot --------------------------------------------------------------------
    private string AlueNimiToId(string s)
    {
        String ALUE_ID = null;
        foreach (var item in AlueCollection)
        {
            if (item.nimi == s)
            {
                ALUE_ID = item.alue_id;
            }
        }
        return ALUE_ID;
    }


    private string AlueIdToNimi(string s)
    {
        String ALUE_NIMI = null;
        foreach (var item in AlueCollection)
        {
            if (item.alue_id == s)
            {
                ALUE_NIMI = item.nimi;
            }
        }
        return ALUE_NIMI;
    }

    private string VarusteluToString()
    {
        List<string> CHEKBOKSI_VALITTU = new List<string>();

        if (VarusteluTvCb.IsChecked == true)
            CHEKBOKSI_VALITTU.Add("tv");
        if (VarusteluAstianpesukoneCb.IsChecked == true)
            CHEKBOKSI_VALITTU.Add("astianpesukone");
        if (VarusteluPyykinpesukoneCb.IsChecked == true)
            CHEKBOKSI_VALITTU.Add("pyykinpesukone");
        if (VarusteluSaunaCb.IsChecked == true)
            CHEKBOKSI_VALITTU.Add("sauna");
        if (VarusteluTakkaCb.IsChecked == true)
            CHEKBOKSI_VALITTU.Add("takka");

        return string.Join(",", CHEKBOKSI_VALITTU);
    }

    //Tarkistaa chekatut varustel Checkboksit ja sitten muuntaa ne stringiksi
    private void MokkiCheckboxReset()
    {
        VarusteluTvCb.IsChecked = false;
        VarusteluAstianpesukoneCb.IsChecked = false;
        VarusteluPyykinpesukoneCb.IsChecked = false;
        VarusteluSaunaCb.IsChecked = false;
        VarusteluTakkaCb.IsChecked = false;
    }

    //iMokki kuvat
    private List<string> MokkiKuvat(int maara)
    {
        List<string> list = new List<string>();
        for (int i = 0; i < maara; i++)
        {
            list.Add("mokki" + (i + 1) + ".jpg");
        }
        return list;
    }

    //Vaihtaa kmökin kuvan kun Pickerissä vaihtuu kohta
    private void MokkiImagePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        String kuva = MokkiImagePicker.SelectedItem.ToString();
        MokkiImg.Source = kuva;

    }

    //saa sql lauseen ja sen jälkeen tekee insertin. Esim muokkaus, lisäys jne
    private void SqlInsert(string sql)
    {
        using (MySqlConnection con = new MySqlConnection())
        {
            con.ConnectionString = connstring;
            con.Open();
            MySqlCommand insertCmd = new MySqlCommand(sql, con);
            try
            {
                insertCmd.ExecuteNonQuery();
            }
            catch (Exception error)
            {
                DisplayAlert("Alert", error.Message, "OK");
            }
        }
    }

    //Ottaa hakua varten Sql lauseen ja Koodin readerin sisään.
    private void SqlGet(string sql, Action<MySqlDataReader> ReaderKoodi)
    {
        MySqlConnection con = new MySqlConnection(connstring);
        try
        {
            con.Open();
            MySqlCommand cmd = new MySqlCommand(sql, con);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    ReaderKoodi(reader);
                }
            }
        }
        finally
        {
            con.Close();
        }
    }

    
}
