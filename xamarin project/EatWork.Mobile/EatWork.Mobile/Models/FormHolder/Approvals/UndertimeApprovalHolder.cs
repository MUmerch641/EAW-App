namespace EatWork.Mobile.Models.FormHolder.Approvals
{
    public class UndertimeApprovalHolder : ApprovalHolder
    {
        public UndertimeApprovalHolder()
        {
            IsSuccess = false;
        }

        private string dateFiled_;

        public string DateFiled
        {
            get { return dateFiled_; }
            set { dateFiled_ = value; RaisePropertyChanged(() => DateFiled); }
        }

        private string undertimeDate_;

        public string UndertimeDate
        {
            get { return undertimeDate_; }
            set { undertimeDate_ = value; RaisePropertyChanged(() => UndertimeDate); }
        }

        private string timerange_;

        public string TimeRange
        {
            get { return timerange_; }
            set { timerange_ = value; RaisePropertyChanged(() => TimeRange); }
        }

        private string undertimeHours_;

        public string UndertimeHours
        {
            get { return undertimeHours_; }
            set { undertimeHours_ = value; RaisePropertyChanged(() => UndertimeHours); }
        }

        private string reason_;

        public string Reason
        {
            get { return reason_; }
            set { reason_ = value; RaisePropertyChanged(() => Reason); }
        }

        private string remarks_;

        public string Remarks
        {
            get { return remarks_; }
            set { remarks_ = value; RaisePropertyChanged(() => Remarks); }
        }

        private UndertimeModel undertimeModel_;

        public UndertimeModel UndertimeModel
        {
            get { return undertimeModel_; }
            set { undertimeModel_ = value; RaisePropertyChanged(() => UndertimeModel); }
        }
    }
}