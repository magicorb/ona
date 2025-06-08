namespace Ona.App.Today;

public partial class TodayPage : ContentPage
{
	public TodayPage()
	{
		InitializeComponent();
	}

	private void ContentPage_Loaded(object sender, EventArgs e)
	{
		BindingContext = Handler.MauiContext.Services.GetService<TodayViewModel>();
	}
}