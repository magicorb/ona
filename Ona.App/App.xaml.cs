using Ona.App.Data;
using System;
using System.Runtime.InteropServices;

namespace Ona.App
{
	public partial class App : Application
	{
#if IOS
		[DllImport("__Internal", EntryPoint = "ReloadWidgets")]
		public extern static int ReloadWidgets();
#endif

		public App(AppShell appShell)
		{
			InitializeComponent();

			PropertyMapperConfigurator.Configure();

			MainPage = appShell;
		}

		protected override void OnStart()
		{
			base.OnStart();

			var dataPublisher = Handler.MauiContext.Services.GetService<IDataPublisher>();
			_ = dataPublisher.StartAsync();
		}

		protected override void OnSleep()
		{
			base.OnSleep();
#if IOS
			ReloadWidgets();
#endif
		}
	}
}
