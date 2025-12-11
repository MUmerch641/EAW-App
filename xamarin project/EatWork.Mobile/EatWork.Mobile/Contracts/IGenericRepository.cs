using System;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IGenericRepository : IDisposable
    {
        Task<T> GetAsync<T>(string uri, string authToken = "");

        Task<T> PostAsync<T>(string uri, T data, string authToken = "");

        Task<T> PutAsync<T>(string uri, T data, string authToken = "");

        Task DeleteAsync(string uri, string authToken = "");

        Task<R> PostAsync<T, R>(string uri, T data, string authToken = "");

        Task<R> PutAsync<T, R>(string path, T data, string authToken = "");
    }
}