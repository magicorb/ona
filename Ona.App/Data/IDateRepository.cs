using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Data
{
	public interface IDateRepository
	{
		Task<DateRecord[]> GetDateRecordsAsync();

		Task<DateRecord> AddDateRecordAsync(DateTime date);

		Task DeleteDateRecordAsync(DateTime date);

		Task DeleteAllDateRecordsAsync();
	}
}
