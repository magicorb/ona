using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Ona.App.Data;
using Ona.App.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Calendar
{
	public class CalendarViewModel : ObservableObject, IDisposable
	{
		private readonly IDateTimeProvider dateTimeProvider;
		private readonly IDateRepository dateRepository;
		private readonly IPeriodStatsProvider periodStatsProvider;
		private readonly IMessenger messenger;
		private readonly MonthViewModelFactory monthViewModelFactory;

		private ObservableCollection<MonthViewModel> months;

		private DateViewModel? selectionStart;
		private IEnumerator<DateTimePeriod> expectedPeriodsEnumerator;
		private DateTimePeriod? expectedCurrentPeriod;

		public CalendarViewModel(
			IDateTimeProvider dateTimeProvider,
			IDateRepository dateRepository,
			IPeriodStatsProvider periodStatsProvider,
			IMessenger messenger,
			MonthViewModelFactory monthViewModelFactory)
		{
			this.dateTimeProvider = dateTimeProvider;
			this.dateRepository = dateRepository;
			this.periodStatsProvider = periodStatsProvider;
			this.messenger = messenger;
			this.monthViewModelFactory = monthViewModelFactory;

			_ = InitializeMonths();

			this.messenger.Register<DateToggledMessage>(this, (recipient, message) => _ = OnDateToggledMessageAsync(message));
		}

		public ReadOnlyObservableCollection<MonthViewModel> Months { get; private set; }

		public MonthViewModel CurentMonth { get; private set; }

		public void Dispose()
		{
			this.messenger.UnregisterAll(this);
		}

		internal async Task AppendMonthAsync()
		{
			var monthStart = Months[Months.Count - 1].MonthStart.AddMonths(1);
			this.months.Add(this.monthViewModelFactory(monthStart.Year, monthStart.Month));

			await LoadDatesAsync();

			ApplyExpectedPeriods();
		}

		internal async Task InsertMonthAsync()
		{
			var monthStart = Months[0].MonthStart.AddMonths(-1);
			this.months.Insert(0, this.monthViewModelFactory(monthStart.Year, monthStart.Month));

			await LoadDatesAsync();

			ApplyExpectedPeriods();
		}

		private async Task InitializeMonths()
		{
			this.months = new ObservableCollection<MonthViewModel>();

			var now = dateTimeProvider.Now;
			var currentMonthStart = new DateTime(now.Year, now.Month, 1);

			CurentMonth = CreateMonthViewModel(currentMonthStart);
			
			this.months.Add(CreateMonthViewModel(currentMonthStart.AddMonths(-2)));
			this.months.Add(CreateMonthViewModel(currentMonthStart.AddMonths(-1)));
			this.months.Add(CurentMonth);
			this.months.Add(CreateMonthViewModel(currentMonthStart.AddMonths(1)));

			foreach (var month in this.months)
				month.IsVisible = false;

			CurentMonth.IsVisible = true;

			Months = new ReadOnlyObservableCollection<MonthViewModel>(this.months);

			await LoadDatesAsync();

			await UpdateExpectedPeriodsAsync();
		}

		private MonthViewModel CreateMonthViewModel(DateTime monthStart)
			=> this.monthViewModelFactory(monthStart.Year, monthStart.Month);

		private async Task OnDateToggledMessageAsync(DateToggledMessage message)
		{
			var date = message.Date;

			if (date > dateTimeProvider.Now.Date)
				return;

			var dateViewModel = this.months.First(m => m.Year == date.Year && m.Month == date.Month).Dates.First(d => d.Date == date);

			if (IsSelectingRange)
			{
				if (this.selectionStart.Date > dateViewModel.Date)
				{
					for (var d = dateViewModel; d != this.selectionStart; d = GetNextDateViewModel(d))
						await MarkDateAsync(d);
				}
				else if (this.selectionStart.Date < dateViewModel.Date)
				{
					for (var d = GetNextDateViewModel(this.selectionStart); true; d = GetNextDateViewModel(d))
					{
						await MarkDateAsync(d);

						if (d == dateViewModel)
							break;
					}
				}
				else
					await UnmarkDateAsync(dateViewModel);

				this.selectionStart = null;
			}
			else
			{
				if (dateViewModel.IsMarked)
					await UnmarkDateAsync(dateViewModel);
				else
				{
					await MarkDateAsync(dateViewModel);
					this.selectionStart = dateViewModel;
				}
			}

			_ = UpdateExpectedPeriodsAsync();
		}

		private async Task LoadDatesAsync()
		{
			var dateRecords = await this.dateRepository.GetDateRecordsAsync();

			foreach (var dateRecord in dateRecords)
			{
				var date = dateRecord.Date;
				var monthViewModel = Months.FirstOrDefault(m => m.Year == date.Year && m.Month == date.Month);
				if (monthViewModel == null)
					continue;
				var dateViewModel = monthViewModel.Dates.First(d => d.Date == date);
				dateViewModel.IsMarked = true;
			}
		}

		private async Task MarkDateAsync(DateViewModel dateViewModel)
		{
			if (dateViewModel.IsMarked)
				return;

			dateViewModel.IsMarked = true;
			await this.dateRepository.AddDateRecordAsync(dateViewModel.Date);
		}

		private async Task UnmarkDateAsync(DateViewModel dateViewModel)
		{
			if (!dateViewModel.IsMarked)
				return;

			dateViewModel.IsMarked = false;
			await this.dateRepository.DeleteDateRecordAsync(dateViewModel.Date);
		}

		private bool IsSelectingRange
			=> this.selectionStart != null;

		private DateViewModel GetNextDateViewModel(DateViewModel dateViewModel)
			=> dateViewModel == dateViewModel.MonthViewModel.Dates.Last(d => d.IsCurrentMonth)
			? Months[Months.IndexOf(dateViewModel.MonthViewModel) + 1].Dates.First(d => d.IsCurrentMonth)
			: dateViewModel.MonthViewModel.Dates[dateViewModel.MonthViewModel.Dates.IndexOf(dateViewModel) + 1];

		private async Task UpdateExpectedPeriodsAsync()
		{
			// TODO: If new call made, cancel all pending, wait for the current to complete and then start

			var dates = (await this.dateRepository.GetDateRecordsAsync()).Select(d => d.Date).ToArray();

			var periods = this.periodStatsProvider.GetDatePeriods(dates.Select(d => d.Date));

			this.expectedPeriodsEnumerator = await Task.Run(()
				=> this.periodStatsProvider.GetExpectedPeriodsEnumerator(periods));

			if (periods.Count > 1)
			{
				var previousPeriods = new List<DateTimePeriod>(periods.Take(periods.Count - 1));

				var previousExpectedPeriodsEnumerator = await Task.Run(()
					=> this.periodStatsProvider.GetExpectedPeriodsEnumerator(previousPeriods));

				if (previousExpectedPeriodsEnumerator.MoveNext())
					this.expectedCurrentPeriod = previousExpectedPeriodsEnumerator.Current;
				else
					this.expectedCurrentPeriod = null;
			}
			else
				this.expectedCurrentPeriod = null;

			ApplyExpectedPeriods();
		}

		private void ApplyExpectedPeriods()
		{
			var lastDate = Months.Last().Dates.Last(d => d.IsCurrentMonth).Date;
			var expectedPeriods = new List<DateTimePeriod>();

			if (this.expectedCurrentPeriod != null)
				expectedPeriods.Add(this.expectedCurrentPeriod);

			this.expectedPeriodsEnumerator.Reset();
			while (this.expectedPeriodsEnumerator.MoveNext())
			{
				var period = this.expectedPeriodsEnumerator.Current;

				if (period.Start > lastDate)
					break;

				expectedPeriods.Add(period);
			}

			var today = this.dateTimeProvider.Now.Date;

			foreach (var date in Months.SelectMany(m => m.Dates.Where(d => d.IsCurrentMonth)).Where(d => d.Date >= today))
				date.IsExpected = expectedPeriods.Any(p => date.Date >= p.Start && date.Date <= p.End);
		}
	}
}
