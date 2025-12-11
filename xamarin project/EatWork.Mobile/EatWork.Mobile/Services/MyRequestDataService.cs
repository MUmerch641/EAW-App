using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Utils;
using EAW.API.DataContracts.Models;
using EAW.API.DataContracts.Requests;
using EAW.API.DataContracts.Responses;
using Syncfusion.DataSource;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EatWork.Mobile.Services
{
    public class MyRequestDataService : IMyRequestDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly StringHelper string_;

        public MyRequestDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService,
            StringHelper url)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            string_ = url;
        }

        public long TotalListItem { get; set; }

        public async Task<SfListView> InitListView(SfListView listview)
        {
            var retValue = listview;

            await Task.Run(() =>
            {
                if (retValue.DataSource.GroupDescriptors.Count == 0)
                {
                    retValue.DataSource.GroupDescriptors.Add(new GroupDescriptor()
                    {
                        PropertyName = "DateFiledDisplay",
                        KeySelector = (object obj1) =>
                        {
                            return (obj1 as MyRequestListModel).DateFiled.ToString(Constants.ListDefaultDateFormat);
                        }
                    });

                    retValue.DataSource.SortDescriptors.Add(new SortDescriptor()
                    {
                        PropertyName = "DateFiled",
                        Direction = ListSortDirection.Descending
                    });
                }
            });

            return retValue;
        }

        public async Task<ObservableCollection<MyRequestListModel>> RetrieveMyRequestList(ObservableCollection<Models.MyRequestListModel> list, ListParam obj)
        {
            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var userInfo = PreferenceHelper.UserInfo();

                var builder = new UriBuilder(url)
                {
                    Path = ApiConstants.MyRequest
                };

                var param = new MyApprovalRequest
                {
                    ProfileId = userInfo.ProfileId,
                    Page = (obj.ListCount == 0 ? 1 : ((obj.ListCount + obj.Count) / obj.Count)),
                    Rows = obj.Count,
                    SortOrder = (obj.IsAscending ? 0 : 1),
                    Keyword = obj.KeyWord,
                    TransactionTypes = obj.FilterTypes
                };

                var request = string_.CreateUrl<MyApprovalRequest>(builder.ToString(), param);

                var response = await genericRepository_.GetAsync<ListResponse<MyRequestList>>(request);
                obj.Count = (response.ListData.Count <= obj.Count ? response.ListData.Count : obj.Count);

                if (response.TotalListCount != 0)
                {
                    try
                    {
                        var transactionTypes = new List<long>()
                        {
                            TransactionType.Document,
                            TransactionType.ChangeRestDay,
                            TransactionType.Loan,
                            TransactionType.TimeLog,
                            TransactionType.Leave,
                            TransactionType.Travel
                        };

                        var isTravelAvail = MenuHelper.Forms().FirstOrDefault(x => x.FormCode == MenuItemType.TravelRequest.ToString());

                        foreach (var item in response.ListData)
                        {
                            var data = new Models.MyRequestListModel();
                            PropertyCopier<MyRequestList, Models.MyRequestListModel>.Copy(item, data);

                            if (item.TransactionTypeId == TransactionType.Travel && isTravelAvail == null)
                                continue;

                            data.RequestedHoursNumber = Convert.ToDecimal(data.RequestedHours);
                            data.DisplayItemName = (!string.IsNullOrWhiteSpace(data.ItemName));
                            data.IsDocumentRequest = (item.TransactionTypeId == TransactionType.Document);
                            data.IsChangeRestday = (item.TransactionTypeId == TransactionType.ChangeRestDay);
                            data.IsLoanRequest = (item.TransactionTypeId == TransactionType.Loan);
                            data.IsTimeEntryLogRequest = (item.TransactionTypeId == TransactionType.TimeLog);
                            data.IsLeaveReqeust = (item.TransactionTypeId == TransactionType.Leave);
                            data.IsTravelRequest = (item.TransactionTypeId == TransactionType.Travel);
                            data.IsScheduleRequest = !transactionTypes.Contains(item.TransactionTypeId);

                            data.RequestedHoursSuffixDisplay = (!string.IsNullOrWhiteSpace(item.RequestedHoursSuffixDisplay) ? item.RequestedHoursSuffixDisplay : "hr/s");

                            list.Add(data);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }

                TotalListItem = response.TotalListCount;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }

            return list;
        }
    }
}