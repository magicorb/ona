using CommunityToolkit.Mvvm.ComponentModel;
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
        }

        public string? LastPeriodStart { get => lastPeriodStart; private set => SetProperty(ref lastPeriodStart, value); }

        public string? AverageCycleLength { get => averageCycleLength; private set => SetProperty(ref averageCycleLength, value); }

        public string? AveragePeriodLength { get => averagePeriodLength; private set => SetProperty(ref averagePeriodLength, value); }

		public async Task RefreshAsync()
        {
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
    }
}
