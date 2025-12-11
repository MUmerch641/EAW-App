using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Utils;
using EAW.API.DataContracts.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services.SuggestionCorner
{
    public class SuggestionListDataService : ISuggestionListDataService
    {
        public long TotalListItem { get; set; }

        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonService_;
        private readonly StringHelper string_;

        public SuggestionListDataService(IGenericRepository genericRepository,
            ICommonDataService commonService)
        {
            genericRepository_ = genericRepository;
            commonService_ = commonService;
            string_ = new StringHelper();
        }

        public async Task<ObservableCollection<SuggestionListDto>> GetListAsync(ObservableCollection<SuggestionListDto> list, ListParam args)
        {
            try
            {
                var url = await commonService_.RetrieveClientUrl();
                await commonService_.HasInternetConnection(url);

                var user = PreferenceHelper.UserInfo();

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.Suggestion}/list"
                };

                DateTime? startDate = Constants.NullDate;
                DateTime? endDate = Constants.NullDate;

                if (!string.IsNullOrWhiteSpace(args.StartDate))
                    startDate = Convert.ToDateTime(args.StartDate);

                if (!string.IsNullOrWhiteSpace(args.EndDate))
                    endDate = Convert.ToDateTime(args.EndDate);

                var param = new R.Requests.SuggestionListRequest
                {
                    EndDate = endDate,
                    StartDate = startDate,
                    Keyword = args.KeyWord,
                    Page = (args.ListCount == 0 ? 1 : ((args.ListCount + args.Count) / args.Count)),
                    Rows = args.Count,
                    SortOrder = (args.IsAscending ? 0 : 1),
                    ProfileId = user.ProfileId,
                };

                var request = string_.CreateUrl<R.Requests.SuggestionListRequest>(builder.ToString(), param);

                var response = await genericRepository_.GetAsync<R.Responses.ListResponse<R.Models.SuggestionListDto>>(request);
                args.Count = (response.ListData.Count <= args.Count ? response.ListData.Count : args.Count);

                if (response.TotalListCount != 0)
                {
                    foreach (var item in response.ListData)
                    {
                        list.Add(new SuggestionListDto()
                        {
                            Category = item.Category,
                            CreateDate = item.CreateDate,
                            ProfileId = item.ProfileId,
                            SuggestionDetail = item.SuggestionDetail,
                            EmployeeName = item.EmployeeName,
                        });
                    }
                }

                TotalListItem = response.TotalListCount;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name}");
                throw ex;
            }

            return list;
        }
    }
}