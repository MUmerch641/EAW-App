using EatWork.Mobile.Contants;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Utils;
using System;
using System.Collections.ObjectModel;

namespace EatWork.Mobile.Models.FormHolder.Approvals
{
    public class MyApprovalHolder : ExtendedBindableObject
    {
        public MyApprovalHolder()
        {
            MyApprovalList = new ObservableCollection<MyApprovalListModel>();
            ShowAll = false;
            ShowAllIcon = Constants.Circle;
            Status = RequestStatusValue.ForApproval.ToString();
            SelectedStatus = new ObservableCollection<SelectableListModel>();
            StatusList = new ObservableCollection<SelectableListModel>();
        }

        private ObservableCollection<MyApprovalListModel> myApprovalList_;

        public ObservableCollection<MyApprovalListModel> MyApprovalList
        {
            get { return myApprovalList_; }
            set { myApprovalList_ = value; RaisePropertyChanged(() => MyApprovalList); }
        }

        private bool showAll_;

        public bool ShowAll
        {
            get { return showAll_; }
            set { showAll_ = value; RaisePropertyChanged(() => ShowAll); }
        }

        private string showAllIcon_;

        public string ShowAllIcon
        {
            get { return showAllIcon_; }
            set { showAllIcon_ = value; RaisePropertyChanged(() => ShowAllIcon); }
        }

        private string status_;

        public string Status
        {
            get { return status_; }
            set { status_ = value; RaisePropertyChanged(() => Status); }
        }

        private DateTime? startDate_;

        public DateTime? StartDate
        {
            get { return startDate_; }
            set { startDate_ = value; RaisePropertyChanged(() => StartDate); }
        }

        private DateTime? endDate_;

        public DateTime? EndDate
        {
            get { return endDate_; }
            set { endDate_ = value; RaisePropertyChanged(() => EndDate); }
        }

        private ObservableCollection<SelectableListModel> statusList_;

        public ObservableCollection<SelectableListModel> StatusList
        {
            get { return statusList_; }
            set { statusList_ = value; RaisePropertyChanged(() => StatusList); }
        }

        private ObservableCollection<SelectableListModel> selectedStatus_;

        public ObservableCollection<SelectableListModel> SelectedStatus
        {
            get { return selectedStatus_; }
            set { selectedStatus_ = value; RaisePropertyChanged(() => SelectedStatus); }
        }
    }
}