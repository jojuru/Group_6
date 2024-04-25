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
            new MokkiInfo { Nimi = "Metsälammen Mökki", HenkiloCount = "4", Hinta = "120€ per yö" },
            new MokkiInfo { Nimi = "Järvenrannan Huvila", HenkiloCount = "6", Hinta = "150€ per yö" },
            new MokkiInfo { Nimi = "Talviturkin Tupa", HenkiloCount = "2", Hinta = "90€ per yö"  },
            new MokkiInfo { Nimi = "Korpikuusen Kota", HenkiloCount = "4", Hinta = "130€ per yö" },
            new MokkiInfo { Nimi = "Aurinkolammen Mökki", HenkiloCount = "6", Hinta = "180€ per yö" },
            new MokkiInfo { Nimi = "Tunturituvan Taika", HenkiloCount = "2", Hinta = "110€ per yö" },
            new MokkiInfo { Nimi = "Rantasaunan Riemu", HenkiloCount = "8", Hinta = "250€ per yö" },
            new MokkiInfo { Nimi = "Kuusihuvilan Keidas", HenkiloCount = "3", Hinta = "140€ per yö" },
            new MokkiInfo { Nimi = "Pikkutupa", HenkiloCount = "2", Hinta = "80€ per yö" },
            new MokkiInfo { Nimi = "Kesäkumpu", HenkiloCount = "4", Hinta = "120€ per yö" },
            new MokkiInfo { Nimi = "Kuusikulma", HenkiloCount = "6", Hinta = "150€ per yö" },
            new MokkiInfo { Nimi = "Syysmaisema", HenkiloCount = "3", Hinta = "100€ per yö" },
            new MokkiInfo { Nimi = "Talvikoto", HenkiloCount = "5", Hinta = "130€ per yö" },
            new MokkiInfo { Nimi = "Kevätkeidas", HenkiloCount = "4", Hinta = "140€ per yö" },
            new MokkiInfo { Nimi = "Kesämökki", HenkiloCount = "6", Hinta = "160€ per yö" },
            new MokkiInfo { Nimi = "Talvimaja", HenkiloCount = "3", Hinta = "110€ per yö" }
        };


        BindingContext = this;
    }
    private void valitseBtn_Clicked(object sender, EventArgs e)
    {
    }
}
