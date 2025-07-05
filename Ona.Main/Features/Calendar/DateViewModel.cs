using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Ona.Main.Model;
using System.Windows.Input;

namespace Ona.Main.Features.Calendar;

public class DateViewModel : ObservableObject
{
    private readonly IDateTimeProvider dateTimeProvider;
    private readonly IMessenger messenger;

    private bool isMarked;
    private bool isExpected;

    public DateViewModel(
        IDateTimeProvider dateTimeProvider,
        IMessenger messenger,
        DateTime date,
        MonthViewModel monthViewModel,
        int currentYear,
        int currentMonth)
    {
        this.dateTimeProvider = dateTimeProvider;
        this.messenger = messenger;

        Date = date;
        MonthViewModel = monthViewModel;
        IsCurrentMonth = date.Year == currentYear && date.Month == currentMonth;

        var today = this.dateTimeProvider.Now.Date;

        IsPast = Date.Date < today;
        IsFuture = Date.Date > today;

        TapCommand = new RelayCommand(ExecuteTap);
    }

    public DateTime Date { get; }

    public MonthViewModel MonthViewModel { get; }

    public bool IsPast { get; }

    public bool IsFuture { get; }

    public bool IsMarked { get => isMarked; set => SetProperty(ref isMarked, value); }

    public bool IsExpected { get => isExpected; set => SetProperty(ref isExpected, value); }

    public bool IsCurrentMonth { get; }

    public ICommand TapCommand { get; }

    private void ExecuteTap()
        => messenger.Send(new DateTappedMessage(Date));
}

public delegate DateViewModel DateViewModelFactory(DateTime date, MonthViewModel monthViewModel, int currentYear, int currentMonth);
