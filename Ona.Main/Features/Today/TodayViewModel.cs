﻿using CommunityToolkit.Mvvm.Messaging;
using Ona.Main.Controls;
using Ona.Main.Environment;
using Ona.Main.Features.Calendar;
using Ona.Main.Model;

namespace Ona.Main.Features.Today;

public class TodayViewModel : ViewModelBase
{
    private readonly IDateTimeProvider timeProvider;
    private readonly IMessenger messenger;
    private readonly IMainModel mainModel;

    private string? title;
    private string? subtitle;

    public TodayViewModel(
        IDateTimeProvider timeProvider,
        IMessenger messenger,
        IMainModel mainModel,
        CalendarViewModel calendarViewModel)
    {
        this.timeProvider = timeProvider;
        this.messenger = messenger;
        this.mainModel = mainModel;
        CalendarViewModel = calendarViewModel;

        this.messenger.Register<TodayViewModel, DatesChangedMessage>(this, (r, m) => _ = r.OnDatesChangedMessageAsync(m));
    }

    public string? Title { get => this.title; private set => SetProperty(ref this.title, value); }

    public string? Subtitle { get => this.subtitle; private set => SetProperty(ref this.subtitle, value); }

    public CalendarViewModel CalendarViewModel { get; }

    protected override async Task RefreshAsync()
    {
        await this.mainModel.OnInitializedAsync();

        if (this.mainModel.MarkedDates.Count == 0)
        {
            Title = "Tap";
            Subtitle = "on the day when your last period started.";
        }
        else
        {
            var cycles = await Task.Run(() => this.mainModel.Cycles);
            var lastPeriodStart = cycles.Last()[0];
            var today = this.timeProvider.Now.Date;
            var periodElapsedDays = (today - lastPeriodStart).Days;

            Title = $"Day {periodElapsedDays + 1} of your cycle";

            var periodLeftDays = this.mainModel.ExpectedPeriodLength - periodElapsedDays;

            if (periodLeftDays > 0)
                Subtitle = $"This period ends in {periodLeftDays} days.\r\nTap on the day to adjust.";
            else
            {
                var averageInterval = this.mainModel.ExpectedCycleLength;
                var cycleLeftDays = (lastPeriodStart.AddDays(averageInterval) - today).Days;

                Subtitle = cycleLeftDays > 0
                    ? $"New period starts in {cycleLeftDays} days.\r\nTap on the day to adjust."
                    : "Tap on the day to record new period started";
            }
        }
    }

    private async Task OnDatesChangedMessageAsync(DatesChangedMessage message)
        => await RequestRefreshAsync();
}
