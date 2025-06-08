using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Ona.App.Data;
using Ona.App.Model;


namespace Ona.App.Model
{
	public class MainModel
	{
		private readonly IDateRepository dateRepository;
		private readonly IPeriodStatsProvider periodStatsProvider;

		private IEnumerator<DateTimePeriod> expectedPeriodsEnumerator;
		private DateTimePeriod? expectedCurrentPeriod;

		public MainModel(
			IDateRepository dateRepository,
			IPeriodStatsProvider periodStatsProvider)
		{
			this.dateRepository = dateRepository;
			this.periodStatsProvider = periodStatsProvider;
		}

		public DateTime EndDate { get; set; }

		public IReadOnlyList<DateTimePeriod> Periods { get; private set; }

		public IEnumerator<DateTimePeriod> ExpectedPeriodsEnumerator { get; private set; }

		public async Task UpdateExpectedPeriodsAsync()
		{
			var dates = (await dateRepository.GetDateRecordsAsync()).Select(d => d.Date).ToArray();

			Periods = this.periodStatsProvider.GetDatePeriods(dates.Select(d => d.Date));

			expectedPeriodsEnumerator = await Task.Run(()
				=> periodStatsProvider.GetExpectedPeriodsEnumerator(Periods));

			if (Periods.Count > 1)
			{
				var previousPeriods = new List<DateTimePeriod>(this.Periods.Take(this.Periods.Count - 1));

				var previousExpectedPeriodsEnumerator = await Task.Run(()
					=> periodStatsProvider.GetExpectedPeriodsEnumerator(previousPeriods));

				if (previousExpectedPeriodsEnumerator.MoveNext())
				{
					var expectedCurrentPeriod = previousExpectedPeriodsEnumerator.Current;
					var lastPeriod = this.Periods[this.Periods.Count - 1];

					this.expectedCurrentPeriod = expectedCurrentPeriod.Start == lastPeriod.Start
						? expectedCurrentPeriod
						: new DateTimePeriod() { Start = lastPeriod.Start, End = lastPeriod.Start.Add(expectedCurrentPeriod.Length) };
				}
				else
					this.expectedCurrentPeriod = null;
			}
			else
				this.expectedCurrentPeriod = null;
		}

		public IReadOnlyList<DateTimePeriod> ExpectedPeriods
		{
			get
			{
				var expectedPeriods = new List<DateTimePeriod>();

				if (this.expectedCurrentPeriod != null)
					expectedPeriods.Add(this.expectedCurrentPeriod);

				this.expectedPeriodsEnumerator.Reset();
				while (this.expectedPeriodsEnumerator.MoveNext())
				{
					var period = this.expectedPeriodsEnumerator.Current;

					if (period.Start > EndDate)
						break;

					expectedPeriods.Add(period);
				}

				return expectedPeriods;
			}
		}
	}
}
