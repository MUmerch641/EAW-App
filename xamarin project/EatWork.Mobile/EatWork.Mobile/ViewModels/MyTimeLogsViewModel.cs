using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Views.Requests;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class MyTimeLogsViewModel : ListViewModel
    {
        public ICommand AddNewCommand { get; set; }
        public ICommand EditItemCommand { get; set; }
        public ICommand FilterCommand { get; set; }
        public ICommand SelectedStatusCommand { get; set; }

        private MyTimeLogsListHolder _holder;

        public MyTimeLogsListHolder Holder
        {
            get { return _holder; }
            set { _holder = value; RaisePropertyChanged(() => Holder); }
        }

        private readonly IMyTimeLogsDataService myTimeLogsDataService_;

        public MyTimeLogsViewModel(IMyTimeLogsDataService myTimeLogsDataService)
        {
            myTimeLogsDataService_ = myTimeLogsDataService;
        }

        public void Init(INavigation navigation, SfListView listView)
        {
            NavigationBack = navigation;
            ListView = listView;
            Holder = new MyTimeLogsListHolder();

            InitForm();
            RetrieveStatusFilter();
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
            AddNewCommand = new Command(async () => await ExecuteAddNewCommand());
            LoadItemsCommand = new Command<object>(ExecuteLoadItemsCommand, CanLoadMoreItems);
            ViewItemCommand = new Command(ExecuteViewItemCommand);
            EditItemCommand = new Command<long>(ExecuteEditItemCommand);
            FilterTypeCommand = new Command(() => LoadListItems());
            SelectedStatusCommand = new Command<SelectableListModel>(ExecuteSelectedStatusCommand);

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

            SortCommand = new Command(() =>
            {
                Ascending = !Ascending;
                LoadListItems();
            });

            ResetFilteredTypeCommand = new Command(() =>
            {
                Holder.StartDate = null;
                Holder.EndDate = null;
                Holder.SelectedStatus = new ObservableCollection<SelectableListModel>();
                RetrieveStatusFilter();
                LoadListItems();
            });
        }

        private async void LoadListItems()
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);
                    Holder.MyTimeLogsList = new ObservableCollection<MyTimeLogsListModel>();

                    await RetrieveList();
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
                        ListCount = Holder.MyTimeLogsList.Count,
                        Count = TotalItems,
                        IsAscending = Ascending,
                        KeyWord = KeyWord,
                        FilterTypes = "",
                        StartDate = Holder.StartDate.GetValueOrDefault().ToString(Constants.DateFormatMMDDYYYY),
                        EndDate = Holder.EndDate.GetValueOrDefault().AddDays(1).ToString(Constants.DateFormatMMDDYYYY),
                    };

                    Holder.MyTimeLogsList = await myTimeLogsDataService_.RetrieveMyRequestList(Holder.MyTimeLogsList, param);

                    ShowList = (Holder.MyTimeLogsList.Count != 0 || !string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count != 0);
                    NoItems = (Holder.MyTimeLogsList.Count == 0 && (!string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count > 0));

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

        private async void RetrieveStatusFilter()
        {
            Holder.Status = new ObservableCollection<SelectableListModel>();
            Holder.Status = await myTimeLogsDataService_.RetrieveStatus();
        }

        private bool CanLoadMoreItems(object obj)
        {
            if (Holder.MyTimeLogsList.Count >= myTimeLogsDataService_.TotalListItem)
            {
                return false;
            }

            return true;
        }

        private async Task ExecuteAddNewCommand()
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);
                    var param = new MyRequestListModel() { TransactionId = 0 };
                    await NavigationService.PushPageAsync(new TimeEntryRequestPage(param));
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async void ExecuteViewItemCommand(object obj)
        {
            if (!IsBusy)
            {
                try
                {
                    if (obj != null)
                    {
                        var eventArgs = obj as Syncfusion.ListView.XForms.ItemTappedEventArgs;
                        var item = (eventArgs.ItemData as MyTimeLogsListModel);

                        if (item != null)
                        {
                            var param = new MyRequestListModel() { TransactionId = item.TimeEntryLogId };

                            IsBusy = true;
                            await Task.Delay(100);
                            await NavigationService.PushPageAsync(new TimeEntryRequestPage(param));
                        }
                    }
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

        private async void ExecuteEditItemCommand(long id)
        {
            if (!IsBusy)
            {
                try
                {
                    if (id > 0)
                    {
                        var param = new MyRequestListModel() { TransactionId = id };
                        FormSession.IsEditTimeLogs = true;

                        IsBusy = true;
                        await Task.Delay(100);
                        await NavigationService.PushPageAsync(new TimeEntryRequestPage(param));
                    }
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

        private void ExecuteSelectedStatusCommand(SelectableListModel obj)
        {
            if (obj != null)
            {
                if (!obj.IsChecked)
                    Holder.SelectedStatus.Remove(obj);
                else
                    Holder.SelectedStatus.Add(obj);
            }
        }

        private async Task RetrieveList()
        {
            var enddate = Holder.EndDate.GetValueOrDefault(Constants.NullDate);

            if (enddate > Constants.NullDate)
                enddate = enddate.AddDays(1);

            var obj = new ListParam()
            {
                ListCount = Holder.MyTimeLogsList.Count,
                Count = TotalItems,
                IsAscending = Ascending,
                KeyWord = KeyWord,
                FilterTypes = "",
                Status = (Holder.SelectedStatus.Count == 0 ? "" : string.Join(",", Holder.SelectedStatus.Select(p => p.Id))),
                StartDate = Holder.StartDate.GetValueOrDefault(Constants.NullDate).ToString(Constants.DateFormatMMDDYYYY),
                EndDate = enddate.ToString(Constants.DateFormatMMDDYYYY),
            };

            Holder.MyTimeLogsList = await myTimeLogsDataService_.RetrieveMyRequestList(Holder.MyTimeLogsList, obj);

            ShowList = (Holder.MyTimeLogsList.Count != 0 || !string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count != 0);
            NoItems = (Holder.MyTimeLogsList.Count == 0 && (!string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count > 0));
            Ascending = Ascending;
        }
    }
}