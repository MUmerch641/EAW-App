using System.Collections.ObjectModel;

namespace EatWork.Mobile.Models.FormHolder.Approvals
{
    public class LeaveApprovalHolder : ApprovalHolder
    {
        public LeaveApprovalHolder()
        {
            IsSuccess = false;
            LeaveDocumentModel = new ObservableCollection<LeaveRequestDocumentModel>();
            LeaveRequestDetailListToDisplay = new ObservableCollection<LeaveRequestDetailModel>();
            RequestedHoursDisplayType = "Hours";
        }

        private string leaveType_;

        public string LeaveType
        {
            get { return leaveType_; }
            set { leaveType_ = value; RaisePropertyChanged(() => LeaveType); }
        }

        private string remainingBalance_;

        public string RemainingBalance
        {
            get { return remainingBalance_; }
            set { remainingBalance_ = value; RaisePropertyChanged(() => RemainingBalance); }
        }

        private string inclusiveDate_;

        public string InclusiveDate
        {
            get { return inclusiveDate_; }
            set { inclusiveDate_ = value; RaisePropertyChanged(() => InclusiveDate); }
        }

        private string totalRequestedHours_;

        public string TotalRequestedHours
        {
            get { return totalRequestedHours_; }
            set { totalRequestedHours_ = value; RaisePropertyChanged(() => TotalRequestedHours); }
        }

        private string isPartialDay_;

        public string IsPartialDay
        {
            get { return isPartialDay_; }
            set { isPartialDay_ = value; RaisePropertyChanged(() => IsPartialDay); }
        }

        private string applyTo_;

        public string ApplyTo
        {
            get { return applyTo_; }
            set { applyTo_ = value; RaisePropertyChanged(() => ApplyTo); }
        }

        private string dateFiled_;

        public string DateFiled
        {
            get { return dateFiled_; }
            set { dateFiled_ = value; RaisePropertyChanged(() => DateFiled); }
        }

        private bool showPartialApplyTo_;

        public bool ShowPartialApplyTo
        {
            get { return showPartialApplyTo_; }
            set { showPartialApplyTo_ = value; RaisePropertyChanged(() => ShowPartialApplyTo); }
        }

        private string requestedHoursDisplayType_;

        public string RequestedHoursDisplayType
        {
            get { return requestedHoursDisplayType_; }
            set { requestedHoursDisplayType_ = value; RaisePropertyChanged(() => RequestedHoursDisplayType); }
        }

        private ObservableCollection<LeaveRequestDocumentModel> leaveDocumentModel_;

        public ObservableCollection<LeaveRequestDocumentModel> LeaveDocumentModel
        {
            get { return leaveDocumentModel_; }
            set { leaveDocumentModel_ = value; RaisePropertyChanged(() => LeaveDocumentModel); }
        }

        private ObservableCollection<LeaveRequestDetailModel> leaveRequestDetailListDisplay_;

        public ObservableCollection<LeaveRequestDetailModel> LeaveRequestDetailListToDisplay
        {
            get { return leaveRequestDetailListDisplay_; }
            set { leaveRequestDetailListDisplay_ = value; RaisePropertyChanged(() => LeaveRequestDetailListToDisplay); }
        }

        private LeaveRequestModel leaveRequestModel_;

        public LeaveRequestModel LeaveRequestModel
        {
            get { return leaveRequestModel_; }
            set { leaveRequestModel_ = value; RaisePropertyChanged(() => LeaveRequestModel); }
        }
    }
}