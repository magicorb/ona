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
		private readonly int year;
		private readonly int month;

		public MonthViewModel(int year, int month)
		{
			this.year = year;
			this.month = month;
		}

		public string MonthName
			=> new DateTime(this.year, this.month, 1).ToString("MMMM", CultureInfo.CreateSpecificCulture("en-us"));
	}
}
