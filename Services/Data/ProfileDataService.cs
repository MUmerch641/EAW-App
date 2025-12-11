using MauiHybridApp.Services.Data;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class ProfileDataService : IProfileDataService
    {
        private readonly IGenericRepository _repository;

        public ProfileDataService(IGenericRepository repository)
        {
            _repository = repository;
        }

        public async Task<object> GetProfileAsync()
        {
            await Task.Delay(100);
            return new { };
        }

        public async Task<object> UpdateProfileAsync(object profile)
        {
            await Task.Delay(100);
            return new { };
        }
    }
}
