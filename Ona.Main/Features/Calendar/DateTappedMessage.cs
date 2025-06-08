using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.Main.Features.Calendar
{
	public class DateTappedMessage
	{
		public DateTappedMessage(DateTime date)
		{
			Date = date;
		}

		public DateTime Date { get; }
	}
}
