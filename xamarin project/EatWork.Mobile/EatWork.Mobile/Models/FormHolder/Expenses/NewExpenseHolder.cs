using EatWork.Mobile.Utils;
using EatWork.Mobile.Validations;
using System;
using System.Collections.ObjectModel;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Models.FormHolder.Expenses
{
    public class NewExpenseHolder : RequestHolder
    {
        public NewExpenseHolder()
        {
            ExpenseType = new ValidatableObject<long>();
            ORNumber = new ValidatableObject<string>();
            SupplierName = new ValidatableObject<string>();
            Amount = new ValidatableObject<decimal>();
            Notes = new ValidatableObject<string>();
            Model = new R.Models.AppExpenseReportDetail();
            ExpenseDate = DateTime.UtcNow;
            ExpenseTypes = new ObservableCollection<ExpenseSetupModel>();
            SelectedExpenseType = new ExpenseSetupModel();
            Vendors = new ObservableCollection<VendorModel>();
            SelectedVendor = new VendorModel();
            ShowVendorForm = false;
            NewSupplierName = new ValidatableObject<string>();
            NewTinNumber = new ValidatableObject<string>();
            NewAddress = new ValidatableObject<string>();
            NewVendor = new VendorModel();
            Icon = Xamarin.Forms.Application.Current.Resources["EllipsisIcon"].ToString();
        }

        private ValidatableObject<long> expenseType_;

        public ValidatableObject<long> ExpenseType
        {
            get { return expenseType_; }
            set { expenseType_ = value; RaisePropertyChanged(() => ExpenseType); }
        }

        private ValidatableObject<string> orNumber_;

        public ValidatableObject<string> ORNumber
        {
            get { return orNumber_; }
            set { orNumber_ = value; RaisePropertyChanged(() => ORNumber); }
        }

        private ValidatableObject<string> supplierName_;

        public ValidatableObject<string> SupplierName
        {
            get { return supplierName_; }
            set { supplierName_ = value; RaisePropertyChanged(() => SupplierName); }
        }

        private ValidatableObject<decimal> amount_;

        public ValidatableObject<decimal> Amount
        {
            get { return amount_; }
            set { amount_ = value; RaisePropertyChanged(() => Amount); }
        }

        private ValidatableObject<string> notes_;

        public ValidatableObject<string> Notes
        {
            get { return notes_; }
            set { notes_ = value; RaisePropertyChanged(() => Notes); }
        }

        private DateTime expenseDate_;

        public DateTime ExpenseDate
        {
            get { return expenseDate_; }
            set { expenseDate_ = value; RaisePropertyChanged(() => ExpenseDate); }
        }

        private ObservableCollection<ExpenseSetupModel> expenseTypes_;

        public ObservableCollection<ExpenseSetupModel> ExpenseTypes
        {
            get { return expenseTypes_; }
            set { expenseTypes_ = value; RaisePropertyChanged(() => ExpenseTypes); }
        }

        private ExpenseSetupModel selectedExpenseType_;

        public ExpenseSetupModel SelectedExpenseType
        {
            get { return selectedExpenseType_; }
            set { selectedExpenseType_ = value; RaisePropertyChanged(() => SelectedExpenseType); }
        }

        private ObservableCollection<VendorModel> vendors_;

        public ObservableCollection<VendorModel> Vendors
        {
            get { return vendors_; }
            set { vendors_ = value; RaisePropertyChanged(() => Vendors); }
        }

        private VendorModel selectedVendor_;

        public VendorModel SelectedVendor
        {
            get { return selectedVendor_; }
            set { selectedVendor_ = value; RaisePropertyChanged(() => SelectedVendor); }
        }

        private string icon_;

        public string Icon
        {
            get { return icon_; }
            set { icon_ = value; RaisePropertyChanged(() => Icon); }
        }

        private bool showVendorForm_;

        public bool ShowVendorForm
        {
            get { return showVendorForm_; }
            set { showVendorForm_ = value; RaisePropertyChanged(() => ShowVendorForm); }
        }

        private ValidatableObject<string> newSupplierName_;

        public ValidatableObject<string> NewSupplierName
        {
            get { return newSupplierName_; }
            set { newSupplierName_ = value; RaisePropertyChanged(() => NewSupplierName); }
        }

        private ValidatableObject<string> newTinNumber_;

        public ValidatableObject<string> NewTinNumber
        {
            get { return newTinNumber_; }
            set { newTinNumber_ = value; RaisePropertyChanged(() => NewTinNumber); }
        }

        private ValidatableObject<string> newAddress_;

        public ValidatableObject<string> NewAddress
        {
            get { return newAddress_; }
            set { newAddress_ = value; RaisePropertyChanged(() => NewAddress); }
        }

        private VendorModel newVendor_;

        public VendorModel NewVendor
        {
            get { return newVendor_; }
            set { newVendor_ = value; RaisePropertyChanged(() => NewVendor); }
        }

        private R.Models.AppExpenseReportDetail model_;

        public R.Models.AppExpenseReportDetail Model
        {
            get { return model_; }
            set { model_ = value; RaisePropertyChanged(() => Model); }
        }

        public bool ExecuteSubmit()
        {
            var user = PreferenceHelper.UserInfo();

            Model = new R.Models.AppExpenseReportDetail()
            {
                Amount = Amount.Value,
                ExpenseDate = ExpenseDate.Date,
                ExpenseSetupId = SelectedExpenseType.ExpenseSetupId,
                ExpenseType = SelectedExpenseType.ExpenseType,
                AppExpenseReportDetailId = 0,
                Notes = Notes.Value,
                ORNo = ORNumber.Value,
                VendorId = SelectedVendor.VendorId,
                VendorName = SelectedVendor.Name,
                ProfileId = user.ProfileId,
                FileName = FileData.FileName,
            };

            return IsValid();
        }

        public bool IsValid()
        {
            ExpenseType.Validations.Clear();
            /*
            ExpenseType.Validations.Add(new NumberRule<long>
            {
                ValidationMessage = ""
            });
            */

            ORNumber.Validations.Clear();
            ORNumber.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });

            SupplierName.Validations.Clear();
            /*
            SupplierName.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });
            */

            Amount.Validations.Clear();
            Amount.Validations.Add(new NumberRule<decimal>
            {
                ValidationMessage = ""
            });

            Notes.Validations.Clear();
            Notes.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });

            ExpenseType.Validate();
            ORNumber.Validate();
            SupplierName.Validate();
            Amount.Validate();
            Notes.Validate();

            if (SelectedExpenseType.ExpenseSetupId == 0)
            {
                ExpenseType.Errors.Add("");
            }

            if (string.IsNullOrWhiteSpace(SelectedVendor.Name))
            {
                SupplierName.Errors.Add("");
            }

            return ExpenseType.IsValid
                    && ORNumber.IsValid
                    && SupplierName.IsValid
                    && Amount.IsValid
                    && Notes.IsValid;
        }
    }
}