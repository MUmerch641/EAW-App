using EatWork.Mobile.Models.DataObjects;
using EAW.API.DataContracts.Models;
using Syncfusion.SfNumericTextBox.XForms;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace EatWork.Mobile.Models.FormHolder.Request
{
    public class LoanRequestHolder : RequestHolder
    {
        public LoanRequestHolder()
        {
            EmployeeName = string.Empty;
        }

        private ObservableCollection<SelectableListModel> loanTypeList_;

        public ObservableCollection<SelectableListModel> LoanTypeList
        {
            get { return loanTypeList_; }
            set { loanTypeList_ = value; RaisePropertyChanged(() => LoanTypeList); }
        }

        private SelectableListModel selectedLoanType_;

        public SelectableListModel SelectedLoanType
        {
            get { return selectedLoanType_; }
            set { selectedLoanType_ = value; RaisePropertyChanged(() => SelectedLoanType); }
        }

        private bool agreed_;

        public bool Aggreed
        {
            get { return agreed_; }
            set { agreed_ = value; RaisePropertyChanged(() => Aggreed); }
        }

        private bool enableSubmitButton;

        public bool EnableSubmitButton
        {
            get { return enableSubmitButton; }
            set { enableSubmitButton = value; RaisePropertyChanged(() => EnableSubmitButton); }
        }

        private string employeeName_;

        public string EmployeeName
        {
            get { return employeeName_; }
            set { employeeName_ = value; RaisePropertyChanged(() => EmployeeName); }
        }

        private DateTime requestedDate_;

        public DateTime RequestedDate
        {
            get { return requestedDate_; }
            set { requestedDate_ = value; RaisePropertyChanged(() => RequestedDate); }
        }

        private LoanRequestModel model_;

        public LoanRequestModel LoanRequestModel
        {
            get { return model_; }
            set { model_ = value; RaisePropertyChanged(() => LoanRequestModel); }
        }

        public LoanRequestFile LoanRequestFile { get; set; }

        #region validators

        private bool errorLoanType_;

        public bool ErrorLoanType
        {
            get { return errorLoanType_; }
            set { errorLoanType_ = value; RaisePropertyChanged(() => ErrorLoanType); }
        }

        private bool errorRequestedAmount_;

        public bool ErrorRequestedAmount
        {
            get { return errorRequestedAmount_; }
            set { errorRequestedAmount_ = value; RaisePropertyChanged(() => ErrorRequestedAmount); }
        }

        #endregion validators
    }
}