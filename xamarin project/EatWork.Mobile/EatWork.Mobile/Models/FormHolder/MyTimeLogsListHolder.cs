using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Utils;
using System;
using System.Collections.ObjectModel;

namespace EatWork.Mobile.Models.FormHolder
{
    public class MyTimeLogsListHolder : ExtendedBindableObject
    {
        public MyTimeLogsListHolder()
        {
            MyTimeLogsList = new ObservableCollection<MyTimeLogsListModel>();
            Status = new ObservableCollection<SelectableListModel>();
            SelectedStatus = new ObservableCollection<SelectableListModel>();
        }

        private ObservableCollection<MyTimeLogsListModel> myTimeLogs_;

        public ObservableCollection<MyTimeLogsListModel> MyTimeLogsList
        {
            get { return myTimeLogs_; }
            set { myTimeLogs_ = value; RaisePropertyChanged(() => MyTimeLogsList); }
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

        private ObservableCollection<SelectableListModel> _status;

        public ObservableCollection<SelectableListModel> Status
        {
            get { return _status; }
            set { _status = value; RaisePropertyChanged(() => Status); }
        }

        private ObservableCollection<SelectableListModel> _selectedStatus;

        public ObservableCollection<SelectableListModel> SelectedStatus
        {
            get { return _selectedStatus; }
            set { _selectedStatus = value; RaisePropertyChanged(() => SelectedStatus); }
        }
    }
}