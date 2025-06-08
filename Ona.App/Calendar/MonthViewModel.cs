using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Ona.App.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Calendar
{
	public class MonthViewModel : ObservableObject
	{
		private readonly DateViewModelFactory dateViewModelFactory;

		private bool isVisible = true;

		public MonthViewModel(
			DateViewModelFactory dateViewModelFactory,
			int year,
			int month)
		{
			this.dateViewModelFactory = dateViewModelFactory;

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

			return firstDate.DateRange(MonthStart.AddMonths(1).AddDays(-1))
				.Select(d => dateViewModelFactory(d, Year, Month))
				.ToArray();
		}

		public DateTime MonthStart
			=> new DateTime(Year, Month, 1);

		public bool IsVisible { get => this.isVisible; set => SetProperty(ref this.isVisible, value); }
	}

	public delegate MonthViewModel MonthViewModelFactory(int year, int month);
}
