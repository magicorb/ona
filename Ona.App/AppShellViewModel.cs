using Ona.App.Features.Insights;
using Ona.App.Features.Settings;
using Ona.App.Features.Today;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App
{
	public class AppShellViewModel
	{
		public AppShellViewModel(
			TodayViewModel todayViewModel,
			InsightsViewModel insightsViewModel,
			SettingsViewModel settingsViewModel)
		{
			TodayViewModel = todayViewModel;
			InsightsViewModel = insightsViewModel;
			SettingsViewModel = settingsViewModel;
		}

		public TodayViewModel TodayViewModel { get; }

		public InsightsViewModel InsightsViewModel { get; }

		public SettingsViewModel SettingsViewModel { get; }
	}
}
