using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Model
{
	public class PeriodStats
	{
		public PeriodStats(int duration, int interval)
		{
			Duration = duration;
			Interval = interval;
		}

		public int Duration { get; }

		public int Interval { get; }
	}
}
