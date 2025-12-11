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
    internal class OfficialBusinessViewModel : BaseViewModel
    {
        #region commands

        public ICommand SubmitCommand { get; set; }
        public ICommand CameraCommand { get; set; }
        public ICommand SelectFileCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand StartTimePreviousDayCommand { get; set; }
        public ICommand EndTimeNextDayCommand { get; set; }
        public ICommand StartDateCommand { get; set; }
        public ICommand RemoveFileAttachedCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand ViewTransactionHistoryCommand { get; set; }
        public ICommand StartTimeChangeCommand { get; set; }
        public ICommand EndTimeChangeCommand { get; set; }
        public ICommand ViewFileAttachmentCommand { get; set; }
        public ICommand IncludeHolidayCommand { get; set; }
        public ICommand IncludeRestdayCommand { get; set; }
        public ICommand FileChooseOptionCommand { get; set; }
        public ICommand PageAppearingCommand { get; set; }
        public ICommand PageDisappearingCommand { get; set; }

        #endregion commands

        private OfficialBusinessHolder formHeler_;

        public OfficialBusinessHolder Holder
        {
            get { return formHeler_; }
            set { formHeler_ = value; RaisePropertyChanged(() => Holder); }
        }

        private readonly IOfficialBusinessRequestDataService officialbusinessRequestDataService_;
        private readonly ICommonDataService myRequestCommonDataService_;
        private readonly IDialogService dialogService_;

        public OfficialBusinessViewModel(IOfficialBusinessRequestDataService officialbusinessRequestDataService,
            ICommonDataService myRequestCommonDataService)
        {
            officialbusinessRequestDataService_ = officialbusinessRequestDataService;
            myRequestCommonDataService_ = myRequestCommonDataService;
            dialogService_ = AppContainer.Resolve<IDialogService>();
        }

        public void Init(INavigation navigation, long recordId, int obTypeId, DateTime? selectedDate)
        {
            NavigationBack = navigation;
            IsBusy = false;

            SubmitCommand = new Command(SubmitRequest);
            CameraCommand = new Command(async () => await CameraRequest());
            SelectFileCommand = new Command(async () => await SelectFile());
            CloseCommand = new Command(async () => await NavigationService.PopPageAsync());
            StartTimePreviousDayCommand = new Command<Syncfusion.XForms.Buttons.StateChangedEventArgs>(StartTimePreviousDayEvent);
            EndTimeNextDayCommand = new Command<Syncfusion.XForms.Buttons.StateChangedEventArgs>(EndTimeNextDayEvent);
            StartDateCommand = new Command<DateChangedEventArgs>(StartDateEvent);
            RemoveFileAttachedCommand = new Command(() => Holder.FileData = new Plugin.FilePicker.Abstractions.FileData());
            CancelCommand = new Command(async () => await CancelRequest());
            ViewTransactionHistoryCommand = new Command(async () =>
            {
                await NavigationService.PushModalAsync(new TransactionHistoryPage((obTypeId == Constants.OfficialBusiness ? TransactionType.OfficialBusiness : TransactionType.TimeOff),
                    Holder.OfficialBusinessModel.OfficialBusinessId));
            });

            StartTimeChangeCommand = new Command<TimeSpan>(StartTimeChangeEvent);
            EndTimeChangeCommand = new Command<TimeSpan>(EndTimeChangeEvent);
            ViewFileAttachmentCommand = new Command(async () => await ViewFileAttachments());
            FileChooseOptionCommand = new Command<FileUploadResponse>(ExecuteFileChooseOptionCommand);
            PageAppearingCommand = new Command(ExecutePageAppearingCommand);
            PageDisappearingCommand = new Command(ExecutePageDisappearingCommand);

            IncludeHolidayCommand = new Command<Syncfusion.XForms.Buttons.StateChangedEventArgs>(IncludeHolidays);
            IncludeRestdayCommand = new Command<Syncfusion.XForms.Buttons.StateChangedEventArgs>(IncludeRestdays);

            InitForm(recordId, obTypeId, selectedDate);
        }

        private async void InitForm(long recordId, int obTypeId, DateTime? selectedDate)
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);
                    Holder = await officialbusinessRequestDataService_.InitForm(recordId, obTypeId, selectedDate);
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

        private async void SubmitRequest()
        {
            try
            {
                Holder = await officialbusinessRequestDataService_.SubmitRequest(Holder);

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
                var prefix = (Holder.OfficialBusinessModel.TypeId == Constants.OfficialBusiness ? "OB" : "TIMEOFF");
                var file = await myRequestCommonDataService_.TakePhotoAsync(prefix);
                /*var file = await myRequestCommonDataService_.AttachPhoto2(response, prefix);*/

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

        private void StartTimePreviousDayEvent(Syncfusion.XForms.Buttons.StateChangedEventArgs obj)
        {
            if (Holder != null)
            {
                var val = ((bool)obj.IsChecked && !(bool)Holder.OfficialBusinessModel.EndTimeNextDay);

                Holder.OfficialBusinessModel.StartTimePreviousDay = val;
                Holder.EndCheckboxEnabled = !val;
                Holder = Holder;
            }
        }

        private void EndTimeNextDayEvent(Syncfusion.XForms.Buttons.StateChangedEventArgs obj)
        {
            if (Holder != null)
            {
                var val = ((bool)obj.IsChecked && !(bool)Holder.OfficialBusinessModel.StartTimePreviousDay);

                Holder.OfficialBusinessModel.EndTimeNextDay = val;
                Holder.StartCheckboxEnabled = !val;
                Holder = Holder;
            }
        }

        private void IncludeHolidays(Syncfusion.XForms.Buttons.StateChangedEventArgs obj)
        {
            if (Holder != null)
            {
                var val = (bool)obj.IsChecked;

                Holder.OfficialBusinessModel.IncludeHolidays = val;
                Holder = Holder;
            }
        }

        private void IncludeRestdays(Syncfusion.XForms.Buttons.StateChangedEventArgs obj)
        {
            if (Holder != null)
            {
                var val = (bool)obj.IsChecked;

                Holder.OfficialBusinessModel.IncludeRestdays = val;
                Holder = Holder;
            }
        }

        private void StartDateEvent(DateChangedEventArgs obj)
        {
            var result = DateTime.Compare(Holder.OBEndDate.Value, obj.NewDate);
            if (result < 0)
            {
                Holder.OBEndDate = obj.NewDate;
                Holder = Holder;
            }
        }

        private async Task CancelRequest()
        {
            try
            {
                Holder.ActionTypeId = ActionTypeId.Cancel;
                Holder.Msg = Messages.Cancel;
                Holder = await officialbusinessRequestDataService_.WorkflowTransactionRequest(Holder);

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
            ComputeOBHours();
            Holder.StartTimeString = new DateTime(obj.Ticks).ToString(Constants.TimeFormatHHMMTT);
            Holder = Holder;
        }

        private void EndTimeChangeEvent(TimeSpan obj)
        {
            ComputeOBHours();
            Holder.EndTimeString = new DateTime(obj.Ticks).ToString(Constants.TimeFormatHHMMTT);
            Holder = Holder;
        }

        private void ComputeOBHours()
        {
            if (Holder != null)
            {
                if (Holder.OfficialBusinessModel.OfficialBusinessId == 0)
                {
                    if (Holder.StartTime.HasValue && Holder.EndTime.HasValue)
                    {
                        var dt1 = new DateTime(Holder.StartTime.Value.Ticks);
                        var dt2 = new DateTime(Holder.EndTime.Value.Ticks);
                        dt2 = dt2.AddDays((dt2 <= dt1 ? 1 : 0));
                        Holder.OfficialBusinessModel.NoOfHours = Convert.ToDecimal((dt2 - dt1).TotalHours);
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
                        /*ModuleFormId = (Holder.OfficialBusinessModel.TypeId == Constants.OfficialBusiness ? ModuleForms.OfficialBusiness : ModuleForms.TimeOffRequest),*/
                        ModuleFormId = ModuleForms.OfficialBusiness,
                        TransactionId = Holder.OfficialBusinessModel.OfficialBusinessId
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

            MessagingCenter.Subscribe<OfficialBusinessRequestPage>(this, "onback", (sender) =>
            {
                BackItemPage();
            });

            MessagingCenter.Subscribe<TimeOffRequestPage>(this, "onback", (sender) =>
            {
                BackItemPage();
            });
        }

        private void ExecutePageDisappearingCommand()
        {
            MessagingCenter.Unsubscribe<FileOptionViewModel, long>(this, "FileOptionSelectedValue");
            MessagingCenter.Unsubscribe<TimeOffRequestPage>(this, "onback");
            MessagingCenter.Unsubscribe<OfficialBusinessRequestPage>(this, "onback");
        }

        protected override async void BackItemPage()
        {
            if (Holder.OfficialBusinessModel.OfficialBusinessId == 0)
            {
                if (Holder.OfficialBusinessModel.NoOfHours != 0 ||
                    Holder.OBReasonSelectedItem.Id != 0 ||
                    !string.IsNullOrWhiteSpace(Holder.OfficialBusinessModel.Remarks) ||
                    Holder.OBApplyToSelectedItem.Id != 0)
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