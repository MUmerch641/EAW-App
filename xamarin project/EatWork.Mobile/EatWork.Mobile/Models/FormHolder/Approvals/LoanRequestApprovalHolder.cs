namespace EatWork.Mobile.Models.FormHolder.Approvals
{
    public class LoanRequestApprovalHolder : ApprovalHolder
    {
        public LoanRequestApprovalHolder()
        {
            IsSuccess = false;
        }

        private string dateFiled_;

        public string DateFiled
        {
            get { return dateFiled_; }
            set { dateFiled_ = value; RaisePropertyChanged(() => DateFiled); }
        }

        private string loanType_;

        public string LoanType
        {
            get { return loanType_; }
            set { loanType_ = value; RaisePropertyChanged(() => LoanType); }
        }

        private string reason_;

        public string Reason
        {
            get { return reason_; }
            set { reason_ = value; RaisePropertyChanged(() => Reason); }
        }

        private LoanRequestModel model_;

        public LoanRequestModel LoanRequestModel
        {
            get { return model_; }
            set { model_ = value; RaisePropertyChanged(() => LoanRequestModel); }
        }
    }
}