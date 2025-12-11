namespace EatWork.Mobile.Models.FormHolder.Approvals
{
    public class DocumentRequestApprovalHolder : ApprovalHolder
    {
        public DocumentRequestApprovalHolder()
        {
            IsSuccess = false;
        }

        private string dateFiled_;

        public string DateFiled
        {
            get { return dateFiled_; }
            set { dateFiled_ = value; RaisePropertyChanged(() => DateFiled); }
        }

        private string dateRequested_;

        public string DateRequested
        {
            get { return dateRequested_; }
            set { dateRequested_ = value; RaisePropertyChanged(() => DateRequested); }
        }

        private string documentType_;

        public string DocumentType
        {
            get { return documentType_; }
            set { documentType_ = value; RaisePropertyChanged(() => DocumentType); }
        }

        private string periodCovered_;

        public string PeriodCovered
        {
            get { return periodCovered_; }
            set { periodCovered_ = value; RaisePropertyChanged(() => PeriodCovered); }
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

        private DocumentRequestModel documentRequestModel_;

        public DocumentRequestModel DocumentRequestModel
        {
            get { return documentRequestModel_; }
            set { documentRequestModel_ = value; RaisePropertyChanged(() => DocumentRequestModel); }
        }
    }
}