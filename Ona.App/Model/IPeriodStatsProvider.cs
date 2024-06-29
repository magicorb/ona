using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Model
{
	public interface IPeriodStatsProvider
	{
		DateTimePeriod? GetNextPeriod(DateTime[] orderedDates);
	}
}
