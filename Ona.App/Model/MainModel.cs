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
	public class MainModel : IMainModel
	{
		private readonly IDateRepository dateRepository;
		private readonly IPeriodStatsProvider periodStatsProvider;

		private Task? initializeTask;

		private List<DateTime> markedDates = null!;
		private IReadOnlyList<DateTimePeriod>? markedPeriods;
		private List<DateTimePeriod>? expectedPeriods;
		private PeriodStats? currentStats;
		private PeriodStats? previousStats;
		private DateTime? endDate;

		public MainModel(
			IDateRepository dateRepository,
			IPeriodStatsProvider periodStatsProvider)
		{
			this.dateRepository = dateRepository;
			this.periodStatsProvider = periodStatsProvider;
		}

		public IReadOnlyList<DateTime> MarkedDates => this.markedDates;

		public DateTime ObservedEnd
		{
			get => this.endDate!.Value;
			set
			{
				this.endDate = value;

				OnDatesChanged();
			}
		}

		public async Task InitializeAsync()
		{
			this.initializeTask = InitializeInternalAsync();
			await this.initializeTask;
		}

		public async Task OnInitializedAsync()
			=> await this.initializeTask!;

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

			await this.dateRepository.AddDateRecordAsync(date);

			OnDatesChanged();
		}

		public async Task DeleteDateAsync(DateTime date)
		{
			this.markedDates.Remove(date);

			await this.dateRepository.DeleteDateRecordAsync(date);

			OnDatesChanged();
		}

		public IReadOnlyList<DateTimePeriod> MarkedPeriods
		{
			get
			{
				if (this.markedPeriods == null)
					this.markedPeriods = this.periodStatsProvider.GetDatePeriods(this.markedDates.Select(d => d.Date));

				return this.markedPeriods!;
			}
		}

		public IReadOnlyList<DateTimePeriod> ExpectedPeriods
		{
			get
			{
				if (this.expectedPeriods == null)
					UpdateExpectedPeriods();

				return this.expectedPeriods!;
			}
		}

		public PeriodStats CurrentStats
		{
			get
			{
				if (this.currentStats == null)
					this.currentStats = this.periodStatsProvider.GetAveragePeriodStats(MarkedPeriods);
				return this.currentStats;
			}
		}

		public PeriodStats PreviousStats
		{
			get
			{
				if (this.previousStats == null)
					this.previousStats = this.periodStatsProvider.GetAveragePeriodStats(MarkedPeriods.Take(MarkedPeriods.Count - 1).ToList());
				return this.previousStats;
			}
		}

		public async Task DeleteAllAsync()
		{
			await this.dateRepository.DeleteAllDateRecordsAsync();
			this.markedDates.Clear();

			OnDatesChanged();
		}

		public async Task ImportAsync(IEnumerable<DateTime> dates)
		{
			await this.dateRepository.DeleteAllDateRecordsAsync();

			foreach (var date in dates)
				await this.dateRepository.AddDateRecordAsync(date);
			
			await InitializeInternalAsync();
			
			OnDatesChanged();
		}

		private async Task InitializeInternalAsync()
		{
			this.markedDates = (await this.dateRepository.GetDateRecordsAsync()).Select(d => d.Date).ToList();
		}

		private void UpdateExpectedPeriods()
		{
			var periods = MarkedPeriods;

			var expectedPeriodsEnumerator = this.periodStatsProvider.GetExpectedPeriodsEnumerator(periods);

			this.expectedPeriods = new List<DateTimePeriod>();
			
			DateTimePeriod? expectedCurrentPeriod = null;

			if (periods.Count > 1)
			{
				var previousPeriods = new List<DateTimePeriod>(periods.Take(periods.Count - 1));

				var previousExpectedPeriodsEnumerator = this.periodStatsProvider.GetExpectedPeriodsEnumerator(previousPeriods);

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

				if (period.Start > ObservedEnd)
					break;

				this.expectedPeriods.Add(period);
			}
		}

		private void OnDatesChanged()
		{
			this.expectedPeriods = null;
			this.markedPeriods = null;
			this.currentStats = null;
			this.previousStats = null;
		}
	}
}
