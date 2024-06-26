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
		private readonly IMessenger messenger;
		private readonly MonthViewModelFactory monthViewModelFactory;

		private readonly ObservableCollection<MonthViewModel> months;

		public CalendarViewModel(
			IDateTimeProvider dateTimeProvider,
			IDateRepository dateRepository,
			IMessenger messenger,
			MonthViewModelFactory monthViewModelFactory)
		{
			this.dateTimeProvider = dateTimeProvider;
			this.dateRepository = dateRepository;
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

			this.messenger.Register<DateToggledMessage>(this, OnDateToggledMessage);
		}

		public ReadOnlyObservableCollection<MonthViewModel> Months { get; }

		public MonthViewModel CurentMonth { get; }

		internal void AppendMonth()
		{
			var monthStart = Months[Months.Count - 1].MonthStart.AddMonths(1);
			this.months.Add(this.monthViewModelFactory(monthStart.Year, monthStart.Month));

			_ = RefreshAsync();
		}

		internal void InsertMonth()
		{
			var monthStart = Months[0].MonthStart.AddMonths(-1);
			this.months.Insert(0, this.monthViewModelFactory(monthStart.Year, monthStart.Month));

			_ = RefreshAsync();
		}

		private void OnDateToggledMessage(object recipient, DateToggledMessage message)
		{
			var date = message.Date;
			var dateViewModel = this.months.First(m => m.Year == date.Year && m.Month == date.Month).Dates.First(d => d.Date == date);

			if (dateViewModel.IsMarked)
				_ = UnmarkDateAsync(date);
			else
				_ = MarkDateAsync(date);
		}

		private async Task RefreshAsync()
		{
			await LoadDatesAsync();
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

		private async Task MarkDateAsync(DateTime date)
		{
			await this.dateRepository.AddDateRecordAsync(date);
			await RefreshAsync();
		}

		private async Task UnmarkDateAsync(DateTime date)
		{
			await this.dateRepository.DeleteDateRecordAsync(date);
			await RefreshAsync();
		}
	}
}
