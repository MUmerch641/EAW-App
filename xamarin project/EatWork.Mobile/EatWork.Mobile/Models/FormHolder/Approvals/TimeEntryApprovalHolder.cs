namespace EatWork.Mobile.Models.FormHolder.Approvals
{
    public class TimeEntryApprovalHolder : ApprovalHolder
    {
        public TimeEntryApprovalHolder()
        {
            IsSuccess = false;
        }

        private string date_;

        public string LogDate
        {
            get { return date_; }
            set { date_ = value; RaisePropertyChanged(() => LogDate); }
        }

        private string timeEntryType_;

        public string TimeEntryType
        {
            get { return timeEntryType_; }
            set { timeEntryType_ = value; RaisePropertyChanged(() => TimeEntryType); }
        }

        private string time_;

        public string LogTime
        {
            get { return time_; }
            set { time_ = value; RaisePropertyChanged(() => LogTime); }
        }

        private string reason_;

        public string Reason
        {
            get { return reason_; }
            set { reason_ = value; RaisePropertyChanged(() => Reason); }
        }

        private TimeEntryLogModel timeEntryModel_;

        public TimeEntryLogModel TimeEntryModel
        {
            get { return timeEntryModel_; }
            set { timeEntryModel_ = value; RaisePropertyChanged(() => TimeEntryModel); }
        }
    }
}