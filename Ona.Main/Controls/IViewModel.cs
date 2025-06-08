using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.Main.Controls
{
	public interface IViewModel
	{
		Task OnLoadedAsync();

		Task OnUnloadedAsync();
	}
}
