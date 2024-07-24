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
	}
}
