using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Utils;
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
    public class MyScheduleViewModel : ListViewModel
    {
        public ICommand CreateRequestCommand { get; set; }
        public ICommand DisplayRequestOptionCommand { get; set; }
        public ICommand HideRequestOptionCommand { get; set; }

        private RequestType requestType_;

        private ObservableCollection<SelectableListModel> requestTypes_;

        public ObservableCollection<SelectableListModel> RequestTypes
        {
            get { return requestTypes_; }
            set { requestTypes_ = value; RaisePropertyChanged(() => RequestTypes); }
        }

        private ObservableCollection<MyScheduleListModel> mySchedule_;

        public ObservableCollection<MyScheduleListModel> MySchedule
        {
            get { return mySchedule_; }
            set { mySchedule_ = value; RaisePropertyChanged(() => MySchedule); }
        }

        private MyScheduleListModel currentSchedule_;

        public MyScheduleListModel CurrentSchedule
        {
            get { return currentSchedule_; }
            set { currentSchedule_ = value; RaisePropertyChanged(() => CurrentSchedule); }
        }

        private DateTime currentDate_;

        public DateTime CurrentDate
        {
            get { return currentDate_; }
            set { currentDate_ = value; RaisePropertyChanged(() => CurrentDate); }
        }

        private DateTime startDate_;

        public DateTime StartDate
        {
            get { return startDate_; }
            set { startDate_ = value; RaisePropertyChanged(() => StartDate); }
        }

        private DateTime endDate_;

        public DateTime EndDate
        {
            get { return endDate_; }
            set { endDate_ = value; RaisePropertyChanged(() => EndDate); }
        }

        private bool displayRequestNavigator_;

        public bool DisplayRequestNavigator
        {
            get { return displayRequestNavigator_; }
            set { displayRequestNavigator_ = value; RaisePropertyChanged(() => DisplayRequestNavigator); }
        }

        private DateTime? selectedDate_;

        private readonly IMyScheduleDataService myScheduleDataService_;

        public MyScheduleViewModel(IMyScheduleDataService myScheduleDataService)
        {
            myScheduleDataService_ = myScheduleDataService;
        }

        public void Init(INavigation navigation, SfListView listview)
        {
            NavigationBack = navigation;
            ListView = listview;

            InitForm();
            LoadListItems();
        }

        public void CheckListUpdate()
        {
            if (FormSession.IsSubmitted)
            {
                LoadListItems();
                FormSession.IsSubmitted = false;
                IsBusy = false;
            }
        }

        private void InitForm()
        {
            SelectableListItemSource = new ObservableCollection<SelectableListModel>();

            Ascending = true;
            CurrentDate = DateTime.Now.Date;
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            EndDate = StartDate.AddMonths(1).AddDays(-1);
            CurrentSchedule = new MyScheduleListModel();
            DisplayRequestNavigator = false;
            requestType_ = new RequestType();
            RequestTypes = new ObservableCollection<SelectableListModel>();

            LoadItemsCommand = new Command<object>(ExecuteLoadItemsCommand, CanLoadMoreItems);

            FilterTypeCommand = new Command(() => LoadListItems());

            SortCommand = new Command(() =>
            {
                Ascending = !Ascending;
                LoadListItems();
            });

            SearchCommand = new Command(() =>
            {
                LoadListItems();
                Keyboard.Dismiss();
            });

            ResetSearchCommand = new Command(() =>
            {
                LoadListItems();
                KeyWord = string.Empty;
                Keyboard.Dismiss();
            });

            CreateRequestCommand = new Command(CreateRequestEvent);

            DisplayRequestOptionCommand = new Command<DateTime>(SelectedDateEvent);
            HideRequestOptionCommand = new Command(() => { DisplayRequestNavigator = false; });

            foreach (var item in requestType_.RequetTypeList.Where(p => p.IsVisible == 1))
                RequestTypes.Add(new SelectableListModel() { Id = item.RequestTypeId, DisplayText = item.Title, IsChecked = false });
        }

        private async void LoadListItems()
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);
                    MySchedule = new ObservableCollection<MyScheduleListModel>();
                    //ListView = await myScheduleDataService_.InitListView(ListView);
                    await RetrieveList();
                    await GetCurrentWorkSchedule();
                }
                catch (Exception ex)
                {
                    Error(content: ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async Task GetCurrentWorkSchedule()
        {
            var obj = new ListParam()
            {
                StartDate = CurrentDate.ToString(Constants.DateFormatMMDDYYYY),
                EndDate = CurrentDate.ToString(Constants.DateFormatMMDDYYYY),
            };

            var current = await myScheduleDataService_.RetrieveCurrentSchedule(obj);

            if (current != null)
            {
                CurrentSchedule = current;
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
                    await Task.Delay(2500);

                    await RetrieveList();

                    /*
                    var param = new ListParam()
                    {
                        ListCount = MySchedule.Count,
                        Count = TotalItems,
                        IsAscending = Ascending,
                        KeyWord = KeyWord,
                        FilterTypes = "",
                        StartDate = StartDate.ToString(Constants.DateFormatMMDDYYYY),
                        EndDate = EndDate.AddDays(1).ToString(Constants.DateFormatMMDDYYYY),
                    };

                    MySchedule = await myScheduleDataService_.RetrieveMyScheduleList(MySchedule, param);

                    ShowList = (MySchedule.Count != 0 || !string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count != 0);
                    NoItems = (MySchedule.Count == 0 && (!string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count > 0));
                    */
                }
                catch (Exception ex)
                {
                    Error(content: ex.Message);
                }
                finally
                {
                    listview.IsBusy = false;
                }
            }
        }

        private bool CanLoadMoreItems(object obj)
        {
            if (MySchedule.Count >= myScheduleDataService_.TotalListItem)
            {
                return false;
            }

            return true;
        }

        public void SelectedDateEvent(DateTime date)
        {
            if (date != null)
            {
                DisplayRequestNavigator = true;
                selectedDate_ = date.Date;
            }
        }

        private async void CreateRequestEvent(object obj)
        {
            try
            {
                if (obj != null)
                {
                    var eventArgs = obj as Syncfusion.ListView.XForms.ItemTappedEventArgs;
                    var item2 = (eventArgs.ItemData as SelectableListModel);
                    if (item2 != null)
                    {
                        using (Dialogs.Loading())
                        {
                            await Task.Delay(500);

                            DisplayRequestNavigator = false;
                            var item = new MyRequestListModel() { TransactionId = 0, SelectedDate = selectedDate_ };
                            FormSession.IsMySchedule = true;

                            var response = await RequestType.GetPageByRequestType(item2.Id);
                            Page page;
                            if (response.RequestPage != typeof(ComingSoonPage))
                                page = (Page)Activator.CreateInstance(response.RequestPage, item);
                            else
                            {
                                page = (Page)Activator.CreateInstance(response.RequestPage);
                                page.Title = response.Title;
                            }

                            await NavigationService.PushPageAsync(page);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
            finally
            {
            }
        }

        private async Task RetrieveList()
        {
            var obj = new ListParam()
            {
                ListCount = MySchedule.Count,
                Count = TotalItems,
                IsAscending = Ascending,
                KeyWord = KeyWord,
                FilterTypes = "",
                StartDate = StartDate.ToString(Constants.DateFormatMMDDYYYY),
                EndDate = EndDate.AddDays(1).ToString(Constants.DateFormatMMDDYYYY),
            };

            MySchedule = await myScheduleDataService_.RetrieveMyScheduleList(MySchedule, obj);

            ShowList = (MySchedule.Count != 0 || !string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count != 0);
            NoItems = (MySchedule.Count == 0 && (!string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count > 0));
        }
    }
}