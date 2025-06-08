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

	private async void CalendarView_SizeChanged(object sender, EventArgs e)
	{
		if (!this.isScrollingInitialized)
		{
			await ViewModel.OnInitializedAsync();
			this.isScrollingInitialized = true;
			((CalendarView)sender).InitializeScrolling();
		}
	}

	private TodayViewModel ViewModel
		=> (TodayViewModel)BindingContext;
}