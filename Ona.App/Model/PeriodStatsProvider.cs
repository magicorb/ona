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
		private const int DefaultDuration = 5;
		private const int DefaultInterval = 28;

		public IEnumerator<DateTimePeriod> GetExpectedPeriodsEnumerator(IReadOnlyList<DateTimePeriod> orderedPeriods)
			=> GetExpectedPeriods(orderedPeriods).Memorize();

		public IReadOnlyList<DateTimePeriod> GetDatePeriods(IEnumerable<DateTime> orderedDates)
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

		public PeriodStats GetAveragePeriodStats(IReadOnlyList<DateTimePeriod> orderedPeriods)
		{
			if (orderedPeriods.Count == 0)
				return new PeriodStats(duration: DefaultDuration, interval: DefaultInterval);

			if (orderedPeriods.Count == 1)
				return new PeriodStats(duration: GetDays(orderedPeriods[0]), interval: DefaultInterval);

			var intervals = new List<int>();
			for (var i = 0; i < orderedPeriods.Count - 1; i++)
				intervals.Add((orderedPeriods[i + 1].Start - orderedPeriods[i].Start).Days);

			var result = new PeriodStats(
				duration: (int)Math.Round(orderedPeriods.Average(GetDays), MidpointRounding.AwayFromZero),
				interval: (int)Math.Round(intervals.Average(), MidpointRounding.AwayFromZero));

			return result;
		}

		private int GetDays(DateTimePeriod period)
			=> (period.End - period.Start).Days + 1;

		private IEnumerable<DateTimePeriod> GetExpectedPeriods(IReadOnlyList<DateTimePeriod> orderedPeriods)
		{
			if (orderedPeriods.Count == 0)
				yield break;

			var average = GetAveragePeriodStats(orderedPeriods);
			var durationDayDifference = average.Duration - 1;

			var start = orderedPeriods.Last().Start.AddDays(average.Interval);
			do
			{
				yield return new DateTimePeriod
				{
					Start = start.Date,
					End = start.AddDays(durationDayDifference).Date
				};
				start = start.AddDays(average.Interval);
			} while (true);
		}
	}
}
