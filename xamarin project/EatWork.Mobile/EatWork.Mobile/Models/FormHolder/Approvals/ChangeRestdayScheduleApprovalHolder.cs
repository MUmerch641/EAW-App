namespace EatWork.Mobile.Models.FormHolder.Approvals
{
    public class ChangeRestdayScheduleApprovalHolder : ApprovalHolder
    {
        public ChangeRestdayScheduleApprovalHolder()
        {
            IsSuccess = false;
        }

        private string dateFiled_;

        public string DateFiled
        {
            get { return dateFiled_; }
            set { dateFiled_ = value; RaisePropertyChanged(() => DateFiled); }
        }

        private string restdayDate_;

        public string RestdayDate
        {
            get { return restdayDate_; }
            set { restdayDate_ = value; RaisePropertyChanged(() => RestdayDate); }
        }

        private string requestedDate_;

        public string RequestedDate
        {
            get { return requestedDate_; }
            set { requestedDate_ = value; RaisePropertyChanged(() => RequestedDate); }
        }

        private string swapWith_;

        public string SwapWithEmployeeName
        {
            get { return swapWith_; }
            set { swapWith_ = value; RaisePropertyChanged(() => SwapWithEmployeeName); }
        }

        private ChangeRestdayModel model_;

        public ChangeRestdayModel ChangeRestdayModel
        {
            get { return model_; }
            set { model_ = value; RaisePropertyChanged(() => ChangeRestdayModel); }
        }
    }
}