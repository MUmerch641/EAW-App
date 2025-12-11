using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.Request;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Views;
using EatWork.Mobile.Views.Requests;
using EatWork.Mobile.Views.Requests.LeaveRequest;
using EatWork.Mobile.Views.Shared;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class LeaveRequestViewModel : BaseViewModel
    {
        #region commands

        public ICommand SubmitCommand { get; set; }
        public ICommand CameraCommand { get; set; }
        public ICommand SelectFileCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand CloseModalCommand { get; set; }
        public ICommand PartialDayOptionCommand { get; set; }
        public ICommand LeaveTypeSelectionCommand { get; set; }
        public ICommand InclusiveStartDateChangedCommand { get; set; }
        public ICommand RemoveFileAttachedCommand { get; set; }
        public ICommand ItemSelectedCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand ViewTransactionHistoryCommand { get; set; }
        public ICommand ViewFileAttachmentCommand { get; set; }
        public ICommand GenerateLeaveScheduleCommand { get; set; }
        public ICommand ViewLeaveDetailsCommand { get; set; }
        public ICommand ViewLeaveDocumentCommand { get; set; }
        public ICommand AddDocumentCommand { get; set; }
        public ICommand HideFormPopupCommand { get; set; }
        public ICommand AddDocumentToListCommand { get; set; }
        public ICommand RemoveFileAttachmentCommand { get; set; }
        public ICommand ContinueSubmitRequestCommand { get; set; }
        public ICommand PopToRootCommand { get; set; }
        public ICommand ApplyToEventCommand { get; set; }
        public ICommand PageAppearingCommand { get; set; }
        public ICommand PageDisappearingCommand { get; set; }
        public ICommand DownloadAttachedFileCommand { get; set; }

        #endregion commands

        private LeaveRequestHolder formHelper_;

        public LeaveRequestHolder FormHelper
        {
            get { return formHelper_; }
            set { formHelper_ = value; RaisePropertyChanged(() => FormHelper); }
        }

        private ObservableCollection<SelectableListModel> leaveTypeList_;

        public ObservableCollection<SelectableListModel> LeaveTypeList
        {
            get { return leaveTypeList_; }
            set { leaveTypeList_ = value; RaisePropertyChanged(() => LeaveTypeList); }
        }

        private bool showLeaveTypeList_;

        public bool ShowLeaveTypeList
        {
            get { return showLeaveTypeList_; }
            set { showLeaveTypeList_ = value; RaisePropertyChanged(() => ShowLeaveTypeList); }
        }

        private bool showAttachmentForm_;

        public bool ShowAttachmentForm
        {
            get { return showAttachmentForm_; }
            set { showAttachmentForm_ = value; RaisePropertyChanged(() => ShowAttachmentForm); }
        }

        private ObservableCollection<LeaveRequestDocumentModel> leaveDocumentModel_;

        public ObservableCollection<LeaveRequestDocumentModel> LeaveDocumentModel
        {
            get { return leaveDocumentModel_; }
            set { leaveDocumentModel_ = value; RaisePropertyChanged(() => LeaveDocumentModel); }
        }

        //public INavigation NavigationBack { get; set; }
        public NavigationPageHelper PageHelper { get; set; }

        private readonly ILeaveRequestDataService leaveRequestDataService_;
        private readonly ICommonDataService myRequestCommonDataService_;
        private readonly IPopupNavigation popupNavigation_;
        private readonly IDialogService dialogService_;
        protected Page CurrentMainPage => Application.Current.MainPage;

        public LeaveRequestViewModel(ILeaveRequestDataService leaveRequestDataService,
            ICommonDataService myRequestCommonDataService)
        {
            leaveRequestDataService_ = leaveRequestDataService;
            myRequestCommonDataService_ = myRequestCommonDataService;
            popupNavigation_ = PopupNavigation.Instance;
            dialogService_ = AppContainer.Resolve<IDialogService>();
        }

        public void Init(INavigation navigation, long recordId, DateTime? selectedDate)
        {
            NavigationBack = navigation;
            PageHelper = new NavigationPageHelper(navigation);
            IsBusy = false;
            FormHelper = new LeaveRequestHolder();
            LeaveTypeList = new ObservableCollection<SelectableListModel>();
            ShowLeaveTypeList = false;

            //==commands
            SubmitCommand = new Command(async () => await SubmitRequest());

            CloseCommand = new Command(async () => await NavigationService.PopPageAsync());

            /*PartialDayOptionCommand = new Command<Syncfusion.XForms.Buttons.SelectionChangedEventArgs>(PartialDayOptionEvent);*/
            PartialDayOptionCommand = new Command<Syncfusion.XForms.Buttons.SwitchStateChangedEventArgs>(PartialDayOptionEvent);

            InclusiveStartDateChangedCommand = new Command<DateChangedEventArgs>(DateChangeEvent);

            /*RemoveFileAttachedCommand = new Command(() => FormHelper.FileData = new Plugin.FilePicker.Abstractions.FileData());*/
            RemoveFileAttachedCommand = new Command(() => FormHelper.SelectedLeaveDocumentModel = new LeaveRequestDocumentModel());

            LeaveTypeSelectionCommand = new Command(() => ShowLeaveTypeList = !ShowLeaveTypeList);
            //LeaveTypeSelectionCommand = new Command(async () => await NavigationService.PushModalAsync(new LeaveTypeListPage(FormHelper, LeaveTypeList)));

            ItemSelectedCommand = new Command<SelectableListModel>(LeaveTypeSetupSelectionEvent);

            CancelCommand = new Command(async () => await CancelRequest());

            ViewTransactionHistoryCommand = new Command(async () => await NavigationService.PushModalAsync(new TransactionHistoryPage(TransactionType.Leave, FormHelper.LeaveRequestHeaderModel.LeaveRequestHeaderId)));

            ViewFileAttachmentCommand = new Command(async () => await ViewFileAttachments());

            GenerateLeaveScheduleCommand = new Command(async () => await ValidateLeaveRequestGeneration());

            //GenerateLeaveScheduleCommand = new Command(async () => await ValidateLeaveRequestGeneration());

            ViewLeaveDetailsCommand = new Command(async () => await NavigationService.PushModalAsync(new GeneratedLeaveListPage(FormHelper)));

            ViewLeaveDocumentCommand = new Command(async () => await NavigationService.PushModalAsync(new LeaveDocumentListPage(FormHelper)));

            PopToRootCommand = new Command(async () => await NavigationService.PopToRootAsync());

            ApplyToEventCommand = new Command<Syncfusion.XForms.Buttons.SelectionChangedEventArgs>(ApplyToOptionEvent);

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
                    FormHelper = await leaveRequestDataService_.InitLeaveRequestForm(recordId, selectedDate);

                    LeaveTypeList = FormHelper.LeaveType;
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

        /// <summary>
        /// Submit request if has warning details
        /// </summary>
        private async Task WarningPageClosed()
        {
            try
            {
                if (PreferenceHelper.WarningPageClosed())
                {
                    FormHelper = await leaveRequestDataService_.SubmitRequestEngine(FormHelper);

                    if (FormHelper.Success)
                    {
                        Success(true, Messages.RecordSaved);
                        await NavigationService.PopToRootAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
            finally
            {
                PreferenceHelper.WarningPageClosed(false);
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private async Task SubmitRequest()
        {
            try
            {
                FormHelper = await leaveRequestDataService_.GenerateAndSubmitRequest(FormHelper);

                if (FormHelper.HasWarning)
                {
                    await ShowWarningPageAndWaitForClose(FormHelper.ConflictList, FormHelper.ConflictMessage);
                    await WarningPageClosed();
                    return;
                }

                /*
                if (FormHelper.LeaveRequestDetailList.Count == 0)
                {
                    Error(false, "No leave(s) generated.");
                    return;
                }
                */

                await SubmissionHelper();
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

        private async Task ShowWarningPageAndWaitForClose(IEnumerable<string> conflictList, string conflictMessage)
        {
            var list = new ObservableCollection<string>(conflictList);
            var page = new EatWork.Mobile.Views.Shared.WarningPage(list, conflictMessage);
            await Application.Current.MainPage.Navigation.PushModalAsync(page);

            await page.WaitForModalToCloseAsync();
        }

        private async Task SubmissionHelper()
        {
            if (FormHelper.IsMultipleLeave && FormHelper.IsGenerated)
            {
                /*await Navigation.PushModalAsync(new GeneratedLeaveListPage(FormHelper, Navigation));*/
                await GenerateLeaveModal();
                await ContinueSubmitRequest();
                return;
            }

            if (FormHelper.Success)
            {
                Success(true, Messages.RecordSaved);
                await NavigationService.PopToRootAsync();
            }
        }

        /// <summary>
        /// Submit request coming from multiple leaves list page
        /// </summary>
        public async Task ContinueSubmitRequest()
        {
            try
            {
                if (PreferenceHelper.LeaveFormContinue())
                {
                    FormHelper = await leaveRequestDataService_.SubmitRequestEngine(FormHelper);

                    if (FormHelper.Success)
                    {
                        Success(true, Messages.RecordSaved);
                        await NavigationBack.PopToRootAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
            finally
            {
                PreferenceHelper.LeaveFormContinue(false);
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private void PartialDayOptionEvent(Syncfusion.XForms.Buttons.SwitchStateChangedEventArgs obj)
        {
            FormHelper.ShowPartialOptions = obj.NewValue.Value;

            if (obj.NewValue.Value)
                FormHelper.LeaveRequestModel.PartialDayLeave = 1;
            else
                FormHelper.LeaveRequestModel.PartialDayLeave = 0;
        }

        private void ApplyToOptionEvent(Syncfusion.XForms.Buttons.SelectionChangedEventArgs obj)
        {
            var val = 0;
            if (obj.Index == 1)
                val = 2;
            else
                val = 1;

            FormHelper.ApplyToSelectedItem = FormHelper.ApplyTo.Where(p => p.Id == val).FirstOrDefault();
            FormHelper = FormHelper;
        }

        private async void LeaveTypeSetupSelectionEvent(SelectableListModel obj)
        {
            if (obj != null)
            {
                try
                {
                    FormHelper.LeaveTypeSelectedItem = obj;
                    FormHelper = await leaveRequestDataService_.GetLeaveBalance(FormHelper);
                }
                catch (Exception ex)
                {
                    await dialogService_.AlertAsync(ex.Message);
                }
                finally
                {
                    ShowLeaveTypeList = false;
                }
            }
        }

        private void DateChangeEvent(DateChangedEventArgs obj)
        {
            var result = DateTime.Compare(FormHelper.InclusiveEndDate, obj.NewDate);
            if (result < 0)
            {
                FormHelper.InclusiveEndDate = obj.NewDate;
                FormHelper = FormHelper;
            }
        }

        private async Task CancelRequest()
        {
            try
            {
                FormHelper.ActionTypeId = ActionTypeId.Cancel;
                FormHelper.Msg = Messages.Cancel;
                FormHelper = await leaveRequestDataService_.WorkflowTransactionRequest(FormHelper);

                if (FormHelper.Success)
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
                    var param = new FileAttachmentParams()
                    {
                        ModuleFormId = ModuleForms.LeaveRequest,
                        TransactionId = FormHelper.LeaveRequestModel.LeaveRequestHeaderId.Value
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

        private async Task ValidateLeaveRequestGeneration()
        {
            try
            {
                FormHelper = await leaveRequestDataService_.ValidateLeaveRequestGeneration(FormHelper);

                if (FormHelper.IsGenerated && !FormHelper.HasWarning)
                {
                    await NavigationService.PushModalAsync(new GeneratedLeaveListPage(FormHelper));
                }
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
        }

        #region LEAVE DETAILS LIST

        public void InitLeaveDetailsList(INavigation navigation, LeaveRequestHolder holder)
        {
            FormHelper = holder;
            NavigationBack = navigation;

            CloseModalCommand = new Command(async () => await NavigationService.PopModalAsync());

            /*ContinueSubmitRequestCommand = new Command(async () => await ContinueSubmitRequest());*/
            ContinueSubmitRequestCommand = new Command(async () =>
            {
                /*
                FormHelper.IsContinue = true;

                MessagingCenter.Send<LeaveRequestViewModel>(this, "continuemultiplesubmit");
                */
                PreferenceHelper.LeaveFormContinue(true);
                await NavigationService.PopModalAsync();
            });
        }

        private async Task GenerateLeaveModal() 
        {
            var page = new GeneratedLeaveListPage(FormHelper);
            await NavigationService.PushModalAsync(page);
            await page.WaitForModalToCloseAsync();
        }

        #endregion LEAVE DETAILS LIST

        #region LEAVE DOCUMENTS

        public void InitLeaveDocuments(INavigation navigation, LeaveRequestHolder holder)
        {
            FormHelper = holder;
            NavigationBack = navigation;

            AddDocumentCommand = new Command(async () =>
            {
                FormHelper.SelectedLeaveDocumentModel = new LeaveRequestDocumentModel();
                await popupNavigation_.PushAsync(new LeaveDocumentAttachmentPage(holder));
            });
            //HideFormPopupCommand = new Command(() => { ShowAttachmentForm = false; });
            CloseModalCommand = new Command(async () => await NavigationService.PopModalAsync());
            RemoveFileAttachmentCommand = new Command(RemoveItemClicked);
            DownloadAttachedFileCommand = new Command(ExecuteDownloadAttachedFileCommand);
        }

        public void InitDocumentForm(LeaveRequestHolder holder)
        {
            FormHelper = holder;
            LeaveDocumentModel = new ObservableCollection<LeaveRequestDocumentModel>();

            CameraCommand = new Command(async () => await TakePhoto());

            SelectFileCommand = new Command(async () => await SelectFile());

            CloseModalCommand = new Command(async () => await popupNavigation_.PopAsync(true));
            AddDocumentToListCommand = new Command(async () => await AddDocumentToList());
        }

        private async Task TakePhoto()
        {
            try
            {
                var file = await myRequestCommonDataService_.TakePhotoAsync("LEAVE");
                /*var file = await myRequestCommonDataService_.AttachPhoto2(response, "LEAVE");*/

                if (file != null)
                {
                    FormHelper.SelectedLeaveDocumentModel = new LeaveRequestDocumentModel()
                    {
                        FileType = file.FileType,
                        cmbLeaveTypeDocumentId = FormHelper.SelectedLeaveTypeDocument.Id,
                        DocumentName = FormHelper.SelectedLeaveTypeDocument.DisplayText,
                        FileBytes = string.Format("data:{0};base64,{1}", file.MimeType, Convert.ToBase64String(file.FileDataArray)),
                        FileName = file.FileName,
                    };

                    FormHelper = FormHelper;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                /*//await dialogService_.AlertAsync(ex.Message);*/
            }
        }

        private async Task SelectFile()
        {
            try
            {
                var response = await myRequestCommonDataService_.FileUploadAsync();

                if (response != null)
                {
                    FormHelper.SelectedLeaveDocumentModel = new LeaveRequestDocumentModel()
                    {
                        FileType = response.FileType,
                        cmbLeaveTypeDocumentId = FormHelper.SelectedLeaveTypeDocument.Id,
                        DocumentName = FormHelper.SelectedLeaveTypeDocument.DisplayText,
                        FileBytes = string.Format("data:{0};base64,{1}", response.MimeType, Convert.ToBase64String(response.FileDataArray)),
                        FileName = response.FileName,
                    };

                    FormHelper = FormHelper;
                }
            }
            catch (Exception ex)
            {
                await Dialogs.AlertAsync(ex.Message, "", "Close");
            }
        }

        private async Task AddDocumentToList()
        {
            FormHelper.LeaveDocumentModel.Add(FormHelper.SelectedLeaveDocumentModel);
            FormHelper.ShowDocumentList = true;
            FormHelper = FormHelper;
            await popupNavigation_.PopAsync(true);
        }

        private async void RemoveItemClicked(object obj)
        {
            await Task.Run(() =>
            {
                if (obj is LeaveRequestDocumentModel item)
                {
                    FormHelper.LeaveDocumentModel.Remove(item);

                    if (FormHelper.LeaveDocumentModel.Count == 0)
                        FormHelper.ShowDocumentList = false;

                    FormHelper = FormHelper;
                }
            });
        }

        private async void ExecuteDownloadAttachedFileCommand(object obj)
        {
            try
            {
                if (obj is LeaveRequestDocumentModel item)
                {
                    using (Dialogs.Loading())
                    {
                        await Task.Delay(500);
                        await myRequestCommonDataService_.PreviewFileBase64(item.FileBytes, item.FileType, item.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
            }
        }

        #endregion LEAVE DOCUMENTS

        #region leave type list

        public void InitLeaveTypeList(INavigation navigation, LeaveRequestHolder holder, ObservableCollection<SelectableListModel> list)
        {
            LeaveTypeList = list;
            NavigationBack = navigation;
            FormHelper = holder;

            ItemSelectedCommand = new Command<SelectableListModel>(LeaveTypeSetupSelectionEvent);

            CloseModalCommand = new Command(async () => await NavigationService.PopModalAsync());
        }

        #endregion leave type list

        private void ExecutePageAppearingCommand()
        {
            /*
            MessagingCenter.Subscribe<LeaveRequestViewModel>(this, "continuemultiplesubmit", (sender) =>
            {
                ContinueSubmitRequest();
            });
            */
            MessagingCenter.Unsubscribe<LeaveRequestPage>(this, "onback");
            MessagingCenter.Subscribe<LeaveRequestPage>(this, "onback", (sender) =>
            {
                BackItemPage();
            });

            /*ContinueSubmitRequest();*/

            /*WarningPageClosed();*/
        }

        private void ExecutePageDisappearingCommand()
        {
            /*MessagingCenter.Unsubscribe<LeaveRequestPage>(this, "onback");
            MessagingCenter.Unsubscribe<LeaveRequestViewModel>(this, "continuemultiplesubmit");*/
        }

        protected override async void BackItemPage()
        {
            if (FormHelper.LeaveRequestHeaderModel.LeaveRequestHeaderId == 0)
            {
                if (!string.IsNullOrWhiteSpace(FormHelper.LeaveTypeSelectedItem.DisplayText) ||
                    !string.IsNullOrWhiteSpace(FormHelper.LeaveRequestModel.Reason))
                {
                    if (await dialogService_.ConfirmDialogAsync(Messages.LEAVEPAGE))
                    {
                        MessagingCenter.Unsubscribe<LeaveRequestPage>(this, "onback");
                        MessagingCenter.Unsubscribe<LeaveRequestViewModel>(this, "continuemultiplesubmit");
                        base.BackItemPage();
                    }
                }
                else
                {
                    MessagingCenter.Unsubscribe<LeaveRequestPage>(this, "onback");
                    MessagingCenter.Unsubscribe<LeaveRequestViewModel>(this, "continuemultiplesubmit");
                    base.BackItemPage();
                }
            }
            else
            {
                MessagingCenter.Unsubscribe<LeaveRequestPage>(this, "onback");
                MessagingCenter.Unsubscribe<LeaveRequestViewModel>(this, "continuemultiplesubmit");
                base.BackItemPage();
            }
        }
    }
}