using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.Main.Model
{
	public class CultureInfoProvider : ICultureInfoProvider
	{
		public CultureInfo CurrentUICulture
			=> CultureInfo.CurrentUICulture;
	}
}
