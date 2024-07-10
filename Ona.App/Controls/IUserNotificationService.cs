using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Controls
{
	public interface IUserNotificationService
	{
		Task<bool> ConfirmAsync(string title, string message, string accept, string cancel);

		Task NotifyAsync(string message);
	}
}
