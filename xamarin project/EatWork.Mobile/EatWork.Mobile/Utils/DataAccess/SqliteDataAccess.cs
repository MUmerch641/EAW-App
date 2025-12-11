using EatWork.Mobile.Contracts;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EatWork.Mobile.Utils
{
    public class SqliteDataAccess<T> : IAsyncDisposable, IEntityController<T> where T : class, new()
    {
        private static readonly object locker = new object();
        private SQLiteAsyncConnection database;

        public SQLiteAsyncConnection Database
        {
            get => database;
            private set => database = value;
        }

        public SqliteDataAccess()
        {
            this.database = DependencyService.Get<ISQLite>().DbConnection();
            this.database.CreateTableAsync<T>();
        }

        public async Task<IEnumerable<T>> RetrieveList()
        {
            return await this.database.Table<T>().ToListAsync();
        }

        public async Task<T> GetRecord(int id)
        {
            return await this.database.FindAsync<T>(id);
        }

        public async Task DeleteData(T model)
        {
            await this.database.DeleteAsync(model);
        }

        public async Task SaveRecord(T model)
        {
            await this.database.InsertAsync(model);
        }

        public async Task UpdateRecord(T model)
        {
            await this.database.UpdateAsync(model);
        }

        public async ValueTask DisposeAsync()
        {
            await database.CloseAsync();
        }
    }
}