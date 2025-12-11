using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.IndividualObjectives;
using EatWork.Mobile.Utils;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using APIM = EAW.API.DataContracts;

namespace EatWork.Mobile.Services
{
    public class IndividualObjectivesDataService : IIndividualObjectivesDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly StringHelper string_;

        public IndividualObjectivesDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            string_ = new StringHelper();
        }

        public long TotalListItem { get; set; }

        public async Task<ObservableCollection<IndividualObjectivesDto>> GetListAsync(ObservableCollection<IndividualObjectivesDto> list, ListParam args)
        {
            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var userInfo = PreferenceHelper.UserInfo();

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.IndividualObjectives}/list"
                };

                var param = new APIM.Requests.MyApprovalRequest
                {
                    ProfileId = userInfo.ProfileId,
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

                var response = await genericRepository_.GetAsync<APIM.Responses.ListResponse<APIM.Models.EmployeeIndividualObjectiveList>>(request);
                args.Count = (response.ListData.Count <= args.Count ? response.ListData.Count : args.Count);

                if (response.TotalListCount != 0)
                {
                    foreach (var item in response.ListData)
                    {
                        var data = new IndividualObjectivesDto()
                        {
                            StatusId = item.StatusId,
                            DatePrepared = item.DatePrepared,
                            Details = item.Details,
                            EffectiveYear = item.EffectiveYear.GetValueOrDefault(),
                            IndividualOjbectiveId = item.RecordId,
                            Period = item.Period,
                            ProfileId = item.ProfileId,
                            Status = item.Status,
                            DatePrepared_String = item.DatePrepared.ToString(Constants.ListDefaultDateFormat),
                            Icon = Application.Current.Resources["StringFlagIcon"].ToString(),
                            IsChecked = false,
                            Header = item.Header,
                        };

                        list.Add(data);
                    }
                }

                TotalListItem = response.TotalListCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
    }
}