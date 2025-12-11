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
    internal class OvertimeViewModel : BaseViewModel
    {
        #region commands

        public ICommand SubmitCommand { get; set; }
        public ICommand CameraCommand { get; set; }
        public ICommand SelectFileCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand PreshiftCommand { get; set; }
        public ICommand OffSettingCommand { get; set; }
        public ICommand RemoveFileAttachedCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand ViewTransactionHistoryCommand { get; set; }
        public ICommand StartTimeChangeCommand { get; set; }
        public ICommand StartOpenCommand { get; set; }
        public ICommand EndTimeChangeCommand { get; set; }
        public ICommand EndOpenCommand { get; set; }
        public ICommand ViewFileAttachmentCommand { get; set; }
        public ICommand DateSelectedCommand { get; set; }
        public ICommand FileChooseOptionCommand { get; set; }
        public ICommand PageAppearingCommand { get; set; }
        public ICommand PageDisappearingCommand { get; set; }

        #endregion commands

        private readonly IOvertimeRequestDataService overtimeRequestDataService_;
        private readonly ICommonDataService myRequestCommonDataService_;
        private readonly IDialogService dialogService_;

        public OvertimeViewModel(IOvertimeRequestDataService overtimeRequestDataService,
            ICommonDataService myRequestCommonDataService)
        {
            overtimeRequestDataService_ = overtimeRequestDataService;
            myRequestCommonDataService_ = myRequestCommonDataService;
            dialogService_ = AppContainer.Resolve<IDialogService>();
        }

        /*

        private bool startIsOpen_;

        public bool StartIsOpen
        {
            get { return startIsOpen_; }
            set { startIsOpen_ = value; OnPropertyChanged(nameof(StartIsOpen)); }
        }

        private bool endIsOpen_;

        public bool EndIsOpen
        {
            get { return endIsOpen_; }
            set { endIsOpen_ = value; OnPropertyChanged(nameof(EndIsOpen)); }
        }

        */
        private OvertimeRequestHolder formHelper_;

        public OvertimeRequestHolder Holder
        {
            get { return formHelper_; }
            set { formHelper_ = value; RaisePropertyChanged(() => Holder); }
        }

        public void Init(INavigation navigation, long recordId, DateTime? selectedDate)
        {
            NavigationBack = navigation;
            IsBusy = false;
            //Holder = new OvertimeRequestHolder();
            /*
            StartIsOpen = false;
            EndIsOpen = false;
            */

            //==commands
            SubmitCommand = new Command(async () => await SubmitRequest());
            CameraCommand = new Command(async () => await CameraRequest());
            SelectFileCommand = new Command(async () => await SelectFile());
            CloseCommand = new Command(async () => await NavigationService.PopPageAsync());

            /*PreshiftCommand = new Command<Syncfusion.XForms.Buttons.SelectionChangedEventArgs>(PreshiftOptionEvent);*/
            PreshiftCommand = new Command<Syncfusion.XForms.Buttons.SwitchStateChangedEventArgs>(PreshiftOptionEvent);
            OffSettingCommand = new Command<Syncfusion.XForms.Buttons.SwitchStateChangedEventArgs>(OffSetOptionEvent);

            RemoveFileAttachedCommand = new Command(() => Holder.FileData = new Plugin.FilePicker.Abstractions.FileData());
            ViewTransactionHistoryCommand = new Command(async () => await NavigationService.PushModalAsync(new TransactionHistoryPage(TransactionType.Overtime, Holder.OvertimeModel.OvertimeId)));
            CancelCommand = new Command(async () => await CancelRequest());

            /*
            StartTimeChangeCommand = new Command<TimeSpan>(StartTimeChangeEvent);
            EndTimeChangeCommand = new Command<TimeSpan>(EndTimeChangeEvent);

            StartTimeChangeCommand = new Command<System.ComponentModel.PropertyChangedEventArgs>(StartTimeChangeEvent);

            EndTimeChangeCommand = new Command<System.ComponentModel.PropertyChangedEventArgs>(EndTimeChangeEvent);
            */

            StartTimeChangeCommand = new Command<TimeSpan>(StartTimeChangeEvent);
            EndTimeChangeCommand = new Command<TimeSpan>(EndTimeChangeEvent);

            ViewFileAttachmentCommand = new Command(async () => await ViewFileAttachments());
            DateSelectedCommand = new Command<DateChangedEventArgs>(DateChangeEvent);
            FileChooseOptionCommand = new Command<FileUploadResponse>(ExecuteFileChooseOptionCommand);
            PageAppearingCommand = new Command(ExecutePageAppearingCommand);
            PageDisappearingCommand = new Command(ExecutePageDisappearingCommand);

            /*
            StartOpenCommand = new Command(() => StartIsOpen = !StartIsOpen);
            EndOpenCommand = new Command(() => EndIsOpen = !EndIsOpen);
            StartTimeChangeCommand = new Command<ObservableCollection<object>>(StartTimeChangeEvent);
            EndTimeChangeCommand = new Command<ObservableCollection<object>>(EndTimeChangeEvent);
            */

            InitForm(recordId, selectedDate);
        }

        private async Task SubmitRequest()
        {
            try
            {
                Holder = await overtimeRequestDataService_.SubmitRequest(Holder);

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

        private async Task CameraRequest()
        {
            try
            {
                var file = await myRequestCommonDataService_.TakePhotoAsync("OVERTIME");
                /*var file = await myRequestCommonDataService_.AttachPhoto2(response, "OVERTIME");*/

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

        private async void InitForm(long recordId, DateTime? selectedDate)
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);
                    Holder = await overtimeRequestDataService_.InitForm(recordId, selectedDate);
                }
                catch (Exception ex)
                {
                    Error(false, ex.Message);
                }
                finally
                {
                    IsBusy = false;

                    DateChangeEvent(null);
                }
            }
        }

        private void PreshiftOptionEvent(Syncfusion.XForms.Buttons.SwitchStateChangedEventArgs obj)
        {
            if (Holder != null)
            {
                if (Holder.IsEnabled)
                {
                    Holder.StartTimeString = string.Empty;
                    Holder.EndTimeString = string.Empty;
                    Holder.StartTime = null;
                    Holder.EndTime = null;
                    Holder.OvertimeModel.OROTHrs = 0;

                    if (obj.NewValue.Value)
                    {
                        Holder.IsPreshift = 1;

                        if (Holder.ShowWSField)
                        {
                            Holder.EndTime = Holder.WSStartTime.GetValueOrDefault();
                            Holder.EndTimeString = new DateTime(Holder.EndTime.GetValueOrDefault().Ticks).ToString(Constants.TimeFormatHHMMTT);
                        }
                    }
                    else
                    {
                        Holder.IsPreshift = 0;

                        if (Holder.ShowWSField)
                        {
                            Holder.StartTime = Holder.WSEndTime.GetValueOrDefault();
                            Holder.StartTimeString = new DateTime(Holder.StartTime.GetValueOrDefault().Ticks).ToString(Constants.TimeFormatHHMMTT);
                        }
                    }

                    Holder = Holder;
                }
            }
        }

        private void OffSetOptionEvent(Syncfusion.XForms.Buttons.SwitchStateChangedEventArgs obj)
        {
            if (Holder != null)
            {
                /*Holder.IsPreshift = (short)obj.Index;*/
                Holder.IsOffSetting = obj.NewValue.Value;
                Holder = Holder;
            }
        }

        private async Task CancelRequest()
        {
            try
            {
                Holder.ActionTypeId = ActionTypeId.Cancel;
                Holder.Msg = Messages.Cancel;
                Holder = await overtimeRequestDataService_.WorkflowTransactionRequest(Holder);

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
            if (Holder.StartTime.HasValue)
                Holder.StartTimeString = new DateTime(Holder.StartTime.GetValueOrDefault().Ticks).ToString(Constants.TimeFormatHHMMTT);

            CalculateOT();
        }

        private void EndTimeChangeEvent(TimeSpan obj)
        {
            if (Holder.EndTime.HasValue)
                Holder.EndTimeString = new DateTime(Holder.EndTime.GetValueOrDefault().Ticks).ToString(Constants.TimeFormatHHMMTT);

            CalculateOT();
        }

        private void CalculateOT()
        {
            if (Holder != null && Holder.IsEnabled)
            {
                if (Holder.StartTime.HasValue && Holder.EndTime.HasValue && Holder.OvertimeModel.OvertimeId == 0)
                {
                    var dt1 = new DateTime(Holder.StartTime.GetValueOrDefault().Ticks);
                    var dt2 = new DateTime(Holder.EndTime.GetValueOrDefault().Ticks);
                    dt2 = dt2.AddDays((dt2 <= dt1 ? 1 : 0));

                    if (Holder.OvertimeModel != null)
                    {
                        Holder.OvertimeModel.OROTHrs = Convert.ToDecimal((dt2 - dt1).TotalHours);
                    }
                }

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
                        ModuleFormId = ModuleForms.OvertimeRequest,
                        TransactionId = Holder.OvertimeModel.OvertimeId
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

        private async void DateChangeEvent(DateChangedEventArgs obj)
        {
            if (!IsBusy)
            {
                try
                {
                    Holder = await overtimeRequestDataService_.PreOTValidation(Holder);
                }
                catch (Exception ex)
                {
                    Error(false, ex.Message);
                }
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

            MessagingCenter.Subscribe<OvertimeRequestPage>(this, "onback", (sender) =>
            {
                BackItemPage();
            });
        }

        private void ExecutePageDisappearingCommand()
        {
            MessagingCenter.Unsubscribe<FileOptionViewModel, long>(this, "FileOptionSelectedValue");
            MessagingCenter.Unsubscribe<OvertimeRequestPage>(this, "onback");
        }

        protected override async void BackItemPage()
        {
            if (Holder.OvertimeModel.OvertimeId == 0)
            {
                if (Holder.OvertimeModel.OROTHrs != 0 ||
                    !string.IsNullOrWhiteSpace(Holder.OvertimeModel.Reason))
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