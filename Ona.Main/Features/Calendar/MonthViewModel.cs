using CommunityToolkit.Mvvm.ComponentModel;
using Ona.Main.Environment;
using Ona.Main.Model;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

namespace Ona.Main.Features.Calendar;

public class MonthViewModel : ObservableObject
{
    private readonly ICultureInfoProvider cultureProvider;
    private readonly DateViewModelFactory dateViewModelFactory;

    private bool isVisible;

    public MonthViewModel(
        ICultureInfoProvider cultureProvider,
        DateViewModelFactory dateViewModelFactory,
        int year,
        int month,
        int currentYear)
    {
        this.cultureProvider = cultureProvider;
        this.dateViewModelFactory = dateViewModelFactory;

        Year = year;
        Month = month;

        var monthName = MonthStart.ToString("MMMM", CultureInfo.CreateSpecificCulture("en-us"));
        Title = year == currentYear ? monthName : $"{monthName} {year}";

        DaysOfWeek = CultureInfo.CurrentUICulture.DateTimeFormat.GetAbbreviatedDayNames();

        Dates = GenerateDates();
    }

    public int Year { get; }

    public int Month { get; }

    public string Title { get; }

    public IEnumerable<string> DaysOfWeek { get; }

    public ReadOnlyCollection<DateViewModel> Dates { get; }

    public IEnumerable<DateViewModel> MonthDates
        => Dates.Where(d => d.IsCurrentMonth);

    public DateTime MonthStart
        => new DateTime(Year, Month, 1);

    public ICommand? TriggerShowCommand { get; set; }

    public void Show()
    {
        if (this.isVisible)
            return;
        this.isVisible = true;
        TriggerShowCommand?.Execute(CancellationToken.None);
    }

    private ReadOnlyCollection<DateViewModel> GenerateDates()
        => new ReadOnlyCollection<DateViewModel>(
            MonthStart.StartOfWeek(this.cultureProvider.CurrentUICulture.DateTimeFormat.FirstDayOfWeek)
                .DateRange(MonthStart.AddMonths(1).AddDays(-1))
                .Select(d => dateViewModelFactory(d, this, Year, Month))
                .ToList());
}

public delegate MonthViewModel MonthViewModelFactory(int year, int month, int currentYear);
