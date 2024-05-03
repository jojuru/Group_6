using System.Collections.ObjectModel;

namespace Mokkivaraus;

public partial class MokkiPage : ContentPage
{
    public ObservableCollection<Alue> AlueCollection { get; set; }
    public ObservableCollection<Mokki> MokkiCollection { get; set; }
    public ObservableCollection<Palvelu> PalveluCollection { get; set; }
    public Dictionary<string, List<string>> ServicesByArea { get; set; }

    public MokkiPage(
        ObservableCollection<Alue> alueCollection,
        ObservableCollection<Mokki> mokkiCollection,
        ObservableCollection<Palvelu> palveluCollection,
        Dictionary<string, List<string>> servicesByArea)
    {
        Main();
        AlueCollection = alueCollection;
        MokkiCollection = mokkiCollection;
        PalveluCollection = palveluCollection;
        ServicesByArea = servicesByArea;
    }
    public void Main()
    {
        InitializeComponent();
    }

}