using SQLite;

namespace MauiHybridApp.Services.Platform;

public interface ISQLite
{
    SQLiteAsyncConnection GetConnection();
}

public class SQLiteImplementation : ISQLite
{
    public SQLiteAsyncConnection GetConnection()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "EatWork.db3");
        return new SQLiteAsyncConnection(dbPath);
    }
}

