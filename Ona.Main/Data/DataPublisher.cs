﻿using CommunityToolkit.Mvvm.Messaging;
using Ona.Main.Model;
using System.Text.Json;

namespace Ona.Main.Data;

public class DataPublisher : IDataPublisher
{
    private const string SharedName = "group.com.natalianaumova.ona";
    private const string Key = "PeriodState";

    private readonly IMainModel mainModel;
    private readonly IMessenger messenger;

    public DataPublisher(
        IMainModel mainModel,
        IMessenger messenger)
    {
        this.mainModel = mainModel;
        this.messenger = messenger;
    }

    public async Task StartAsync()
    {
        await this.mainModel.OnInitializedAsync();

        this.messenger.Register<DataPublisher, DatesChangedMessage>(this, (r, m) => _ = r.PublishAsync());

        await PublishAsync();
    }

    private async Task PublishAsync()
        => await Task.Run(() =>
        {
            var cycles = this.mainModel.Cycles;
            if (!cycles.Any())
            {
                Preferences.Remove(Key, SharedName);
                return;
            }

            var lastPeriodStart = cycles.Last()[0];
            var periodState = new PeriodState
            {
                startDate = lastPeriodStart.ToString("yyyy-MM-dd"),
                duration = this.mainModel.ExpectedPeriodLength,
                interval = this.mainModel.ExpectedCycleLength
            };

            var periodStateString = JsonSerializer.Serialize(periodState);
            Preferences.Set(Key, periodStateString, SharedName);
        });
}
