using CommunityToolkit.Mvvm.ComponentModel;
using Ona.App.Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Today
{
    public class TodayViewModel : ObservableObject
    {
        public TodayViewModel(CalendarViewModel calendarViewModel)
        {
            CalendarViewModel = calendarViewModel;
        }

        public CalendarViewModel CalendarViewModel { get; }
    }
}
