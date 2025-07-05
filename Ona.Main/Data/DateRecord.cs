using SQLite;

namespace Ona.Main.Data;

public class DateRecord
{
    [PrimaryKey]
    public Guid Id { get; set; }

    public DateTime Date { get; set; }
}
