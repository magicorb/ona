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

namespace Ona.App.Features.Calendar
{
	public class CalendarViewModel : ObservableObject, IDisposable
	{
		private readonly IDateTimeProvider dateTimeProvider;
		private readonly IMessenger messenger;
		private readonly IMainModel mainModel;
		private readonly MonthViewModelFactory monthViewModelFactory;

		private ObservableCollection<MonthViewModel> months;

		private DateViewModel? selectionStart;
		
		public CalendarViewModel(
			IDateTimeProvider dateTimeProvider,
			IMessenger messenger,
			IMainModel mainModel,
			MonthViewModelFactory monthViewModelFactory)
		{
			this.dateTimeProvider = dateTimeProvider;
			this.messenger = messenger;
			this.mainModel = mainModel;
			this.monthViewModelFactory = monthViewModelFactory;

			Initialize();

			this.messenger.Register<DateToggledMessage>(this, (recipient, message) => _ = OnDateToggledMessageAsync(message));
		}

		public ObservableCollection<object> Items { get; private set; }

		public MonthViewModel CurentMonth { get; private set; }

		public async Task RefreshAsync()
		{
			await this.mainModel.OnInitializedAsync();

			RefreshMarkedDates();
			await RefreshExpectedDatesAsync();
		}

		public void ShowHiddenMonths()
		{
			for (var i = 0; i < this.months.Count; i++)
				this.months[i].IsVisible = true;
		}

		public void Dispose()
		{
			messenger.UnregisterAll(this);
		}

		internal async Task AppendMonthAsync()
		{
			var monthStart = this.months.Last().MonthStart.AddMonths(1);
			var newItem = monthViewModelFactory(monthStart.Year, monthStart.Month);
			this.months.Add(newItem);
			Items.Insert(Items.Count - 1, newItem);

			RefreshMarkedDates();
			await RefreshExpectedDatesAsync();
		}

		internal async Task InsertMonthAsync()
		{
			var monthStart = this.months[0].MonthStart.AddMonths(-1);
			var newItem = monthViewModelFactory(monthStart.Year, monthStart.Month);
			this.months.Insert(0, newItem);
			Items.Insert(1, newItem);

			RefreshMarkedDates();
			await RefreshExpectedDatesAsync();
		}

		private void Initialize()
		{
			months = new ObservableCollection<MonthViewModel>();

			var now = dateTimeProvider.Now;
			var currentMonthStart = new DateTime(now.Year, now.Month, 1);

			CurentMonth = CreateMonthViewModel(currentMonthStart);

			months.Add(CreateMonthViewModel(currentMonthStart.AddMonths(-2)));
			months.Add(CreateMonthViewModel(currentMonthStart.AddMonths(-1)));
			months.Add(CurentMonth);
			months.Add(CreateMonthViewModel(currentMonthStart.AddMonths(1)));

			foreach (var month in months)
				month.IsVisible = false;

			CurentMonth.IsVisible = true;

			Items = new ObservableCollection<object>(this.months);
			Items.Insert(0, new SpinnerViewModel());
			Items.Add(new SpinnerViewModel());
		}

		private MonthViewModel CreateMonthViewModel(DateTime monthStart)
			=> monthViewModelFactory(monthStart.Year, monthStart.Month);

		private async Task OnDateToggledMessageAsync(DateToggledMessage message)
		{
			var date = message.Date;

			if (date > dateTimeProvider.Now.Date)
				return;

			var dateViewModel = months.First(m => m.Year == date.Year && m.Month == date.Month).Dates.First(d => d.Date == date);

			if (IsSelectingRange)
			{
				if (selectionStart.Date > dateViewModel.Date)
				{
					for (var d = dateViewModel; d != selectionStart; d = GetNextDateViewModel(d))
						await MarkDateAsync(d);
				}
				else if (selectionStart.Date < dateViewModel.Date)
				{
					for (var d = GetNextDateViewModel(selectionStart); true; d = GetNextDateViewModel(d))
					{
						await MarkDateAsync(d);

						if (d == dateViewModel)
							break;
					}
				}
				else
					await UnmarkDateAsync(dateViewModel);

				selectionStart = null;
			}
			else
			{
				if (dateViewModel.IsMarked)
					await UnmarkDateAsync(dateViewModel);
				else
				{
					await MarkDateAsync(dateViewModel);
					selectionStart = dateViewModel;
				}
			}

			_ = RefreshExpectedDatesAsync();
		}

		private async Task MarkDateAsync(DateViewModel dateViewModel)
		{
			if (dateViewModel.IsMarked)
				return;

			dateViewModel.IsMarked = true;
			await this.mainModel.AddDateAsync(dateViewModel.Date);
		}

		private async Task UnmarkDateAsync(DateViewModel dateViewModel)
		{
			if (!dateViewModel.IsMarked)
				return;

			dateViewModel.IsMarked = false;
			await this.mainModel.DeleteDateAsync(dateViewModel.Date);
		}

		private bool IsSelectingRange
			=> selectionStart != null;

		private DateViewModel GetNextDateViewModel(DateViewModel dateViewModel)
			=> dateViewModel == dateViewModel.MonthViewModel.MonthDates.Last()
			? this.months[this.months.IndexOf(dateViewModel.MonthViewModel) + 1].MonthDates.First()
			: dateViewModel.MonthViewModel.Dates[dateViewModel.MonthViewModel.Dates.IndexOf(dateViewModel) + 1];

		private void RefreshMarkedDates()
		{
			foreach (var date in this.mainModel.MarkedDates)
			{
				var monthViewModel = this.months.FirstOrDefault(m => m.Year == date.Year && m.Month == date.Month);
				if (monthViewModel == null)
					continue;
				var dateViewModel = monthViewModel.Dates.First(d => d.Date == date);
				dateViewModel.IsMarked = true;
			}
		}
		
		private async Task RefreshExpectedDatesAsync()
		{
			this.mainModel.ObservedEnd = this.months.Last().MonthDates.Last().Date;

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
}
