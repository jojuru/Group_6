using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Mokkivaraus;

public partial class MokkiPage : ContentPage
{
    public ObservableCollection<Mokki> ClickedMokkiList { get; }
    static private String connstring = "server=localhost;uid=root;port=3306;pwd=root;database=vn";

    public MokkiPage(Mokki clickedMokki)
    {
        InitializeComponent();
        ClickedMokkiList = new ObservableCollection<Mokki> { clickedMokki };
        BindingContext = this;
        // Print all properties of the clickedMokki object
        foreach (var property in typeof(Mokki).GetProperties())
        {
            var value = property.GetValue(clickedMokki);
            Debug.WriteLine($"{property.Name}: {value}");
        }

    }


}