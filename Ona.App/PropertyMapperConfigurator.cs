using Microsoft.Maui.Handlers;

namespace Ona.App
{
	public static class PropertyMapperConfigurator
	{
		public static void Configure()
		{
			ScrollViewHandler.Mapper.AppendToMapping(
				"DisableBounces",
				(handler, view) =>
				{
					handler.PlatformView.Bounces = false;
				});
		}
	}
}