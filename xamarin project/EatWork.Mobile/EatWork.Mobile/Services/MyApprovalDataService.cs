using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
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
using System.Threading.Tasks;

namespace EatWork.Mobile.Services
{
    public class MyApprovalDataService : IMyApprovalDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IEmployeeProfileDataService employeeProfileDataService_;

        public MyApprovalDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            employeeProfileDataService_ = AppContainer.Resolve<IEmployeeProfileDataService>();
        }

        public long TotalListItem { get; set; }

        public async Task<SfListView> InitListView(SfListView listview, bool isAsceding = false)
        {
            var retValue = listview;

            await Task.Run((Action)(() =>
            {
                if (retValue.DataSource.GroupDescriptors.Count == 0)
                {
                    retValue.DataSource.GroupDescriptors.Add(new GroupDescriptor()
                    {
                        PropertyName = "DateFiledDisplay",
                        KeySelector = (object obj1) =>
                        {
                            return (object)(obj1 as Models.MyApprovalListModel).DateFiled.ToString(Constants.ListDefaultDateFormat);
                        },
                    });
                }

                //if (retValue.DataSource.SortDescriptors.Count > 0)
                //    retValue.DataSource.SortDescriptors.Clear();

                //retValue.DataSource.SortDescriptors.Add(new SortDescriptor()
                //{
                //    PropertyName = "DateFiled",
                //    Direction = isAsceding ? ListSortDirection.Ascending : ListSortDirection.Descending
                //});
            }));

            return retValue;
        }

        public async Task<ObservableCollection<Models.MyApprovalListModel>> RetrieveApprovalList(ObservableCollection<Models.MyApprovalListModel> list, ListParam obj)
        {
            try
            {
                var url = await commonDataService_.RetrieveClientUrl();

                await commonDataService_.HasInternetConnection(url);

                var userInfo = PreferenceHelper.UserInfo();

                var builder = new UriBuilder(url)
                {
                    Path = ApiConstants.MyApproval
                };

                var param = new MyApprovalRequest
                {
                    ProfileId = userInfo.ProfileId,
                    Page = (obj.ListCount == 0 ? 1 : ((obj.ListCount + obj.Count) / obj.Count)),
                    Rows = obj.Count,
                    SortOrder = (obj.IsAscending ? 0 : 1),
                    Keyword = obj.KeyWord,
                    TransactionTypes = obj.FilterTypes,
                    Status = obj.Status,
                    StartDate = obj.StartDate,
                    EndDate = obj.EndDate
                };

                var response = await genericRepository_.PostAsync<MyApprovalRequest, ListResponse<MyApprovalList>>(builder.ToString(), param);
                obj.Count = (response.ListData.Count <= obj.Count ? response.ListData.Count : obj.Count);

                if (response.TotalPages != 0)
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

                        foreach (var item in response.ListData)
                        {
                            var data = new Models.MyApprovalListModel();
                            PropertyCopier<MyApprovalList, Models.MyApprovalListModel>.Copy(item, data);

                            data.RequestedHoursNumber = Convert.ToDecimal(data.RequestedHours);
                            data.DisplayItemName = (!string.IsNullOrWhiteSpace(data.ItemName));
                            data.IsDocumentRequest = (item.TransactionTypeId == TransactionType.Document);
                            data.IsChangeRestday = (item.TransactionTypeId == TransactionType.ChangeRestDay);
                            data.IsLoanRequest = (item.TransactionTypeId == TransactionType.Loan);
                            data.IsTimeEntryLogRequest = (item.TransactionTypeId == TransactionType.TimeLog);
                            data.IsLeaveRequest = (item.TransactionTypeId == TransactionType.Leave);
                            data.IsTravelRequest = (item.TransactionTypeId == TransactionType.Travel);
                            data.IsScheduleRequest = !transactionTypes.Contains(item.TransactionTypeId);
                            data.ProfileId = item.ProfileId;

                            data.RequestedHoursSuffixDisplay = (!string.IsNullOrWhiteSpace(item.RequestedHoursSuffixDisplay) ? item.RequestedHoursSuffixDisplay : "hr/s");
                            /*data.ImageSource = await employeeProfileDataService_.GetProfileImage(item.ProfileId);*/
                            list.Add(data);
                        }

                        /*
                        for (int i = listcount; i < listcount + count; i++)
                        {
                            if (response.ListData.ElementAtOrDefault(i) != null)
                            {
                                var data = new MyApprovalListModel();
                                PropertyCopier<object, MyApprovalListModel>.Copy(response.ListData[i], data);
                                retValue.Add(data);
                            }
                        }
                        */
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }

                    TotalListItem = response.TotalListCount;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return list;
        }
    }
}