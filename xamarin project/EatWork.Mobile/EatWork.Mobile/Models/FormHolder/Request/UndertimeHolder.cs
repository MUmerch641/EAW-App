using EatWork.Mobile.Models.DataObjects;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections.ObjectModel;

namespace EatWork.Mobile.Models.FormHolder.Request
{
    public class UndertimeHolder : RequestHolder
    {
        public UndertimeHolder()
        {
            FileData = new FileData();
            //DepartureTime = DateTime.Now.TimeOfDay;
            //ArrivalTime = DateTime.Now.TimeOfDay;
        }

        private TimeSpan? departureTime_;

        public TimeSpan? DepartureTime
        {
            get { return departureTime_; }
            set { departureTime_ = value; RaisePropertyChanged(() => DepartureTime); }
        }

        private TimeSpan? arrivalTime_;

        public TimeSpan? ArrivalTime
        {
            get { return arrivalTime_; }
            set { arrivalTime_ = value; RaisePropertyChanged(() => ArrivalTime); }
        }

        private string startTimeString_;

        public string StartTimeString
        {
            get { return startTimeString_; }
            set { startTimeString_ = value; RaisePropertyChanged(() => StartTimeString); }
        }

        private string endTimeString_;

        public string EndTimeString
        {
            get { return endTimeString_; }
            set { endTimeString_ = value; RaisePropertyChanged(() => EndTimeString); }
        }

        private ObservableCollection<ComboBoxObject> undertimeReason_;

        public ObservableCollection<ComboBoxObject> UndertimeReason
        {
            get { return undertimeReason_; }
            set { undertimeReason_ = value; RaisePropertyChanged(() => UndertimeReason); }
        }

        private ComboBoxObject undertimeReasonSelectedItem_;

        public ComboBoxObject UndertimeReasonSelectedItem
        {
            get { return undertimeReasonSelectedItem_; }
            set { undertimeReasonSelectedItem_ = value; RaisePropertyChanged(() => UndertimeReasonSelectedItem); }
        }

        private UndertimeModel undertimeModel_;

        public UndertimeModel UndertimeModel
        {
            get { return undertimeModel_; }
            set { undertimeModel_ = value; RaisePropertyChanged(() => UndertimeModel); }
        }

        private DateTime? _testDate;

        public DateTime? TestDate
        {
            get { return _testDate; }
            set { _testDate = value; RaisePropertyChanged(() => TestDate); }
        }

        #region validators

        private bool errorUndertimeDate_;

        public bool ErrorUndertimeDate
        {
            get { return errorUndertimeDate_; }
            set { errorUndertimeDate_ = value; RaisePropertyChanged(() => ErrorUndertimeDate); }
        }

        private bool errorUTHrs_;

        public bool ErrorUTHrs
        {
            get { return errorUTHrs_; }
            set { errorUTHrs_ = value; RaisePropertyChanged(() => ErrorUTHrs); }
        }

        private bool errorUndertimeReason_;

        public bool ErrorUndertimeReason
        {
            get { return errorUndertimeReason_; }
            set { errorUndertimeReason_ = value; RaisePropertyChanged(() => ErrorUndertimeReason); }
        }

        private bool errorReason_;

        public bool ErrorReason
        {
            get { return errorReason_; }
            set { errorReason_ = value; RaisePropertyChanged(() => ErrorReason); }
        }

        #endregion validators
    }
}