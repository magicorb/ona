using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Model
{
	public class DatesChangedMessage
	{
		public DatesChangedMessage(object sender)
		{
			Sender = sender;
		}

		public object Sender { get; }
	}
}
