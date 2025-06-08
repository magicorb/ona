using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Data
{
	public interface IDataPublisher
	{
		Task StartAsync();
	}
}
