using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Utils;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class DocumentDataService : IDocumentDataService
    {
        private readonly IGenericRepository _repository;

        public DocumentDataService(IGenericRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<DocumentRequestListModel>> GetDocumentRequestsAsync()
        {
            try
            {
                var profileIdStr = await SecureStorage.GetAsync("profile_id");
                long.TryParse(profileIdStr, out long pid);

                var url = $"{ApiEndpoints.BaseApiUrl}/api/documentrequest/list?ProfileId={pid}&Page=1&Rows=100&SortOrder=0";
                var response = await _repository.GetAsync<DocumentListResponseWrapper>(url);
                return response?.ListData ?? new List<DocumentRequestListModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetDocumentRequestsAsync Error: {ex.Message}");
                return new List<DocumentRequestListModel>();
            }
        }

        public async Task<DocumentRequestModel> GetDocumentRequestByIdAsync(long id)
        {
            return new DocumentRequestModel();
        }

        public async Task<bool> SubmitDocumentRequestAsync(DocumentRequestModel request)
        {
            try
            {
                var profileIdStr = await SecureStorage.GetAsync("profile_id");
                long.TryParse(profileIdStr, out long pid);
                request.ProfileId = pid;
                request.RequestDate = DateTime.Now;

                var payload = new { data = request };
                var response = await _repository.PostAsync<object, LeaveApiResponse>($"{ApiEndpoints.BaseApiUrl}/api/documentrequest", payload);

                return response != null && (response.IsSuccess || response.Model != null || string.IsNullOrEmpty(response.ValidationMessage));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SubmitDocumentRequestAsync Error: {ex.Message}");
                return false;
            }
        }

        public async Task<List<DocumentTypeModel>> GetDocumentTypesAsync()
        {
            try
            {
                var url = $"{ApiEndpoints.BaseApiUrl}/api/documenttype/list";
                var response = await _repository.GetAsync<DocumentTypeListResponseWrapper>(url);
                return response?.ListData ?? new List<DocumentTypeModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetDocumentTypesAsync Error: {ex.Message}");
                return new List<DocumentTypeModel>();
            }
        }

        public async Task<List<ReasonModel>> GetReasonsAsync()
        {
            try
            {
                var url = $"{ApiEndpoints.BaseApiUrl}/api/reasonpurpose/list";
                var response = await _repository.GetAsync<ReasonListResponseWrapper>(url);
                return response?.ListData ?? new List<ReasonModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetReasonsAsync Error: {ex.Message}");
                return new List<ReasonModel>();
            }
        }
    }

    public class DocumentListResponseWrapper
    {
        public List<DocumentRequestListModel> ListData { get; set; }
        public bool IsSuccess { get; set; }
    }

    public class DocumentTypeListResponseWrapper
    {
        public List<DocumentTypeModel> ListData { get; set; }
        public bool IsSuccess { get; set; }
    }

    public class ReasonListResponseWrapper
    {
        public List<ReasonModel> ListData { get; set; }
        public bool IsSuccess { get; set; }
    }
}
