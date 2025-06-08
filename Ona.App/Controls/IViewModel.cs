using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Controls
{
	public interface IViewModel
	{
		Task OnLoadedAsync();

		Task OnUnloadedAsync();
	}
}
