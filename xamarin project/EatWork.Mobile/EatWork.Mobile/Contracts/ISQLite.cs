using SQLite;

namespace EatWork.Mobile.Contracts
{
    public interface ISQLite
    {
        //SQLiteConnection DbConnection();
        SQLiteAsyncConnection DbConnection();
    }
}