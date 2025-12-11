namespace EatWork.Mobile.Models.FormHolder.Approvals
{
    public class LeaveUsageListHolder : LeaveUsageList
    {
        public LeaveUsageListHolder()
        {
            IsVisible = false;
        }

        private bool isVisible_;

        public bool IsVisible
        {
            get { return isVisible_; }
            set { isVisible_ = value; RaisePropertyChanged(() => IsVisible); }
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
    }
}