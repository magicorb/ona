namespace Ona.Main.Controls;

public interface IUserNotificationService
{
    Task<bool> ConfirmAsync(string title, string message, string accept, string cancel);

    Task NotifyAsync(string message);
}
