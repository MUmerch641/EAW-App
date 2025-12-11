using MauiHybridApp.Services.Data;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class CommonDataService : ICommonDataService
    {
        private readonly IGenericRepository _repository;

        public CommonDataService(IGenericRepository repository)
        {
            _repository = repository;
        }

        public async Task<object> GetAppSettingsAsync()
        {
            await Task.Delay(100);
            return new { };
        }
    }
}
