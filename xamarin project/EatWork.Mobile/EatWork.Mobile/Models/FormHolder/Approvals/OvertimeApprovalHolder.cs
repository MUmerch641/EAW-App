namespace EatWork.Mobile.Models.FormHolder.Approvals
{
    public class OvertimeApprovalHolder : ApprovalHolder
    {
        public OvertimeApprovalHolder()
        {
            IsSuccess = false;
            ApprovedHours = 0;
        }

        private string dateFiled_;

        public string DateFiled
        {
            get { return dateFiled_; }
            set { dateFiled_ = value; RaisePropertyChanged(() => DateFiled); }
        }

        private string overtimeDate_;

        public string OvertimeDate
        {
            get { return overtimeDate_; }
            set { overtimeDate_ = value; RaisePropertyChanged(() => OvertimeDate); }
        }

        private string timeRange_;

        public string TimeRange
        {
            get { return timeRange_; }
            set { timeRange_ = value; RaisePropertyChanged(() => TimeRange); }
        }

        private string overtimeHours_;

        public string OvertimeHours
        {
            get { return overtimeHours_; }
            set { overtimeHours_ = value; RaisePropertyChanged(() => OvertimeHours); }
        }

        private string isPreshift_;

        public string IsPreshift
        {
            get { return isPreshift_; }
            set { isPreshift_ = value; RaisePropertyChanged(() => IsPreshift); }
        }

        private string reason_;

        public string Reason
        {
            get { return reason_; }
            set { reason_ = value; RaisePropertyChanged(() => Reason); }
        }

        private string isOffSetting_;

        public string IsOffSetting
        {
            get { return isOffSetting_; }
            set { isOffSetting_ = value; RaisePropertyChanged(() => IsOffSetting); }
        }

        private string offsettingExpirationDate_;

        public string OffsettingExpirationDate
        {
            get { return offsettingExpirationDate_; }
            set { offsettingExpirationDate_ = value; RaisePropertyChanged(() => OffsettingExpirationDate); }
        }

        private bool showOffsetting_;

        public bool ShowOffsetting
        {
            get { return showOffsetting_; }
            set { showOffsetting_ = value; RaisePropertyChanged(() => ShowOffsetting); }
        }

        private decimal approvedHours_;

        public decimal ApprovedHours
        {
            get { return approvedHours_; }
            set { approvedHours_ = value; RaisePropertyChanged(() => ApprovedHours); }
        }


        private OvertimeModel overtimeModel_;

        public OvertimeModel OvertimeModel
        {
            get { return overtimeModel_; }
            set { overtimeModel_ = value; RaisePropertyChanged(() => OvertimeModel); }
        }
    }
}