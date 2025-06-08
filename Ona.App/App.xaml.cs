using Ona.App.Data;

namespace Ona.App
{
	public partial class App : Application
	{
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
	}
}
