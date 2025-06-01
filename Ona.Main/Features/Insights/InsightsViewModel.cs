using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Ona.Main.Data;
using Ona.Main.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ona.Main.Controls;

namespace Ona.Main.Features.Insights;

public class InsightsViewModel : ViewModelBase
{
	private readonly IMainModel mainModel;
	private readonly IMessenger messenger;

	private string? lastPeriodStart;
	private string? averageCycleLength;
	private string? averagePeriodLength;

	public InsightsViewModel(
		IMainModel mainModel,
		IMessenger messenger)
	{
		this.mainModel = mainModel;
		this.messenger = messenger;

		this.messenger.Register<InsightsViewModel, DatesChangedMessage>(this, (r, m) => _ = r.OnDatesChangedMessageAsync(m));
	}

	public string? LastPeriodStart { get => lastPeriodStart; private set => SetProperty(ref lastPeriodStart, value); }

	public string? AverageCycleLength { get => averageCycleLength; private set => SetProperty(ref averageCycleLength, value); }

	public string? AveragePeriodLength { get => averagePeriodLength; private set => SetProperty(ref averagePeriodLength, value); }

	protected override async Task RefreshAsync()
	{
		await this.mainModel.OnInitializedAsync();

		var periods = await Task.Run(() => this.mainModel.MarkedPeriods);
		int duration = default;
		int interval = default;
		await Task.Run(() =>
		{
			duration = this.mainModel.ExpectedDuration;
			interval = this.mainModel.ExpectedInterval;
		});

		LastPeriodStart = periods.Any()
			? periods.Last().Start.ToString("dd MMM yyyy")
			: "No data";

		AverageCycleLength = $"{duration} days";

		AveragePeriodLength = $"{interval} days";
	}

	private async Task OnDatesChangedMessageAsync(DatesChangedMessage m)
		=> await RequestRefreshAsync();
}
