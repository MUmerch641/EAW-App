using EatWork.Mobile.Utils;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace EatWork.Mobile.Models
{
    public class MyAttendanceListModel : ExtendedBindableObject
    {
        public MyAttendanceListModel()
        {
            HasTimeIn = false;
            HasTimeOut = false;
            HasScheduleInOut = false;
            HasScheduleBreak = false;
            HasDetails = false;
            TimeEntryDetail = new ObservableCollection<TimeEntryDetailModel>();
            HasTimeLog = true;
            Icon1 = string.Empty;
            Icon2 = string.Empty;
            TextColor1 = (Color)Application.Current.Resources["Gray-900"];
            TextColor2 = (Color)Application.Current.Resources["Gray-900"];
            ShowDetails = false;
        }

        public long? ProfileId { get; set; }
        public long TimeEntryHeaderDetailId { get; set; }
        public long TimeEntryHeaderId { get; set; }
        public DateTime? WorkDate { get; set; }
        public string ShiftCode { get; set; }
        public string ScheduleInOut { get; set; }
        public string ScheduleLunchInOut { get; set; }
        public string ActualInOut { get; set; }
        public string ScheduleIn { get; set; }
        public string ScheduleOut { get; set; }
        public string ScheduleLunchIn { get; set; }
        public string ScheduleLunchOut { get; set; }
        public string ActualIn { get; set; }
        public string ActualOut { get; set; }
        public string HolidayName { get; set; }
        public string HourType { get; set; }
        public int RenderedHour { get; set; }
        public int TotalCount { get; set; }
        public string WorkDateDisplay { get; set; }

        private bool isVisible_ = false;

        public bool IsVisible
        {
            get { return isVisible_; }
            set { isVisible_ = value; RaisePropertyChanged(() => IsVisible); }
        }

        public ObservableCollection<TimeEntryDetailModel> TimeEntryDetail { get; set; }

        public bool IsRestday { get; set; }
        public string Remarks { get; set; }

        public bool HasTimeIn { get; set; }
        public bool HasTimeOut { get; set; }
        public bool HasScheduleInOut { get; set; }
        public bool HasScheduleBreak { get; set; }
        public bool HasTimeLog { get; set; }
        public bool HasDetails { get; set; }
        public bool ShowDetails { get; set; }

        public string Icon1 { get; set; }
        public Color TextColor1 { get; set; }
        public string Icon2 { get; set; }
        public Color TextColor2 { get; set; }
        public string ActualInDto { get; set; }
        public string ActualOutDto { get; set; }
        public bool ShowRemarks { get; set; }
    }

    public class TimeEntryDetailModel
    {
        public long TimeEntryDetailId { get; set; }
        public long? TimeEntryHeaderDetailId { get; set; }
        public long? ProfileId { get; set; }
        public DateTime? WorkDate { get; set; }
        public string Type { get; set; }
        public decimal Value { get; set; }
        public long CreateId { get; set; }
        public DateTime CreateDate { get; set; }
        public string Group { get; set; }
    }

    public class IndividualAttendance : ExtendedBindableObject
    {
        public IndividualAttendance()
        {
            HasTimeIn = false;
            HasTimeOut = false;
            HasScheduleInOut = false;
            HasScheduleBreak = false;
            HasDetails = false;
            HasTimeLog = true;
            Icon1 = string.Empty;
            Icon2 = string.Empty;
            TextColor1 = (Color)Application.Current.Resources["Gray-900"];
            TextColor2 = (Color)Application.Current.Resources["Gray-900"];
            ShowRemarks = false;
        }

        public long ProfileId { get; set; }
        public long TimeEntryHeaderDetailId { get; set; }
        public long TimeEntryHeaderId { get; set; }
        public DateTime? WorkDate { get; set; }
        public string ShiftCode { get; set; }
        public string ScheduleInOut { get; set; }
        public string ScheduleLunchInOut { get; set; }
        public string ActualInOut { get; set; }
        public string ScheduleIn { get; set; }
        public string ScheduleOut { get; set; }
        public string ScheduleLunchIn { get; set; }
        public string ScheduleLunchOut { get; set; }
        public string ActualIn { get; set; }
        public string ActualOut { get; set; }
        public string HolidayName { get; set; }
        public bool IsRestday { get; set; }
        public string Remarks { get; set; }
        public decimal Late { get; set; }
        public decimal Undertime { get; set; }
        public decimal Absent { get; set; }
        public decimal? TimeOffHrs { get; set; }
        public decimal? HolidayHrs { get; set; }
        public decimal ExcessTime { get; set; }
        public decimal? ApprovedRegularOT { get; set; }
        public decimal? ApprovedNSOT { get; set; }
        public decimal? ApprovePreshiftOT { get; set; }
        public decimal? ApprovePreshiftNSOT { get; set; }
        public decimal VLHrs { get; set; }
        public decimal SLHrs { get; set; }
        public decimal OtherLeave { get; set; }
        public decimal LWOP { get; set; }
        public decimal LWP { get; set; }
        public int? TotalCount { get; set; }

        private bool isVisible_ = false;

        public bool IsVisible
        {
            get { return isVisible_; }
            set { isVisible_ = value; RaisePropertyChanged(() => IsVisible); }
        }

        public bool HasTimeIn { get; set; }
        public bool HasTimeOut { get; set; }
        public bool HasScheduleInOut { get; set; }
        public bool HasScheduleBreak { get; set; }
        public bool HasTimeLog { get; set; }
        public bool HasDetails { get; set; }

        public string Icon1 { get; set; }
        public Color TextColor1 { get; set; }
        public string Icon2 { get; set; }
        public Color TextColor2 { get; set; }

        public string WorkDateDisplay { get; set; }
        public bool ShowRemarks { get; set; }
    }
}