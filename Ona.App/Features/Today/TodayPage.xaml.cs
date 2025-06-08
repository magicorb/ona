using Ona.App.Controls;
using Ona.App.Features.Calendar;

namespace Ona.App.Features.Today;

public partial class TodayPage : ContentPage
{
	private bool isScrollingInitialized;

	public TodayPage()
	{
		InitializeComponent();

		this.RegisterViewModel();
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

	private void ContentPage_Unloaded(object sender, EventArgs e)
	{
		ViewModel.CalendarViewModel.ClearSelection();
	}

	private void ContentPage_Loaded(object sender, EventArgs e)
	{
		if (this.isScrollingInitialized)
			_ = CalendarView.ScrollToCurrentMonthAsync();
	}
}