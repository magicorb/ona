namespace Ona.App
{
	public partial class AppShell : Shell
	{
		public AppShell(AppShellViewModel viewModel)
		{
			InitializeComponent();

			BindingContext = viewModel;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			await ((AppShellViewModel)BindingContext).InitializeAsync();
		}
	}
}
