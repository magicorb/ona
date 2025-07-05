using Ona.Main.Controls;

namespace Ona.Main.Features.Calendar;

public partial class CalendarView : ContentView
{
    private double scrollY;

    public CalendarView()
    {
        InitializeComponent();

        this.RegisterViewModel();
    }

    public async Task InitializeScrollingAsync()
    {
        await ScrollToCurrentMonthAsync();
        ViewModel.CurentMonth.Show();

        MonthListViewLite.Scrolled += MonthListViewLite_FirstScrolled;
    }

    public async Task ScrollToCurrentMonthAsync()
    {
        var index = ViewModel.Items.IndexOf(ViewModel.CurentMonth);
        await MonthListViewLite.ScrollToIndexAsync(index, ScrollToPosition.End, false);
    }

    private CalendarViewModel ViewModel
        => (CalendarViewModel)BindingContext;

    private void MonthListViewLite_FirstScrolled(object? sender, ScrolledEventArgs e)
    {
        Dispatcher.Dispatch(() =>
        {
            ViewModel.ShowHiddenMonths();

            MonthListViewLite.Scrolled -= MonthListViewLite_FirstScrolled;
            MonthListViewLite.Scrolled += MonthListViewLite_Scrolled;
        });
    }

    private void MonthListViewLite_Scrolled(object? sender, ScrolledEventArgs e)
    {
        Dispatcher.Dispatch(() =>
        {
            var delta = e.ScrollY - this.scrollY;
            this.scrollY = e.ScrollY;

            if (delta < 0 && e.ScrollY <= 1)
                _ = ViewModel.InsertMonthAsync();
            else if (delta > 0 && e.ScrollY >= MonthListViewLite.ContentHeight - MonthListViewLite.Height - 1)
                _ = ViewModel.AppendMonthAsync();
        });
    }

    private void MonthListViewLite_ItemAdded(object sender, ListItemAddedEventArgs e)
    {
        if (e.Index != 1)
            return;

        var view = (View)e.Item;

#if IOS
	var sizeChangeCount = 0;
#endif

        void InsertedItem_SizeChanged(object? sender2, EventArgs e2)
        {
#if IOS
		if (sizeChangeCount == 0)
		{
			sizeChangeCount++;
			return;
		}
#endif
            view.SizeChanged -= InsertedItem_SizeChanged;
            _ = MonthListViewLite.ScrollToOffsetAsync(MonthListViewLite.ScrollY + view.DesiredSize.Height, false);
        }

        view.SizeChanged += InsertedItem_SizeChanged;
    }
}