using CommunityToolkit.Mvvm.Messaging;
using Ona.App.Model;
using Ona.App.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Calendar
{
	public class CalendarViewModel : ViewModelBase
	{
		private readonly IDateTimeProvider dateTimeProvider;
		private readonly IMessenger messenger;

		private readonly ObservableCollection<MonthViewModel> months;

		public CalendarViewModel(
			IDateTimeProvider dateTimeProvider,
			IMessenger messenger)
		{
			this.dateTimeProvider = dateTimeProvider;
			this.messenger = messenger;

			this.months = new ObservableCollection<MonthViewModel>();
			Months = new ReadOnlyObservableCollection<MonthViewModel>(this.months);

			var now = dateTimeProvider.Now;
			CurentMonth = new MonthViewModel(this.dateTimeProvider, messenger, now.Year, now.Month);
			this.months.Add(CurentMonth);
			InsertMonth();
			InsertMonth();
			AppendMonth();

			foreach (var month in Months)
				month.IsVisible = false;

			// TODO: unregiseter
			this.messenger.Register<DateToggledMessage>(this, OnDateToggledMessage);
		}

		public ReadOnlyObservableCollection<MonthViewModel> Months { get; }

		public MonthViewModel CurentMonth { get; }

		internal void AppendMonth()
		{
			var monthStart = Months[Months.Count - 1].MonthStart.AddMonths(1);
			this.months.Add(new MonthViewModel(this.dateTimeProvider, this.messenger, monthStart.Year, monthStart.Month));
		}

		internal void InsertMonth()
		{
			var monthStart = Months[0].MonthStart.AddMonths(-1);
			this.months.Insert(0, new MonthViewModel(this.dateTimeProvider, this.messenger, monthStart.Year, monthStart.Month));
		}

		private void OnDateToggledMessage(object recipient, DateToggledMessage message)
		{
			var date = message.Date;

			var dateViewModel = this.months.First(m => m.Year == date.Year && m.Month == date.Month).Dates.First(d => d.Date == date);

			dateViewModel.IsMarked = !dateViewModel.IsMarked;
		}
	}
}
