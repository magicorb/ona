using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Ona.App.Data
{
	public class DateRecord
	{
		[PrimaryKey]
		public Guid Id { get; set; }

		public DateTime Date { get; set; }
	}
}
