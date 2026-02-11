using SQLite;

namespace Parkhaus.Models;

public class ParkingEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [SQLite.NotNull] //NotNull = mehrdeutiger Verweis = error
    public string LicensePlate { get; set; } = string.Empty;

    public DateTime EntryTime { get; set; }

    public DateTime? ExitTime { get; set; }

    public decimal TotalFee { get; set; }

    public bool IsActive { get; set; }
}