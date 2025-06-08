using CommunityToolkit.Mvvm.ComponentModel;
using Ona.App.Data;
using Ona.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Insights
{
	public class InsightsViewModel : ObservableObject
	{
		private readonly IDateRepository dateRepository;
		private readonly IPeriodStatsProvider periodStatsProvider;

		private string lastPeriodStart;
		private string averageCycleLength;
		private string averagePeriodLength;

		public InsightsViewModel(
			IDateRepository dateRepository,
			IPeriodStatsProvider periodStatsProvider)
		{
			this.dateRepository = dateRepository;
			this.periodStatsProvider = periodStatsProvider;

			_ = InitializeAsync();
		}

		public string? LastPeriodStart { get => this.lastPeriodStart; private set => SetProperty(ref this.lastPeriodStart, value); }

		public string? AverageCycleLength { get => this.averageCycleLength; private set => SetProperty(ref this.averageCycleLength, value); }

		public string? AveragePeriodLength { get => this.averagePeriodLength; private set => SetProperty(ref this.averagePeriodLength, value); }

		private async Task InitializeAsync()
		{
			var datesRecords = await this.dateRepository.GetDateRecordsAsync();
			
			if (datesRecords.Length == 0)
				return;

			IReadOnlyList<DateTimePeriod> periods = null;
			PeriodStats periodStats = null;

			await Task.Run(() =>
			{
				periods = this.periodStatsProvider.GetDatePeriods(datesRecords.Select(d => d.Date));
				periodStats = this.periodStatsProvider.GetAveragePeriodStats(periods);
			});

			LastPeriodStart = periods![periods.Count - 1].Start.ToString("dd MMM yyyy");

			AverageCycleLength = $"{(int)Math.Round(periodStats!.Duration!.Value, MidpointRounding.AwayFromZero)} days";

			AveragePeriodLength = periodStats!.Interval != null
				? $"{(int)Math.Round(periodStats!.Interval.Value, MidpointRounding.AwayFromZero)} days"
				: null;
		}
	}
}
