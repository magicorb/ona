using CommunityToolkit.Mvvm.Messaging;
using Ona.Main.Controls;
using Ona.Main.Environment;
using Ona.Main.Model;
using System.Collections.ObjectModel;

namespace Ona.Main.Features.Calendar;

public class CalendarViewModel : ViewModelBase, IDisposable
{
    private readonly IDateTimeProvider dateTimeProvider;
    private readonly IDispatcher dispatcher;
    private readonly IMessenger messenger;
    private readonly IMainModel mainModel;
    private readonly MonthViewModelFactory monthViewModelFactory;

    private ObservableCollection<MonthViewModel> months = null!;
    private SpinnerViewModel topSpinner = null!;
    private SpinnerViewModel bottomSpinner = null!;

    public CalendarViewModel(
        IDateTimeProvider dateTimeProvider,
        IDispatcher dispatcher,
        IMessenger messenger,
        IMainModel mainModel,
        MonthViewModelFactory monthViewModelFactory)
    {
        this.dateTimeProvider = dateTimeProvider;
        this.dispatcher = dispatcher;
        this.messenger = messenger;
        this.mainModel = mainModel;
        this.monthViewModelFactory = monthViewModelFactory;

        Initialize();

        this.messenger.Register<CalendarViewModel, DateTappedMessage>(this, (r, m) => _ = r.OnDateTappedMessageAsync(m));
        this.messenger.Register<CalendarViewModel, DatesChangedMessage>(this, (r, m) => _ = r.OnDatesChangedMessageAsync(m));
    }

    public ObservableCollection<object> Items { get; private set; } = null!;

    public MonthViewModel CurentMonth { get; private set; } = null!;

    public void ShowHiddenMonths()
    {
        for (var i = 0; i < this.months.Count; i++)
            this.months[i].Show();
    }

    public void Dispose()
    {
        messenger.UnregisterAll(this);
    }

    internal async Task AppendMonthAsync()
    {
        this.bottomSpinner.IsRunning = true;
        await this.dispatcher.DoEventsAsync();

        var monthStart = this.months.Last().MonthStart.AddMonths(1);
        var newItem = monthViewModelFactory(monthStart.Year, monthStart.Month, this.dateTimeProvider.Now.Year);
        this.months.Add(newItem);
        Items.Insert(Items.Count - 1, newItem);

        this.mainModel.ObservedEnd = this.months.Last().MonthDates.Last().Date;

        await RefreshMarkedDatesAsync();
        await RefreshExpectedDatesAsync();

        newItem.Show();

        this.bottomSpinner.IsRunning = false;
    }

    internal async Task InsertMonthAsync()
    {
        this.topSpinner.IsRunning = true;
        await this.dispatcher.DoEventsAsync();

        var monthStart = this.months[0].MonthStart.AddMonths(-1);
        var newItem = monthViewModelFactory(monthStart.Year, monthStart.Month, this.dateTimeProvider.Now.Year);
        this.months.Insert(0, newItem);
        Items.Insert(1, newItem);

        await RefreshMarkedDatesAsync();
        await RefreshExpectedDatesAsync();

        newItem.Show();

        this.topSpinner.IsRunning = false;
    }

    protected override async Task RefreshAsync()
    {
        await this.mainModel.OnInitializedAsync();

        this.mainModel.ObservedEnd = this.months.Last().MonthDates.Last().Date;

        await RefreshMarkedDatesAsync();
        await RefreshExpectedDatesAsync();
    }

    private void Initialize()
    {
        this.months = new ObservableCollection<MonthViewModel>();

        var now = dateTimeProvider.Now;
        var currentMonthStart = new DateTime(now.Year, now.Month, 1);

        CurentMonth = CreateMonthViewModel(currentMonthStart);

        this.months.Add(CreateMonthViewModel(currentMonthStart.AddMonths(-2)));
        this.months.Add(CreateMonthViewModel(currentMonthStart.AddMonths(-1)));
        this.months.Add(CurentMonth);
        this.months.Add(CreateMonthViewModel(currentMonthStart.AddMonths(1)));

        Items = new ObservableCollection<object>(this.months);

        this.topSpinner = new SpinnerViewModel();
        Items.Insert(0, this.topSpinner);
        this.bottomSpinner = new SpinnerViewModel();
        Items.Add(this.bottomSpinner);
    }

    private MonthViewModel CreateMonthViewModel(DateTime monthStart)
        => monthViewModelFactory(monthStart.Year, monthStart.Month, this.dateTimeProvider.Now.Year);

    private async Task OnDateTappedMessageAsync(DateTappedMessage message)
    {
        var date = message.Date;

        if (date > dateTimeProvider.Now.Date)
            return;

        var dateViewModel = months.First(m => m.Year == date.Year && m.Month == date.Month).Dates.First(d => d.Date == date);

        if (dateViewModel.IsMarked)
        {
            dateViewModel.IsMarked = false;
            await this.mainModel.DeleteDateAsync(dateViewModel.Date);
        }
        else
        {
            var isAdjacent = GetNextDateViewModel(dateViewModel).IsMarked || GetPreviousDateViewModel(dateViewModel).IsMarked;
            if (isAdjacent)
                await MarkDateAsync(dateViewModel);
            else
            {
                var expectedDuration = this.mainModel.ExpectedPeriodLength;
                var dateToMark = dateViewModel;
                for (var i = 0; i < expectedDuration; i++)
                {
                    if (!dateToMark.IsMarked)
                        await MarkDateAsync(dateToMark);
                    dateToMark = GetNextDateViewModel(dateToMark);
                }
            }
        }

        this.messenger.Send(new DatesChangedMessage(this));

        await RefreshExpectedDatesAsync();
    }

    private async Task MarkDateAsync(DateViewModel dateViewModel)
    {
        dateViewModel.IsMarked = true;
        await this.mainModel.AddDateAsync(dateViewModel.Date);
    }

    private async Task OnDatesChangedMessageAsync(DatesChangedMessage message)
    {
        if (message.Sender != this)
            await RequestRefreshAsync();
    }

    private DateViewModel GetNextDateViewModel(DateViewModel dateViewModel)
        => dateViewModel == dateViewModel.MonthViewModel.MonthDates.Last()
        ? this.months[this.months.IndexOf(dateViewModel.MonthViewModel) + 1].MonthDates.First()
        : dateViewModel.MonthViewModel.Dates[dateViewModel.MonthViewModel.Dates.IndexOf(dateViewModel) + 1];

    private DateViewModel GetPreviousDateViewModel(DateViewModel dateViewModel)
        => dateViewModel == dateViewModel.MonthViewModel.MonthDates.First()
        ? this.months[this.months.IndexOf(dateViewModel.MonthViewModel) - 1].MonthDates.Last()
        : dateViewModel.MonthViewModel.Dates[dateViewModel.MonthViewModel.Dates.IndexOf(dateViewModel) - 1];

    private async Task RefreshMarkedDatesAsync()
    {
        var lastMarkedDate = this.mainModel.MarkedDates.Last();

        foreach (var date in this.months.SelectMany(m => m.MonthDates))
            date.IsMarked = date.Date <= lastMarkedDate && this.mainModel.MarkedDates.Contains(date.Date);
    }

    private async Task RefreshExpectedDatesAsync()
    {
        var expectedPeriods = await Task.Run(() => this.mainModel.ExpectedPeriods);

        if (expectedPeriods.Count == 0)
        {
            foreach (var date in this.months.SelectMany(m => m.MonthDates))
                date.IsExpected = false;
            return;
        }

        var lastMarkedDate = this.mainModel.MarkedDates.Last();

        foreach (var date in this.months.SelectMany(m => m.MonthDates))
            date.IsExpected = date.Date > lastMarkedDate
                && expectedPeriods.Any(p => date.Date >= p.Start && date.Date <= p.End);
    }
}
