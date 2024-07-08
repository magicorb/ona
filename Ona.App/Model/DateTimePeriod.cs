using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Model
{
	public class DateTimePeriod
	{
		public DateTime Start { get; set; }

		public DateTime End { get; set; }

		public TimeSpan Length => End - Start;
	}
}
