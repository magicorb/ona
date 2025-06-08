using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.Main.Data;

public interface IDataPublisher
{
	Task StartAsync();
}
