namespace Ona.App
{
	public partial class MainPage : ContentPage
	{
		int count = 0;

		public MainPage()
		{
			InitializeComponent();
		}

		private void CalendarView_SizeChanged(object sender, EventArgs e)
		{
			_ = ScrollView.ScrollToAsync(CalendarView, ScrollToPosition.End, false);
		}
	}
}