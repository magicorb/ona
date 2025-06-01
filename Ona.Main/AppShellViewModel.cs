using CommunityToolkit.Mvvm.ComponentModel;
using Ona.Main.Features.Insights;
using Ona.Main.Features.Settings;
using Ona.Main.Features.Today;
using Ona.Main.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.Main
{
	public class AppShellViewModel : ObservableObject
	{
		private readonly IMainModel mainModel;

		public AppShellViewModel(
			IMainModel mainModel,
			TodayViewModel todayViewModel,
			InsightsViewModel insightsViewModel,
			SettingsViewModel settingsViewModel)
		{
			this.mainModel = mainModel;
			TodayViewModel = todayViewModel;
			InsightsViewModel = insightsViewModel;
			SettingsViewModel = settingsViewModel;
		}

		public TodayViewModel TodayViewModel { get; }

		public InsightsViewModel InsightsViewModel { get; }

		public SettingsViewModel SettingsViewModel { get; }

		internal async Task InitializeAsync()
		{
			await this.mainModel.InitializeAsync();
		}
	}
}
