using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using Microsoft.Maui.Storage;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class DashboardDataService : IDashboardDataService
    {
        private readonly IGenericRepository _repository;

        public DashboardDataService(IGenericRepository repository)
        {
            _repository = repository;
        }

        public async Task<DashboardResponse> GetDashboardAsync()
        {
            var profileId = await SecureStorage.GetAsync("profile_id");
            
            // Agar profileId na mile to return null
            if (string.IsNullOrEmpty(profileId)) return new DashboardResponse();

            // API Call
            var result = await _repository.GetAsync<DashboardResponse>($"api/v1/dashboard/{profileId}/default");
            
            return result ?? new DashboardResponse();
        }
    }
}
