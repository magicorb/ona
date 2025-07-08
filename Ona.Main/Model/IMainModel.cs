namespace Ona.Main.Model;

public interface IMainModel
{
    IReadOnlyList<DateTime> MarkedDates { get; }

    DateTime ObservedEnd { get; set; }

    IList<IList<DateTime>> Cycles { get; }

    IReadOnlyList<DateTimePeriod> ExpectedPeriods { get; }

    int ExpectedPeriodLength { get; }

    int ExpectedCycleLength { get; }

    Task InitializeAsync();

    Task OnInitializedAsync();

    Task AddDateAsync(DateTime date);

    Task DeleteDateAsync(DateTime date);

    Task DeleteAllAsync();

    Task ImportAsync(IEnumerable<DateTime> dates);
}
