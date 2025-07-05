using CommunityToolkit.Maui.Animations;

namespace Ona.Main.Controls;

public class FadeToAnimation : BaseAnimation
{
    public double Opacity { get; set; }

    public override async Task Animate(VisualElement view, CancellationToken token = default)
        => await view.FadeTo(Opacity, Length, Easing);
}
