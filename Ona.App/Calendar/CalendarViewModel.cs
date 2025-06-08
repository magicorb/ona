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
			var currentMonth = new MonthViewModel(this.dateTimeProvider, now.Year, now.Month);
			this.months.Add(currentMonth);
			InsertMonth();
			InsertMonth();
			AppendMonth();

			foreach (var month in Months)
				month.IsVisible = false;
		}

		public ReadOnlyObservableCollection<MonthViewModel> Months { get; }

		internal void AppendMonth()
		{
			var monthStart = Months[Months.Count - 1].MonthStart.AddMonths(1);
			this.months.Add(new MonthViewModel(this.dateTimeProvider, monthStart.Year, monthStart.Month));
		}

		internal void InsertMonth()
		{
			var monthStart = Months[0].MonthStart.AddMonths(-1);
			this.months.Insert(0, new MonthViewModel(this.dateTimeProvider, monthStart.Year, monthStart.Month));
		}

		internal void ShowAllMonths()
		{
			foreach (var month in Months)
				month.IsVisible = true;
		}
	}
}
