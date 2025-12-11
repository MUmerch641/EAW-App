using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.iOS.Helpers;
using SQLite;
using System;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(SqliteDataAccess))]

namespace EatWork.Mobile.iOS.Helpers
{
    public class SqliteDataAccess : ISQLite
    {
        public SQLiteAsyncConnection DbConnection()
        {
            string personalFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libraryFolder = Path.Combine(personalFolder, "..", "Library");
            var path = Path.Combine(libraryFolder, Constants.DatabaseName);
            return new SQLiteAsyncConnection(path);
        }
    }
}