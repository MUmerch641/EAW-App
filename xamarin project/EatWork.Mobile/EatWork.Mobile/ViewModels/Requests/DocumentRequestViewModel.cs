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
    public class DocumentRequestViewModel : ListViewModel
    {
        #region commands

        public ICommand SubmitCommand { get; set; }
        public ICommand CameraCommand { get; set; }
        public ICommand SelectFileCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand SelectDocumentCommand { get; set; }
        public ICommand ItemSelectedCommand { get; set; }
        public ICommand ItemSelected2Command { get; set; }
        public ICommand SelectReasonCommand { get; set; }
        public ICommand CloseModalCommand { get; set; }
        public ICommand RemoveFileAttachedCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand ViewTransactionHistoryCommand { get; set; }
        public ICommand StartDateChangedCommand { get; set; }
        public ICommand ViewFileAttachmentCommand { get; set; }
        public ICommand FileChooseOptionCommand { get; set; }
        public ICommand PageAppearingCommand { get; set; }
        public ICommand PageDisappearingCommand { get; set; }

        #endregion commands

        #region properties

        private DocumentRequestHolder formHelper_;

        public DocumentRequestHolder Holder
        {
            get { return formHelper_; }
            set { formHelper_ = value; RaisePropertyChanged(() => Holder); }
        }

        private bool showPopupDocumentList_;

        public bool ShowPopupDocumentList
        {
            get { return showPopupDocumentList_; }
            set { showPopupDocumentList_ = value; RaisePropertyChanged(() => ShowPopupDocumentList); }
        }

        private bool showPopupReasonList_;

        public bool ShowPopupReasonList
        {
            get { return showPopupReasonList_; }
            set { showPopupReasonList_ = value; RaisePropertyChanged(() => ShowPopupReasonList); }
        }

        #endregion properties

        private readonly IDocumentRequestDataService documentRequestDataService_;
        private readonly ICommonDataService myRequestCommonDataService_;
        private readonly IDialogService dialogService_;

        public DocumentRequestViewModel(IDocumentRequestDataService documentRequestDataService,
            ICommonDataService myRequestCommonDataService)
        {
            documentRequestDataService_ = documentRequestDataService;
            myRequestCommonDataService_ = myRequestCommonDataService;
            dialogService_ = AppContainer.Resolve<IDialogService>();
        }

        public void Init(INavigation navigation, long recordId, DateTime? selectedDate)
        {
            NavigationBack = navigation;
            Holder = new DocumentRequestHolder();
            ShowPopupDocumentList = false;
            ShowPopupReasonList = false;

            SubmitCommand = new Command(async () => await SubmitRequest());
            CameraCommand = new Command(async () => await TakePhoto());
            SelectFileCommand = new Command(async () => await SelectFile());

            ItemSelected2Command = new Command<SelectableListModel>(GetSelectedReason);

            SelectDocumentCommand = new Command(() => ShowPopupDocumentList = !ShowPopupDocumentList);
            SelectReasonCommand = new Command(() => ShowPopupReasonList = !ShowPopupReasonList);
            ItemSelectedCommand = new Command<SelectableListModel>(GetSelectedDocument);

            StartDateChangedCommand = new Command<DateChangedEventArgs>(DateChangeEvent);

            CloseCommand = new Command(async () => await NavigationService.PopPageAsync());
            RemoveFileAttachedCommand = new Command(() => Holder.FileData = new Plugin.FilePicker.Abstractions.FileData());
            CancelCommand = new Command(async () => await CancelRequest());
            ViewTransactionHistoryCommand = new Command(async () => await NavigationService.PushModalAsync(new TransactionHistoryPage(TransactionType.Document, Holder.DocumentRequestModel.DocumentRequestId)));
            ViewFileAttachmentCommand = new Command(async () => await ViewFileAttachments());
            FileChooseOptionCommand = new Command<FileUploadResponse>(ExecuteFileChooseOptionCommand);
            PageAppearingCommand = new Command(ExecutePageAppearingCommand);
            PageDisappearingCommand = new Command(ExecutePageDisappearingCommand);

            InitForm(recordId, selectedDate);
        }

        public async void InitForm(long recordId, DateTime? selectedDate)
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);
                    Holder = await documentRequestDataService_.InitForm(recordId, selectedDate);
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

        private async void GetSelectedDocument(SelectableListModel item)
        {
            if (item != null)
            {
                Holder.DocumentType = item;
                ShowPopupDocumentList = false;
            }
        }

        private async void GetSelectedReason(SelectableListModel item)
        {
            if (item != null)
            {
                Holder.DocumentRequestModel.Reason = item.DisplayText;
                Holder = Holder;
                ShowPopupReasonList = false;
            }
        }

        private void SelectedReasonEvent(SelectableListModel obj)
        {
            if (obj != null)
            {
                Holder.DocumentRequestModel.Reason = obj.DisplayText;
                Holder = Holder;
            }
        }

        private async Task SubmitRequest()
        {
            try
            {
                Holder = await documentRequestDataService_.SubmitRequest(Holder);

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
                var file = await myRequestCommonDataService_.TakePhotoAsync("DOCUMENT");
                /*var file = await myRequestCommonDataService_.AttachPhoto2(response, "DOCUMENT");*/

                if (file != null && !string.IsNullOrWhiteSpace(file.FileName))
                {
                    /*Holder.FileData = file;*/
                    Holder.FileAttachments.Add(file);
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
                    Holder.FileAttachments.Add(response);
                    Holder = Holder;
                }
            }
            catch (Exception ex)
            {
                await dialogService_.AlertAsync(ex.Message);
            }
        }

        private async Task CancelRequest()
        {
            try
            {
                Holder.ActionTypeId = ActionTypeId.Cancel;
                Holder.Msg = Messages.Cancel;
                Holder = await documentRequestDataService_.WorkflowTransactionRequest(Holder);

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

        private void DateChangeEvent(DateChangedEventArgs obj)
        {
            var result = DateTime.Compare(Holder.DocumentRequestModel.DateEnd.Value, obj.NewDate);

            if (result < 0)
            {
                Holder.DocumentRequestModel.DateEnd = obj.NewDate;
                Holder = Holder;
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
                        ModuleFormId = ModuleForms.DocumentRequest,
                        TransactionId = Holder.DocumentRequestModel.DocumentRequestId
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

            MessagingCenter.Subscribe<DocumentRequestPage>(this, "onback", (sender) =>
            {
                BackItemPage();
            });
        }

        private void ExecutePageDisappearingCommand()
        {
            MessagingCenter.Unsubscribe<FileOptionViewModel, long>(this, "FileOptionSelectedValue");
            MessagingCenter.Unsubscribe<DocumentRequestPage>(this, "onback");
        }

        protected override async void BackItemPage()
        {
            if (Holder.DocumentRequestModel.DocumentRequestId == 0)
            {
                if (!string.IsNullOrWhiteSpace(Holder.DocumentType.DisplayText) ||
                    !string.IsNullOrWhiteSpace(Holder.DocumentRequestModel.Details) ||
                    !string.IsNullOrWhiteSpace(Holder.DocumentRequestModel.Reason))
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