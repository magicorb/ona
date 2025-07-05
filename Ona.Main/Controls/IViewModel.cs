namespace Ona.Main.Controls;

public interface IViewModel
{
    Task OnLoadedAsync();

    Task OnUnloadedAsync();
}
