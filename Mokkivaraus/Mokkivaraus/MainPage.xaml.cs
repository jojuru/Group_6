namespace Mokkivaraus
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void VaraaMokkiBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new VarausPage());
        }
        private async void LaskutusBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LaskutusPage());
        }
        private async void HallintaBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HallintaPage());
        }
    }
}