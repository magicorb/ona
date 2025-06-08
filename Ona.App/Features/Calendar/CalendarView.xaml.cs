using CommunityToolkit.Mvvm.Messaging;
using Ona.App.Controls;
using Ona.App.Features.Calendar;
using Ona.App.Model;

namespace Ona.App.Features.Calendar;

public partial class CalendarView : ContentView
{
	private double scrollY;

	public CalendarView()
	{
		InitializeComponent();
	}

	private CalendarViewModel ViewModel
		=> (CalendarViewModel)BindingContext;

	private void MonthListViewLite_FirstScrolled(object sender, ScrolledEventArgs e)
	{
		Dispatcher.Dispatch(() =>
		{
			ViewModel.ShowHiddenMonths();

			MonthListViewLite.Scrolled -= MonthListViewLite_FirstScrolled;
			MonthListViewLite.Scrolled += MonthListViewLite_Scrolled;
		});
	}

	private void MonthListViewLite_Scrolled(object sender, ScrolledEventArgs e)
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
		if (e.Index == 1)
			MonthListViewLite.ScrollToOffsetAsync(MonthListViewLite.ScrollY + ((View)e.Item).DesiredSize.Height, false).Wait();
	}

	private void ContentView_Loaded(object sender, EventArgs e)
	{
		Dispatcher.Dispatch(async () =>
		{
			await MonthListViewLite.ScrollToIndexAsync(3, ScrollToPosition.End, false);
			MonthListViewLite.Scrolled += MonthListViewLite_FirstScrolled;
		});
	}

	private void ContentView_Unloaded(object sender, EventArgs e)
	{
		ViewModel?.Dispose();
	}
}