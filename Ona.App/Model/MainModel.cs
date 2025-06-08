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

		private List<DateTime> markedDates;
		private List<DateTimePeriod>? expectedPeriods;
		private DateTime endDate;

		public MainModel(
			IDateRepository dateRepository,
			IPeriodStatsProvider periodStatsProvider)
		{
			this.dateRepository = dateRepository;
			this.periodStatsProvider = periodStatsProvider;
		}

		public IReadOnlyList<DateTime> MarkedDates => this.markedDates;

		public DateTime EndDate
		{
			get => this.endDate;
			set
			{
				this.endDate = value;

				this.expectedPeriods = null;
			}
		}

		public async Task InitializeAsync()
		{
			this.markedDates = (await dateRepository.GetDateRecordsAsync()).Select(d => d.Date).ToList();
		}

		public async Task AddDateAsync(DateTime date)
		{
			var i = 0;
			foreach (var date2 in this.markedDates)
			{
				if (date2 > date)
					break;
				i++;
			}
			this.markedDates.Insert(i, date);

			this.expectedPeriods = null;

			await this.dateRepository.AddDateRecordAsync(date);
		}

		public async Task DeleteDateAsync(DateTime date)
		{
			this.markedDates.Remove(date);

			this.expectedPeriods = null;

			await this.dateRepository.DeleteDateRecordAsync(date);
		}

		public async Task<IReadOnlyList<DateTimePeriod>> GetExpectedPeriodsAsync()
		{
			if (this.expectedPeriods == null)
				await UpdateExpectedPeriodsAsync();

			return this.expectedPeriods;
		}

		private async Task UpdateExpectedPeriodsAsync()
		{
			var periods = this.periodStatsProvider.GetDatePeriods(this.markedDates.Select(d => d.Date));

			var expectedPeriodsEnumerator = await Task.Run(()
				=> this.periodStatsProvider.GetExpectedPeriodsEnumerator(periods));

			this.expectedPeriods = new List<DateTimePeriod>();
			
			DateTimePeriod? expectedCurrentPeriod = null;

			if (periods.Count > 1)
			{
				var previousPeriods = new List<DateTimePeriod>(periods.Take(periods.Count - 1));

				var previousExpectedPeriodsEnumerator = await Task.Run(()
					=> this.periodStatsProvider.GetExpectedPeriodsEnumerator(previousPeriods));

				if (previousExpectedPeriodsEnumerator.MoveNext())
				{
					expectedCurrentPeriod = previousExpectedPeriodsEnumerator.Current;
					var lastPeriod = periods.Last();

					expectedCurrentPeriod = expectedCurrentPeriod.Start == lastPeriod.Start
						? expectedCurrentPeriod
						: new DateTimePeriod() { Start = lastPeriod.Start, End = lastPeriod.Start.Add(expectedCurrentPeriod.Length) };
				}
				else
					expectedCurrentPeriod = null;
			}
			else
				expectedCurrentPeriod = null;

			if (expectedCurrentPeriod != null)
				this.expectedPeriods.Add(expectedCurrentPeriod);

			expectedPeriodsEnumerator.Reset();
			while (expectedPeriodsEnumerator.MoveNext())
			{
				var period = expectedPeriodsEnumerator.Current;

				if (period.Start > EndDate)
					break;

				this.expectedPeriods.Add(period);
			}
		}
	}
}
