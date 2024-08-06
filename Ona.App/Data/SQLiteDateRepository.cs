using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Data
{
	public class SQLiteDateRepository : IDateRepository
	{
		private readonly SemaphoreSlim initializeSemaphore = new SemaphoreSlim(1);
		private SQLiteAsyncConnection connection = null!;
		private bool isInitialized;

		public async Task<DateRecord> AddDateRecordAsync(DateTime date)
		{
			await EnsureInitializeAsync();

			var result = new DateRecord
			{
				Id = Guid.NewGuid(),
				Date = date
			};

			await this.connection.InsertAsync(result);

			return result;
		}

		public async Task DeleteAllDateRecordsAsync()
		{
			await EnsureInitializeAsync();

			await this.connection.Table<DateRecord>().DeleteAsync(d => true);
		}

		public async Task DeleteDateRecordAsync(DateTime date)
		{
			await EnsureInitializeAsync();

			await this.connection.Table<DateRecord>().DeleteAsync(d => d.Date == date);
		}

		public async Task<DateRecord[]> GetDateRecordsAsync()
		{
			await EnsureInitializeAsync();

			return await this.connection.Table<DateRecord>().OrderBy(d => d.Date).ToArrayAsync();
		}

		private async Task EnsureInitializeAsync()
		{
			await initializeSemaphore.WaitAsync();

			try
			{
				if (this.isInitialized)
					return;

				this.connection = new SQLiteAsyncConnection(
					Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Database.db3"),
					SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

				await this.connection.CreateTablesAsync(CreateFlags.None, typeof(DateRecord));

				this.isInitialized = true;
			}
			finally
			{
				initializeSemaphore.Release();
			}
		}
	}
}
