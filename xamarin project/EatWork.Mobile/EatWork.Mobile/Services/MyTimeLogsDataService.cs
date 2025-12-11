using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Utils;
using EAW.API.DataContracts.Requests;
using EAW.API.DataContracts.Responses;
using Newtonsoft.Json;
using Syncfusion.DataSource;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services
{
    public class MyTimeLogsDataService : IMyTimeLogsDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly StringHelper string_;

        public MyTimeLogsDataService(IGenericRepository genericRepository,
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
                        PropertyName = "WorkDateDisplay",
                        KeySelector = (object obj1) =>
                        {
                            return (obj1 as MyTimeLogsListModel).TimeEntry.GetValueOrDefault().ToString(Constants.ListDefaultDateFormat);
                        }
                    });

                    retValue.DataSource.SortDescriptors.Add(new SortDescriptor()
                    {
                        PropertyName = "TimeEntry",
                        Direction = ListSortDirection.Descending
                    });
                }
            });

            return retValue;
        }

        public async Task<ObservableCollection<MyTimeLogsListModel>> RetrieveMyRequestList(ObservableCollection<MyTimeLogsListModel> list, ListParam obj)
        {
            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var userInfo = PreferenceHelper.UserInfo();

                var builder = new UriBuilder(url)
                {
                    Path = ApiConstants.MyTimeLogsList
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
                    EndDate = obj.EndDate,
                };

                var request = string_.CreateUrl<MyApprovalRequest>(builder.ToString(), param);

                var response = await genericRepository_.GetAsync<ListResponse<R.Models.MyTimeLogsList>>(request);
                obj.Count = (response.ListData.Count <= obj.Count ? response.ListData.Count : obj.Count);

                if (response.TotalListCount != 0)
                {
                    try
                    {
                        foreach (var item in response.ListData)
                        {
                            var data = new Models.MyTimeLogsListModel();
                            PropertyCopier<R.Models.MyTimeLogsList, Models.MyTimeLogsListModel>.Copy(item, data);
                            data.Status = (!string.IsNullOrWhiteSpace(data.Status) ? data.Status : "---");
                            data.Source = (!string.IsNullOrWhiteSpace(data.Source) ? data.Source : "---");
                            data.WorkDateDisplay = data.TimeEntry.GetValueOrDefault().ToString(Constants.ListDefaultDateFormat);

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
                throw ex;
            }

            return list;
        }

        public async Task<ObservableCollection<SelectableListModel>> RetrieveStatus()
        {
            var retValue = new ObservableCollection<SelectableListModel>();
            try
            {
                retValue.Add(new SelectableListModel() { Id = RequestStatusValue.Draft, DisplayText = RequestStatus.Draft });
                retValue.Add(new SelectableListModel() { Id = RequestStatusValue.Approved, DisplayText = RequestStatus.Approved });
                retValue.Add(new SelectableListModel() { Id = RequestStatusValue.Cancelled, DisplayText = RequestStatus.Cancelled });
                retValue.Add(new SelectableListModel() { Id = RequestStatusValue.Disapproved, DisplayText = RequestStatus.Disapproved });
                retValue.Add(new SelectableListModel() { Id = RequestStatusValue.ForApproval, DisplayText = RequestStatus.ForApproval });
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return await Task.FromResult(retValue);
        }
    }
}