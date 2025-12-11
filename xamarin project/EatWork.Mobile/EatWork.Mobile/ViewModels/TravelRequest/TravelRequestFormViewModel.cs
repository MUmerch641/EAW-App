using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Excemptions;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder;
using EatWork.Mobile.Views;
using EatWork.Mobile.Views.Shared;
using EatWork.Mobile.Views.TravelRequest;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class TravelRequestFormViewModel : BaseViewModel
    {
        public ICommand SubmitCommand { get; set; }
        public ICommand CameraCommand { get; set; }
        public ICommand SelectFileCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand RemoveFileAttachedCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand ViewTransactionHistoryCommand { get; set; }
        public ICommand StartTimeChangeCommand { get; set; }
        public ICommand StartOpenCommand { get; set; }
        public ICommand EndTimeChangeCommand { get; set; }
        public ICommand EndOpenCommand { get; set; }
        public ICommand ViewFileAttachmentCommand { get; set; }
        public ICommand CallSpecialNoteCommand { get; set; }
        public ICommand PageAppearingCommand { get; set; }
        public ICommand PageDisappearingCommand { get; set; }
        public ICommand RemoveSpecialNoteCommand { get; set; }
        public ICommand FileChooseOptionCommand { get; set; }
        public ICommand TripTypeSelectionCommand { get; set; }

        private TravelRequestHolder holder_;

        public TravelRequestHolder FormHelper
        {
            get { return holder_; }
            set { holder_ = value; RaisePropertyChanged(() => FormHelper); }
        }

        private readonly ICommonDataService commonService_;
        private readonly IDialogService dialogService_;
        private readonly ITravelRequestDataService service_;

        public TravelRequestFormViewModel()
        {
            commonService_ = AppContainer.Resolve<ICommonDataService>();
            dialogService_ = AppContainer.Resolve<IDialogService>();
            service_ = AppContainer.Resolve<ITravelRequestDataService>();
        }

        public void Init(INavigation navigation, long recordId = 0, DateTime? selectedDate = null)
        {
            NavigationBack = navigation;
            IsBusy = false;

            InitHelpers();
            InitForm(recordId, selectedDate);
        }

        private void InitHelpers()
        {
            FormHelper = new TravelRequestHolder();
            SubmitCommand = new Command(async () => await SubmitRequest());
            CameraCommand = new Command(async () => await TakePhoto());
            SelectFileCommand = new Command(async () => await SelectFile());
            CloseCommand = new Command(async () => await NavigationService.PopPageAsync());
            RemoveFileAttachedCommand = new Command(() => FormHelper.FileData = new Plugin.FilePicker.Abstractions.FileData());
            CancelCommand = new Command(async () => await CancelRequest());
            ViewTransactionHistoryCommand = new Command(async () => await NavigationService.PushModalAsync(new TransactionHistoryPage(TransactionType.Travel, FormHelper.Model.TravelRequestId)));
            CallSpecialNoteCommand = new Command<string>(ExecuteCallSpecialNoteCommand);
            PageAppearingCommand = new Command(ExecutePageAppearingCommand);
            PageDisappearingCommand = new Command(ExecutePageDisappearingCommand);
            RemoveSpecialNoteCommand = new Command(() =>
            {
                FormHelper.SpecialRequestNote = string.Empty;
                FormHelper.ShowDeleteNote = false;
            });

            ViewFileAttachmentCommand = new Command(async () => await ViewFileAttachments());
            FileChooseOptionCommand = new Command<FileUploadResponse>(ExecuteFileChooseOptionCommand);
            TripTypeSelectionCommand = new Command(ExecuteTripTypeSelectionCommand);
        }

        private async void InitForm(long id, DateTime? selectedDate)
        {
            if (!IsBusy)
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);

                    FormHelper = await service_.InitForm(id, selectedDate);
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

        private async Task SubmitRequest()
        {
            try
            {
                FormHelper = await service_.SubmitRecord(FormHelper);

                if (FormHelper.Success)
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

        private async Task TakePhoto()
        {
            try
            {
                var file = await commonService_.TakePhotoAsync("TRAVEL");
                /*var file = await commonService_.AttachPhoto2(response, "TRAVEL");*/

                if (file != null && !string.IsNullOrWhiteSpace(file.FileName))
                {
                    FormHelper.UploadedFilesDisplay.Add(new Models.DataObjects.FileUploadResponse()
                    {
                        FileData = file.FileData,
                        FileDataArray = file.FileDataArray,
                        FileName = file.FileName,
                        FileResult = file.FileResult,
                        FileSize = file.FileSize,
                        FileType = file.FileType,
                        MimeType = file.MimeType,
                        RawFileSize = file.RawFileSize,
                    });

                    FormHelper = FormHelper;
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
                var file = new Models.DataObjects.FileUploadResponse();

                using (Dialogs.Loading())
                {
                    await Task.Delay(500);
                    file = await commonService_.FileUploadAsync();
                }

                if (file != null && !string.IsNullOrWhiteSpace(file.FileName))
                {
                    /*
                    FormHelper.UploadedFilesDisplay.Add(new Models.DataObjects.FileUploadResponse()
                    {
                        FileData = file.FileData,
                        FileDataArray = file.FileDataArray,
                        FileName = file.FileName,
                        FileResult = file.FileResult,
                        FileSize = file.FileSize,
                        FileType = file.FileType,
                        MimeType = file.MimeType,
                        RawFileSize = file.RawFileSize,
                    });
                    */
                    FormHelper.UploadedFilesDisplay.Add(file);
                    FormHelper = FormHelper;
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
                FormHelper.ActionTypeId = ActionTypeId.Cancel;
                FormHelper.Msg = Messages.Cancel;
                FormHelper = await service_.RequestCancelRequest(FormHelper);

                if (FormHelper.Success)
                {
                    Success(true);
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

        private async void ExecuteCallSpecialNoteCommand(string note)
        {
            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(new SpecialNoteRequestModal(note));
        }

        private void ExecutePageAppearingCommand()
        {
            MessagingCenter.Subscribe<SpecialNoteRequestViewModel, string>(this, "specialNoteRequestCompleted", (s, param) =>
            {
                UpdateNote(param);
            });

            MessagingCenter.Subscribe<FileOptionViewModel, long>(this, "FileOptionSelectedValue", (s, param) =>
            {
                if (param > 0)
                {
                    ExecuteChosenOption(param);
                }
            });

            MessagingCenter.Subscribe<TravelRequestFormPage>(this, "onback", (sender) =>
            {
                BackItemPage();
            });
        }

        private void UpdateNote(string note)
        {
            if (!string.IsNullOrWhiteSpace(note))
            {
                FormHelper.SpecialRequestNote = note;
                FormHelper.ShowDeleteNote = true;
            }
        }

        private void ExecutePageDisappearingCommand()
        {
            MessagingCenter.Unsubscribe<SpecialNoteRequestViewModel, string>(this, "specialNoteRequestCompleted");
            MessagingCenter.Unsubscribe<FileOptionViewModel, long>(this, "FileOptionSelectedValue");
            MessagingCenter.Unsubscribe<TravelRequestFormPage>(this, "onback");
        }

        private async Task ViewFileAttachments()
        {
            try
            {
                if (!IsBusy)
                {
                    var param = new FileAttachmentParams()
                    {
                        ModuleFormId = FormHelper.ModuleFormId, /*ModuleForms.TravelRequest,*/
                        TransactionId = FormHelper.Model.TravelRequestId
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
                FormHelper.SelectedFile = file;

                using (Dialogs.Loading())
                {
                    await Task.Delay(500);
                    await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(new FileOptionModal());
                }
            }
        }

        private async void ExecuteChosenOption(long option)
        {
            var file = FormHelper.SelectedFile;

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
                if (FormHelper.IsEnabled)
                {
                    FormHelper.FileAttachments.Remove(file);
                    FormHelper.UploadedFilesDisplay.Remove(file);
                    FormHelper = FormHelper;
                }
            }
        }

        private void ExecuteTripTypeSelectionCommand()
        {
            var type = FormHelper.SelectedTripType;

            if (type != null)
            {
                if (type.Id == 1)
                    FormHelper.EnabledDetailField = true;
                else
                {
                    FormHelper.EnabledDetailField = false;

                    if (FormHelper.Model.TravelRequestId == 0)
                        FormHelper.Details.Value = string.Empty;
                }
            }
        }

        protected override async void BackItemPage()
        {
            if (FormHelper.Model.TravelRequestId == 0)
            {
                if (!string.IsNullOrWhiteSpace(holder_.SelectedTripType.Value)
                    || !string.IsNullOrWhiteSpace(holder_.Details.Value)
                    || !string.IsNullOrWhiteSpace(holder_.SpecialRequestNote)
                    || !string.IsNullOrWhiteSpace(holder_.FirstOrigin.Value)
                    || !string.IsNullOrWhiteSpace(holder_.FirstDestination.Value)
                    || !string.IsNullOrWhiteSpace(holder_.SecondOrigin.Value)
                    || !string.IsNullOrWhiteSpace(holder_.SecondDestination.Value))
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