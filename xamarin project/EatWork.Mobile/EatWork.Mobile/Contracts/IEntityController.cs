using System.Collections.Generic;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IEntityController<T> where T : class, new()
    {
        Task<IEnumerable<T>> RetrieveList();

        Task<T> GetRecord(int id);

        Task DeleteData(T model);

        Task SaveRecord(T model);

        Task UpdateRecord(T model);
    }
}