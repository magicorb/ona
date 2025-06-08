using Ona.App.Model;
using Ona.App.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Calendar
{
	public class CalendarViewModel : ViewModelBase
	{
		private readonly IDateTimeProvider dateTimeProvider;

		private readonly ObservableCollection<MonthViewModel> months;

		public CalendarViewModel(IDateTimeProvider dateTimeProvider)
		{
			this.dateTimeProvider = dateTimeProvider;

			this.months = new ObservableCollection<MonthViewModel>();
			Months = new ReadOnlyObservableCollection<MonthViewModel>(this.months);

			var now = dateTimeProvider.Now;
			this.months.Add(new MonthViewModel(this.dateTimeProvider, now.Year, now.Month));
		}

		public ReadOnlyObservableCollection<MonthViewModel> Months { get; }
	}
}
