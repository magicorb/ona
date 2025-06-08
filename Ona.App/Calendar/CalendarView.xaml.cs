using CommunityToolkit.Mvvm.Messaging;
using Ona.App.Model;

namespace Ona.App.Calendar;

public partial class CalendarView : ContentView
{
	private bool isLoading;
	private double scrollY;

	public CalendarView()
	{
		InitializeComponent();

		this.isLoading = true;
		Dispatcher.DispatchDelayed(
			TimeSpan.FromSeconds(5),
			() => _ = MonthListViewLite.ScrollToIndexAsync(2, ScrollToPosition.End, false));
		Dispatcher.DispatchDelayed(
			TimeSpan.FromSeconds(4),
			() => this.isLoading = false);
	}

	private CalendarViewModel ViewModel
		=> (CalendarViewModel)BindingContext;

	private void ListViewLite_Scrolled(object sender, ScrolledEventArgs e)
	{
		Dispatcher.Dispatch(() =>
		{
			var delta = e.ScrollY - this.scrollY;
			this.scrollY = e.ScrollY;

			var curentMonthIndex = ViewModel.Months.IndexOf(ViewModel.CurentMonth);

			if (delta > 0)
			{
				for (var i = curentMonthIndex + 1; i < ViewModel.Months.Count; i++)
					ViewModel.Months[i].IsVisible = true;
			}
			else if (delta < 0)
			{
				for (var i = curentMonthIndex - 1; i >= 0; i--)
					ViewModel.Months[i].IsVisible = true;
			}

			if (e.ScrollY <= 1)
			{
				if (this.isLoading)
					return;
				
				this.isLoading = true;
				ViewModel.InsertMonth();
				this.isLoading = false;
			}
			else if (e.ScrollY >= MonthListViewLite.ContentHeight - MonthListViewLite.Height - 1)
			{
				if (this.isLoading)
					return;

				this.isLoading = true;
				ViewModel.AppendMonth();
				this.isLoading = false;
			}
		});
	}
}