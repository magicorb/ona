using CommunityToolkit.Mvvm.Messaging;
using Ona.App.Features.Calendar;
using Ona.App.Model;

namespace Ona.App.Features.Calendar;

public partial class CalendarView : ContentView
{
	private bool isLoading;
	private double scrollY;

	public CalendarView()
	{
		InitializeComponent();

		this.isLoading = true;
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
				_ = ViewModel.InsertMonthAsync();
				this.isLoading = false;
			}
			else if (e.ScrollY >= MonthListViewLite.ContentHeight - MonthListViewLite.Height - 1)
			{
				if (this.isLoading)
					return;

				this.isLoading = true;
				_ = ViewModel.AppendMonthAsync();
				this.isLoading = false;
			}
		});
	}

	private void ContentView_Loaded(object sender, EventArgs e)
	{
		Dispatcher.Dispatch(async () =>
		{
			await MonthListViewLite.ScrollToIndexAsync(2, ScrollToPosition.End, false);
			this.isLoading = false;
		});
	}

	private void ContentView_Unloaded(object sender, EventArgs e)
	{
		ViewModel?.Dispose();
	}
}