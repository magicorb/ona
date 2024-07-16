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
			if (orderedPeriods.Count == 1)
			{
				var datePeriod = orderedPeriods[0];

				return new PeriodStats
				{
					Duration = GetDays(datePeriod),
					Interval = DefaultInterval
				};
			}

			var intervals = new List<int>();
			for (var i = 0; i < orderedPeriods.Count - 1; i++)
				intervals.Add((orderedPeriods[i + 1].Start - orderedPeriods[i].Start).Days);

			var result = new PeriodStats()
			{
				Duration = (int)Math.Round(orderedPeriods.Average(GetDays), MidpointRounding.AwayFromZero),
				Interval = (int)Math.Round(intervals.Average(), MidpointRounding.AwayFromZero)
			};

			return result;

		}

		private int GetDays(DateTimePeriod period)
			=> (period.End - period.Start).Days + 1;

		private IEnumerable<DateTimePeriod> GetExpectedPeriods(IReadOnlyList<DateTimePeriod> orderedPeriods)
		{
			if (orderedPeriods.Count == 0)
				yield break;

			var average = GetAveragePeriodStats(orderedPeriods);
			var interval = average.Interval == null
				? DefaultInterval
				: average.Interval.Value;
			var duration = average.Duration.Value - 1;

			var start = orderedPeriods.Last().Start.AddDays(interval);
			do
			{
				yield return new DateTimePeriod
				{
					Start = start.Date,
					End = start.AddDays(duration).Date
				};
				start = start.AddDays(interval);
			} while (true);
		}
	}
}
