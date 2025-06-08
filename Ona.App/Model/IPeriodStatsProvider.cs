using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Model
{
	public interface IPeriodStatsProvider
	{
		IEnumerator<DateTimePeriod> GetExpectedPeriodsEnumerator(IReadOnlyList<DateTimePeriod> orderedPeriods);

		IReadOnlyList<DateTimePeriod> GetDatePeriods(IEnumerable<DateTime> orderedDates);

		int GetAverageDuration(IEnumerable<DateTimePeriod> orderedPeriods);

		int GetAverageInterval(IReadOnlyList<DateTimePeriod> orderedPeriods);
	}
}
