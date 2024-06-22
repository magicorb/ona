using Ona.App.Model;
using Ona.App.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Calendar
{
	public class DateViewModel : ViewModelBase
	{
		private readonly IDateTimeProvider dateTimeProvider;

		private bool isMarked;
		private bool isExpected;

		public DateViewModel(
			IDateTimeProvider dateTimeProvider,
			DateTime date,
			int currentYear,
			int currentMonth)
		{
			this.dateTimeProvider = dateTimeProvider;

			Date = date;
			IsCurrentMonth = date.Year == currentYear && date.Month == currentMonth;
		}

		public DateTime Date { get; }

		public bool IsToday
			=> Date.Date == this.dateTimeProvider.Now.Date;

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
	}
}
