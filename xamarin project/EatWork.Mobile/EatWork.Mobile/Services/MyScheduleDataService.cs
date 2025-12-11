using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Utils;
using Syncfusion.DataSource;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services
{
    public class MyScheduleDataService : IMyScheduleDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly StringHelper string_;

        public MyScheduleDataService(IGenericRepository genericRepository,
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
                            return (obj1 as MyScheduleListModel).WorkDate.GetValueOrDefault().ToString(Constants.ListDefaultDateFormat);
                        }
                    });

                    retValue.DataSource.SortDescriptors.Add(new SortDescriptor()
                    {
                        PropertyName = "WorkDate",
                        Direction = ListSortDirection.Descending
                    });
                }
            });

            return retValue;
        }

        public async Task<MyScheduleListModel> RetrieveCurrentSchedule(ListParam obj)
        {
            var retValue = new MyScheduleListModel();

            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);
                var userInfo = PreferenceHelper.UserInfo();

                var builder = new UriBuilder(url)
                {
                    Path = ApiConstants.CurrentSchedule
                };

                var param = new R.Requests.MySchedulesRequest
                {
                    ProfileId = userInfo.ProfileId,
                    StartDate = obj.StartDate,
                    EndDate = obj.EndDate,
                };

                var request = string_.CreateUrl<R.Requests.MySchedulesRequest>(builder.ToString(), param);
                var response = await genericRepository_.GetAsync<R.Models.MyScheduleList>(request);

                if (response != null)
                {
                    try
                    {
                        var data = new Models.MyScheduleListModel();
                        PropertyCopier<R.Models.MyScheduleList, Models.MyScheduleListModel>.Copy(response, data);

                        if (string.IsNullOrWhiteSpace(data.WorkSchedule) && !data.IsRestday && !data.IsHoliday)
                            data.HasSchedule = false;

                        data.OTSchedule = string.Format("{0}{1}{2}", data.ASOTDuration,
                            ((!string.IsNullOrWhiteSpace(data.ASOTDuration) && !string.IsNullOrWhiteSpace(data.PSOTDuration)) ? Constants.NextLine : "")
                            , data.PSOTDuration);

                        data.OBDuration = string.Format("{0}{1}{2}", data.OBDuration
                            , ((!string.IsNullOrWhiteSpace(data.OBDuration) && !string.IsNullOrWhiteSpace(data.OBReason)) ? Constants.NextLine : "")
                            , data.OBReason);

                        data.TODuration = string.Format("{0}{1}{2}", data.TODuration
                            , ((!string.IsNullOrWhiteSpace(data.TODuration) && !string.IsNullOrWhiteSpace(data.TOReason)) ? Constants.NextLine : "")
                            , data.TOReason);

                        data.UTDuration = string.Format("{0}{1}{2}", data.UTDuration
                            , ((!string.IsNullOrWhiteSpace(data.UTDuration) && !string.IsNullOrWhiteSpace(data.UTReason)) ? Constants.NextLine : "")
                            , data.UTReason);

                        data.WorkDateDisplay = data.WorkDate.GetValueOrDefault().ToString(Constants.ListDefaultDateFormat);

                        data.HasSchedule = true;
                        retValue = data;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retValue;
        }

        public async Task<ObservableCollection<MyScheduleListModel>> RetrieveMyScheduleList(ObservableCollection<MyScheduleListModel> list, ListParam obj)
        {
            var retValue = list;

            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);
                var userInfo = PreferenceHelper.UserInfo();

                var builder = new UriBuilder(url)
                {
                    Path = ApiConstants.MySchedule
                };

                var param = new R.Requests.MySchedulesRequest
                {
                    ProfileId = userInfo.ProfileId,
                    Page = (obj.ListCount == 0 ? 1 : ((obj.ListCount + obj.Count) / obj.Count)),
                    Rows = obj.Count,
                    SortOrder = (obj.IsAscending ? 0 : 1),
                    Keyword = obj.KeyWord,
                    StartDate = obj.StartDate,
                    EndDate = obj.EndDate,
                };

                var request = string_.CreateUrl<R.Requests.MySchedulesRequest>(builder.ToString(), param);

                var response = await genericRepository_.GetAsync<R.Responses.ListResponse<R.Models.MyScheduleList>>(request);
                obj.Count = (response.ListData.Count <= obj.Count ? response.ListData.Count : obj.Count);

                if (response.TotalListCount != 0)
                {
                    try
                    {
                        foreach (var item in response.ListData)
                        {
                            var data = new Models.MyScheduleListModel();
                            PropertyCopier<R.Models.MyScheduleList, Models.MyScheduleListModel>.Copy(item, data);

                            data.HasSchedule = !string.IsNullOrWhiteSpace(data.WorkSchedule) || (data.IsRestday || data.IsHoliday);

                            data.OTSchedule = string.Format("{0}{1}{2}", item.ASOTDuration,
                                ((!string.IsNullOrWhiteSpace(item.ASOTDuration) && !string.IsNullOrWhiteSpace(item.PSOTDuration)) ? Constants.NextLine : "")
                                , item.PSOTDuration);

                            data.OBDuration = string.Format("{0}{1}{2}", item.OBDuration
                                , ((!string.IsNullOrWhiteSpace(item.OBDuration) && !string.IsNullOrWhiteSpace(item.OBReason)) ? Constants.NextLine : "")
                                , item.OBReason);

                            data.TODuration = string.Format("{0}{1}{2}", item.TODuration
                                , ((!string.IsNullOrWhiteSpace(item.TODuration) && !string.IsNullOrWhiteSpace(item.TOReason)) ? Constants.NextLine : "")
                                , item.TOReason);

                            data.UTDuration = string.Format("{0}{1}{2}", item.UTDuration
                                , ((!string.IsNullOrWhiteSpace(item.UTDuration) && !string.IsNullOrWhiteSpace(item.UTReason)) ? Constants.NextLine : "")
                                , item.UTReason);

                            data.WorkDateDisplay = item.WorkDate.GetValueOrDefault().ToString("ddd, MMM. dd, yyyy");
                            retValue.Add(data);
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

            return retValue;
        }
    }
}