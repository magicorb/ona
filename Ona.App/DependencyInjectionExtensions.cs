using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Ona.App.Calendar;
using Ona.App.Data;
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
			services.AddSingleton<SQLiteDateRepository>();
			services.AddSingleton<IDateRepository, SQLiteDateRepository>();
			
			services.AddSingleton<IMessenger, WeakReferenceMessenger>();

			services.AddTransient<MainViewModel>();
			services.AddTransient<CalendarViewModel>();
			services.AddSingleton<MonthViewModelFactory>(sp => (year, month)
				=> new MonthViewModel(sp.GetService<DateViewModelFactory>(), year, month));
			services.AddSingleton<DateViewModelFactory>(sp => (date, currentYear, currentMonth)
				=> new DateViewModel(sp.GetService<IDateTimeProvider>(), sp.GetService<IMessenger>(), date, currentYear, currentMonth));

			return builder;
		}
	}
}
