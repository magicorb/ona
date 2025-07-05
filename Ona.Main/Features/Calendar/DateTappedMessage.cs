namespace Ona.Main.Features.Calendar;

public class DateTappedMessage
{
    public DateTappedMessage(DateTime date)
    {
        Date = date;
    }

    public DateTime Date { get; }
}
