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
    public class ChangeWorkScheduleViewModel : ListViewModel
    {
        #region commands

        public ICommand SubmitCommand { get; set; }
        public ICommand CameraCommand { get; set; }
        public ICommand SelectFileCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand SelectShiftCommand { get; set; }
        public ICommand ItemSelectedCommand { get; set; }
        public ICommand GetScheduleCommand { get; set; }
        public ICommand CloseModalCommand { get; set; }
        public ICommand RemoveFileAttachedCommand { get; set; }
        public ICommand StartTimePreviousDayCommand { get; set; }
        public ICommand EndTimeNextDayCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand ViewTransactionHistoryCommand { get; set; }
        public ICommand StartTimeChangeCommand { get; set; }
        public ICommand EndTimeChangeCommand { get; set; }
        public ICommand LunchStartTimeChangeCommand { get; set; }
        public ICommand LunchEndTimeChangeCommand { get; set; }
        public ICommand ViewFileAttachmentCommand { get; set; }
        public ICommand FileChooseOptionCommand { get; set; }
        public ICommand PageAppearingCommand { get; set; }
        public ICommand PageDisappearingCommand { get; set; }

        #endregion commands

        #region properties

        private ChangeWorkScheduleHolder formHelper_;

        public ChangeWorkScheduleHolder Holder
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

        #endregion properties

        private readonly IChangeWorkScheduleDataService changeWorkScheduleDataService_;
        private readonly ICommonDataService myRequestCommonDataService_;
        private readonly IEmployeeDataService employeeListDataService_;
        private readonly IDialogService dialogService_;

        public ChangeWorkScheduleViewModel(IChangeWorkScheduleDataService changeWorkScheduleDataService,
            ICommonDataService myRequestCommonDataService,
            IEmployeeDataService employeeListDataService)
        {
            changeWorkScheduleDataService_ = changeWorkScheduleDataService;
            myRequestCommonDataService_ = myRequestCommonDataService;
            employeeListDataService_ = employeeListDataService;
            dialogService_ = AppContainer.Resolve<IDialogService>();
        }

        #region change work schedule

        public void Init(INavigation navigation, long recordId, DateTime? selectedDate)
        {
            NavigationBack = navigation;
            /*Holder = new ChangeWorkScheduleHolder();*/
            ShowShiftList = false;
            IsBusy = false;

            //==commands
            SubmitCommand = new Command(async () => await SubmitRequest());
            CameraCommand = new Command(async () => await TakePhoto());
            SelectFileCommand = new Command(async () => await SelectFile());
            CloseCommand = new Command(async () => await NavigationService.PopPageAsync());
            //SelectEmployeeCommand = new Command(async () => await MoreEmployeeList());
            SelectShiftCommand = new Command(() => { ShowShiftList = !ShowShiftList; });
            GetScheduleCommand = new Command<string>(GetEmployeeScheduleEvent);
            RemoveFileAttachedCommand = new Command(() => Holder.FileData = new Plugin.FilePicker.Abstractions.FileData());
            OpenSearchCommand = new Command(() => { ShowSearchField = !ShowSearchField; });
            ItemSelectedCommand = new Command<ShiftDto>(AddSelectedTransactionType2);
            CancelCommand = new Command(async () => await CancelRequest());
            ViewTransactionHistoryCommand = new Command(async () => await NavigationService.PushModalAsync(new TransactionHistoryPage(TransactionType.ChangeWorkSchedule, Holder.ChangeWorkScheduleModel.ChangeWorkScheduleId)));
            StartTimePreviousDayCommand = new Command<Syncfusion.XForms.Buttons.StateChangedEventArgs>(StartTimePreviousDayEvent);
            EndTimeNextDayCommand = new Command<Syncfusion.XForms.Buttons.StateChangedEventArgs>(EndTimeNextDayEvent);

            StartTimeChangeCommand = new Command<TimeSpan>(StartTimeChangeEvent);
            EndTimeChangeCommand = new Command<TimeSpan>(EndTimeChangeEvent);
            LunchStartTimeChangeCommand = new Command<TimeSpan>(LunchStartTimeChangeEvent);
            LunchEndTimeChangeCommand = new Command<TimeSpan>(LunchEndTimeChangeEvent);
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
                    Holder = await changeWorkScheduleDataService_.InitForm(recordId, selectedDate);

                    ShiftList = Holder.ShiftList;
                    ShowList = (ShiftList.Count == 0 && (string.IsNullOrWhiteSpace(KeyWord) && SelectedTransactionTypes.Count == 0) ? false : true);
                    NoItems = (ShiftList.Count == 0 && (!string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count > 0) ? true : false);
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
                Holder = await changeWorkScheduleDataService_.SubmitRequest(Holder);

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
                var file = await myRequestCommonDataService_.TakePhotoAsync("CWS");
                /*var file = await myRequestCommonDataService_.AttachPhoto2(response, "CWS");*/

                if (file != null && !string.IsNullOrWhiteSpace(file.FileName))
                {
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

        private async void GetEmployeeScheduleEvent(string option)
        {
            if (Holder != null)
            {
                if (!IsBusy)
                {
                    try
                    {
                        IsBusy = true;
                        await Task.Delay(500);
                        Holder = await changeWorkScheduleDataService_.GetEmployeeSchedule(Holder, Convert.ToInt16(option));
                        ShiftList = new ObservableCollection<ShiftDto>();
                        ShiftList = Holder.ShiftList;
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
        }

        private void StartTimePreviousDayEvent(Syncfusion.XForms.Buttons.StateChangedEventArgs obj)
        {
            if (Holder != null)
            {
                var val = ((bool)obj.IsChecked && !(bool)Holder.ChangeWorkScheduleModel.EndTimeNextDay);

                Holder.ChangeWorkScheduleModel.StartTimePreviousDay = val;
                Holder.EnableEndTimeNextDay = !val;
                Holder = Holder;
            }
        }

        private void EndTimeNextDayEvent(Syncfusion.XForms.Buttons.StateChangedEventArgs obj)
        {
            if (Holder != null)
            {
                var val = ((bool)obj.IsChecked && !(bool)Holder.ChangeWorkScheduleModel.StartTimePreviousDay);

                Holder.ChangeWorkScheduleModel.EndTimeNextDay = val;
                Holder.EnableStartTimePreviousDay = !val;
                Holder = Holder;
            }
        }

        public void AddSelectedTransactionType2(ShiftDto obj)
        {
            if (obj != null)
            {
                if (Holder != null)
                {
                    Task.Run(async () =>
                    {
                        try
                        {
                            await Task.Delay(500);
                            Holder.ShiftSelectedItem = obj;

                            using (Dialogs.Loading())
                                Holder = await changeWorkScheduleDataService_.GetEmployeeSchedule(Holder, 2);
                        }
                        catch (Exception ex)
                        {
                            Error(false, ex.Message);
                        }
                        finally
                        {
                            ShowShiftList = false;
                        }
                    });
                }
            }
        }

        private async Task CancelRequest()
        {
            try
            {
                Holder.ActionTypeId = ActionTypeId.Cancel;
                Holder.Msg = Messages.Cancel;
                Holder = await changeWorkScheduleDataService_.WorkflowTransactionRequest(Holder);

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
            ComputeWorkingHours();
        }

        private void EndTimeChangeEvent(TimeSpan obj)
        {
            ComputeWorkingHours();
        }

        private void ComputeWorkingHours()
        {
            if (Holder != null)
            {
                if (Holder.ShiftSelectedItem.ShiftId < 0 && Holder.ChangeWorkScheduleModel.ChangeWorkScheduleId == 0)
                {
                    if (Holder.ScheduleStartTime.HasValue && Holder.ScheduleEndTime.HasValue)
                    {
                        var dt1 = new DateTime(Holder.ScheduleStartTime.Value.Ticks);
                        var dt2 = new DateTime(Holder.ScheduleEndTime.Value.Ticks);
                        dt2 = dt2.AddDays((dt2 <= dt1 ? 1 : 0));
                        Holder.ChangeWorkScheduleModel.WorkingHours = Convert.ToDecimal((dt2 - dt1).TotalHours);
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
                if (Holder.ShiftSelectedItem.ShiftId < 0 && Holder.ChangeWorkScheduleModel.ChangeWorkScheduleId == 0)
                {
                    if (Holder.LunchStartTime.HasValue && Holder.LunchEndTime.HasValue)
                    {
                        var dt1 = new DateTime(Holder.LunchStartTime.Value.Ticks);
                        var dt2 = new DateTime(Holder.LunchEndTime.Value.Ticks);
                        dt2 = dt2.AddDays((dt2 <= dt1 ? 1 : 0));
                        Holder.ChangeWorkScheduleModel.LunchDuration = Convert.ToDecimal((dt2 - dt1).TotalHours);
                        Holder = Holder;
                    }
                }
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
                        TransactionId = Holder.ChangeWorkScheduleModel.ChangeWorkScheduleId
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

        #endregion change work schedule

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

            MessagingCenter.Subscribe<ChangeWorkSchedulePage>(this, "onback", (sender) =>
            {
                BackItemPage();
            });
        }

        private void ExecutePageDisappearingCommand()
        {
            MessagingCenter.Unsubscribe<FileOptionViewModel, long>(this, "FileOptionSelectedValue");
            MessagingCenter.Unsubscribe<ChangeWorkSchedulePage>(this, "onback");
        }

        protected override async void BackItemPage()
        {
            if (Holder.ChangeWorkScheduleModel.ChangeWorkScheduleId == 0)
            {
                if (Holder.ShiftSelectedItem.ShiftId != 0 ||
                    !string.IsNullOrWhiteSpace(Holder.ChangeWorkScheduleModel.Details))
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