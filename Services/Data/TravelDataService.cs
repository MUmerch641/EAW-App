using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Utils;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class TravelDataService : ITravelDataService
    {
        private readonly IGenericRepository _repository;

        public TravelDataService(IGenericRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TravelRequestListModel>> GetTravelRequestsAsync()
        {
            try
            {
                var profileIdStr = await SecureStorage.GetAsync("profile_id");
                long.TryParse(profileIdStr, out long pid);

                var url = $"{ApiEndpoints.BaseApiUrl}/api/travelrequest/list?ProfileId={pid}&Page=1&Rows=100&SortOrder=0";
                var response = await _repository.GetAsync<TravelListResponseWrapper>(url);
                return response?.ListData ?? new List<TravelRequestListModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetTravelRequestsAsync Error: {ex.Message}");
                return new List<TravelRequestListModel>();
            }
        }

        public async Task<TravelRequestModel> GetTravelRequestByIdAsync(long id)
        {
            return new TravelRequestModel();
        }

        public async Task<bool> SubmitTravelRequestAsync(TravelRequestModel request)
        {
            try
            {
                var profileIdStr = await SecureStorage.GetAsync("profile_id");
                long.TryParse(profileIdStr, out long pid);
                request.ProfileId = pid;
                request.RequestDate = DateTime.Now;

                var payload = new { data = request };
                var response = await _repository.PostAsync<object, LeaveApiResponse>($"{ApiEndpoints.BaseApiUrl}/api/travelrequest", payload);

                return response != null && (response.IsSuccess || response.Model != null || string.IsNullOrEmpty(response.ValidationMessage));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SubmitTravelRequestAsync Error: {ex.Message}");
                return false;
            }
        }

        public async Task<TravelInitModel> GetInitDataAsync()
        {
            try
            {
                var url = $"{ApiEndpoints.BaseApiUrl}/api/travelrequest/init-form";
                // Assuming the response structure matches TravelInitModel or needs mapping
                // For now, let's assume direct mapping or simple wrapper
                var response = await _repository.GetAsync<TravelInitModel>(url);
                return response ?? new TravelInitModel { Origins = new List<string>(), Destinations = new List<string>() };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetInitDataAsync Error: {ex.Message}");
                return new TravelInitModel { Origins = new List<string>(), Destinations = new List<string>() };
            }
        }

        public async Task<List<TripTypeModel>> GetTripTypesAsync()
        {
            try
            {
                var url = $"{ApiEndpoints.BaseApiUrl}/api/enums/get?enumName=TypeOfBusinessTrip";
                
                // Try Wrapper first
                try 
                {
                    var response = await _repository.GetAsync<PaginatedResponse<TripTypeModel>>(url);
                    if (response != null && response.Data != null && response.Data.Any())
                        return response.Data;
                }
                catch { }

                var listResponse = await _repository.GetAsync<List<TripTypeModel>>(url);
                return listResponse ?? new List<TripTypeModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetTripTypesAsync Error: {ex.Message}");
                return new List<TripTypeModel>();
            }
        }
    }

    public class TravelListResponseWrapper
    {
        public List<TravelRequestListModel> ListData { get; set; }
        public bool IsSuccess { get; set; }
    }
}
