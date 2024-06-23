using CommunityToolkit.Mvvm.Messaging;
using Ona.App.Model;

namespace Ona.App.Calendar;

public partial class CalendarView : ContentView
{
	private bool isLoading;

	public CalendarView()
	{
		InitializeComponent();

		this.isLoading = true;
		Dispatcher.DispatchDelayed(
			TimeSpan.FromSeconds(2),
			() =>
			{
				MonthCollectionView.ScrollTo(2, -1, ScrollToPosition.End, false);
				ViewModel.Months[2].IsVisible = true;
			});
		Dispatcher.DispatchDelayed(
			TimeSpan.FromSeconds(4),
			() => this.isLoading = false);
	}

	private CalendarViewModel ViewModel
		=> (CalendarViewModel)BindingContext;

	private void CollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
	{
		if (!this.isLoading)
		{
			var curentMonthIndex = ViewModel.Months.IndexOf(ViewModel.CurentMonth);

			if (e.VerticalDelta > 0)
			{
				for (var i = curentMonthIndex + 1; i < ViewModel.Months.Count; i++)
					ViewModel.Months[i].IsVisible = true;
			}
			else if (e.VerticalDelta < 0)
			{
				for (var i = curentMonthIndex - 1; i > 0; i--)
					ViewModel.Months[i].IsVisible = true;
			}
		}

		if (e.LastVisibleItemIndex == ViewModel.Months.Count - 1)
			AppendMonth();
		else if (e.FirstVisibleItemIndex == 0)
			InsertMonth();
	}

	private void AppendMonth()
	{
		Dispatcher.Dispatch(() =>
		{
			if (this.isLoading)
				return;
			this.isLoading = true;
			ViewModel.AppendMonth();
			this.isLoading = false;
		});
	}

	private void InsertMonth()
	{
		Dispatcher.Dispatch(() =>
		{
			if (this.isLoading)
				return;
			this.isLoading = true;
			ViewModel.InsertMonth();
			this.isLoading = false;
		});
	}
}