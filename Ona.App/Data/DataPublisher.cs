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
		private const string SharedName = "group.com.natalianaumova.ona";
		private const string Key = "PeriodState";

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
				var periods = this.mainModel.MarkedPeriods;
				if (!periods.Any())
				{
					Preferences.Remove(Key, SharedName);
					return;
				}

				var lastPeriodStart = periods.Last().Start;
				var periodState = new PeriodState
				{
					startDate = lastPeriodStart.ToString("yyyy-MM-dd"),
					duration = this.mainModel.ExpectedDuration,
					interval = this.mainModel.ExpectedInterval
				};

				var periodStateString = JsonSerializer.Serialize(periodState);
				Preferences.Set(Key, periodStateString, SharedName);
			});
	}
}
