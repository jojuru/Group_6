namespace Mokkivaraus;

public partial class VarausPage : ContentPage
{
    private bool isToggled = false;
    public VarausPage()
	{
		InitializeComponent();
	}
    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
    private void valitseBtn_Clicked(object sender, EventArgs e)
    {
        // toggle valitseBtn joka muuttaa väriä
        if (isToggled)
        {
            valitseBtn.BackgroundColor = Color.FromRgba(0,0,0, 255);
            mokkiBorder.Stroke = Color.FromRgba(0, 0, 0, 255);
        }
        else
        {
            valitseBtn.BackgroundColor = Color.FromRgba(0, 255, 0, 120);
            mokkiBorder.Stroke = Color.FromRgba(0, 255, 0, 120);
        }
        isToggled = !isToggled; 
    }
}
