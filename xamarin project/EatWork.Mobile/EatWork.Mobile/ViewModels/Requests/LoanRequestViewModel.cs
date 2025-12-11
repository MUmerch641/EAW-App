using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.Request;
using EatWork.Mobile.Views;
using EatWork.Mobile.Views.Requests;
using EatWork.Mobile.Views.Shared;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class LoanRequestViewModel : BaseViewModel
    {
        #region commands

        public ICommand SubmitCommand { get; set; }
        public ICommand CameraCommand { get; set; }
        public ICommand SelectFileCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand SelectLoanTypesCommand { get; set; }
        public ICommand ItemSelectedCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand ViewTransactionHistoryCommand { get; set; }
        public ICommand ViewFileAttachmentCommand { get; set; }
        public ICommand RemoveFileAttachedCommand { get; set; }
        public ICommand FileChooseOptionCommand { get; set; }
        public ICommand PageAppearingCommand { get; set; }
        public ICommand PageDisappearingCommand { get; set; }

        #endregion commands

        private LoanRequestHolder formHelper_;

        public LoanRequestHolder Holder
        {
            get { return formHelper_; }
            set { formHelper_ = value; RaisePropertyChanged(() => Holder); }
        }

        private bool showPopupList_;

        public bool ShowPopupList
        {
            get { return showPopupList_; }
            set { showPopupList_ = value; RaisePropertyChanged(() => ShowPopupList); }
        }

        private readonly ILoanRequestDataService loanRequestDataService_;
        private readonly ICommonDataService myRequestCommonDataService_;
        private readonly IDialogService dialogService_;

        public LoanRequestViewModel(ILoanRequestDataService loanRequestDataService, ICommonDataService myRequestCommonDataService)
        {
            loanRequestDataService_ = loanRequestDataService;
            myRequestCommonDataService_ = myRequestCommonDataService;
            dialogService_ = AppContainer.Resolve<IDialogService>();
        }

        public void Init(INavigation navigation, long recordId, DateTime? selectedDate)
        {
            NavigationBack = navigation;
            Holder = new LoanRequestHolder();
            IsBusy = false;

            //==commands
            SubmitCommand = new Command(async () => await SubmitRequest());
            CameraCommand = new Command(async () => await TakePhoto());
            SelectFileCommand = new Command(async () => await SelectFile());
            CloseCommand = new Command(async () => await NavigationService.PopPageAsync());
            SelectLoanTypesCommand = new Command(() => ShowPopupList = !ShowPopupList);
            ItemSelectedCommand = new Command<SelectableListModel>(GetSelectedLoanType);
            CancelCommand = new Command(async () => await CancelRequest());
            ViewTransactionHistoryCommand = new Command(async () => await NavigationService.PushModalAsync(new TransactionHistoryPage(TransactionType.Loan, Holder.LoanRequestModel.LoanRequestId)));
            ViewFileAttachmentCommand = new Command(async () => await ViewFileAttachments());
            RemoveFileAttachedCommand = new Command(() => Holder.FileData = new Plugin.FilePicker.Abstractions.FileData());
            FileChooseOptionCommand = new Command<FileUploadResponse>(ExecuteFileChooseOptionCommand);
            PageAppearingCommand = new Command(ExecutePageAppearingCommand);
            PageDisappearingCommand = new Command(ExecutePageDisappearingCommand);

            InitForm(recordId, selectedDate);
        }

        private async void InitForm(long recordId, DateTime? selectedDate)
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);
                    Holder = await loanRequestDataService_.InitForm(recordId, selectedDate);
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

        private async Task SubmitRequest()
        {
            try
            {
                Holder = await loanRequestDataService_.SubmitRequest(Holder);

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

        private async Task TakePhoto()
        {
            try
            {
                var file = await myRequestCommonDataService_.TakePhotoAsync("LOAN");
                /*var file = await myRequestCommonDataService_.AttachPhoto2(response, "LOAN");*/

                if (file != null && !string.IsNullOrWhiteSpace(file.FileName))
                {
                    Holder.FileAttachments = new System.Collections.ObjectModel.ObservableCollection<FileUploadResponse>
                    {
                        file
                    };

                    Holder = Holder;
                }
            }
            catch (Exception ex)
            {
                await dialogService_.AlertAsync(ex.Message);
            }
        }

        private async Task SelectFile()
        {
            try
            {
                var response = await myRequestCommonDataService_.FileUploadAsync();

                if (response != null && !string.IsNullOrWhiteSpace(response.FileName))
                {
                    /*Holder.FileData = response;*/
                    Holder.FileAttachments = new System.Collections.ObjectModel.ObservableCollection<FileUploadResponse>
                    {
                        response
                    };
                    Holder = Holder;
                }
            }
            catch (Exception ex)
            {
                await dialogService_.AlertAsync(ex.Message);
            }
        }

        private async void GetSelectedLoanType(SelectableListModel item)
        {
            if (item != null)
            {
                Holder.SelectedLoanType = item;
                ShowPopupList = false;
            }
        }

        private async Task CancelRequest()
        {
            try
            {
                Holder.ActionTypeId = ActionTypeId.Cancel;
                Holder.Msg = Messages.Cancel;
                Holder = await loanRequestDataService_.WorkflowTransactionRequest(Holder);

                if (Holder.Success)
                {
                    Success(true, Messages.ApprovalFormSuccessMessage);
                    await NavigationService.PopToRootAsync();
                }
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
        }

        private async Task ViewFileAttachments()
        {
            try
            {
                if (!IsBusy)
                {
                    var files = new System.Collections.Generic.List<FileAttachmentListModel>
                    {
                        new FileAttachmentListModel()
                        {
                            Attachment = Holder.LoanRequestFile.Attachment,
                            FileName = Holder.LoanRequestFile.FileName,
                            FileAttachmentId = Holder.LoanRequestFile.LoanRequestFileId,
                            TransactionId = Holder.LoanRequestFile.LoanRequestId,
                            FileType = Holder.LoanRequestFile.FileType,
                        }
                    };

                    var param = new FileAttachmentParams()
                    {
                        ModuleFormId = ModuleForms.LoanRequest,
                        TransactionId = Holder.LoanRequestModel.LoanRequestId,
                        FileAttachments = files,
                        URI = $"{ApiConstants.GetLoanRequestFile}{Holder.LoanRequestFile.LoanRequestId}",
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
                        await myRequestCommonDataService_.PreviewFileBase64(base64, file.FileType, file.FileName);
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
                if (Holder.IsEnabled)
                {
                    Holder.FileAttachments.Remove(file);
                    Holder = Holder;
                }
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

            MessagingCenter.Subscribe<LoanRequestPage>(this, "onback", (sender) =>
            {
                BackItemPage();
            });
        }

        private void ExecutePageDisappearingCommand()
        {
            MessagingCenter.Unsubscribe<FileOptionViewModel, long>(this, "FileOptionSelectedValue");
            MessagingCenter.Unsubscribe<LoanRequestPage>(this, "onback");
        }

        protected override async void BackItemPage()
        {
            if (Holder.LoanRequestModel.LoanRequestId == 0)
            {
                if (Holder.SelectedLoanType.Id != 0 ||
                    Holder.LoanRequestModel.RequestedAmount != 0)
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
            else
            {
                base.BackItemPage();
            }
        }
    }
}