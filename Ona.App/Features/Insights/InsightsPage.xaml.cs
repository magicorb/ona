namespace Ona.App.Features.Insights;

public partial class InsightsPage : ContentPage
{
	public InsightsPage()
	{
		InitializeComponent();
	}

	private async void ContentPage_Appearing(object sender, EventArgs e)
	{
		await ((InsightsViewModel)BindingContext).RefreshAsync();
	}
}