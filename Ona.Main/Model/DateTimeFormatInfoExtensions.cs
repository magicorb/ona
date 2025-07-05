using System.Globalization;

namespace Ona.Main.Model;

public static class DateTimeFormatInfoExtensions
{
    public static IEnumerable<string> GetAbbreviatedDayNames(this DateTimeFormatInfo formatInfo)
    {
        var firstIndex = (int)formatInfo.FirstDayOfWeek;
        for (var i = 0; i < 7; i++)
        {
            var index = (i + firstIndex) % 7;
            yield return formatInfo.GetAbbreviatedDayName((DayOfWeek)index);
        }
    }
}
