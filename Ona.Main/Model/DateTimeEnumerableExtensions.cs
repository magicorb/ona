using Ona.Main.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.Main.Model;

public static class DateTimeEnumerableExtensions
{
	public static IReadOnlyList<DateTimePeriod> GetDatePeriods(this IEnumerable<DateTime> orderedDates)
	{
		var periods = new List<DateTimePeriod>();

		var enumerator = orderedDates.GetEnumerator();

		if (!enumerator.MoveNext())
			return periods;

		var currentPeriod = new DateTimePeriod()
		{
			Start = enumerator.Current.Date,
			End = enumerator.Current.Date
		};
		periods.Add(currentPeriod);

		while (enumerator.MoveNext())
		{
			if (enumerator.Current == currentPeriod.End.AddDays(1))
				currentPeriod.End = enumerator.Current;
			else
			{
				currentPeriod = new DateTimePeriod()
				{
					Start = enumerator.Current.Date,
					End = enumerator.Current.Date
				};
				periods.Add(currentPeriod);
			}
		}

		return periods;
	}
}
