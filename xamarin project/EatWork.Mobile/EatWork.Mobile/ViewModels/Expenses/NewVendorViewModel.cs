using EatWork.Mobile.Contants;
using EatWork.Mobile.Models;
using EatWork.Mobile.Validations;
using Rg.Plugins.Popup.Services;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.Expenses
{
    public class NewVendorViewModel : BaseViewModel
    {
        public ICommand CloseModalCommand { get; set; }
        public ICommand AddVendorCommand { get; set; }

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

        public NewVendorViewModel()
        {
        }

        public void Init()
        {
            CloseModalCommand = new Command(async () => await PopupNavigation.Instance.PopAsync(true));
            AddVendorCommand = new Command(ExecuteAddVendorCommand);

            NewSupplierName = new ValidatableObject<string>();
            NewTinNumber = new ValidatableObject<string>();
            NewAddress = new ValidatableObject<string>();
        }

        private async void ExecuteAddVendorCommand()
        {
            if (Isvalid())
            {
                var vendor = new VendorModel()
                {
                    Address = NewAddress.Value,
                    Name = NewSupplierName.Value,
                    TINNo = NewTinNumber.Value,
                    SourceId = (short)SourceEnum.Mobile,
                    VendorId = 0,
                };
                MessagingCenter.Send(this, "NewVendorCreated", vendor);
                await PopupNavigation.Instance.PopAsync(true);
            }
        }

        private bool Isvalid()
        {
            NewSupplierName.Validations.Clear();
            NewSupplierName.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });

            NewSupplierName.Validate();

            NewTinNumber.Validations.Clear();
            NewTinNumber.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });

            NewTinNumber.Validate();

            NewAddress.Validations.Clear();
            NewAddress.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });

            NewAddress.Validate();

            return NewSupplierName.IsValid && NewTinNumber.IsValid && NewAddress.IsValid;
        }
    }
}