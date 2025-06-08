namespace Ona.App.Features.Today;

public partial class TodayPage : ContentPage
{
	public TodayPage()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		await ((TodayViewModel)BindingContext).InititalizeAsync();
	}
}