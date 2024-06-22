using Ona.App.Model;
using Ona.App.Mvvm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Calendar
{
	public class MonthViewModel : ViewModelBase
	{
		private readonly IDateTimeProvider dateTimeProvider;

		private bool isVisible = true;

		public MonthViewModel(
			IDateTimeProvider dateTimeProvider,
			int year,
			int month)
		{
			this.dateTimeProvider = dateTimeProvider;
			Year = year;
			Month = month;

			DaysOfWeek = CultureInfo.CurrentUICulture.DateTimeFormat.AbbreviatedDayNames;

			Dates = GenerateDates();
		}

		public int Year { get; }

		public int Month { get; }

		public string MonthName
			=> MonthStart.ToString("MMMM", CultureInfo.CreateSpecificCulture("en-us"));

		public IEnumerable<string> DaysOfWeek { get; }

		public IEnumerable<DateViewModel> Dates { get; }

		private IEnumerable<DateViewModel> GenerateDates()
		{
			var firstDate = MonthStart.StartOfWeek(DayOfWeek.Monday);

			return firstDate.DateRange(firstDate.AddMonths(1).AddDays(-1))
				.Select(d => new DateViewModel(this.dateTimeProvider, d, Year, Month))
				.ToArray();
		}

		public DateTime MonthStart
			=> new DateTime(Year, Month, 1);

		public bool IsVisible { get => this.isVisible; set => SetProperty(ref this.isVisible, value); }
	}
}
