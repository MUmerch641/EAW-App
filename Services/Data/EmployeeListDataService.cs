using MauiHybridApp.Models;
using MauiHybridApp.Models.DataObjects;
using MauiHybridApp.Models.Requests;
using MauiHybridApp.Utils;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class EmployeeListDataService : IEmployeeDataService
    {
        private readonly IGenericRepository _repository;
        private readonly ICommonDataService _commonDataService;

        public EmployeeListDataService(IGenericRepository repository, ICommonDataService commonDataService)
        {
            _repository = repository;
            _commonDataService = commonDataService;
        }

        public async Task<ObservableCollection<EmployeeListModel>> RetrieveEmployeeList(ObservableCollection<EmployeeListModel> list, ListParam obj)
        {
            try
            {
                var request = new GetEmployeeListRequest
                {
                    Page = (obj.ListCount == 0 ? 1 : ((obj.ListCount + obj.Count) / obj.Count)) + 1,
                    Rows = obj.Count,
                    SortOrder = (obj.IsAscending ? 0 : 1),
                    Keyword = obj.KeyWord ?? string.Empty,
                    BranchId = 0,
                    DepartmentId = 0,
                    TeamId = 0
                };

                // Construct Query String for GET request
                var queryString = $"?Page={request.Page}&Rows={request.Rows}&SortOrder={request.SortOrder}";
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    queryString += $"&Keyword={Uri.EscapeDataString(request.Keyword)}";
                }
                queryString += $"&BranchId={request.BranchId}&DepartmentId={request.DepartmentId}&TeamId={request.TeamId}";

                var url = $"{ApiEndpoints.GetEmployeeList}{queryString}";

                // Use GET instead of POST
                var response = await _repository.GetAsync<EmployeeListResponse>(url);

                if (response != null && response.ListData != null)
                {
                    foreach (var emp in response.ListData)
                    {
                        list.Add(emp);
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving employees: {ex.Message}");
                return list;
            }
        }
    }

    // Response model for Employee List API
    public class EmployeeListResponse
    {
        public List<EmployeeListModel>? ListData { get; set; }
        public bool IsSuccess { get; set; }
    }
}
