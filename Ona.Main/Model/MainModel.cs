using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ona.Main.Data;

namespace Ona.Main.Model;

public class MainModel : IMainModel
{
	private const int DefaultDuration = 5;
	private const int DefaultInterval = 28;

	private readonly IDateRepository dateRepository;
	private readonly IDateTimeProvider dateTimeProvider;
	private Task? initializeTask;

	private List<DateTime> markedDates = null!;
	private HashSet<DateTime> draftDates = new HashSet<DateTime>();
	private IReadOnlyList<DateTimePeriod>? markedPeriods;
	private List<DateTimePeriod>? expectedPeriods;
	private int? expectedDuration;
	private int? expectedInterval;
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

	public async Task AddDraftDateAsync(DateTime date)
	{
		this.draftDates.Add(date);
		await AddDateAsync(date);
	}

	public void CompleteDraftDate(DateTime date)
	{
		this.draftDates.Remove(date);
		OnDatesChanged();
	}

	public IReadOnlyList<DateTimePeriod> MarkedPeriods
		=> this.markedPeriods
		?? (this.markedPeriods = this.markedDates.GetDatePeriods());

	public IReadOnlyList<DateTimePeriod> ExpectedPeriods
		=> this.expectedPeriods
		?? (this.expectedPeriods = GetExpectedPeriods().ToList());

	public int ExpectedInterval
		=> this.expectedInterval
		?? (this.expectedInterval = GetExpectedInterval()).Value;

	public int ExpectedDuration
		=> this.expectedDuration
		?? (this.expectedDuration = GetExpectedDuration()).Value;

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

	private int GetExpectedInterval()
	{
		var intervals = new List<int>();
		for (var i = 0; i < MarkedPeriods.Count - 1; i++)
			intervals.Add((MarkedPeriods[i + 1].Start - MarkedPeriods[i].Start).Days);

		return intervals.Any()
			? (int)Math.Round(intervals.Average(), MidpointRounding.AwayFromZero)
			: DefaultInterval;
	}

	private int GetExpectedDuration()
	{
		var nonDraftPeriods = this.markedDates.Where(d => !IsDraft(d)).GetDatePeriods();

		if (nonDraftPeriods.Count == 0)
			return DefaultDuration;

		var lastPeriod = MarkedPeriods.Last();
		var averageDuration = GetAverageDuration(nonDraftPeriods);

		if (lastPeriod.Dates().All(IsDraft))
			return averageDuration;

		var previousAverageDuration = GetAverageDuration(nonDraftPeriods.SkipLast(1));
		var isLastPeriodInProgress = lastPeriod.Start.AddDays(previousAverageDuration - 1) > this.dateTimeProvider.Now.Date;

		return isLastPeriodInProgress ? previousAverageDuration : averageDuration;
	}

	private IEnumerable<DateTimePeriod> GetExpectedPeriods()
	{
		if (MarkedPeriods.Count == 0)
			yield break;

		var lastPeriod = MarkedPeriods.Last();
		var isLastPeriodInProgress = lastPeriod.Start.AddDays(ExpectedDuration - 1) > this.dateTimeProvider.Now.Date;

		for (var start = isLastPeriodInProgress ? lastPeriod.Start : lastPeriod.Start.AddDays(ExpectedInterval);
			start <= ObservedEnd;
			start = start.AddDays(ExpectedInterval))
		{
			yield return new DateTimePeriod
			{
				Start = start,
				End = start.AddDays(ExpectedDuration - 1)
			};
		}
	}
	
	private int GetAverageDuration(IEnumerable<DateTimePeriod> periods)
		=> periods.Any()
		? (int)Math.Round(periods.Average(p => p.DayCount()), MidpointRounding.AwayFromZero)
		: DefaultDuration;

	private bool IsDraft(DateTime date)
		=> this.draftDates.Contains(date);

	private void OnDatesChanged()
	{
		this.expectedPeriods = null;
		this.markedPeriods = null;
		this.expectedDuration = null;
		this.expectedInterval = null;
	}
}
