using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Ona.App.Controls
{
	public abstract class ViewModelBase : ObservableObject, IViewModel
	{
		private bool isLoaded;
		private bool isRefreshRequested = true;

		public async Task OnLoadedAsync()
		{
			this.isLoaded = true;

			if (this.isRefreshRequested)
			{
				await RefreshAsync();
				this.isRefreshRequested = false;
			}
		}

		public async Task OnUnloadedAsync()
			=> this.isLoaded = false;

		protected async Task RequestRefreshAsync()
		{
			if (this.isLoaded)
				await RefreshAsync();
			else
				this.isRefreshRequested = true;
		}

		protected abstract Task RefreshAsync();
	}
}
