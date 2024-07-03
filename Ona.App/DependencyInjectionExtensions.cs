using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Ona.App.Calendar;
using Ona.App.Controls;
using Ona.App.Data;
using Ona.App.Insights;
using Ona.App.Model;
using Ona.App.Settings;
using Ona.App.Today;
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
			
			services.AddSingleton<IPeriodStatsProvider, PeriodStatsProvider>();
			
			services.AddSingleton<IMessenger, WeakReferenceMessenger>();

			services.AddSingleton<Func<Page, IUserConfirmationService>>(sp => page
				=> new UserConfirmationService(page));

			services.AddTransient<TodayViewModel>();
			
			services.AddTransient<CalendarViewModel>();
			
			services.AddSingleton<MonthViewModelFactory>(sp => (year, month)
				=> new MonthViewModel(sp.GetService<DateViewModelFactory>(), year, month));
			
			services.AddSingleton<DateViewModelFactory>(sp => (date, monthViewModel, currentYear, currentMonth)
				=> new DateViewModel(sp.GetService<IDateTimeProvider>(), sp.GetService<IMessenger>(), date, monthViewModel, currentYear, currentMonth));

			services.AddTransient<InsightsViewModel>();

			return builder;
		}
	}
}
