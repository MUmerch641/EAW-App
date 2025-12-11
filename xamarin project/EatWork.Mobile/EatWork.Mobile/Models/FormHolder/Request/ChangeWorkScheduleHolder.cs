using EatWork.Mobile.Models.DataObjects;
using System;
using System.Collections.ObjectModel;

namespace EatWork.Mobile.Models.FormHolder.Request
{
    public class ChangeWorkScheduleHolder : RequestHolder
    {
        public ChangeWorkScheduleHolder()
        {
            OriginalShiftCode = string.Empty;
            SwapWith = string.Empty;
            Success = false;
            //ScheduleStartTime = Constants.NullDate.TimeOfDay;
            //ScheduleEndTime = Constants.NullDate.TimeOfDay;
            //LunchStartTime = Constants.NullDate.TimeOfDay;
            //LunchEndTime = Constants.NullDate.TimeOfDay;
            EnableStartTimePreviousDay = true;
            EnableEndTimeNextDay = true;
            EnableCustomSched = false;
            SwapWith = string.Empty;
            WorkDate = DateTime.Now.Date;
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

        private ObservableCollection<ComboBoxObject> reasonList_;

        public ObservableCollection<ComboBoxObject> ReasonList
        {
            get { return reasonList_; }
            set { reasonList_ = value; RaisePropertyChanged(() => ReasonList); }
        }

        private ComboBoxObject reasonSelectedItem_;

        public ComboBoxObject ReasonSelectedItem
        {
            get { return reasonSelectedItem_; }
            set { reasonSelectedItem_ = value; RaisePropertyChanged(() => ReasonSelectedItem); }
        }

        private DateTime workDate_;

        public DateTime WorkDate
        {
            get { return workDate_; }
            set { workDate_ = value; RaisePropertyChanged(() => WorkDate); }
        }

        private string originalShiftCode_;

        public string OriginalShiftCode
        {
            get { return originalShiftCode_; }
            set { originalShiftCode_ = value; RaisePropertyChanged(() => OriginalShiftCode); }
        }

        private bool enableCustomSched_;

        public bool EnableCustomSched
        {
            get { return enableCustomSched_; }
            set { enableCustomSched_ = value; RaisePropertyChanged(() => EnableCustomSched); }
        }

        private bool enableStartTimePreviousDay_;

        public bool EnableStartTimePreviousDay
        {
            get { return enableStartTimePreviousDay_; }
            set { enableStartTimePreviousDay_ = value; RaisePropertyChanged(() => EnableStartTimePreviousDay); }
        }

        private bool enableEndTimeNextDay_;

        public bool EnableEndTimeNextDay
        {
            get { return enableEndTimeNextDay_; }
            set { enableEndTimeNextDay_ = value; RaisePropertyChanged(() => EnableEndTimeNextDay); }
        }

        private string swapWith_;

        public string SwapWith
        {
            get { return swapWith_; }
            set { swapWith_ = value; RaisePropertyChanged(() => SwapWith); }
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

        private string orignalSchedule_;

        public string OriginalSchedule
        {
            get { return orignalSchedule_; }
            set { orignalSchedule_ = value; RaisePropertyChanged(() => OriginalSchedule); }
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

        private ChangeWorkScheduleModel model_;

        public ChangeWorkScheduleModel ChangeWorkScheduleModel
        {
            get { return model_; }
            set { model_ = value; RaisePropertyChanged(() => ChangeWorkScheduleModel); }
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

        private bool errorRequestedStartTime_;

        public bool ErrorRequestedStartTime
        {
            get { return errorRequestedStartTime_; }
            set { errorRequestedStartTime_ = value; RaisePropertyChanged(() => ErrorRequestedStartTime); }
        }

        private bool errorRequestedEndTime_;

        public bool ErrorRequestedEndTime
        {
            get { return errorRequestedEndTime_; }
            set { errorRequestedEndTime_ = value; RaisePropertyChanged(() => ErrorRequestedEndTime); }
        }

        private bool errorReason_;

        public bool ErrorReason
        {
            get { return errorReason_; }
            set { errorReason_ = value; RaisePropertyChanged(() => ErrorReason); }
        }

        private bool errorDetails_;

        public bool ErrorDetails
        {
            get { return errorDetails_; }
            set { errorDetails_ = value; RaisePropertyChanged(() => ErrorDetails); }
        }

        #endregion validators
    }
}