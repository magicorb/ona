namespace Ona.Main.Controls;

public static class DispatcherExtensions
{
    public static Task DoEventsAsync(this IDispatcher dispatcher)
    {
        var tcs = new TaskCompletionSource();
        dispatcher.Dispatch(tcs.SetResult);
        return tcs.Task;
    }
}
