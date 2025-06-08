using CommunityToolkit.Mvvm.ComponentModel;
using Ona.App.Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App
{
	public class MainViewModel : ObservableObject
	{
		public MainViewModel(CalendarViewModel calendarViewModel)
		{
			CalendarViewModel = calendarViewModel;
		}

		public CalendarViewModel CalendarViewModel { get; }
	}
}
