namespace EatWork.Mobile.Models.FormHolder.Approvals
{
    public class SpecialWorkScheduleApprovalHolder : ApprovalHolder
    {
        public SpecialWorkScheduleApprovalHolder()
        {
            IsSuccess = false;
        }

        private string dateFiled_;

        public string DateFiled
        {
            get { return dateFiled_; }
            set { dateFiled_ = value; RaisePropertyChanged(() => DateFiled); }
        }

        private string requestType_;

        public string RequestType
        {
            get { return requestType_; }
            set { requestType_ = value; RaisePropertyChanged(() => RequestType); }
        }

        private string workDate_;

        public string WorkDate
        {
            get { return workDate_; }
            set { workDate_ = value; RaisePropertyChanged(() => WorkDate); }
        }

        private string shiftcode_;

        public string ShiftCode
        {
            get { return shiftcode_; }
            set { shiftcode_ = value; RaisePropertyChanged(() => ShiftCode); }
        }

        private string lunchSchedule_;

        public string LunchSchedule
        {
            get { return lunchSchedule_; }
            set { lunchSchedule_ = value; RaisePropertyChanged(() => LunchSchedule); }
        }


        private string workingHours_;

        public string WorkingHours
        {
            get { return workingHours_; }
            set { workingHours_ = value; RaisePropertyChanged(() => WorkingHours); }
        }

        private string lunchDuration_;

        public string LunchDuration
        {
            get { return lunchDuration_; }
            set { lunchDuration_ = value; RaisePropertyChanged(() => LunchDuration); }
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


        private SpecialWorkScheduleRequestModel specialWorkScheduleRequestModel_;

        public SpecialWorkScheduleRequestModel SpecialWorkScheduleRequestModel
        {
            get { return specialWorkScheduleRequestModel_; }
            set { specialWorkScheduleRequestModel_ = value; RaisePropertyChanged(() => SpecialWorkScheduleRequestModel); }
        }
    }
}