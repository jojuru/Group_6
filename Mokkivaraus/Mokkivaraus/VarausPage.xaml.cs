using System.Collections.ObjectModel;

namespace Mokkivaraus;

public partial class VarausPage
{
    public List<MokkiInfo> MokkiList { get; set; }
    public VarausPage()
	{
		InitializeComponent();

        // Create some sample data
        MokkiList = new List<MokkiInfo>
        {
            new MokkiInfo { Nimi = "Mets�lammen M�kki", HenkiloCount = "4", Hinta = "120� per y�" },
            new MokkiInfo { Nimi = "J�rvenrannan Huvila", HenkiloCount = "6", Hinta = "150� per y�" },
            new MokkiInfo { Nimi = "Talviturkin Tupa", HenkiloCount = "2", Hinta = "90� per y�"  },
            new MokkiInfo { Nimi = "Korpikuusen Kota", HenkiloCount = "4", Hinta = "130� per y�" },
            new MokkiInfo { Nimi = "Aurinkolammen M�kki", HenkiloCount = "6", Hinta = "180� per y�" },
            new MokkiInfo { Nimi = "Tunturituvan Taika", HenkiloCount = "2", Hinta = "110� per y�" },
            new MokkiInfo { Nimi = "Rantasaunan Riemu", HenkiloCount = "8", Hinta = "250� per y�" },
            new MokkiInfo { Nimi = "Kuusihuvilan Keidas", HenkiloCount = "3", Hinta = "140� per y�" },
            new MokkiInfo { Nimi = "Pikkutupa", HenkiloCount = "2", Hinta = "80� per y�" },
            new MokkiInfo { Nimi = "Kes�kumpu", HenkiloCount = "4", Hinta = "120� per y�" },
            new MokkiInfo { Nimi = "Kuusikulma", HenkiloCount = "6", Hinta = "150� per y�" },
            new MokkiInfo { Nimi = "Syysmaisema", HenkiloCount = "3", Hinta = "100� per y�" },
            new MokkiInfo { Nimi = "Talvikoto", HenkiloCount = "5", Hinta = "130� per y�" },
            new MokkiInfo { Nimi = "Kev�tkeidas", HenkiloCount = "4", Hinta = "140� per y�" },
            new MokkiInfo { Nimi = "Kes�m�kki", HenkiloCount = "6", Hinta = "160� per y�" },
            new MokkiInfo { Nimi = "Talvimaja", HenkiloCount = "3", Hinta = "110� per y�" }
        };


        BindingContext = this;
    }
    private void valitseBtn_Clicked(object sender, EventArgs e)
    {
    }
}
