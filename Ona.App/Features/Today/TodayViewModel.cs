using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Ona.App.Features.Calendar;
using Ona.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Features.Today
{
	public class TodayViewModel : ObservableObject
	{
		private readonly IDateTimeProvider timeProvider;
		private readonly IMessenger messenger;
		private readonly IMainModel mainModel;

		private bool isInitialized;
		private string title;
		private string subtitle;

		public TodayViewModel(
			IDateTimeProvider timeProvider,
			IMessenger messenger,
			IMainModel mainModel,
			CalendarViewModel calendarViewModel)
		{
			this.timeProvider = timeProvider;
			this.messenger = messenger;
			this.mainModel = mainModel;
			CalendarViewModel = calendarViewModel;

			this.messenger.Register<TodayViewModel, DatesChangedMessage>(this, (r, m) => r.OnDatesChanged(m));
		}

		public string Title { get => this.title; private set => SetProperty(ref this.title, value); }

		public string Subtitle { get => this.subtitle; private set => SetProperty(ref this.subtitle, value); }
	
		public CalendarViewModel CalendarViewModel { get; }

		public async Task InititalizeAsync()
		{
			if (this.isInitialized)
				return;

			this.isInitialized = true;

			await this.mainModel.OnInitializedAsync();

			RefreshTitles();

			await CalendarViewModel.RefreshAsync();
		}

		private void OnDatesChanged(DatesChangedMessage e)
		{
			RefreshTitles();
		}

		private void RefreshTitles()
		{
			if (this.mainModel.MarkedDates.Count == 0)
			{
				Title = "Tap";
				Subtitle = "on the day when your last period started.";
			}
			else
			{
				var lastPeriodStart = this.mainModel.MarkedPeriods.Last().Start;
				var today = this.timeProvider.Now.Date;
				var periodElapsedDays = (today - lastPeriodStart).Days;

				Title = $"Day {periodElapsedDays + 1} of your cycle";

				var periodLeftDays = this.mainModel.PreviousStats!.Duration!.Value - periodElapsedDays;

				if (periodLeftDays > 0)
					Subtitle = $"This period ends in {periodLeftDays} days.\r\nTap on the day to adjust.";
				else
				{
					var averageInterval = this.mainModel.CurrentStats!.Interval!.Value;
					var cycleLeftDays = (lastPeriodStart.AddDays(averageInterval) - today).Days;

					Subtitle = cycleLeftDays > 0
						? $"New period starts in {cycleLeftDays} days.\r\nTap on the day to adjust."
						: "Tap on the day to record new period started";
				}
			}
		}
	}
}
