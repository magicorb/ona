using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.Messaging;
using Ona.App.Controls;
using Ona.App.Data;
using Ona.App.Features.Calendar;
using Ona.App.Features.Insights;
using Ona.App.Features.Settings;
using Ona.App.Features.Today;
using Ona.App.Model;

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

			services.AddSingleton<Func<Page, IUserNotificationService>>(sp => page
				=> new UserNotificationService(page));

			services.AddSingleton(FilePicker.Default);

			services.AddSingleton(FileSaver.Default);

			services.AddSingleton<IMainModel, MainModel>();

			services.AddTransient<TodayViewModel>();
			
			services.AddTransient<CalendarViewModel>();
			
			services.AddSingleton<MonthViewModelFactory>(sp => (year, month)
				=> new MonthViewModel(sp.GetService<DateViewModelFactory>(), year, month));
			
			services.AddSingleton<DateViewModelFactory>(sp => (date, monthViewModel, currentYear, currentMonth)
				=> new DateViewModel(sp.GetService<IDateTimeProvider>(), sp.GetService<IMessenger>(), date, monthViewModel, currentYear, currentMonth));

			services.AddTransient<InsightsViewModel>();

			services.AddTransient<SettingsViewModel>();

			services.AddSingleton<AppShellViewModel>();

			services.AddSingleton<AppShell>();

			return builder;
		}
	}
}
