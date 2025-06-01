using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.Main.Model;

public class DateTimePeriod
{
	public DateTime Start { get; set; }

	public DateTime End { get; set; }
}

public static class DateTimePeriodExtensions
{
	public static TimeSpan Length(this DateTimePeriod period) => period.End - period.Start;

	public static int DayCount(this DateTimePeriod period) => (period.End - period.Start).Days + 1;

	public static IEnumerable<DateTime> Dates(this DateTimePeriod period)
	{
		for (var date = period.Start; date <= period.End; date = date.AddDays(1))
			yield return date;
	}
}
