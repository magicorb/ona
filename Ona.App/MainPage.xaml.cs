namespace Ona.App
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		private void ContentPage_Loaded(object sender, EventArgs e)
		{
			BindingContext = Handler.MauiContext.Services.GetService<MainViewModel>();
		}
	}
}