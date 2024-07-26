using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Features.Calendar
{
	public class SpinnerViewModel : ObservableObject
	{
		private bool isRunning;

		public bool IsRunning { get => this.isRunning; set => SetProperty(ref this.isRunning, value); }
	}
}