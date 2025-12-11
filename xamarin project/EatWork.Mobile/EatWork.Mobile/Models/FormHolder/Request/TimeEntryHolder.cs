using System;

namespace EatWork.Mobile.Models.FormHolder.Request
{
    public class TimeEntryHolder : RequestHolder
    {
        public TimeEntryHolder()
        {
            LogDate = DateTime.Now;
            LogTime = DateTime.Now.TimeOfDay;
            TimeInChecked = false;
            TimeOutChecked = false;
            BreakInChecked = false;
            BreakOutChecked = false;
            EnabledTime = true;
            OverrideLeaveConflict = true;
        }

        private DateTime? timeEntryDate_;

        public DateTime? LogDate
        {
            get { return timeEntryDate_; }
            set { timeEntryDate_ = value; RaisePropertyChanged(() => LogDate); }
        }

        private TimeSpan? timeEntryTime_;

        public TimeSpan? LogTime
        {
            get { return timeEntryTime_; }
            set { timeEntryTime_ = value; RaisePropertyChanged(() => LogTime); }
        }

        private bool timeIn_;

        public bool TimeInChecked
        {
            get { return timeIn_; }
            set { timeIn_ = value; RaisePropertyChanged(() => TimeInChecked); }
        }

        private bool timeOut_;

        public bool TimeOutChecked
        {
            get { return timeOut_; }
            set { timeOut_ = value; RaisePropertyChanged(() => TimeOutChecked); }
        }

        private bool breakIn_;

        public bool BreakInChecked
        {
            get { return breakIn_; }
            set { breakIn_ = value; RaisePropertyChanged(() => BreakInChecked); }
        }

        private bool breakOut_;

        public bool BreakOutChecked
        {
            get { return breakOut_; }
            set { breakOut_ = value; RaisePropertyChanged(() => BreakOutChecked); }
        }

        private bool errorTimeEntryDate_;

        public bool ErrorTimeEntryDate
        {
            get { return errorTimeEntryDate_; }
            set { errorTimeEntryDate_ = value; RaisePropertyChanged(() => ErrorTimeEntryDate); }
        }

        private bool errorTimeEntryTime_;

        public bool ErrorTimeEntryTime
        {
            get { return errorTimeEntryTime_; }
            set { errorTimeEntryTime_ = value; RaisePropertyChanged(() => ErrorTimeEntryTime); }
        }

        private bool errorRemark_;

        public bool ErrorRemark
        {
            get { return errorRemark_; }
            set { errorRemark_ = value; RaisePropertyChanged(() => ErrorRemark); }
        }

        private TimeEntryLogModel timeEntryLogModel_;

        public TimeEntryLogModel TimeEntryLogModel
        {
            get { return timeEntryLogModel_; }
            set { timeEntryLogModel_ = value; RaisePropertyChanged(() => TimeEntryLogModel); }
        }

        private bool enabledTime_;

        public bool EnabledTime
        {
            get { return enabledTime_; }
            set { enabledTime_ = value; RaisePropertyChanged(() => EnabledTime); }
        }

        private bool overrideLeaveConflict_;

        public bool OverrideLeaveConflict
        {
            get { return overrideLeaveConflict_; }
            set { overrideLeaveConflict_ = value; RaisePropertyChanged(() => OverrideLeaveConflict); }
        }
    }
}