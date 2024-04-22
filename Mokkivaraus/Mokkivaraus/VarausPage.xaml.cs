using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;

namespace Mokkivaraus;

public partial class VarausPage : ContentPage
{
    public List<MokkiInfo> MokkiList { get; set; }
    public VarausPage()
	{
		InitializeComponent();

        // Create some sample data
        MokkiList = new List<MokkiInfo>
        {
            new MokkiInfo { Nimi = "Example Cottage 1", HenkiloCount = "4", Hinta = "100€ per night" },
            new MokkiInfo { Nimi = "Example Cottage 2", HenkiloCount = "6", Hinta = "120€ per night" },
            new MokkiInfo { Nimi = "Example Cottage 3", HenkiloCount = "2", Hinta = "80€ per night"  },
            new MokkiInfo { Nimi = "Cozy Cabin Retreat", HenkiloCount = "4", Hinta = "120€ per night" },
            new MokkiInfo { Nimi = "Lakefront Getaway", HenkiloCount = "6", Hinta = "200€ per night" },
            new MokkiInfo { Nimi = "Forest Hideaway", HenkiloCount = "2", Hinta = "100€ per night" },
            new MokkiInfo { Nimi = "Rustic Lakeside Cabin", HenkiloCount = "8", Hinta = "250€ per night" },
            new MokkiInfo { Nimi = "Mountain Retreat Cottage", HenkiloCount = "3", Hinta = "150€ per night" }

        };

        BindingContext = this;
    }
    private void valitseBtn_Clicked(object sender, EventArgs e)
    {
    }
}
