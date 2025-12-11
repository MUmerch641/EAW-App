namespace EatWork.Mobile.Models.FormHolder.Approvals
{
    public class OfficialBusinessApprovalHolder : ApprovalHolder
    {
        public OfficialBusinessApprovalHolder()
        {
            IsSuccess = false;
        }

        private string dateFiled_;

        public string DateFiled
        {
            get { return dateFiled_; }
            set { dateFiled_ = value; RaisePropertyChanged(() => DateFiled); }
        }

        private string obDate_;

        public string OBDate
        {
            get { return obDate_; }
            set { obDate_ = value; RaisePropertyChanged(() => OBDate); }
        }

        private string obTime_;

        public string OBTime
        {
            get { return obTime_; }
            set { obTime_ = value; RaisePropertyChanged(() => OBTime); }
        }

        private string obHours_;

        public string OBHours
        {
            get { return obHours_; }
            set { obHours_ = value; RaisePropertyChanged(() => OBHours); }
        }

        private string reason_;

        public string Reason
        {
            get { return reason_; }
            set { reason_ = value; RaisePropertyChanged(() => Reason); }
        }

        private string applyAgainst_;

        public string ApplyAgainst
        {
            get { return applyAgainst_; }
            set { applyAgainst_ = value; RaisePropertyChanged(() => ApplyAgainst); }
        }

        private OfficialBusinessModel officialBusinessModel_;

        public OfficialBusinessModel OfficialBusinessModel
        {
            get { return officialBusinessModel_; }
            set { officialBusinessModel_ = value; RaisePropertyChanged(() => OfficialBusinessModel); }
        }
    }
}