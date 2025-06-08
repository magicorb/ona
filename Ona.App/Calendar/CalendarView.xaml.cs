using Ona.App.Model;

namespace Ona.App.Calendar;

public partial class CalendarView : ContentView
{
	public CalendarView()
	{
		InitializeComponent();

		BindingContext = new CalendarViewModel(new DateTimeProvider());
	}
}