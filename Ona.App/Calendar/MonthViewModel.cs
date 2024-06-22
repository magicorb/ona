using Ona.App.Model;
using Ona.App.Mvvm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ona.App.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ona.App.Calendar
{
	public class MonthViewModel : ViewModelBase
	{
		private readonly IDateTimeProvider dateTimeProvider;

		private readonly int year;
		private readonly int month;

		public MonthViewModel(
			IDateTimeProvider dateTimeProvider,
			int year,
			int month)
		{
			this.dateTimeProvider = dateTimeProvider;
			this.year = year;
			this.month = month;

			DaysOfWeek = CultureInfo.CurrentUICulture.DateTimeFormat.AbbreviatedDayNames;

			Dates = GenerateDates();
		}

		public string MonthName
			=> MonthStart.ToString("MMMM", CultureInfo.CreateSpecificCulture("en-us"));

		public IEnumerable<string> DaysOfWeek { get; }

		public IEnumerable<DateViewModel> Dates { get; }

		private IEnumerable<DateViewModel> GenerateDates()
		{
			var firstDate = MonthStart.StartOfWeek(DayOfWeek.Monday);

			return firstDate.DateRange(firstDate.AddMonths(1).AddDays(-1))
				.Select(d => new DateViewModel(d, this.dateTimeProvider))
				.ToArray();
		}

		private DateTime MonthStart
			=> new DateTime(this.year, this.month, 1);
	}
}
