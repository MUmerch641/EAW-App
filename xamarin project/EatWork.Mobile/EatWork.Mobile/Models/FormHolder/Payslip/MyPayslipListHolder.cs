using EatWork.Mobile.Models.Payslip;
using EatWork.Mobile.Utils;
using System.Collections.ObjectModel;

namespace EatWork.Mobile.Models.FormHolder.Payslip
{
    public class MyPayslipListHolder : ExtendedBindableObject
    {
        public MyPayslipListHolder()
        {
            MyPayslipList = new ObservableCollection<MyPayslipListModel>();
        }

        private ObservableCollection<MyPayslipListModel> myPayslipList_;

        public ObservableCollection<MyPayslipListModel> MyPayslipList
        {
            get { return myPayslipList_; }
            set { myPayslipList_ = value; RaisePropertyChanged(() => MyPayslipList); }
        }
    }
}