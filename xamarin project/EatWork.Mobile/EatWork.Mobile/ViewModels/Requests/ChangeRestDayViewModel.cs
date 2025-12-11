using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Excemptions;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.Request;
using EatWork.Mobile.Views;
using EatWork.Mobile.Views.Requests;
using EatWork.Mobile.Views.Requests.ChangeRestDay;
using EatWork.Mobile.Views.Shared;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class ChangeRestDayViewModel : ListViewModel
    {
        #region commands

        public ICommand SubmitCommand { get; set; }
        public ICommand CameraCommand { get; set; }
        public ICommand SelectFileCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand SelectEmployeeCommand { get; set; }
        public ICommand GetScheduleCommand { get; set; }
        public ICommand SelectedEmployeeCommand { get; set; }
        public ICommand CloseModalCommand { get; set; }
        public ICommand RemoveFileAttachedCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand ViewTransactionHistoryCommand { get; set; }
        public ICommand ViewFileAttachmentCommand { get; set; }
        public ICommand FileChooseOptionCommand { get; set; }
        public ICommand PageAppearingCommand { get; set; }
        public ICommand PageDisappearingCommand { get; set; }

        #endregion commands

        #region properties

        private ChangeRestdayHolder formHelper_;

        public ChangeRestdayHolder Holder
        {
            get { return formHelper_; }
            set { formHelper_ = value; RaisePropertyChanged(() => Holder); }
        }

        private ObservableCollection<EmployeeListModel> employeeList_;

        public ObservableCollection<EmployeeListModel> EmployeeList
        {
            get { return employeeList_; }
            set { employeeList_ = value; RaisePropertyChanged(() => EmployeeList); }
        }

        private ObservableCollection<ChangeRestday> restdayList_;

        public ObservableCollection<ChangeRestday> RestdayList
        {
            get { return restdayList_; }
            set { restdayList_ = value; RaisePropertyChanged(() => RestdayList); }
        }

        #endregion properties

        private readonly IChangeRestdayScheduleDataService changeRestdayScheduleDataService_;
        private readonly ICommonDataService myRequestCommonDataService_;
        private readonly IEmployeeDataService employeeListDataService_;
        private readonly IDialogService dialogService_;

        public ChangeRestDayViewModel(IChangeRestdayScheduleDataService changeRestdayScheduleDataService,
            ICommonDataService myRequestCommonDataService,
            IEmployeeDataService employeeListDataService)
        {
            changeRestdayScheduleDataService_ = changeRestdayScheduleDataService;
            myRequestCommonDataService_ = myRequestCommonDataService;
            employeeListDataService_ = employeeListDataService;
            dialogService_ = AppContainer.Resolve<IDialogService>();
        }

        public void Init(INavigation navigation, long recordId, DateTime? selectedDate)
        {
            NavigationBack = navigation;
            Holder = new ChangeRestdayHolder();
            IsBusy = false;
            //RestdayList = new ObservableCollection<ChangeRestday>();

            //==commands
            SubmitCommand = new Command(async () => await SubmitRequest());
            CameraCommand = new Command(async () => await TakePhoto());
            SelectFileCommand = new Command(async () => await SelectFile());
            CloseCommand = new Command(async () => await NavigationService.PopPageAsync());
            SelectEmployeeCommand = new Command(async () => await MoreEmployeeList());
            GetScheduleCommand = new Command<string>(GetEmployeeScheduleEvent);
            RemoveFileAttachedCommand = new Command(() => Holder.FileData = new Plugin.FilePicker.Abstractions.FileData());
            CancelCommand = new Command(async () => await CancelRequest());
            ViewTransactionHistoryCommand = new Command(async () => await NavigationService.PushModalAsync(new TransactionHistoryPage(TransactionType.ChangeRestDay, Holder.ChangeRestdayModel.ChangeRestDayId)));
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
                    Holder = await changeRestdayScheduleDataService_.InitForm(recordId, selectedDate);
                }
                catch (Exception ex)
                {
                    //await Dialogs.AlertAsync(ex.Message);
                    Error(false, ex.Message);
                }
                finally
                {
                    IsBusy = false;

                    GetEmployeeScheduleEvent(null);
                }
            }
        }

        private async Task SubmitRequest()
        {
            try
            {
                Holder = await changeRestdayScheduleDataService_.SubmitRequest(Holder);

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
                var response = await myRequestCommonDataService_.TakePhotoAsync("RESTDAY");
                /*var file = await myRequestCommonDataService_.AttachPhoto2(response, "RESTDAY");*/

                if (response != null && !string.IsNullOrWhiteSpace(response.FileName))
                {
                    /*Holder.FileData = file;*/
                    Holder.FileAttachments.Add(response);
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

        private async Task MoreEmployeeList()
        {
            //await NavigationService.PushPageAsync(new EmployeeListPage(Holder));
            /*await NavigationService.PushModalAsync(new EmployeeListPage(Holder));*/
            await NavigationService.PushModalAsync(new EmployeeListPage(Holder));
        }

        private async void GetEmployeeScheduleEvent(string obj)
        {
            try
            {
                Holder = await changeRestdayScheduleDataService_.GetEmployeeSchedule(Holder);
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
        }

        private async Task CancelRequest()
        {
            try
            {
                Holder.ActionTypeId = ActionTypeId.Cancel;
                Holder.Msg = Messages.Cancel;
                Holder = await changeRestdayScheduleDataService_.WorkflowTransactionRequest(Holder);

                if (Holder.Success)
                {
                    Success(true, Messages.ApprovalFormSuccessMessage);
                    await NavigationService.PopToRootAsync();
                }
            }
            catch (Exception ex)
            {
                //await Dialogs.AlertAsync(ex.Message);
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
                        ModuleFormId = ModuleForms.ChangeRestDaySchedule_Schedule,
                        TransactionId = Holder.ChangeRestdayModel.ChangeRestDayId
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

            MessagingCenter.Subscribe<ChangeRestdayPage>(this, "onback", (sender) =>
            {
                BackItemPage();
            });
        }

        private void ExecutePageDisappearingCommand()
        {
            MessagingCenter.Unsubscribe<FileOptionViewModel, long>(this, "FileOptionSelectedValue");
            MessagingCenter.Unsubscribe<ChangeRestdayPage>(this, "onback");
        }

        #region employee list

        public async void InitList(SfListView sflist, ChangeRestdayHolder form, INavigation navigation)
        {
            ListView = sflist;
            Holder = form;
            NavigationBack = navigation;
            Ascending = true;
            KeyWord = string.Empty;

            SelectedEmployeeCommand = new Command<EmployeeListModel>(SelectedEmployee);
            //CloseModalCommand = new Command(async () => await NavigationService.PopPageAsync(true));
            CloseModalCommand = new Command(async () => await NavigationService.PopModalAsync());
            LoadItemsCommand = new Command<object>(ExecuteLoadItemsCommand);

            SortCommand = new Command(() =>
            {
                Ascending = !Ascending;
                InitEmployeList();
            });

            SearchCommand = new Command(() =>
            {
                InitEmployeList();
                Keyboard.Dismiss();
            });

            ResetSearchCommand = new Command(() =>
            {
                InitEmployeList();
                KeyWord = string.Empty;
                Keyboard.Dismiss();
            });

            OpenSearchCommand = new Command(() =>
            {
                ShowSearchField = !ShowSearchField;
                if (!ShowSearchField)
                    Keyboard.Dismiss();
            });

            InitEmployeList();
        }

        private async void InitEmployeList()
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);

                    EmployeeList = new ObservableCollection<EmployeeListModel>();

                    var obj = new ListParam()
                    {
                        ListCount = EmployeeList.Count,
                        Count = TotalItems,
                        IsAscending = Ascending,
                        KeyWord = KeyWord,
                    };

                    var employeeList = employeeListDataService_.RetrieveEmployeeList(EmployeeList, obj);
                    var sflist = employeeListDataService_.InitListView(ListView);

                    await Task.WhenAll(employeeList, sflist);
                    EmployeeList = employeeList.Result;
                    ListView = sflist.Result;

                    ShowList = (EmployeeList.Count == 0 && (string.IsNullOrWhiteSpace(KeyWord) && SelectedTransactionTypes.Count == 0) ? false : true);
                    NoItems = (EmployeeList.Count == 0 && (!string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count > 0) ? true : false);
                }
                catch (Exception ex)
                {
                    //await Dialogs.AlertAsync(ex.Message, "", "Close");
                    Error(false, ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async void SelectedEmployee(EmployeeListModel item)
        {
            try
            {
                Holder.SwapWith = item.EmployeeName;
                Holder.ChangeRestdayModel.SwapWithProfileId = item.ProfileId;
                Holder = Holder;
                //await NavigationService.PopPageAsync(true);
                await NavigationService.PopModalAsync();
            }
            catch (Exception ex)
            {
                await Dialogs.AlertAsync(ex.Message, "", "Close");
            }
            finally
            {
                GetEmployeeScheduleEvent(null);
            }
        }

        private async void ExecuteLoadItemsCommand(object obj)
        {
            var listview = obj as SfListView;
            if (!listview.IsBusy)
            {
                try
                {
                    listview.IsBusy = true;
                    await Task.Delay(500);

                    var param = new ListParam()
                    {
                        ListCount = EmployeeList.Count,
                        Count = TotalItems,
                        IsAscending = Ascending,
                        KeyWord = KeyWord,
                        FilterTypes = string.Empty
                    };

                    EmployeeList = await employeeListDataService_.RetrieveEmployeeList(EmployeeList, param);

                    ShowList = (EmployeeList.Count == 0 && (string.IsNullOrWhiteSpace(KeyWord) && SelectedTransactionTypes.Count == 0) ? false : true);
                    NoItems = (EmployeeList.Count == 0 && (!string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count > 0) ? true : false);
                }
                catch (Exception ex)
                {
                    await Dialogs.AlertAsync(ex.Message, "", "Close");
                }
                finally
                {
                    listview.IsBusy = false;
                }
            }
        }

        #endregion employee list

        protected override async void BackItemPage()
        {
            if (Holder.ChangeRestdayModel.ChangeRestDayId == 0)
            {
                if (Holder.ChangeRestdayModel.SwapWithProfileId > 0 ||
                    !string.IsNullOrWhiteSpace(Holder.ChangeRestdayModel.Reason))
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