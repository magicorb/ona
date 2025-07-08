using Ona.Main.Data;
using Ona.Main.Environment;
using Ona.Model;

namespace Ona.Main.Model;

public class MainModel : IMainModel
{
    private const int DefaultPeriodLength = 5;
    private const int DefaultCycleLength = 28;

    private readonly IDateRepository dateRepository;
    private readonly IDateTimeProvider dateTimeProvider;
    private Task? initializeTask;

    private List<DateTime> markedDates = null!;
    private List<DateTimePeriod>? expectedPeriods;
    private IList<IList<DateTime>>? cycles;
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

    public IList<IList<DateTime>> Cycles
        => this.cycles
        ?? (this.cycles = this.markedDates.GetCycles());

    public IReadOnlyList<DateTimePeriod> ExpectedPeriods
        => this.expectedPeriods
        ?? (this.expectedPeriods = GetExpectedPeriods().ToList());

    public int ExpectedCycleLength
        => this.expectedInterval
        ?? (this.expectedInterval = GetExpectedCycleLength()).Value;

    public int ExpectedPeriodLength
        => this.expectedDuration
        ?? (this.expectedDuration = GetExpectedPeriodLength()).Value;

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

    private int GetExpectedCycleLength()
    {
        if (Cycles.Count <= 1)
            return DefaultCycleLength;

        var cycleLengths = new int[Cycles.Count - 1];
        for (var i = 0; i < Cycles.Count - 1; i++)
            cycleLengths[i] = (Cycles[i + 1][0] - Cycles[i][0]).Days;

        return (int)Math.Round(cycleLengths.Average(), MidpointRounding.AwayFromZero);
    }

    private int GetExpectedPeriodLength()
    {
        if (Cycles.Count == 0)
            return DefaultPeriodLength;

        var lastCycle = Cycles[^1];
        var periodLengths = Cycles.Select(x => x.GetPeriodLength()).ToArray();
        var averagePeriodLength = (int)Math.Round(periodLengths.Average(), MidpointRounding.AwayFromZero);
        var previousAveragePeriodLength = periodLengths.Length > 1
            ? (int)Math.Round(periodLengths.SkipLast(1).Average(), MidpointRounding.AwayFromZero)
            : DefaultPeriodLength;
        
        var isLastPeriodInProgress = lastCycle[0].AddDays(previousAveragePeriodLength - 1) > this.dateTimeProvider.Now.Date;

        return isLastPeriodInProgress ? previousAveragePeriodLength : averagePeriodLength;
    }

    private IEnumerable<DateTimePeriod> GetExpectedPeriods()
    {
        if (Cycles.Count == 0)
            yield break;

        var lastCycle = Cycles.Last();
        var isLastPeriodInProgress = lastCycle[0].AddDays(ExpectedPeriodLength - 1) > this.dateTimeProvider.Now.Date;

        for (var start = isLastPeriodInProgress ? lastCycle[0] : lastCycle[0].AddDays(ExpectedCycleLength);
            start <= ObservedEnd;
            start = start.AddDays(ExpectedCycleLength))
        {
            yield return new DateTimePeriod
            {
                Start = start,
                End = start.AddDays(ExpectedPeriodLength - 1)
            };
        }
    }

    private void OnDatesChanged()
    {
        this.expectedPeriods = null;
        this.cycles = null;
        this.expectedDuration = null;
        this.expectedInterval = null;
    }
}
