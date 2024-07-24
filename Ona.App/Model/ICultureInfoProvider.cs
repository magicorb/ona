using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Model
{
	public interface ICultureInfoProvider
	{
		CultureInfo CurrentUICulture { get; }
	}
}
