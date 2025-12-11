namespace EatWork.Mobile.Models.FormHolder.Approvals
{
    public class ChangeWorkScheduleApprovalHolder : ApprovalHolder
    {
        public ChangeWorkScheduleApprovalHolder()
        {
            IsSuccess = false;
        }

        private string dateFiled_;

        public string DateFiled
        {
            get { return dateFiled_; }
            set { dateFiled_ = value; RaisePropertyChanged(() => DateFiled); }
        }

        private string workDate_;

        public string WorkDate
        {
            get { return workDate_; }
            set { workDate_ = value; RaisePropertyChanged(() => WorkDate); }
        }

        private string originalSchedule_;

        public string OriginalSchedule
        {
            get { return originalSchedule_; }
            set { originalSchedule_ = value; RaisePropertyChanged(() => OriginalSchedule); }
        }

        private string requestedSchedule_;

        public string RequestedSchedule
        {
            get { return requestedSchedule_; }
            set { requestedSchedule_ = value; RaisePropertyChanged(() => RequestedSchedule); }
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

        private string swapWithEmployeeName_;

        public string SwapWithEmployeeName
        {
            get { return swapWithEmployeeName_; }
            set { swapWithEmployeeName_ = value; RaisePropertyChanged(() => SwapWithEmployeeName); }
        }

        private string originalShiftName_;

        public string OriginalShiftName
        {
            get { return originalShiftName_; }
            set { originalShiftName_ = value; RaisePropertyChanged(() => OriginalShiftName); }
        }

        private string requestedShiftName_;

        public string RequestedShiftName
        {
            get { return requestedShiftName_; }
            set { requestedShiftName_ = value; RaisePropertyChanged(() => RequestedShiftName); }
        }

        private string reason_;

        public string Reason
        {
            get { return reason_; }
            set { reason_ = value; RaisePropertyChanged(() => Reason); }
        }

        private string details_;

        public string Details
        {
            get { return details_; }
            set { details_ = value; RaisePropertyChanged(() => Details); }
        }

        private ChangeWorkScheduleModel changeWorkScheduleModel_;

        public ChangeWorkScheduleModel ChangeWorkScheduleModel
        {
            get { return changeWorkScheduleModel_; }
            set { changeWorkScheduleModel_ = value; RaisePropertyChanged(() => ChangeWorkScheduleModel); }
        }
    }
}