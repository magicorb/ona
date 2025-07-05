using Ona.Main.Controls;
using Ona.Main.Features.Calendar;

namespace Ona.Main.Features.Today;

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

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        if (this.isScrollingInitialized)
            _ = CalendarView.ScrollToCurrentMonthAsync();
    }
}