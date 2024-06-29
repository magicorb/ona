using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Ona.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ona.App.Calendar
{
	public class DateViewModel : ObservableObject
	{
		private readonly IDateTimeProvider dateTimeProvider;
		private readonly IMessenger messenger;

		private bool isMarked;
		private bool isExpected;

		public DateViewModel(
			IDateTimeProvider dateTimeProvider,
			IMessenger messenger,
			DateTime date,
			MonthViewModel monthViewModel,
			int currentYear,
			int currentMonth)
		{
			this.dateTimeProvider = dateTimeProvider;
			this.messenger = messenger;

			Date = date;
			MonthViewModel = monthViewModel;
			IsCurrentMonth = date.Year == currentYear && date.Month == currentMonth;
			IsToday = Date.Date == this.dateTimeProvider.Now.Date;

			ToggleCommand = new RelayCommand(ExecuteToggle);
		}

		public DateTime Date { get; }

		public MonthViewModel MonthViewModel { get; }

		public bool IsToday { get; }

		public bool IsMarked
		{
			get => this.isMarked;
			set
			{
				SetProperty(ref this.isMarked, value);
				OnPropertyChanged(nameof(BackgroundColor));
			}
		}

		public bool IsExpected
		{
			get => this.isExpected;
			set
			{
				SetProperty(ref this.isExpected, value);
				OnPropertyChanged(nameof(BackgroundColor));
			}
		}

		public bool IsCurrentMonth { get; }

		public Color BackgroundColor
		{
			get
			{
				if (IsMarked)
					return Colors.Yellow;
				if (isExpected)
					return Colors.LightYellow;
				if (this.dateTimeProvider.Now >= Date)
					return Colors.Black;
				return Colors.Gray;
			}
		}

		public ICommand ToggleCommand { get; }

		private void ExecuteToggle()
			=> this.messenger.Send(new DateToggledMessage(Date));
	}

	public delegate DateViewModel DateViewModelFactory(DateTime date, MonthViewModel monthViewModel, int currentYear, int currentMonth);
}
