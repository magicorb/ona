using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Model
{
	public static class DateTimeExtensions
	{
		public static DateTime StartOfWeek(this DateTime dateTime, DayOfWeek startOfWeekDay)
		{
			int diff = (7 + (dateTime.DayOfWeek - startOfWeekDay)) % 7;
			return dateTime.AddDays(-1 * diff).Date;
		}

		public static IEnumerable<DateTime> DateRange(this DateTime first, DateTime last)
		{
			for (var date = first; date <= last; date = date.AddDays(1))
				yield return date;

		}
	}
}
