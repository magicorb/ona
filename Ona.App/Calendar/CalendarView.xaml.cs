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
		Dispatcher.DispatchDelayed(TimeSpan.FromSeconds(2), () => this.isLoading = false);
	}

	private void CollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
	{
		var collectionView = (CollectionView)sender;

		if (e.LastVisibleItemIndex == this.viewModel.Months.Count - 1)
		{
			if (this.isLoading)
				return;
			this.isLoading = true;
			this.viewModel.AppendMonth();
			this.isLoading = false;
		}
		else if (e.FirstVisibleItemIndex == 0)
		{
			if (this.isLoading)
				return;
			this.isLoading = true;
			this.viewModel.InsertMonth();
			this.isLoading = false;
		}
	}
}