using CommunityToolkit.Mvvm.Messaging;
using Ona.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ona.App.Data
{
	public class DataPublisher : IDataPublisher
	{
		private readonly IMainModel mainModel;
		private readonly IMessenger messenger;

		public DataPublisher(
			IMainModel mainModel,
			IMessenger messenger)
		{
			this.mainModel = mainModel;
			this.messenger = messenger;
		}

		public async Task StartAsync()
		{
			await this.mainModel.OnInitializedAsync();

			this.messenger.Register<DataPublisher, DatesChangedMessage>(this, (r, m) => _ = r.PublishAsync());

			await PublishAsync();
		}

		private async Task PublishAsync()
			=> await Task.Run(() =>
			{
				var periodState = CreatePeriodState();
				var periodStateString = JsonSerializer.Serialize(periodState);
				Preferences.Set("PeriodState", periodStateString, "group.com.natalianaumova.ona");
			});

		private PeriodState CreatePeriodState()
		{
			var lastPeriodStart = this.mainModel.MarkedPeriods.Last().Start;
			var currentStats = this.mainModel.CurrentStats;
			return new PeriodState
			{
				start = lastPeriodStart,
				duration = currentStats.Duration,
				interval = currentStats.Interval
			};
		}
	}
}
