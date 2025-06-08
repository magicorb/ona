using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Calendar
{
	public class DateToggledMessage
	{
		public DateToggledMessage(DateTime date)
		{
			Date = date;
		}

		public DateTime Date { get; }
	}
}
