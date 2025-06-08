using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.Messaging;
using Ona.Main.Controls;
using Ona.Main.Data;
using Ona.Main.Features.Calendar;
using Ona.Main.Features.Insights;
using Ona.Main.Features.Settings;
using Ona.Main.Features.Today;
using Ona.Main.Model;

namespace Ona.Main;

public static class DependencyInjectionExtensions
{
	public static MauiAppBuilder RegisterAll(this MauiAppBuilder builder)
	{
		var services = builder.Services;

		services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

		services.AddSingleton<ICultureInfoProvider, CultureInfoProvider>();
		
		services.AddSingleton<SQLiteDateRepository>();
		
		services.AddSingleton<IDateRepository, SQLiteDateRepository>();
		
		services.AddSingleton<IMessenger, WeakReferenceMessenger>();

		services.AddSingleton<Func<Page, IUserNotificationService>>(sp => page
			=> new UserNotificationService(page));

		services.AddSingleton(FilePicker.Default);

		services.AddSingleton(FileSaver.Default);

		services.AddSingleton<IMainModel, MainModel>();

		services.AddSingleton<IDataPublisher, DataPublisher>();

		services.AddTransient<TodayViewModel>();
		
		services.AddTransient<CalendarViewModel>();
		
		services.AddSingleton<MonthViewModelFactory>(sp => (year, month, currentYear)
			=> new MonthViewModel(
				sp.GetService<ICultureInfoProvider>()!,
				sp.GetService<DateViewModelFactory>()!,
				year,
				month,
				currentYear));
		
		services.AddSingleton<DateViewModelFactory>(sp => (date, monthViewModel, currentYear, currentMonth)
			=> new DateViewModel(sp.GetService<IDateTimeProvider>()!, sp.GetService<IMessenger>()!, date, monthViewModel, currentYear, currentMonth));

		services.AddTransient<InsightsViewModel>();

		services.AddTransient<SettingsViewModel>();

		services.AddSingleton<AppShellViewModel>();

		services.AddSingleton<AppShell>();

		return builder;
	}
}
