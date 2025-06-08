using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.Main.Controls;

public class ListItemAddedEventArgs
{
	public ListItemAddedEventArgs(int index, object item)
	{
		Index = index;
		Item = item;
	}

	public int Index { get; }

	public object Item { get; }
}
