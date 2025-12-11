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
    public class SpecialWorkScheduleViewModel : BaseViewModel
    {
        #region commands

        public ICommand SubmitCommand { get; set; }
        public ICommand CameraCommand { get; set; }
        public ICommand SelectFileCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand RemoveFileAttachedCommand { get; set; }
        public ICommand ItemSelectedCommand { get; set; }
        public ICommand SelectShiftCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand ViewTransactionHistoryCommand { get; set; }
        public ICommand StartTimeChangeCommand { get; set; }
        public ICommand EndTimeChangeCommand { get; set; }
        public ICommand LunchStartTimeChangeCommand { get; set; }
        public ICommand LunchEndTimeChangeCommand { get; set; }
        public ICommand ViewFileAttachmentCommand { get; set; }
        public ICommand GetDateScheduleCommand { get; set; }
        public ICommand OffSettingCommand { get; set; }
        public ICommand FileChooseOptionCommand { get; set; }
        public ICommand PageAppearingCommand { get; set; }
        public ICommand PageDisappearingCommand { get; set; }

        #endregion commands

        private SpecialWorkScheduleHolder formHelper_;

        public SpecialWorkScheduleHolder Holder
        {
            get { return formHelper_; }
            set { formHelper_ = value; RaisePropertyChanged(() => Holder); }
        }

        private ObservableCollection<ShiftDto> shiftList_;

        public ObservableCollection<ShiftDto> ShiftList
        {
            get { return shiftList_; }
            set { shiftList_ = value; RaisePropertyChanged(() => ShiftList); }
        }

        private bool showShiftList_;

        public bool ShowShiftList
        {
            get { return showShiftList_; }
            set { showShiftList_ = value; RaisePropertyChanged(() => ShowShiftList); }
        }

        private readonly ISpecialWorkScheduleDataService specialWorkScheduleDataService_;
        private readonly ICommonDataService commonDataService_;
        private readonly IDialogService dialogService_;

        public SpecialWorkScheduleViewModel(ISpecialWorkScheduleDataService specialWorkScheduleDataService,
            ICommonDataService commonDataService)
        {
            specialWorkScheduleDataService_ = specialWorkScheduleDataService;
            commonDataService_ = commonDataService;
            dialogService_ = AppContainer.Resolve<IDialogService>();
        }

        public void Init(INavigation navigation, long recordId, DateTime? selectedDate)
        {
            NavigationBack = navigation;
            IsBusy = false;

            SubmitCommand = new Command(async () => await SubmitRequest());
            CameraCommand = new Command(async () => await CameraRequest());
            SelectFileCommand = new Command(async () => await SelectFile());
            CloseCommand = new Command(async () => await NavigationService.PopPageAsync());
            RemoveFileAttachedCommand = new Command(() => Holder.FileData = new Plugin.FilePicker.Abstractions.FileData());
            ItemSelectedCommand = new Command<ShiftDto>(GetShiftSchedule);
            SelectShiftCommand = new Command(() => { ShowShiftList = !ShowShiftList; });
            CancelCommand = new Command(async () => await CancelRequest());
            ViewTransactionHistoryCommand = new Command(async () => await NavigationService.PushModalAsync(new TransactionHistoryPage(TransactionType.WorkScheduleRequest, Holder.SpecialWorkScheduleRequestModel.WorkScheduleRequestId)));

            StartTimeChangeCommand = new Command<TimeSpan>(StartTimeChangeEvent);
            EndTimeChangeCommand = new Command<TimeSpan>(EndTimeChangeEvent);
            LunchStartTimeChangeCommand = new Command<TimeSpan>(LunchStartTimeChangeEvent);
            LunchEndTimeChangeCommand = new Command<TimeSpan>(LunchEndTimeChangeEvent);
            ViewFileAttachmentCommand = new Command(async () => await ViewFileAttachments());
            GetDateScheduleCommand = new Command<DateChangedEventArgs>(DateChangeEvent);
            OffSettingCommand = new Command<Syncfusion.XForms.Buttons.SwitchStateChangedEventArgs>(OffSetOptionEvent);
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
                    Holder = await specialWorkScheduleDataService_.InitForm(recordId, selectedDate);
                    ShiftList = Holder.ShiftList;
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

        private async void GetShiftSchedule(ShiftDto obj)
        {
            if (obj != null)
            {
                try
                {
                    Holder.ShiftSelectedItem = obj;
                    Holder = await specialWorkScheduleDataService_.GetShiftSchedule(Holder);
                }
                catch (Exception ex)
                {
                    Error(false, ex.Message);
                }
                finally
                {
                    ShowShiftList = false;
                }
            }
        }

        private async Task SubmitRequest()
        {
            try
            {
                Holder = await specialWorkScheduleDataService_.SubmitRequest(Holder);

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
                var file = await commonDataService_.TakePhotoAsync("SWS");
                /*var file = await commonDataService_.AttachPhoto2(response, "SWS");*/

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
                var response = await commonDataService_.FileUploadAsync();

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
                Holder = await specialWorkScheduleDataService_.WorkflowTransactionRequest(Holder);

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
            CalculateHours();
        }

        private void EndTimeChangeEvent(TimeSpan obj)
        {
            CalculateHours();
        }

        private void CalculateHours()
        {
            if (Holder != null)
            {
                /*Holder.EndTimeString = new DateTime(obj.Ticks).ToString(Constants.TimeFormatHHMMTT);*/
                if (Holder.ShiftSelectedItem.ShiftId < 0)
                {
                    if (Holder.ScheduleStartTime.HasValue && Holder.ScheduleEndTime.HasValue)
                    {
                        var dt1 = new DateTime(Holder.ScheduleStartTime.Value.Ticks);
                        var dt2 = new DateTime(Holder.ScheduleEndTime.Value.Ticks);
                        dt2 = dt2.AddDays((dt2 <= dt1 ? 1 : 0));
                        Holder.SpecialWorkScheduleRequestModel.WorkingHours = Convert.ToDecimal((dt2 - dt1).TotalHours);
                        Holder = Holder;
                    }
                }
            }
        }

        private void LunchStartTimeChangeEvent(TimeSpan obj)
        {
            ComputeLunchDuration();
        }

        private void LunchEndTimeChangeEvent(TimeSpan obj)
        {
            ComputeLunchDuration();
        }

        private void ComputeLunchDuration()
        {
            if (Holder != null)
            {
                /*Holder.LunchStartTimeString = new DateTime(obj.Ticks).ToString(Constants.TimeFormatHHMMTT);*/
                if (Holder.ShiftSelectedItem.ShiftId < 0)
                {
                    if (Holder.LunchStartTime.HasValue && Holder.LunchEndTime.HasValue)
                    {
                        var dt1 = new DateTime(Holder.LunchStartTime.Value.Ticks);
                        var dt2 = new DateTime(Holder.LunchEndTime.Value.Ticks);
                        dt2 = dt2.AddDays((dt2 <= dt1 ? 1 : 0));
                        Holder.SpecialWorkScheduleRequestModel.LunchDuration = Convert.ToDecimal((dt2 - dt1).TotalHours);
                        Holder = Holder;
                    }
                }
            }
        }

        private async void DateChangeEvent(DateChangedEventArgs obj)
        {
            if (!IsBusy)
            {
                try
                {
                    Holder = await specialWorkScheduleDataService_.GetDateSchedule(Holder);
                }
                catch (Exception ex)
                {
                    Error(false, ex.Message);
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

        private async Task ViewFileAttachments()
        {
            try
            {
                if (!IsBusy)
                {
                    var param = new FileAttachmentParams()
                    {
                        ModuleFormId = ModuleForms.ChangeWorkScheduleRequest,
                        TransactionId = Holder.SpecialWorkScheduleRequestModel.WorkScheduleRequestId
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
                        await commonDataService_.PreviewFileBase64(base64, file.FileType, file.FileName);
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

            MessagingCenter.Subscribe<SpecialWorkSchedulePage>(this, "onback", (sender) =>
            {
                BackItemPage();
            });
        }

        private void ExecutePageDisappearingCommand()
        {
            MessagingCenter.Unsubscribe<FileOptionViewModel, long>(this, "FileOptionSelectedValue");
            MessagingCenter.Unsubscribe<SpecialWorkSchedulePage>(this, "onback");
        }

        protected override async void BackItemPage()
        {
            if (Holder.SpecialWorkScheduleRequestModel.WorkScheduleRequestId == 0)
            {
                if (Holder.ShiftSelectedItem.ShiftId != 0 ||
                    Holder.SpecialWorkScheduleRequestModel.WorkingHours != 0 ||
                    !string.IsNullOrWhiteSpace(Holder.SpecialWorkScheduleRequestModel.Reason))
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