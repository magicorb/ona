using CommunityToolkit.Mvvm.Messaging;
using Ona.App.Calendar;
using Ona.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App
{
	public static class DependencyInjectionExtensions
	{
		public static MauiAppBuilder RegisterAll(this MauiAppBuilder builder)
		{
			var services = builder.Services;

			services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
			services.AddSingleton<IMessenger, WeakReferenceMessenger>();

			services.AddTransient<MainViewModel>();
			services.AddTransient<CalendarViewModel>();

			return builder;
		}
	}
}
