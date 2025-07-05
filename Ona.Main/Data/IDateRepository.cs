namespace Ona.Main.Data;

public interface IDateRepository
{
    Task<DateRecord[]> GetDateRecordsAsync();

    Task<DateRecord> AddDateRecordAsync(DateTime date);

    Task DeleteDateRecordAsync(DateTime date);

    Task DeleteAllDateRecordsAsync();
}
