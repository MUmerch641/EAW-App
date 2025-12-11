using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Excemptions;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.Request;
using EatWork.Mobile.Views;
using EatWork.Mobile.Views.Requests;
using EatWork.Mobile.Views.Shared;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class UndertimeRequestViewModel : BaseViewModel
    {
        #region commands

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
        public ICommand FileChooseOptionCommand { get; set; }
        public ICommand PageAppearingCommand { get; set; }
        public ICommand PageDisappearingCommand { get; set; }

        #endregion commands

        private UndertimeHolder formHelper_;

        public UndertimeHolder Holder
        {
            get { return formHelper_; }
            set { formHelper_ = value; RaisePropertyChanged(() => Holder); }
        }

        private readonly IUndertimeRequestDataService undertimeRequestDataService_;
        private readonly ICommonDataService myRequestCommonDataService_;
        private readonly IDialogService dialogService_;

        public UndertimeRequestViewModel(IUndertimeRequestDataService undertimeRequestDataService, ICommonDataService myRequestCommonDataService)
        {
            undertimeRequestDataService_ = undertimeRequestDataService;
            myRequestCommonDataService_ = myRequestCommonDataService;
            dialogService_ = AppContainer.Resolve<IDialogService>();
        }

        public void Init(INavigation navigation, long recordId, DateTime? selectedDate)
        {
            NavigationBack = navigation;
            IsBusy = false;
            //Holder = new UndertimeHolder();

            //==commands
            SubmitCommand = new Command(async () => await SubmitRequest());
            CameraCommand = new Command(async () => await TakePhoto());
            SelectFileCommand = new Command(async () => await SelectFile());
            CloseCommand = new Command(async () => await NavigationService.PopPageAsync());
            RemoveFileAttachedCommand = new Command(() => Holder.FileData = new Plugin.FilePicker.Abstractions.FileData());
            CancelCommand = new Command(async () => await CancelRequest());
            ViewTransactionHistoryCommand = new Command(async () => await NavigationService.PushModalAsync(new TransactionHistoryPage(TransactionType.Undertime, Holder.UndertimeModel.UndertimeId)));

            StartTimeChangeCommand = new Command<TimeSpan>(StartTimeChangeEvent);
            EndTimeChangeCommand = new Command<TimeSpan>(EndTimeChangeEvent);
            ViewFileAttachmentCommand = new Command(async () => await ViewFileAttachments());
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
                    Holder = await undertimeRequestDataService_.InitForm(recordId, selectedDate);
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
                Holder = await undertimeRequestDataService_.SubmitRequest(Holder);

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

        private async Task TakePhoto()
        {
            try
            {
                var file = await myRequestCommonDataService_.TakePhotoAsync("UNDERTIME");
                /*var file = await myRequestCommonDataService_.AttachPhoto2(response, "UNDERTIME");*/

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
                Holder = await undertimeRequestDataService_.WorkflowTransactionRequest(Holder);

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

        private void StartTimeChangeEvent(TimeSpan obj)
        {
            if (Holder != null)
            {
                Holder.StartTimeString = new DateTime(obj.Ticks).ToString(Constants.TimeFormatHHMMTT);
                CalculateHours();
            }
        }

        private void EndTimeChangeEvent(TimeSpan obj)
        {
            if (Holder != null)
            {
                Holder.EndTimeString = new DateTime(obj.Ticks).ToString(Constants.TimeFormatHHMMTT);
                CalculateHours();
            }
        }

        private void CalculateHours()
        {
            if (Holder.UndertimeModel.UndertimeId == 0 && Holder.DepartureTime.HasValue && Holder.ArrivalTime.HasValue)
            {
                var dt1 = new DateTime(Holder.DepartureTime.Value.Ticks);
                var dt2 = new DateTime(Holder.ArrivalTime.Value.Ticks);

                if (Holder.UndertimeModel != null)
                {
                    Holder.UndertimeModel.UTHrs = Convert.ToDecimal((dt2 - dt1).TotalHours);
                }
            }

            Holder = Holder;
        }

        private async Task ViewFileAttachments()
        {
            try
            {
                if (!IsBusy)
                {
                    var param = new FileAttachmentParams()
                    {
                        ModuleFormId = ModuleForms.UndertimeRequest,
                        TransactionId = Holder.UndertimeModel.UndertimeId
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

            MessagingCenter.Subscribe<UndertimeRequestPage>(this, "onback", (sender) =>
            {
                BackItemPage();
            });
        }

        private void ExecutePageDisappearingCommand()
        {
            MessagingCenter.Unsubscribe<FileOptionViewModel, long>(this, "FileOptionSelectedValue");
            MessagingCenter.Unsubscribe<UndertimeRequestPage>(this, "onback");
        }

        protected override async void BackItemPage()
        {
            if (Holder.UndertimeModel.UndertimeId == 0)
            {
                if (Holder.UndertimeModel.UTHrs != 0 ||
                    Holder.UndertimeReasonSelectedItem.Id != 0 ||
                    !string.IsNullOrWhiteSpace(Holder.UndertimeModel.Reason))
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