namespace Ona.App.Insights;

public partial class InsightsPage : ContentPage
{
	public InsightsPage()
	{
		InitializeComponent();
	}

	private void ContentPage_Loaded(object sender, EventArgs e)
	{
		BindingContext = Handler.MauiContext.Services.GetService<InsightsViewModel>();
	}
}