using Microsoft.Maui.Handlers;

namespace Ona.Main;

public static class PropertyMapperConfigurator
{
    public static void Configure()
    {
#if IOS
		ScrollViewHandler.Mapper.AppendToMapping("DisableBounces", (handler, view) => handler.PlatformView.Bounces = false);
#endif
    }
}