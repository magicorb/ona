namespace Ona.Model;

public static class DateTimeEnumerableExtensions
{
    public static IList<IList<DateTime>> GetCycles(this IEnumerable<DateTime> orderedDates)
    {
        var cycles = new List<IList<DateTime>>();
        
        var enumerator = orderedDates.GetEnumerator();

        if (!enumerator.MoveNext())
            return cycles;

        var currentCycle = new List<DateTime>();
        cycles.Add(currentCycle);
        currentCycle.Add(enumerator.Current);
        var previousDate = enumerator.Current;

        while (enumerator.MoveNext())
        {
            if (IsNewCycleStart(enumerator.Current, currentCycle[0], previousDate))
            {
                currentCycle = new List<DateTime>();
                cycles.Add(currentCycle);
            }
            currentCycle.Add(enumerator.Current);
            previousDate = enumerator.Current;
        }

        return cycles;
    }

    public static int GetPeriodLength(this IEnumerable<DateTime> cycleDates)
    {
        const int MinPeriodGap = 3;
        const int DefaultPeriodLength = 5;

        var enumerator = cycleDates.GetEnumerator();

        if (!enumerator.MoveNext())
            return DefaultPeriodLength;

        var previousDate = enumerator.Current;

        int length = 1;

        while (enumerator.MoveNext())
        {
            var dayDiff = (enumerator.Current - previousDate).Days;
            if (dayDiff > MinPeriodGap)
                break;

            previousDate = enumerator.Current;

            length += dayDiff;
        }

        return length;
    }

    public static bool IsNewCycleStart(DateTime currentDate, DateTime currentCycleStartDate, DateTime previousDate)
    {
        const int MinCycleLength = 17;
        const int MinCycleGap = 4;

        return (currentDate - currentCycleStartDate).Days >= MinCycleLength
                && (currentDate - previousDate).Days >= MinCycleGap;
    }
}
