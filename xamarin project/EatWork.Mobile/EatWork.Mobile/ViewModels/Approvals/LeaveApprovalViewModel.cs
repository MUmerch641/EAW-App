using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.FormHolder.Approvals;
using EatWork.Mobile.Views;
using EatWork.Mobile.Views.Approvals;
using EatWork.Mobile.Views.Approvals.LeaveRequest;
using EatWork.Mobile.Views.Shared;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class LeaveApprovalViewModel : BaseViewModel
    {
        #region commands

        public ICommand LeaveReaquestUsageCommand { get; set; }
        public ICommand CloseModalCommand { get; set; }
        public ICommand TransactionCommand { get; set; }
        public ICommand ViewLeaveUsageItemCommand { get; set; }
        public ICommand ViewFileAttachmentsCommand { get; set; }
        public ICommand CloseCurrentPageCommand { get; set; }
        public ICommand ViewProfileCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand OpenSearchCommand { get; set; }
        public ICommand ViewTransactionHistoryCommand { get; set; }
        public ICommand ViewLeaveDetailsCommand { get; set; }
        public ICommand DownloadAttachedFileCommand { get; set; }

        #endregion commands

        private ObservableCollection<LeaveUsageListHolder> leaveUsageList_;

        public ObservableCollection<LeaveUsageListHolder> LeaveUsageList
        {
            get { return leaveUsageList_; }
            set { leaveUsageList_ = value; RaisePropertyChanged(() => LeaveUsageList); }
        }

        private LeaveApprovalHolder formHelper_;

        public LeaveApprovalHolder FormHelper
        {
            get { return formHelper_; }
            set { formHelper_ = value; OnPropertyChanged(nameof(FormHelper)); }
        }

        private bool showSearchField_;

        public bool ShowSearchField
        {
            get { return showSearchField_; }
            set { showSearchField_ = value; OnPropertyChanged(nameof(ShowSearchField)); }
        }

        private bool displayUsageList_;

        public bool DisplayUsageList
        {
            get { return displayUsageList_; }
            set { displayUsageList_ = value; OnPropertyChanged(nameof(DisplayUsageList)); }
        }

        private LeaveUsageListHolder leaveUsageItem_;

        private readonly ILeaveRequestDataService leaveRequestDataService_;
        private readonly ICommonDataService commonDataService_;

        public LeaveApprovalViewModel(ILeaveRequestDataService leaveRequestDataService,
            ICommonDataService commonDataService)
        {
            leaveRequestDataService_ = leaveRequestDataService;
            commonDataService_ = commonDataService;
        }

        public void Init(INavigation navigation, MyApprovalListModel param)
        {
            NavigationBack = navigation;
            FormHelper = new LeaveApprovalHolder();

            LeaveReaquestUsageCommand = new Command(async () => await LeaveUsage());
            TransactionCommand = new Command(WorkflowTransaction);
            /*ViewFileAttachmentsCommand = new Command(async () => await ViewFileAttachments());*/

            ViewFileAttachmentsCommand = new Command(async () => await NavigationService.PushModalAsync(new LeaveDocumentListPage(FormHelper)));
            ViewLeaveDetailsCommand = new Command(async () => await NavigationService.PushModalAsync(new GeneratedLeaveListPage(FormHelper)));

            CloseCurrentPageCommand = new Command(async () => await NavigationService.PopPageAsync());
            ViewProfileCommand = new Command(async () => await NavigationService.PushPageAsync(new ComingSoonPage("Employee Profile")));
            ViewTransactionHistoryCommand = new Command(async () => await NavigationService.PushModalAsync(new TransactionHistoryPage(FormHelper.TransactionTypeId, FormHelper.TransactionId)));

            InitForm(param);
        }

        public void InitLeaveUsage(INavigation navigation, LeaveApprovalHolder form)
        {
            NavigationBack = navigation;
            FormHelper = form;

            LeaveUsageList = new ObservableCollection<LeaveUsageListHolder>();

            CloseModalCommand = new Command(async () => await NavigationService.PopModalAsync());
            //CloseModalCommand = new Command(async () => await NavigationService.PopPageAsync());
            ViewLeaveUsageItemCommand = new Command(ViewLeaveUsageItem);

            OpenSearchCommand = new Command(() =>
            {
                ShowSearchField = !ShowSearchField;
            });

            InitLeaveUsage();
        }

        public void InitLeaveDocuments(INavigation navigation, LeaveApprovalHolder holder)
        {
            FormHelper = holder;
            NavigationBack = navigation;

            CloseModalCommand = new Command(async () => await NavigationService.PopModalAsync());
            DownloadAttachedFileCommand = new Command(ExecuteDownloadAttachedFileCommand);
        }

        public void InitLeaveDetailsList(INavigation navigation, LeaveApprovalHolder holder)
        {
            FormHelper = holder;
            NavigationBack = navigation;
            CloseModalCommand = new Command(async () => await NavigationService.PopModalAsync());
        }

        private async void InitForm(MyApprovalListModel param)
        {
            try
            {
                if (!IsBusy)
                {
                    IsBusy = true;
                    await Task.Delay(500);

                    FormHelper = await leaveRequestDataService_.InitApprovalForm(param.TransactionTypeId, param.TransactionId);
                }
            }
            catch (Exception ex)
            {
                ShowPage = false;
                Error(false, ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LeaveUsage()
        {
            if (!IsBusy)
            {
                using (Dialogs.Loading())
                {
                    await Task.Delay(500);
                    await NavigationService.PushModalAsync(new LeaveUsageApprovalPage(FormHelper));
                    //await NavigationService.PushPageAsync(new LeaveUsageApprovalPage(FormHelper));
                }
            }
        }

        private async void WorkflowTransaction(object obj)
        {
            try
            {
                if (obj is Models.DataObjects.WorkflowAction item)
                {
                    FormHelper.SelectedWorkflowAction = item;
                    FormHelper = FormHelper;

                    FormHelper = await leaveRequestDataService_.WorkflowTransaction(FormHelper);
                    if (FormHelper.IsSuccess)
                    {
                        Success(true, Messages.ApprovalFormSuccessMessage);
                        await NavigationService.PopToRootAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
        }

        private async void InitLeaveUsage()
        {
            try
            {
                if (!IsBusy)
                {
                    IsBusy = true;
                    await Task.Delay(500);
                    var ProfileId = Convert.ToInt64(FormHelper.LeaveRequestModel.ProfileId);
                    var LeaveTypeId = Convert.ToInt64(FormHelper.LeaveRequestModel.LeaveTypeId);
                    LeaveUsageList = await leaveRequestDataService_.InitLeaveUsage(ProfileId, LeaveTypeId);

                    if (LeaveUsageList.Count > 0)
                        DisplayUsageList = true;
                }
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

        private async void ViewLeaveUsageItem(object obj)
        {
            try
            {
                if (obj != null)
                {
                    var eventArgs = obj as Syncfusion.ListView.XForms.ItemTappedEventArgs;
                    var item = (eventArgs.ItemData as LeaveUsageListHolder);

                    if (leaveUsageItem_ == item)
                    {
                        item.IsVisible = !item.IsVisible;
                        UpdateRequest(item);
                    }
                    else
                    {
                        if (leaveUsageItem_ != null)
                        {
                            leaveUsageItem_.IsVisible = false;
                            UpdateRequest(leaveUsageItem_);
                        }
                        item.IsVisible = true;
                        UpdateRequest(item);
                    }

                    leaveUsageItem_ = item;
                }
            }
            catch (Exception ex)
            {
                await Dialogs.AlertAsync(ex.Message, "", "Close");
            }
        }

        private void UpdateRequest(LeaveUsageListHolder item)
        {
            var index = LeaveUsageList.IndexOf(item);
            LeaveUsageList.Remove(item);
            LeaveUsageList.Insert(index, item);
        }

        /*
        private async Task ViewFileAttachments()
        {
            try
            {
                if (!IsBusy)
                {
                    var param = new FileAttachmentParams()
                    {
                        ModuleFormId = ModuleForms.LeaveRequest,
                        TransactionId = FormHelper.TransactionId
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
        */

        private async void ExecuteDownloadAttachedFileCommand(object obj)
        {
            try
            {
                if (obj is Syncfusion.ListView.XForms.ItemTappedEventArgs data)
                {
                    if (data.ItemData is LeaveRequestDocumentModel item)
                    {
                        using (Dialogs.Loading())
                        {
                            await Task.Delay(500);
                            await commonDataService_.PreviewFileBase64(item.FileBytes, item.FileType, item.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
            }
        }
    }
}