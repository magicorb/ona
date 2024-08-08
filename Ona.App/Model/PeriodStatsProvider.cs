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

		public int GetAverageDuration(IEnumerable<DateTimePeriod> orderedPeriods)
			=> orderedPeriods.Any()
			? (int)Math.Round(orderedPeriods.Average(GetDays), MidpointRounding.AwayFromZero)
			: DefaultDuration;

		public int GetAverageInterval(IReadOnlyList<DateTimePeriod> orderedPeriods)
		{
			var intervals = new List<int>();
			for (var i = 0; i < orderedPeriods.Count - 1; i++)
				intervals.Add((orderedPeriods[i + 1].Start - orderedPeriods[i].Start).Days);

			return intervals.Any()
				? (int)Math.Round(intervals.Average(), MidpointRounding.AwayFromZero)
				: DefaultInterval;
		}

		private int GetDays(DateTimePeriod period)
			=> (period.End - period.Start).Days + 1;

		private IEnumerable<DateTimePeriod> GetExpectedPeriods(IReadOnlyList<DateTimePeriod> orderedPeriods)
		{
			if (orderedPeriods.Count == 0)
				yield break;

			var durationDayDifference = GetAverageDuration(orderedPeriods) - 1;

			var interval = GetAverageInterval(orderedPeriods);

			var start = orderedPeriods.Last().Start.AddDays(interval);
			do
			{
				yield return new DateTimePeriod
				{
					Start = start.Date,
					End = start.AddDays(durationDayDifference).Date
				};
				start = start.AddDays(interval);
			} while (true);
		}
	}
}
