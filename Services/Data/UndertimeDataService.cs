using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Utils;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class UndertimeDataService : IUndertimeDataService
    {
        private readonly IGenericRepository _repository;

        public UndertimeDataService(IGenericRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<UndertimeRequestListModel>> GetUndertimeRequestsAsync()
        {
            try
            {
                var profileIdStr = await SecureStorage.GetAsync("profile_id");
                long.TryParse(profileIdStr, out long pid);

                var url = $"{ApiEndpoints.BaseApiUrl}/api/undertime/list?ProfileId={pid}&Page=1&Rows=100&SortOrder=0";
                var response = await _repository.GetAsync<UndertimeListResponseWrapper>(url);
                return response?.ListData ?? new List<UndertimeRequestListModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetUndertimeRequestsAsync Error: {ex.Message}");
                return new List<UndertimeRequestListModel>();
            }
        }

        public async Task<UndertimeRequestModel> GetUndertimeRequestByIdAsync(long id)
        {
            return new UndertimeRequestModel();
        }

        public async Task<bool> SubmitUndertimeRequestAsync(UndertimeRequestModel request)
        {
            try
            {
                var profileIdStr = await SecureStorage.GetAsync("profile_id");
                long.TryParse(profileIdStr, out long pid);
                request.ProfileId = pid;

                var payload = new { data = request };
                var response = await _repository.PostAsync<object, LeaveApiResponse>($"{ApiEndpoints.BaseApiUrl}/api/undertime", payload);

                return response != null && (response.IsSuccess || response.Model != null || string.IsNullOrEmpty(response.ValidationMessage));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SubmitUndertimeRequestAsync Error: {ex.Message}");
                return false;
            }
        }

        public async Task<List<UndertimeTypeModel>> GetUndertimeTypesAsync()
        {
            try
            {
                var url = $"{ApiEndpoints.BaseApiUrl}/api/enums/get?enumName=UndertimeType";
                
                // Helper to process dynamic list
                List<dynamic> items = null;

                // Try Wrapper first
                try 
                {
                    var response = await _repository.GetAsync<PaginatedResponse<dynamic>>(url);
                    if (response != null && response.Data != null)
                        items = response.Data;
                }
                catch { }

                // If wrapper failed or returned null, try direct list
                if (items == null)
                {
                    items = await _repository.GetAsync<List<dynamic>>(url);
                }
                
                var list = new List<UndertimeTypeModel>();
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        long id = 0;
                        string val = "";
                        
                        // Handle JObject or dynamic properties
                        try 
                        { 
                            // Check if item is JObject (Newtonsoft)
                            if (item.GetType().Name.Contains("JObject"))
                            {
                                id = (long)item["Value"];
                                val = (string)item["DisplayText"];
                            }
                            else
                            {
                                id = Convert.ToInt64(item.Value); 
                                val = item.DisplayText;
                            }
                        } 
                        catch { /* Ignore parse errors */ }

                        if (id != 0)
                            list.Add(new UndertimeTypeModel { Id = id, Value = val });
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetUndertimeTypesAsync Error: {ex.Message}");
                return new List<UndertimeTypeModel>();
            }
        }
    }

    public class UndertimeListResponseWrapper
    {
        public List<UndertimeRequestListModel> ListData { get; set; }
        public bool IsSuccess { get; set; }
    }
}
