using Ona.App.Features.Calendar;

namespace Ona.App.Features.Today;

public partial class TodayPage : ContentPage
{
	private bool isScrollingInitialized;

	public TodayPage()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		await ViewModel.InititalizeAsync();
	}

	private void CalendarView_SizeChanged(object sender, EventArgs e)
	{
		var areTitlesLoaded = ViewModel.Title != null && ViewModel.Subtitle != null;
		if (!this.isScrollingInitialized && areTitlesLoaded)
		{
			this.isScrollingInitialized = true;
			Dispatcher.Dispatch(async () => await ((CalendarView)sender).InitializeScrollingAsync());
		}
	}

	private TodayViewModel ViewModel
		=> (TodayViewModel)BindingContext;
}