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

		IReadOnlyList<DateTimePeriod> MarkedPeriods { get; }

		IReadOnlyList<DateTimePeriod> ExpectedPeriods { get; }

		int ExpectedDuration { get; }

		int ExpectedInterval { get; }

		Task InitializeAsync();

		Task OnInitializedAsync();

		Task AddDateAsync(DateTime date);

		Task DeleteDateAsync(DateTime date);

		Task AddDraftDateAsync(DateTime date);

		void CompleteDraftDate(DateTime date);

		Task DeleteAllAsync();

		Task ImportAsync(IEnumerable<DateTime> dates);
	}
}
