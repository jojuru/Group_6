namespace Mokkivaraus;

public partial class VarausPage : ContentPage
{
	public VarausPage()
	{
		InitializeComponent();
	}
    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}