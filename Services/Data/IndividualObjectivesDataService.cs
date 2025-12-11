using MauiHybridApp.Models;
using MauiHybridApp.Models.DataObjects;
using MauiHybridApp.Utils;
using MauiHybridApp.Models.IndividualObjectives;
using MauiHybridApp.Services.Data;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class IndividualObjectivesDataService : IIndividualObjectivesDataService
    {
        private readonly IGenericRepository _repository;

        public IndividualObjectivesDataService(IGenericRepository repository)
        {
            _repository = repository;
        }

        public long TotalListItem { get; set; }

        public async Task<ListResponse<IndividualObjectivesDto>> GetListAsync(ObservableCollection<IndividualObjectivesDto> currentList, ListParam param)
        {
            try
            {
                var profileIdStr = await SecureStorage.GetAsync("profile_id");
                long.TryParse(profileIdStr, out long pid);

                var request = new MyApprovalRequest
                {
                    ProfileId = pid,
                    Page = (param.ListCount == 0 ? 1 : ((param.ListCount + param.Count) / param.Count)),
                    Rows = param.Count,
                    SortOrder = (param.IsAscending ? 0 : 1),
                    Keyword = param.KeyWord,
                    TransactionTypes = param.FilterTypes,
                    StartDate = param.StartDate,
                    EndDate = param.EndDate,
                    Status = param.Status,
                };

                var queryString = $"?ProfileId={request.ProfileId}&Page={request.Page}&Rows={request.Rows}&SortOrder={request.SortOrder}&Keyword={request.Keyword}&Status={request.Status}&TransactionTypes={request.TransactionTypes}&StartDate={request.StartDate}&EndDate={request.EndDate}";
                var url = $"{ApiEndpoints.IndividualObjectives}/list{queryString}";

                var response = await _repository.GetAsync<ListResponse<EmployeeIndividualObjectiveList>>(url);

                var result = new ListResponse<IndividualObjectivesDto>();
                
                if (response != null && response.TotalCount > 0)
                {
                    TotalListItem = response.TotalCount;
                    result.TotalCount = response.TotalCount;
                    result.ListData = new List<IndividualObjectivesDto>();

                    if (response.ListData != null)
                    {
                        foreach (var item in response.ListData)
                        {
                            result.ListData.Add(new IndividualObjectivesDto
                            {
                                StatusId = item.StatusId,
                                DatePrepared = item.DatePrepared,
                                Details = item.Details,
                                EffectiveYear = item.EffectiveYear.GetValueOrDefault(),
                                IndividualOjbectiveId = item.RecordId,
                                Period = item.Period,
                                ProfileId = item.ProfileId,
                                Status = item.Status,
                                DatePrepared_String = item.DatePrepared.ToString("MMM dd, yyyy"),
                                IsChecked = false,
                                Header = item.Header,
                            });
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IndividualObjectives GetList Error: {ex.Message}");
                throw;
            }
        }
    }
}
