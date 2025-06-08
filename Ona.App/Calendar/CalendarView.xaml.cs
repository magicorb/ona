using Java.Time;
using Microsoft.Maui.Controls;
using Ona.App.Model;

namespace Ona.App.Calendar;

public partial class CalendarView : ContentView
{
	private bool isLoading;
	private CalendarViewModel viewModel;

	public CalendarView()
	{
		InitializeComponent();

		BindingContext = this.viewModel = new CalendarViewModel(new DateTimeProvider());

		this.isLoading = true;
		Dispatcher.DispatchDelayed(
			TimeSpan.FromSeconds(2),
			() =>
			{
				MonthCollectionView.ScrollTo(2, -1, ScrollToPosition.End, false);
				viewModel.Months[2].IsVisible = true;
			});
		Dispatcher.DispatchDelayed(
			TimeSpan.FromSeconds(4),
			() => this.isLoading = false);
	}

	private void CollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
	{
		if (!this.isLoading)
		{
			var curentMonthIndex = this.viewModel.Months.IndexOf(this.viewModel.CurentMonth);

			if (e.VerticalDelta > 0)
			{
				for (var i = curentMonthIndex + 1; i < this.viewModel.Months.Count; i++)
					this.viewModel.Months[i].IsVisible = true;
			}
			else if (e.VerticalDelta < 0)
			{
				for (var i = curentMonthIndex - 1; i > 0; i--)
					this.viewModel.Months[i].IsVisible = true;
			}
		}

		if (e.LastVisibleItemIndex == this.viewModel.Months.Count - 1)
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
			this.viewModel.AppendMonth();
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
			this.viewModel.InsertMonth();
			this.isLoading = false;
		});
	}
}