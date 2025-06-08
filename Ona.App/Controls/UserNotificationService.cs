using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Controls
{
	public class UserNotificationService : IUserNotificationService
	{
		private readonly Page page;

		public UserNotificationService(Page page)
		{
			this.page = page;
		}

		public async Task<bool> ConfirmAsync(string title, string message, string accept, string cancel)
			=> await this.page.DisplayAlert(title, message, accept, cancel);

		public async Task NotifyAsync(string title, string message, string ok)
			=> await this.page.DisplayAlert(title, message, ok);
	}
}
