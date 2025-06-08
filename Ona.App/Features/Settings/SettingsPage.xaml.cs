using CommunityToolkit.Maui.Storage;
using Ona.App.Controls;
using Ona.App.Data;
using System;

namespace Ona.App.Features.Settings;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
	}

	private void ContentPage_Loaded(object sender, EventArgs e)
	{
		var userConfirmationServiceFactory = Handler.MauiContext.Services.GetService<Func<Page, IUserNotificationService>>();

		((SettingsViewModel)BindingContext).UserNotificationService = userConfirmationServiceFactory(this);
	}
}