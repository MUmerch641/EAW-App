using EatWork.Mobile.Contants;
using EatWork.Mobile.Models.DataObjects;
using System;
using System.Collections.ObjectModel;

namespace EatWork.Mobile.Models.FormHolder.Request
{
    public class SpecialWorkScheduleHolder : RequestHolder
    {
        public SpecialWorkScheduleHolder()
        {
            SpecialWorkScheduleRequestModel = new SpecialWorkScheduleRequestModel();
            ErrorReason = false;
            ErrorShift = false;
            ErrorWorkDate = false;
            EnableCustomSched = false;
            WorkDate = DateTime.Now;
            IsOffSetting = false;
            OffsetExpirationDateToggle = true;
            IsOffsetEnabled = false;
            OffSetExpirationDate = Constants.NullDate;
        }

        private ObservableCollection<ShiftDto> shiftList_;

        public ObservableCollection<ShiftDto> ShiftList
        {
            get { return shiftList_; }
            set { shiftList_ = value; RaisePropertyChanged(() => ShiftList); }
        }

        private ShiftDto shiftSelectedItem_;

        public ShiftDto ShiftSelectedItem
        {
            get { return shiftSelectedItem_; }
            set { shiftSelectedItem_ = value; RaisePropertyChanged(() => ShiftSelectedItem); }
        }

        private string requestType_;

        public string RequestType
        {
            get { return requestType_; }
            set { requestType_ = value; RaisePropertyChanged(() => RequestType); }
        }

        private bool enableCustomSched_;

        public bool EnableCustomSched
        {
            get { return enableCustomSched_; }
            set { enableCustomSched_ = value; RaisePropertyChanged(() => EnableCustomSched); }
        }

        private DateTime workDate_;

        public DateTime WorkDate
        {
            get { return workDate_; }
            set { workDate_ = value; RaisePropertyChanged(() => WorkDate); }
        }

        private TimeSpan? scheduleStartTime_;

        public TimeSpan? ScheduleStartTime
        {
            get { return scheduleStartTime_; }
            set { scheduleStartTime_ = value; RaisePropertyChanged(() => ScheduleStartTime); }
        }

        private TimeSpan? scheduleEndTime_;

        public TimeSpan? ScheduleEndTime
        {
            get { return scheduleEndTime_; }
            set { scheduleEndTime_ = value; RaisePropertyChanged(() => ScheduleEndTime); }
        }

        private TimeSpan? lunchStartTime_;

        public TimeSpan? LunchStartTime
        {
            get { return lunchStartTime_; }
            set { lunchStartTime_ = value; RaisePropertyChanged(() => LunchStartTime); }
        }

        private TimeSpan? lunchEndTime_;

        public TimeSpan? LunchEndTime
        {
            get { return lunchEndTime_; }
            set { lunchEndTime_ = value; RaisePropertyChanged(() => LunchEndTime); }
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

        private string lunchStartTimeString_;

        public string LunchStartTimeString
        {
            get { return lunchStartTimeString_; }
            set { lunchStartTimeString_ = value; RaisePropertyChanged(() => LunchStartTimeString); }
        }

        private string lunchEndTimeString_;

        public string LunchEndTimeString
        {
            get { return lunchEndTimeString_; }
            set { lunchEndTimeString_ = value; RaisePropertyChanged(() => LunchEndTimeString); }
        }

        private bool isOffSetting_;

        public bool IsOffSetting
        {
            get { return isOffSetting_; }
            set { isOffSetting_ = value; RaisePropertyChanged(() => IsOffSetting); }
        }

        private bool offsetExpirationDateToggle_;

        public bool OffsetExpirationDateToggle
        {
            get { return offsetExpirationDateToggle_; }
            set { offsetExpirationDateToggle_ = value; RaisePropertyChanged(() => OffsetExpirationDateToggle); }
        }


        private bool isOffsetEnabled_;

        public bool IsOffsetEnabled
        {
            get { return isOffsetEnabled_; }
            set { isOffsetEnabled_ = value; RaisePropertyChanged(() => IsOffsetEnabled); }
        }

        private DateTime offSetExpirationDate_;

        public DateTime OffSetExpirationDate
        {
            get { return offSetExpirationDate_; }
            set { offSetExpirationDate_ = value; RaisePropertyChanged(() => OffSetExpirationDate); }
        }

        private SpecialWorkScheduleRequestModel model_;

        public SpecialWorkScheduleRequestModel SpecialWorkScheduleRequestModel
        {
            get { return model_; }
            set { model_ = value; RaisePropertyChanged(() => SpecialWorkScheduleRequestModel); }
        }

        #region validators

        private bool errorWorkDate_;

        public bool ErrorWorkDate
        {
            get { return errorWorkDate_; }
            set { errorWorkDate_ = value; RaisePropertyChanged(() => ErrorWorkDate); }
        }

        private bool errorShift_;

        public bool ErrorShift
        {
            get { return errorShift_; }
            set { errorShift_ = value; RaisePropertyChanged(() => ErrorShift); }
        }

        private bool errorReason_;

        public bool ErrorReason
        {
            get { return errorReason_; }
            set { errorReason_ = value; RaisePropertyChanged(() => ErrorReason); }
        }

        private bool errorRequestType_;

        public bool ErrorRequestType
        {
            get { return errorRequestType_; }
            set { errorRequestType_ = value; RaisePropertyChanged(() => ErrorRequestType); }
        }

        #endregion validators
    }
}