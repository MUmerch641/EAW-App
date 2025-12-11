using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.CashAdvance;
using EatWork.Mobile.Views;
using EatWork.Mobile.Views.CashAdvance;
using EatWork.Mobile.Views.Shared;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.CashAdvance
{
    public class CashAdvanceRequestViewModel : BaseViewModel
    {
        public ICommand SubmitCommand { get; set; }
        public ICommand ResetFormCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand CameraCommand { get; set; }
        public ICommand SelectFileCommand { get; set; }
        public ICommand ViewTransactionHistoryCommand { get; set; }
        public ICommand ViewFileAttachmentCommand { get; set; }
        public ICommand FileChooseOptionCommand { get; set; }
        public ICommand PageAppearingCommand { get; set; }
        public ICommand PageDisappearingCommand { get; set; }
        public ICommand ChargeCodeSelectionCommand { get; set; }

        private CashAdvanceRequestHolder holder_;

        public CashAdvanceRequestHolder Holder
        {
            get { return holder_; }
            set { holder_ = value; RaisePropertyChanged(() => Holder); }
        }

        private readonly ICashAdvanceRequestDataService service_;
        private readonly IDialogService dialogs_;
        private readonly ICommonDataService commonService_;

        public CashAdvanceRequestViewModel()
        {
            service_ = AppContainer.Resolve<ICashAdvanceRequestDataService>();
            dialogs_ = AppContainer.Resolve<IDialogService>();
            commonService_ = AppContainer.Resolve<ICommonDataService>();
        }

        public void Init(INavigation navigation, long recordId = 0)
        {
            NavigationBack = navigation;

            InitHelpers();
            InitForm(recordId);
        }

        private void InitHelpers()
        {
            Holder = new CashAdvanceRequestHolder();

            SubmitCommand = new Command(ExecuteSubmitCommand);
            ResetFormCommand = new Command(ExecuteResetFormCommand);
            CancelCommand = new Command(ExecuteCancelCommand);

            CameraCommand = new Command(async () => await ExecuteTakePhotoCommand());
            SelectFileCommand = new Command(async () => await ExecuteAttachFileCommand());

            ViewTransactionHistoryCommand = new Command(async () => await NavigationService.PushModalAsync(new TransactionHistoryPage(TransactionType.CashAdvance, Holder.Model.CashAdvanceId)));
            ViewFileAttachmentCommand = new Command(async () => await ViewFileAttachments());
            PageAppearingCommand = new Command(ExecutePageAppearingCommand);
            PageDisappearingCommand = new Command(ExecutePageDisappearingCommand);
            FileChooseOptionCommand = new Command<FileUploadResponse>(ExecuteFileChooseOptionCommand);
            ChargeCodeSelectionCommand = new Command(ExecuteChargeCodeSelectionCommand);
        }

        private async void InitForm(long recordId)
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);

                    Holder = await service_.InitForm(recordId);
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

        private async void ExecuteSubmitCommand()
        {
            try
            {
                Holder = await service_.SubmitAsync(Holder);

                if (Holder.Success)
                {
                    Success(true, Messages.RecordSaved);
                    await NavigationService.PopToRootAsync();
                }
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

        protected override async void BackItemPage()
        {
            if (Holder.Model.CashAdvanceId == 0)
            {
                if (Holder.Amount.Value > 0 ||
                    !string.IsNullOrWhiteSpace(Holder.ChargeCode.Value) ||
                    !string.IsNullOrWhiteSpace(Holder.Reason.Value))
                {
                    if (await dialogs_.ConfirmDialogAsync(Messages.LEAVEPAGE))
                    {
                        base.BackItemPage();
                    }
                }
                else
                {
                    base.BackItemPage();
                }
            }
            else
            {
                base.BackItemPage();
            }
        }

        private async void ExecuteCancelCommand()
        {
            try
            {
                Holder.ActionTypeId = ActionTypeId.Cancel;
                Holder.Msg = Messages.Cancel;
                Holder = await service_.CancelRequestAsync(Holder);

                if (Holder.Success)
                {
                    Success(true);
                    await NavigationService.PopToRootAsync();
                }
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
        }

        private void ExecuteResetFormCommand()
        {
            Holder.DateNeeded = DateTime.UtcNow.Date;
            Holder.Amount.Value = 0;
            Holder.Reason.Value = string.Empty;
            Holder.ChargeCode.Value = string.Empty;
        }

        private async Task ExecuteTakePhotoCommand()
        {
            try
            {
                using (Dialogs.Loading())
                {
                    await Task.Delay(500);

                    var response = await commonService_.TakePhotoAsync("CA");
                    /*var file = await commonService_.AttachPhoto2(response, "CA");*/

                    if (response != null && !string.IsNullOrWhiteSpace(response.FileName))
                    {
                        Holder.FileAttachments.Add(response);
                    }

                    Holder = Holder;
                }
            }
            catch (Exception ex)
            {
                await dialogs_.AlertAsync(ex.Message);
            }
        }

        private async Task ExecuteAttachFileCommand()
        {
            try
            {
                var file = new Models.DataObjects.FileUploadResponse();

                using (Dialogs.Loading())
                {
                    await Task.Delay(500);
                    file = await commonService_.FileUploadAsync();
                }

                if (file != null && !string.IsNullOrWhiteSpace(file.FileName))
                {
                    Holder.FileAttachments.Add(file);
                }

                Holder = Holder;
            }
            catch (Exception ex)
            {
                await dialogs_.AlertAsync(ex.Message);
            }
        }

        private async Task ViewFileAttachments()
        {
            try
            {
                if (!IsBusy)
                {
                    var param = new FileAttachmentParams()
                    {
                        ModuleFormId = ModuleForms.CashAdvanceRequest, /*ModuleForms.TravelRequest,*/
                        TransactionId = Holder.Model.CashAdvanceId,
                    };

                    using (Dialogs.Loading())
                    {
                        await Task.Delay(500);
                        await NavigationService.PushModalAsync(new FileAttachmentPage(param));
                    }
                }
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
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
                if (Holder.Model.CashAdvanceId == 0)
                {
                    Holder.FileAttachments.Remove(file);
                    Holder = Holder;
                }
                else
                    await dialogs_.AlertAsync("Unable to remove file");
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

            MessagingCenter.Subscribe<CashAdvanceRequestPage>(this, "onback", (sender) =>
            {
                BackItemPage();
            });
        }

        private void ExecutePageDisappearingCommand()
        {
            MessagingCenter.Unsubscribe<FileOptionViewModel, long>(this, "FileOptionSelectedValue");

            MessagingCenter.Unsubscribe<CashAdvanceRequestPage>(this, "onback");
        }

        private void ExecuteChargeCodeSelectionCommand()
        {
            if (Holder.SelectedChargeCode != null)
            {
                if (Holder.SelectedChargeCode.Id < 0)
                {
                    Holder.ShowOtherChargeCode = true;

                    if (Holder.Model.CashAdvanceId == 0)
                        Holder.OtherChargeCode = new Validations.ValidatableObject<string>();
                }
                else
                    Holder.ShowOtherChargeCode = false;
            }
        }
    }
}