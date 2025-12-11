using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.PerformanceEvaluation;
using EatWork.Mobile.Utils;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using APIM = EAW.API.DataContracts;

namespace EatWork.Mobile.Services
{
    public class PEListDataService : IPEListDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly StringHelper string_;

        public long TotalListItem { get; set; }

        public PEListDataService()
        {
            genericRepository_ = AppContainer.Resolve<IGenericRepository>();
            commonDataService_ = AppContainer.Resolve<ICommonDataService>();
            string_ = AppContainer.Resolve<StringHelper>();
        }

        public async Task<ObservableCollection<PEListDto>> GetListAsync(ObservableCollection<PEListDto> list, ListParam args)
        {
            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);
                var userinfo = PreferenceHelper.UserInfo();

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.PerformanceEvaluation}/list-eaw-app"
                };

                var param = new APIM.Requests.MyApprovalRequest
                {
                    ProfileId = userinfo.ProfileId,
                    Page = (args.ListCount == 0 ? 1 : ((args.ListCount + args.Count) / args.Count)),
                    Rows = args.Count,
                    SortOrder = (args.IsAscending ? 0 : 1),
                    Keyword = args.KeyWord,
                    TransactionTypes = args.FilterTypes,
                    StartDate = args.StartDate,
                    EndDate = args.EndDate,
                    Status = args.Status,
                };

                var request = string_.CreateUrl<APIM.Requests.MyApprovalRequest>(builder.ToString(), param);

                var response = await genericRepository_.GetAsync<APIM.Responses.ListResponse<APIM.Models.PerformanceEvaluationList>>(request);
                args.Count = (response.ListData.Count <= args.Count ? response.ListData.Count : args.Count);

                if (response.TotalListCount != 0)
                {
                    foreach (var item in response.ListData)
                    {
                        list.Add(new PEListDto()
                        {
                            DueDate = item.DueDate,
                            DueDate_String = item.DueDate.ToString(Constants.DateFormatMMDDYYYY),
                            EvaluationType = item.EvaluationType,
                            PeriodCovered = item.PeriodCovered,
                            PeriodEndDate = item.PeriodEnd,
                            PeriodStartDate = item.PeriodStart,
                            ProfileId = item.ProfileId,
                            RecordId = item.RecordId,
                            ScheduledDate = item.ScheduledDate,
                            ScheduledEndDate = item.ScheduleStart,
                            ScheduledStartDate = item.ScheduleEnd,
                            Status = item.Status,
                            StatusId = item.StatusId,
                        });
                    }
                }

                TotalListItem = response.TotalListCount;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name + " : " + ex.Message}");
                throw ex;
            }

            return list;
        }
    }
}