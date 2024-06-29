using Ona.App.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Model
{
	public class PeriodStatsProvider : IPeriodStatsProvider
	{
		public DateTimePeriod? GetNextPeriod(DateTime[] orderedDates)
		{
			if (orderedDates.Length == 0)
				return null;

			var periods = GetDatePeriods(orderedDates);
			var average = GetAveragePeriodStats(periods);
			var interval = Math.Round(average.Interval.Value, MidpointRounding.AwayFromZero);
			var duration = Math.Round(average.Duration.Value, MidpointRounding.AwayFromZero);

			var start = periods.Last().Start.AddDays(interval).Date;
			return new DateTimePeriod
			{
				Start = start,
				End = start.AddDays(duration).Date
			};
		}

		private IReadOnlyList<DateTimePeriod> GetDatePeriods(IEnumerable<DateTime> orderedDates)
		{
			var periods = new List<DateTimePeriod>();

			var enumerator = orderedDates.GetEnumerator();

			enumerator.MoveNext();
			var currentPeriod = new DateTimePeriod()
			{
				Start = enumerator.Current.Date,
				End = enumerator.Current
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
						End = enumerator.Current
					};
					periods.Add(currentPeriod);
				}
			}

			return periods;
		}

		private int GetDays(DateTimePeriod period)
			=> (period.End - period.Start).Days;

		public PeriodStats GetAveragePeriodStats(IReadOnlyList<DateTimePeriod> orderedPeriods)
		{
			if (orderedPeriods.Count == 1)
			{
				var datePeriod = orderedPeriods[0];

				return new PeriodStats
				{
					Duration = GetDays(datePeriod),
				};
			}

			var intervals = new List<int>();
			for (var i = 0; i < orderedPeriods.Count - 1; i++)
				intervals.Add((orderedPeriods[i + 1].Start - orderedPeriods[i].Start).Days);

			var result = new PeriodStats()
			{
				Duration = orderedPeriods.Average(GetDays),
				Interval = intervals.Average()
			};

			return result;

		}
	}
}
