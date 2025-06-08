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
		private const int DefaultDuration = 5;
		private const int DefaultInterval = 28;

		private readonly IDateRepository dateRepository;
		private readonly IDateTimeProvider dateTimeProvider;
		private Task? initializeTask;

		private List<DateTime> markedDates = null!;
		private IReadOnlyList<DateTimePeriod>? markedPeriods;
		private List<DateTimePeriod>? expectedPeriods;
		private int? averageDuration;
		private int? averageInterval;
		private DateTime? endDate;

		public MainModel(
			IDateRepository dateRepository,
			IDateTimeProvider dateTimeProvider)
		{
			this.dateRepository = dateRepository;
			this.dateTimeProvider = dateTimeProvider;
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
					this.markedPeriods = this.markedDates.Select(d => d.Date).GetDatePeriods();

				return this.markedPeriods!;
			}
		}

		public IReadOnlyList<DateTimePeriod> ExpectedPeriods
			=> this.expectedPeriods
			?? (this.expectedPeriods = GetExpectedPeriods().ToList());

		public int AverageInterval
			=> this.averageInterval
			?? (this.averageInterval = GetAverageInterval()).Value;

		public int AverageDuration
			=> this.averageDuration
			?? (this.averageDuration = GetAverageDuration()).Value;

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

		private int GetAverageInterval()
		{
			var intervals = new List<int>();
			for (var i = 0; i < MarkedPeriods.Count - 1; i++)
				intervals.Add((MarkedPeriods[i + 1].Start - MarkedPeriods[i].Start).Days);

			return intervals.Any()
				? (int)Math.Round(intervals.Average(), MidpointRounding.AwayFromZero)
				: DefaultInterval;
		}

		private int GetAverageDuration()
		{
			if (MarkedPeriods.Count == 0)
				return DefaultDuration;

			if (MarkedPeriods.Count == 1)
				return MarkedPeriods[0].Days;

			var previousAverageDuration = GetAverageDuration(MarkedPeriods.Take(MarkedPeriods.Count - 1));
			var lastPeriod = MarkedPeriods.Last();
			var expectedLastPeriodEnd = lastPeriod.Start.AddDays(previousAverageDuration - 1);
			var ignoreLastPeriod =
				lastPeriod.End < expectedLastPeriodEnd
				&& this.dateTimeProvider.Now.Date <= expectedLastPeriodEnd;

			return ignoreLastPeriod
				? previousAverageDuration
				: GetAverageDuration(MarkedPeriods);
		}

		private IEnumerable<DateTimePeriod> GetExpectedPeriods()
		{
			if (MarkedPeriods.Count == 0)
				yield break;

			var lastPeriod = MarkedPeriods.Last();

			var previousAverageDuration = MarkedPeriods.Count > 1
				? GetAverageDuration(MarkedPeriods.Take(MarkedPeriods.Count - 1))
				: DefaultDuration;

			yield return new DateTimePeriod
			{
				Start = lastPeriod.Start,
				End = lastPeriod.Start.AddDays(previousAverageDuration - 1)
			};

			var start = lastPeriod.Start.AddDays(AverageInterval);
			var durationDiff = AverageDuration - 1;
			do
			{
				yield return new DateTimePeriod
				{
					Start = start.Date,
					End = start.AddDays(durationDiff).Date
				};
				start = start.AddDays(AverageInterval);
			} while (start <= ObservedEnd);
		}
		
		public int GetAverageDuration(IEnumerable<DateTimePeriod> orderedPeriods)
			=> orderedPeriods.Any()
			? (int)Math.Round(orderedPeriods.Average(p => p.Days), MidpointRounding.AwayFromZero)
			: DefaultDuration;

		private void OnDatesChanged()
		{
			this.expectedPeriods = null;
			this.markedPeriods = null;
			this.averageDuration = null;
			this.averageInterval = null;
		}
	}
}
