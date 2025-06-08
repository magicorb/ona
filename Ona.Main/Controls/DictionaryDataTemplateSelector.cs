using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.Main.Controls
{
	public class DictionaryDataTemplateSelector : DataTemplateSelector
	{
		public ResourceDictionary Templates { get; set; } = new ResourceDictionary();

		protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
		{
			var typeName = item.GetType().Name;
			if (Templates.TryGetValue(typeName, out var template))
				return (DataTemplate)template;
			return null!;
		}
	}
}
