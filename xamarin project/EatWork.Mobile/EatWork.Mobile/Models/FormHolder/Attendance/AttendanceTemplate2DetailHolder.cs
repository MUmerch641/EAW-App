using EatWork.Mobile.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using R = EAW.API.DataContracts.Models;

namespace EatWork.Mobile.Models.FormHolder.Attendance
{
    public class AttendanceTemplate2DetailHolder : ExtendedBindableObject
    {
        public AttendanceTemplate2DetailHolder()
        {
            MyAttendanceList = new ObservableCollection<IndividualAttendance>();
        }

        private string referenceNumber_;

        public string ReferenceNumber
        {
            get { return referenceNumber_; }
            set { referenceNumber_ = value; RaisePropertyChanged(() => ReferenceNumber); }
        }

        private string status_;

        public string Status
        {
            get { return status_; }
            set { status_ = value; RaisePropertyChanged(() => Status); }
        }

        private string cutOffDate_;

        public string CutOffDate
        {
            get { return cutOffDate_; }
            set { cutOffDate_ = value; RaisePropertyChanged(() => CutOffDate); }
        }

        private ObservableCollection<IndividualAttendance> myAttendanceList_;

        public ObservableCollection<IndividualAttendance> MyAttendanceList
        {
            get { return myAttendanceList_; }
            set { myAttendanceList_ = value; RaisePropertyChanged(() => MyAttendanceList); }
        }
    }

    public class MyAttendanceListDto : R.MyAttendanceList
    {
        public MyAttendanceListDto()
        {
            HasTimeIn = false;
            HasTimeOut = false;
            HasScheduleInOut = false;
            HasScheduleBreak = false;
            HasDetails = false;
            TimeEntryDetail = new List<R.TimeEntryDetailModel>();
            HasTimeLog = true;
            Icon1 = string.Empty;
            Icon2 = string.Empty;
            TextColor1 = (Color)Application.Current.Resources["Gray-900"];
            TextColor2 = (Color)Application.Current.Resources["Gray-900"];
        }

        public string WorkDateDisplay { get; set; }

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
        public string ActualInDto { get; set; }
        public string ActualOutDto { get; set; }
        public bool ShowRemarks { get; set; }
    }
}