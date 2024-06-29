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
	public class CalendarViewModel : ObservableObject
	{
		private readonly IDateTimeProvider dateTimeProvider;
		private readonly IDateRepository dateRepository;
		private readonly IPeriodStatsProvider periodStatsProvider;
		private readonly IMessenger messenger;
		private readonly MonthViewModelFactory monthViewModelFactory;

		private readonly ObservableCollection<MonthViewModel> months;

		private DateViewModel? selectionStart;

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

			this.months = new ObservableCollection<MonthViewModel>();
			Months = new ReadOnlyObservableCollection<MonthViewModel>(this.months);

			var now = dateTimeProvider.Now;
			CurentMonth = this.monthViewModelFactory(now.Year, now.Month);
			this.months.Add(CurentMonth);
			InsertMonth();
			InsertMonth();
			AppendMonth();

			foreach (var month in Months)
				month.IsVisible = false;

			this.messenger.Register<DateToggledMessage>(this, (recipient, message) => _ = OnDateToggledMessageAsync(message));
		}

		public ReadOnlyObservableCollection<MonthViewModel> Months { get; }

		public MonthViewModel CurentMonth { get; }

		internal void AppendMonth()
		{
			var monthStart = Months[Months.Count - 1].MonthStart.AddMonths(1);
			this.months.Add(this.monthViewModelFactory(monthStart.Year, monthStart.Month));

			_ = LoadDatesAsync();
		}

		internal void InsertMonth()
		{
			var monthStart = Months[0].MonthStart.AddMonths(-1);
			this.months.Insert(0, this.monthViewModelFactory(monthStart.Year, monthStart.Month));

			_ = LoadDatesAsync();
		}

		private async Task OnDateToggledMessageAsync(DateToggledMessage message)
		{
			var date = message.Date;
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

			_ = UpdateForecastAsync();
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
			dateViewModel.IsMarked = true;
			await this.dateRepository.AddDateRecordAsync(dateViewModel.Date);
		}

		private async Task UnmarkDateAsync(DateViewModel dateViewModel)
		{
			dateViewModel.IsMarked = false;
			await this.dateRepository.DeleteDateRecordAsync(dateViewModel.Date);
		}

		private bool IsSelectingRange
			=> this.selectionStart != null;

		private DateViewModel GetNextDateViewModel(DateViewModel dateViewModel)
			=> dateViewModel == dateViewModel.MonthViewModel.Dates.Last(d => d.IsCurrentMonth)
			? Months[Months.IndexOf(dateViewModel.MonthViewModel) + 1].Dates.First(d => d.IsCurrentMonth)
			: dateViewModel.MonthViewModel.Dates[dateViewModel.MonthViewModel.Dates.IndexOf(dateViewModel) + 1];

		private async Task UpdateForecastAsync()
		{
			// TODO: If new call made, cancel all pending, wait for the current to complete and then start

			var dates = await this.dateRepository.GetDateRecordsAsync();
			var nextPeriod = await Task.Run(() => this.periodStatsProvider.GetNextPeriod(dates.Select(d => d.Date).ToArray()));

			foreach (var date in Months.SelectMany(m => m.Dates))
				date.IsExpected = date.Date >= nextPeriod.Start && date.Date <= nextPeriod.End;
		}
	}
}
