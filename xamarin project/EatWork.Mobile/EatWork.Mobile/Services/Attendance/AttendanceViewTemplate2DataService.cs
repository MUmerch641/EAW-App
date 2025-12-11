using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.Attendance;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Utils;
using EAW.API.DataContracts.Requests;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services
{
    public class AttendanceViewTemplate2DataService : IAttendanceViewTemplate2DataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly StringHelper string_;

        public long TotalListItem { get; set; }
        public long TotalListItemDetail { get; set; }

        public AttendanceViewTemplate2DataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService,
            StringHelper url)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            string_ = url;
        }

        public async Task<ObservableCollection<DetailedAttendanceListModel>> GetListAsync(ObservableCollection<DetailedAttendanceListModel> list, ListParam args)
        {
            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var userInfo = PreferenceHelper.UserInfo();

                var builder = new UriBuilder(url)
                {
                    Path = ApiConstants.DetailedAttendanceList
                };

                var param = new MyAttendanceListRequest
                {
                    ProfileId = userInfo.ProfileId,
                    Page = (args.ListCount == 0 ? 1 : ((args.ListCount + args.Count) / args.Count)),
                    Rows = args.Count,
                    SortOrder = (args.IsAscending ? 0 : 1),
                    Keyword = args.KeyWord,
                    Status = args.Status,
                    StartDate = args.StartDate,
                    EndDate = args.EndDate,
                };

                var request = string_.CreateUrl<MyAttendanceListRequest>(builder.ToString(), param);

                var response = await genericRepository_.GetAsync<R.Responses.ListResponse<R.Models.DetailedAttendanceList>>(request);
                args.Count = (response.ListData.Count <= args.Count ? response.ListData.Count : args.Count);

                if (response == null)
                    return list;

                if (response.ListData.Any())
                {
                    foreach (var item in response.ListData)
                    {
                        var data = new DetailedAttendanceListModel();
                        PropertyCopier<R.Models.DetailedAttendanceList, DetailedAttendanceListModel>.Copy(item, data);

                        list.Add(data);
                    }
                }

                TotalListItem = response.TotalListCount;

                return list;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw;
            }
        }

        public async Task<ObservableCollection<IndividualAttendance>> InitFormAsync(ObservableCollection<IndividualAttendance> list, ListParamsRecordId args)
        {
            try
            {
                var userInfo = PreferenceHelper.UserInfo();

                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.DetailedAttendanceForm}/{args.RecordId}"
                };

                var param = new DetailedAttendanceListRequest
                {
                    ProfileId = userInfo.ProfileId,
                    Page = (args.ListCount == 0 ? 1 : ((args.ListCount + args.Count) / args.Count)),
                    Rows = args.Count,
                    SortOrder = (args.IsAscending ? 0 : 1),
                    Keyword = args.KeyWord,
                    Status = args.Status,
                    StartDate = args.StartDate,
                    EndDate = args.EndDate,
                    TimeEntryHeaderId = args.RecordId
                };

                var request = string_.CreateUrl<DetailedAttendanceListRequest>(builder.ToString(), param);

                var response = await genericRepository_.GetAsync<R.Responses.ListResponse<R.Models.IndividualAttendance>>(request);
                args.Count = (response.ListData.Count <= args.Count ? response.ListData.Count : args.Count);

                if (response.ListData.Any())
                {
                    foreach (var item in response.ListData)
                    {
                        var data = new Models.IndividualAttendance();

                        PropertyCopier<R.Models.IndividualAttendance, Models.IndividualAttendance>.Copy(item, data);
                        data.WorkDateDisplay = data.WorkDate.GetValueOrDefault().ToString(Constants.ListDefaultDateFormat);

                        if (!string.IsNullOrWhiteSpace(item.ActualIn))
                        {
                            data.HasTimeIn = true;
                        }
                        else
                        {
                            data.TextColor1 = (Xamarin.Forms.Color)Application.Current.Resources["Error"];
                            data.Icon1 = Application.Current.Resources["WarningIcon"].ToString();
                            data.ActualIn = " - - : - - ";
                        }

                        if (!string.IsNullOrWhiteSpace(item.ActualOut))
                        {
                            data.HasTimeOut = true;
                        }
                        else
                        {
                            data.TextColor2 = (Xamarin.Forms.Color)Application.Current.Resources["Error"];
                            data.Icon2 = Application.Current.Resources["WarningIcon"].ToString();
                            data.ActualOut = " - - : - - ";
                        }

                        if (!string.IsNullOrWhiteSpace(item.ScheduleInOut))
                            data.HasScheduleInOut = true;

                        if (!string.IsNullOrWhiteSpace(item.ScheduleLunchInOut))
                            data.HasScheduleBreak = true;

                        if (data.IsRestday || !string.IsNullOrWhiteSpace(data.HolidayName))
                        {
                            if (!data.HasTimeIn && !data.HasTimeOut)
                                data.HasTimeLog = false;
                        }

                        if (!data.IsRestday && string.IsNullOrWhiteSpace(data.HolidayName) && !string.IsNullOrWhiteSpace(data.Remarks))
                        {
                            data.ShowRemarks = true;
                        }

                        if ((data.Late
                            + data.Undertime
                            + data.Absent
                            + data.TimeOffHrs
                            + data.HolidayHrs
                            + data.ExcessTime
                            + data.ApprovedRegularOT
                            + data.ApprovedNSOT
                            + data.ApprovePreshiftOT
                            + data.ApprovePreshiftNSOT
                            + data.VLHrs
                            + data.SLHrs
                            + data.OtherLeave
                            + data.LWOP) > 0)
                        {
                            data.HasDetails = true;
                        }

                        list.Add(data);
                    }
                }

                TotalListItemDetail = response.TotalListCount;

                return list;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw;
            }
        }
    }
}