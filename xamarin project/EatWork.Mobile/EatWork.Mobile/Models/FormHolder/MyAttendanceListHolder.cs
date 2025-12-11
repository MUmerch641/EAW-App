using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Utils;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace EatWork.Mobile.Models.FormHolder
{
    public class MyAttendanceListHolder : ExtendedBindableObject
    {
        public MyAttendanceListHolder()
        {
            RequestTypes = new ObservableCollection<SelectableListModel>();
            DisplayRequestNavigator = false;

            RetrieveRequestTypes();
        }

        private ObservableCollection<MyAttendanceListModel> myAttedanceList_;

        public ObservableCollection<MyAttendanceListModel> MyAttendanceList
        {
            get { return myAttedanceList_; }
            set { myAttedanceList_ = value; RaisePropertyChanged(() => MyAttendanceList); }
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

        private bool displayNavigator_;

        public bool DisplayRequestNavigator
        {
            get { return displayNavigator_; }
            set { displayNavigator_ = value; RaisePropertyChanged(() => DisplayRequestNavigator); }
        }

        private ObservableCollection<SelectableListModel> requestTypes_;

        public ObservableCollection<SelectableListModel> RequestTypes
        {
            get { return requestTypes_; }
            set { requestTypes_ = value; RaisePropertyChanged(() => RequestTypes); }
        }

        private void RetrieveRequestTypes()
        {
            var requestType_ = new RequestType();

            foreach (var item in requestType_.RequetTypeList.Where(p => p.IsVisible == 1))
                RequestTypes.Add(new SelectableListModel() { Id = item.RequestTypeId, DisplayText = item.Title, IsChecked = false });
        }
    }

    public class IndividualAttendanceHolder : ExtendedBindableObject
    {
        public IndividualAttendanceHolder()
        {
            RequestTypes = new ObservableCollection<SelectableListModel>();
            DisplayRequestNavigator = false;

            RetrieveRequestTypes();
        }

        private ObservableCollection<IndividualAttendance> myAttedanceList_;

        public ObservableCollection<IndividualAttendance> MyAttendanceList
        {
            get { return myAttedanceList_; }
            set { myAttedanceList_ = value; RaisePropertyChanged(() => MyAttendanceList); }
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

        private bool displayNavigator_;

        public bool DisplayRequestNavigator
        {
            get { return displayNavigator_; }
            set { displayNavigator_ = value; RaisePropertyChanged(() => DisplayRequestNavigator); }
        }

        private ObservableCollection<SelectableListModel> requestTypes_;

        public ObservableCollection<SelectableListModel> RequestTypes
        {
            get { return requestTypes_; }
            set { requestTypes_ = value; RaisePropertyChanged(() => RequestTypes); }
        }

        private void RetrieveRequestTypes()
        {
            var requestType_ = new RequestType();

            foreach (var item in requestType_.RequetTypeList.Where(p => p.IsVisible == 1))
                RequestTypes.Add(new SelectableListModel() { Id = item.RequestTypeId, DisplayText = item.Title, IsChecked = false });
        }
    }
}