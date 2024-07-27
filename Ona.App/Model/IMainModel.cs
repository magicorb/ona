using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Model
{
	public interface IMainModel
	{
		IReadOnlyList<DateTime> MarkedDates { get; }

		DateTime ObservedEnd { get; set; }

		Task InitializeAsync();

		Task OnInitializedAsync();

		Task AddDateAsync(DateTime date);

		Task DeleteDateAsync(DateTime date);

		IReadOnlyList<DateTimePeriod> MarkedPeriods { get; }

		IReadOnlyList<DateTimePeriod> ExpectedPeriods { get; }

		PeriodStats CurrentStats { get; }

		PeriodStats PreviousStats { get; }
	}
}
