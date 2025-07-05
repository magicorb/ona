namespace Ona.Main.Model;

public class PeriodStats
{
    public PeriodStats(int duration, int interval)
    {
        Duration = duration;
        Interval = interval;
    }

    public int Duration { get; }

    public int Interval { get; }
}
