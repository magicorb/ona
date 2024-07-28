using Ona.App.Controls;

namespace Ona.App.Features.Insights;

public partial class InsightsPage : ContentPage
{
	public InsightsPage()
	{
		InitializeComponent();

		this.RegisterViewModel();
	}
}