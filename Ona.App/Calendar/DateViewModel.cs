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
			DateOnly date,
			IDateTimeProvider dateTimeProvider)
		{
			Date = date;
			this.dateTimeProvider = dateTimeProvider;
		}

		public DateOnly Date { get; }

		public bool IsToday
			=> DateOnly.FromDateTime(this.dateTimeProvider.Now) == Date;

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

		public Color BackgroundColor
		{
			get
			{
				if (IsMarked)
					return Colors.Yellow;
				if (isExpected)
					return Colors.LightYellow;
				if (DateOnly.FromDateTime(this.dateTimeProvider.Now) >= Date)
					return Colors.Black;
				return Colors.Gray;
			}
		}
	}
}
