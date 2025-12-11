using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Excemptions;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.Expenses;
using EatWork.Mobile.Views.Expenses;
using EatWork.Mobile.Views.Shared;
using EatWork.Mobile.Views.TravelRequest;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.Expenses
{
    public class NewExpenseViewModel : BaseViewModel
    {
        public ICommand SubmitCommand { get; set; }
        public ICommand TakePhotoCommand { get; set; }
        public ICommand AttachFileCommand { get; set; }
        public ICommand ResetFormCommand { get; set; }
        public ICommand RemoveFileAttachedCommand { get; set; }
        public ICommand SelectedExpenseTypeCommand { get; set; }
        public ICommand AddNewVendorCommand { get; set; }
        public ICommand HideVendorFormCommand { get; set; }
        public ICommand ResetVendorFormFormCommand { get; set; }
        public ICommand SaveNewVendorCommand { get; set; }
        public ICommand FileChooseOptionCommand { get; set; }
        public ICommand PageAppearingCommand { get; set; }
        public ICommand PageDisappearingCommand { get; set; }

        /*public ICommand SelectedVendorCommand { get; set; }*/

        private NewExpenseHolder holder_;

        public NewExpenseHolder Holder
        {
            get { return holder_; }
            set { holder_ = value; RaisePropertyChanged(() => Holder); }
        }

        private readonly ICommonDataService commonService_;
        private readonly IExpenseDataService service_;
        private readonly IDialogService dialogService_;

        public NewExpenseViewModel(ICommonDataService commonService,
            IExpenseDataService service)
        {
            commonService_ = commonService;
            service_ = service;
            dialogService_ = AppContainer.Resolve<IDialogService>();
        }

        public void Init(INavigation navigation)
        {
            NavigationBack = navigation;

            InitForm();
            InitHelpers();
        }

        private void InitForm()
        {
            Holder = new NewExpenseHolder();
            SubmitCommand = new Command(ExecuteSubmitCommand);
            TakePhotoCommand = new Command(ExecuteTakePhotoCommand);
            AttachFileCommand = new Command(ExecuteAttachFileCommand);
            ResetFormCommand = new Command(ExecuteResetFormCommand);
            RemoveFileAttachedCommand = new Command(() => Holder.FileUploadResponse = new Models.DataObjects.FileUploadResponse());
            SelectedExpenseTypeCommand = new Command<ExpenseSetupModel>(ExecuteSelectedExpenseTypeCommand);
            AddNewVendorCommand = new Command<View>(ExecuteAddNewVendorCommand);
            HideVendorFormCommand = new Command(async () =>
            {
                using (Dialogs.Loading())
                {
                    await Task.Delay(500);
                    Holder.ShowVendorForm = false;
                }
            });

            /*SaveNewVendorCommand = new Command(ExecuteSaveNewVendorCommand);*/
            ResetVendorFormFormCommand = new Command(ExecuteResetVendorFormFormCommand);

            FileChooseOptionCommand = new Command<FileUploadResponse>(ExecuteFileChooseOptionCommand);
            PageAppearingCommand = new Command(ExecutePageAppearingCommand);
            PageDisappearingCommand = new Command(ExecutePageDisappearingCommand);

            /*SelectedVendorCommand = new Command<VendorModel>(ExecuteSelectedVendorCommand);*/
        }

        private async void InitHelpers()
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);

                    Holder = await service_.InitExpenseForm();
                }
                catch (Exception ex)
                {
                    Error(false, ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private void ExecuteSelectedExpenseTypeCommand(ExpenseSetupModel item)
        {
            if (item != null)
            {
                Holder.Icon = item.IconEquivalent;
            }
        }

        private async void ExecuteTakePhotoCommand()
        {
            try
            {
                var response = await commonService_.TakePhotoAsync("EXPENSE");
                /*var file = await commonService_.AttachPhoto2(response, "EXPENSE");*/

                if (response != null && !string.IsNullOrWhiteSpace(response.FileName))
                {
                    /*Holder.FileUploadResponse = file;*/
                    Holder.FileAttachments = new ObservableCollection<Models.DataObjects.FileUploadResponse>
                    {
                        response
                    };
                    Holder.FileUploadResponse = response;
                    Holder = Holder;
                }
            }
            catch (Exception ex)
            {
                await dialogService_.AlertAsync(ex.Message);
            }
        }

        private async void ExecuteAttachFileCommand()
        {
            try
            {
                var response = await commonService_.FileUploadAsync();

                if (response != null && !string.IsNullOrWhiteSpace(response.FileName))
                {
                    /*Holder.FileUploadResponse = response;*/
                    Holder.FileAttachments = new ObservableCollection<Models.DataObjects.FileUploadResponse>
                    {
                        response
                    };
                    Holder.FileUploadResponse = response;
                    Holder = Holder;
                }
            }
            catch (Exception ex)
            {
                await dialogService_.AlertAsync(ex.Message);
            }
        }

        private void ExecuteResetFormCommand()
        {
            Holder.SelectedExpenseType = null;
            Holder.ExpenseType = new Validations.ValidatableObject<long>();
            Holder.ORNumber = new Validations.ValidatableObject<string>();
            Holder.SupplierName = new Validations.ValidatableObject<string>();
            Holder.Notes = new Validations.ValidatableObject<string>();
            Holder.Amount = new Validations.ValidatableObject<decimal>();
            Holder.FileUploadResponse = new Models.DataObjects.FileUploadResponse();
            Holder.Icon = Xamarin.Forms.Application.Current.Resources["EllipsisIcon"].ToString();
            Holder.SelectedVendor = null;
        }

        private async void ExecuteSubmitCommand()
        {
            try
            {
                Holder = await service_.SubmitRecord(Holder);

                if (Holder.Success)
                {
                    Success(true, Messages.RecordSaved);
                    await NavigationService.PopToRootAsync();
                }
            }
            catch (HttpRequestExceptionEx ex)
            {
                var list = new ObservableCollection<string>(ex.Model.Errors.Values.Select(p => p[0]));
                Error(results: list, title: ex.Model.Title.ToUpper(), autoHide: false);
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
            finally
            {
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private async void ExecuteAddNewVendorCommand(View view)
        {
            using (Dialogs.Loading())
            {
                await Task.Delay(500);

                Device.BeginInvokeOnMainThread(() =>
                {
                    view?.Unfocus();
                });

                /*Holder.ShowVendorForm = true;*/

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(new NewVendorModal());
            }
        }

        private async void ExecuteSaveNewVendorCommand(VendorModel item)
        {
            using (Dialogs.Loading())
            {
                await Task.Delay(500);

                Holder.NewVendor = item;

                Holder.Vendors.Add(Holder.NewVendor);
                Holder.SelectedVendor = item;

                /*Holder.ShowVendorForm = false;*/
            }
        }

        private void ExecuteResetVendorFormFormCommand()
        {
            Holder.NewSupplierName = new Validations.ValidatableObject<string>();
            Holder.NewTinNumber = new Validations.ValidatableObject<string>();
            Holder.NewAddress = new Validations.ValidatableObject<string>();
        }

        private async void ExecuteFileChooseOptionCommand(FileUploadResponse file)
        {
            if (file != null)
            {
                Holder.SelectedFile = file;

                using (Dialogs.Loading())
                {
                    await Task.Delay(500);
                    await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(new FileOptionModal());
                }
            }
        }

        private async void ExecuteChosenOption(long option)
        {
            var file = Holder.SelectedFile;

            //view file
            if (option == 1)
            {
                using (Dialogs.Loading())
                {
                    await Task.Delay(500);

                    if (file.FileDataArray != null)
                    {
                        var base64 = $"data:{file.MimeType};base64,{Convert.ToBase64String(file.FileDataArray)}";
                        await commonService_.PreviewFileBase64(base64, file.FileType, file.FileName);
                    }
                    else
                    {
                        //var urlFile = await service_.ViewAttachment(file.FileUpload, file.FileName, file.FileType);
                    }
                }
            }
            //delete file
            else
            {
                Holder.FileAttachments.Remove(file);
                Holder = Holder;
            }
        }

        private void ExecutePageAppearingCommand()
        {
            MessagingCenter.Subscribe<FileOptionViewModel, long>(this, "FileOptionSelectedValue", (s, param) =>
            {
                if (param > 0)
                {
                    ExecuteChosenOption(param);
                }
            });

            MessagingCenter.Subscribe<NewVendorViewModel, VendorModel>(this, "NewVendorCreated", (s, param) =>
            {
                if (param != null)
                {
                    ExecuteSaveNewVendorCommand(param);
                }
            });

            MessagingCenter.Subscribe<NewExpensePage>(this, "onback", (sender) =>
            {
                BackItemPage();
            });
        }

        private void ExecutePageDisappearingCommand()
        {
            MessagingCenter.Unsubscribe<FileOptionViewModel, long>(this, "FileOptionSelectedValue");
            MessagingCenter.Unsubscribe<NewVendorViewModel, VendorModel>(this, "NewVendorCreated");
            MessagingCenter.Unsubscribe<NewExpensePage>(this, "onback");
        }

        protected override async void BackItemPage()
        {
            if (Holder.SelectedExpenseType.ExpenseSetupId > 0 || Holder.Amount.Value > 0 ||
                Holder.SelectedVendor.VendorId > 0)
            {
                if (await dialogService_.ConfirmDialogAsync(Messages.LEAVEPAGE))
                {
                    base.BackItemPage();
                }
            }
            else
            {
                base.BackItemPage();
            }
        }
    }
}