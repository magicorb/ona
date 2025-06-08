using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Controls
{
	public static class DispatcherExtensions
	{
		public static Task DoEventsAsync(this IDispatcher dispatcher)
		{
			var tcs = new TaskCompletionSource();
			dispatcher.Dispatch(tcs.SetResult);
			return tcs.Task;
		}
	}
}
