using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Controls
{
	public class ListItemAddedEventArgs
	{
		public ListItemAddedEventArgs(int index, BindableObject item)
		{
			Index = index;
			Item = item;
		}

		public int Index { get; }

		public BindableObject Item { get; }
	}
}
