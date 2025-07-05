using CommunityToolkit.Mvvm.ComponentModel;

namespace Ona.Main.Features.Calendar;

public class SpinnerViewModel : ObservableObject
{
    private bool isRunning;

    public bool IsRunning { get => this.isRunning; set => SetProperty(ref this.isRunning, value); }
}
