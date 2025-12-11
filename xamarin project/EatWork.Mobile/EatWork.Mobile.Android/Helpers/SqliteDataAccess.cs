using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Droid.Helpers;
using SQLite;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(SqliteDataAccess))]

namespace EatWork.Mobile.Droid.Helpers
{
    public class SqliteDataAccess : ISQLite
    {
        public SQLiteAsyncConnection DbConnection()
        {
            var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Constants.DatabaseName);
            return new SQLiteAsyncConnection(path);
        }
    }
}