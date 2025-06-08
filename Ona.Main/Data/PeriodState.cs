using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.Main.Data
{
	public class PeriodState
	{
		public string startDate { get; set; } = null!;

		public int duration { get; set; }

		public int interval { get; set; }
	}
}
