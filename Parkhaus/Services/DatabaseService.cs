using SQLite;
using Parkhaus.Models;

namespace Parkhaus.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;

    private async Task Init()
    {
        if (_database is not null) return;
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "ParkingData.db3");
        _database = new SQLiteAsyncConnection(dbPath);
        await _database.CreateTableAsync<ParkingEntry>();
    }

    public async Task<List<ParkingEntry>> GetActiveEntriesAsync()
    {
        await Init();
        return await _database.Table<ParkingEntry>().Where(i => i.IsActive).ToListAsync();
    }

    public async Task<ParkingEntry?> GetActiveEntryByLicensePlateAsync(string licensePlate)
    {
        await Init();
        return await _database.Table<ParkingEntry>()
            .Where(e => e.LicensePlate == licensePlate && e.IsActive == true)
            .FirstOrDefaultAsync();
    }

    public async Task<int> GetActiveCountAsync()
    {
        await Init();
        return await _database.Table<ParkingEntry>()
            .Where(e => e.IsActive == true)
            .CountAsync();
    }

    public async Task<int> SaveEntryAsync(ParkingEntry entry)
    {
        await Init();
        if (entry.Id != 0)
            return await _database.UpdateAsync(entry);
        else
            return await _database.InsertAsync(entry);
    }
}