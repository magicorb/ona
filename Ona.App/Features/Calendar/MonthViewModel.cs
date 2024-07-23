using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Ona.App.Model;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Features.Calendar
{
	public class MonthViewModel : ObservableObject
	{
		private readonly DateViewModelFactory dateViewModelFactory;

		private bool isVisible = true;

		public MonthViewModel(
			DateViewModelFactory dateViewModelFactory,
			int year,
			int month,
			int currentYear)
		{
			this.dateViewModelFactory = dateViewModelFactory;

			Year = year;
			Month = month;

			var monthName = MonthStart.ToString("MMMM", CultureInfo.CreateSpecificCulture("en-us"));
			Title = year == currentYear ? monthName : $"{monthName} {year}";

			DaysOfWeek = CultureInfo.CurrentUICulture.DateTimeFormat.AbbreviatedDayNames;

			Dates = GenerateDates();
		}

		public int Year { get; }

		public int Month { get; }

		public string Title { get; }

		public IEnumerable<string> DaysOfWeek { get; }

		public ReadOnlyCollection<DateViewModel> Dates { get; }

		public IEnumerable<DateViewModel> MonthDates
			=> Dates.Where(d => d.IsCurrentMonth);

		private ReadOnlyCollection<DateViewModel> GenerateDates()
			=> new ReadOnlyCollection<DateViewModel>(
				MonthStart.StartOfWeek(CultureInfo.CurrentUICulture.DateTimeFormat.FirstDayOfWeek)
					.DateRange(MonthStart.AddMonths(1).AddDays(-1))
					.Select(d => dateViewModelFactory(d, this, Year, Month))
					.ToList());

		public DateTime MonthStart
			=> new DateTime(Year, Month, 1);

		public bool IsVisible { get => isVisible; set => SetProperty(ref isVisible, value); }
	}

	public delegate MonthViewModel MonthViewModelFactory(int year, int month, int currentYear);
}
