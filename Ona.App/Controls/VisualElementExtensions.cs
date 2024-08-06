using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Controls
{
	public static class VisualElementExtensions
	{
		public static void RegisterViewModel(this VisualElement element)
		{
			var bindingContext = default(object);

			element.BindingContextChanged += (sender, __) =>
			{
				// Possibly a bug in MAUI, nested binding results in same context applied twice
				if (element.BindingContext != bindingContext)
				{
					bindingContext = element.BindingContext;
					element.RegisterViewModelInternal();
				}
			};
			
			element.RegisterViewModelInternal();
		}

		private static void RegisterViewModelInternal(this VisualElement element)
		{
			if (!(element.BindingContext is IViewModel viewModel))
				return;

			element.Loaded += (_, __) => _ = viewModel.OnLoadedAsync();
			element.Unloaded += (_, __) => _ = viewModel.OnUnloadedAsync();

			if (element.IsLoaded)
				_ = viewModel.OnLoadedAsync();
		}
	}
}
