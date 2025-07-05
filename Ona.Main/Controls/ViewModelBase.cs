using CommunityToolkit.Mvvm.ComponentModel;

namespace Ona.Main.Controls;

public abstract class ViewModelBase : ObservableObject, IViewModel
{
    private bool isLoaded;
    private bool isRefreshRequested = true;

    public async Task OnLoadedAsync()
    {
        this.isLoaded = true;

        if (this.isRefreshRequested)
        {
            await RefreshAsync();
            this.isRefreshRequested = false;
        }
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task OnUnloadedAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        => this.isLoaded = false;

    protected async Task RequestRefreshAsync()
    {
        if (this.isLoaded)
            await RefreshAsync();
        else
            this.isRefreshRequested = true;
    }

    protected abstract Task RefreshAsync();
}
