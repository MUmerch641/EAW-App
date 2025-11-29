using SQLite;
using MauiHybridApp.Models.DataAccess;
using System.Diagnostics;

namespace MauiHybridApp.Services.Data;

public interface ISQLiteDataService
{
    Task InitializeAsync();
    Task<List<T>> GetAllAsync<T>() where T : class, new();
    Task<T?> GetByIdAsync<T>(object id) where T : class, new();
    Task<int> SaveAsync<T>(T item) where T : class, new();
    Task<int> DeleteAsync<T>(T item) where T : class, new();
    Task<int> DeleteAllAsync<T>() where T : class, new();
    Task<List<T>> QueryAsync<T>(string query, params object[] args) where T : class, new();
    
    // Authentication specific methods
    Task<LoginDataModel?> GetSavedLoginAsync();
    Task SaveLoginAsync(string username, string password);
    Task ClearSavedLoginAsync();
    
    // Cache management
    Task CacheUserDataAsync(string userId, object data, string dataType);
    Task<T?> GetCachedDataAsync<T>(string userId, string dataType) where T : class;
    Task ClearCacheAsync(string userId);
    Task ClearAllCacheAsync();
}

public class SQLiteDataService : ISQLiteDataService
{
    private SQLiteAsyncConnection? _database;
    private readonly string _databasePath;

    public SQLiteDataService()
    {
        _databasePath = Path.Combine(FileSystem.AppDataDirectory, "EverythingAtWork.db3");
    }

    public async Task InitializeAsync()
    {
        if (_database != null)
            return;

        try
        {
            _database = new SQLiteAsyncConnection(_databasePath);

            // Create tables
            await _database.CreateTableAsync<LoginDataModel>();
            await _database.CreateTableAsync<CacheDataModel>();
            await _database.CreateTableAsync<UserPreferenceModel>();
            await _database.CreateTableAsync<OfflineRequestModel>();

            Debug.WriteLine($"SQLite database initialized at: {_databasePath}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error initializing SQLite database: {ex.Message}");
            throw;
        }
    }

    public async Task<List<T>> GetAllAsync<T>() where T : class, new()
    {
        await InitializeAsync();
        return await _database!.Table<T>().ToListAsync();
    }

    public async Task<T?> GetByIdAsync<T>(object id) where T : class, new()
    {
        await InitializeAsync();
        return await _database!.FindAsync<T>(id);
    }

    public async Task<int> SaveAsync<T>(T item) where T : class, new()
    {
        await InitializeAsync();
        
        // Try to get the primary key value
        var type = typeof(T);
        var primaryKeyProperty = type.GetProperties()
            .FirstOrDefault(p => p.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Any());

        if (primaryKeyProperty != null)
        {
            var primaryKeyValue = primaryKeyProperty.GetValue(item);
            if (primaryKeyValue != null && !primaryKeyValue.Equals(GetDefaultValue(primaryKeyProperty.PropertyType)))
            {
                return await _database!.UpdateAsync(item);
            }
        }

        return await _database!.InsertAsync(item);
    }

    public async Task<int> DeleteAsync<T>(T item) where T : class, new()
    {
        await InitializeAsync();
        return await _database!.DeleteAsync(item);
    }

    public async Task<int> DeleteAllAsync<T>() where T : class, new()
    {
        await InitializeAsync();
        return await _database!.DeleteAllAsync<T>();
    }

    public async Task<List<T>> QueryAsync<T>(string query, params object[] args) where T : class, new()
    {
        await InitializeAsync();
        return await _database!.QueryAsync<T>(query, args);
    }

    // Authentication specific methods
    public async Task<LoginDataModel?> GetSavedLoginAsync()
    {
        await InitializeAsync();
        var logins = await _database!.Table<LoginDataModel>().ToListAsync();
        return logins.FirstOrDefault();
    }

    public async Task SaveLoginAsync(string username, string password)
    {
        await InitializeAsync();
        
        // Clear existing login data
        await _database!.DeleteAllAsync<LoginDataModel>();
        
        // Save new login data (password should be encrypted in production)
        var loginData = new LoginDataModel
        {
            UserName = username,
            Password = password, // In production, encrypt this
            CreatedAt = DateTime.UtcNow
        };
        
        await _database!.InsertAsync(loginData);
    }

    public async Task ClearSavedLoginAsync()
    {
        await InitializeAsync();
        await _database!.DeleteAllAsync<LoginDataModel>();
    }

    // Cache management
    public async Task CacheUserDataAsync(string userId, object data, string dataType)
    {
        await InitializeAsync();
        
        var cacheItem = new CacheDataModel
        {
            UserId = userId,
            DataType = dataType,
            Data = System.Text.Json.JsonSerializer.Serialize(data),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(24) // Cache for 24 hours
        };

        // Remove existing cache for this user and data type
        await _database!.ExecuteAsync(
            "DELETE FROM CacheData WHERE UserId = ? AND DataType = ?", 
            userId, dataType);

        await _database!.InsertAsync(cacheItem);
    }

    public async Task<T?> GetCachedDataAsync<T>(string userId, string dataType) where T : class
    {
        await InitializeAsync();
        
        var cacheItem = await _database!.Table<CacheDataModel>()
            .Where(c => c.UserId == userId && c.DataType == dataType)
            .FirstOrDefaultAsync();

        if (cacheItem == null || cacheItem.ExpiresAt < DateTime.UtcNow)
        {
            // Cache expired or doesn't exist
            if (cacheItem != null)
            {
                await _database!.DeleteAsync(cacheItem);
            }
            return null;
        }

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(cacheItem.Data);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deserializing cached data: {ex.Message}");
            await _database!.DeleteAsync(cacheItem);
            return null;
        }
    }

    public async Task ClearCacheAsync(string userId)
    {
        await InitializeAsync();
        await _database!.ExecuteAsync("DELETE FROM CacheData WHERE UserId = ?", userId);
    }

    public async Task ClearAllCacheAsync()
    {
        await InitializeAsync();
        await _database!.DeleteAllAsync<CacheDataModel>();
    }

    private static object? GetDefaultValue(Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }
}
