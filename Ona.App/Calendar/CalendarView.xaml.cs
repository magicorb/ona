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
		Dispatcher.DispatchDelayed(TimeSpan.FromSeconds(4), () => this.isLoading = false);
	}

	private void CollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
	{
		var collectionView = (CollectionView)sender;

		if (e.LastVisibleItemIndex >= this.viewModel.Months.Count - 2)
			AppendMonth();
		else if (e.FirstVisibleItemIndex <= 1)
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