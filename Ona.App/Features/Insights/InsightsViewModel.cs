using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Ona.App.Data;
using Ona.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Features.Insights
{
	public class InsightsViewModel : ObservableObject
	{
		private readonly IDateRepository dateRepository;
		private readonly IMessenger messenger;
		private readonly IPeriodStatsProvider periodStatsProvider;

		private bool isInitialized;
		private string lastPeriodStart;
		private string averageCycleLength;
		private string averagePeriodLength;

		public InsightsViewModel(
			IDateRepository dateRepository,
			IMessenger messenger,
			IPeriodStatsProvider periodStatsProvider)
		{
			this.dateRepository = dateRepository;
			this.messenger = messenger;
			this.periodStatsProvider = periodStatsProvider;

			this.messenger.Register<InsightsViewModel, DatesChangedMessage>(this, (r, m) => _ = r.OnDatesChangedMessageAsync(m));
		}

		public string? LastPeriodStart { get => lastPeriodStart; private set => SetProperty(ref lastPeriodStart, value); }

		public string? AverageCycleLength { get => averageCycleLength; private set => SetProperty(ref averageCycleLength, value); }

		public string? AveragePeriodLength { get => averagePeriodLength; private set => SetProperty(ref averagePeriodLength, value); }

		public async Task InitializeAsync()
		{
			if (this.isInitialized)
				return;

			await RefreshAsync();
		}

		private async Task RefreshAsync()
		{
			this.isInitialized = true;

			var datesRecords = await dateRepository.GetDateRecordsAsync();

			if (datesRecords.Length == 0)
				return;

			IReadOnlyList<DateTimePeriod> periods = null;
			PeriodStats periodStats = null;

			await Task.Run(() =>
			{
				periods = periodStatsProvider.GetDatePeriods(datesRecords.Select(d => d.Date));
				periodStats = periodStatsProvider.GetAveragePeriodStats(periods);
			});

			LastPeriodStart = periods!.Last().Start.ToString("dd MMM yyyy");

			AverageCycleLength = $"{periodStats!.Duration!.Value} days";

			AveragePeriodLength = periodStats!.Interval != null
				? $"{periodStats!.Interval.Value} days"
				: null;
		}

		private async Task OnDatesChangedMessageAsync(DatesChangedMessage m)
			=> await RefreshAsync();
	}
}
