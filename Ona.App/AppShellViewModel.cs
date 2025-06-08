using CommunityToolkit.Mvvm.ComponentModel;
using Ona.App.Features.Insights;
using Ona.App.Features.Settings;
using Ona.App.Features.Today;
using Ona.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App
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
