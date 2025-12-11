using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.Approvals;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Views.Shared;
using Newtonsoft.Json;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class MyApprovalViewModel : ListViewModel
    {
        public ICommand ShowAllCommand { get; set; }
        public ICommand SelectedStatusCommand { get; set; }

        public ICommand PageAppearingCommand { get; set; }
        public ICommand PageDisappearingCommand { get; set; }

        private MyApprovalHolder holder_;

        public MyApprovalHolder Holder
        {
            get { return holder_; }
            set { holder_ = value; RaisePropertyChanged(() => Holder); }
        }

        private readonly IMyApprovalDataService myApprovalDataService_;
        private readonly IMyTimeLogsDataService myTimeLogsDataService_;

        public MyApprovalViewModel(IMyApprovalDataService myApprovalDataService)
        {
            myApprovalDataService_ = myApprovalDataService;
            myTimeLogsDataService_ = AppContainer.Resolve<IMyTimeLogsDataService>();
        }

        public void Init(INavigation navigation, SfListView listView)
        {
            NavigationBack = navigation;
            ListView = listView;
            Holder = new MyApprovalHolder();
            InitForm();
            LoadListItems();
        }

        public void CheckListUpdate()
        {
            if (FormSession.MyApprovalSelectedItemUpdated)
            {
                var data = JsonConvert.DeserializeObject<MyApprovalListModel>(FormSession.MyApprovalSelectedItem);
                var index = (Holder.MyApprovalList.ToList()).FindIndex(p => p.TransactionId == data.TransactionId && p.TransactionTypeId == data.TransactionTypeId);

                /*REMOVE THE REQUEST #1513*/
                Holder.MyApprovalList.RemoveAt(index);
                Holder.MyApprovalList = new ObservableCollection<MyApprovalListModel>(Holder.MyApprovalList);
                (ListView.LayoutManager as LinearLayout).ScrollToRowIndex(index);

                FormSession.ClearApprovalCached();
            }
        }

        private async void InitForm()
        {
            SelectableListItemSource = new ObservableCollection<SelectableListModel>();

            LoadItemsCommand = new Command<object>(ExecuteLoadItemsCommand, CanLoadMoreItems);
            ViewItemCommand = new Command(ViewItemEvent);
            FilterTypeCommand = new Command(() => LoadListItems());
            PageAppearingCommand = new Command(ExecutePageAppearingCommand);
            PageDisappearingCommand = new Command(ExecutePageDisappearingCommand);

            ResetFilteredTypeCommand = new Command(() =>
            {
                Ascending = !Ascending;
                SelectedTransactionTypes = new ObservableCollection<SelectableListModel>();
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

            ShowAllCommand = new Command(() =>
            {
                Holder.ShowAll = !Holder.ShowAll;

                if (Holder.ShowAll)
                {
                    Holder.Status = string.Concat(RequestStatusValue.ForApproval, ",", RequestStatusValue.Approved, ",", RequestStatusValue.Cancelled, ",", RequestStatusValue.Disapproved);
                    Holder.ShowAllIcon = Constants.CheckCircle;
                }
                else
                {
                    Holder.Status = RequestStatusValue.ForApproval.ToString();
                    Holder.ShowAllIcon = Constants.Circle;
                }

                LoadListItems();
            });

            SelectedStatusCommand = new Command<SelectableListModel>(ExecuteSelectedStatusCommand);

            foreach (var item in RequestType.RequetTypeList.Where(p => p.IsVisible == 1))
                SelectableListItemSource.Add(new SelectableListModel() { Id = item.RequestTypeId, DisplayText = item.Title, IsChecked = false });

            Holder.StatusList = await myTimeLogsDataService_.RetrieveStatus();
        }

        private async void LoadListItems()
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);
                    Holder.MyApprovalList = new ObservableCollection<MyApprovalListModel>();

                    ListView = await myApprovalDataService_.InitListView(ListView);

                    await RetrieveList();
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

        private async void ViewItemEvent(object obj)
        {
            try
            {
                if (obj != null)
                {
                    var eventArgs = obj as Syncfusion.ListView.XForms.ItemTappedEventArgs;
                    var item = (eventArgs.ItemData as MyApprovalListModel);
                    if (item != null)
                    {
                        using (Dialogs.Loading())
                        {
                            await Task.Delay(500);
                            var response = await RequestType.GetPageByRequestType(item.TransactionTypeId);
                            Page page;
                            if (response.ApprovalPage != typeof(ComingSoonPage))
                                page = (Page)Activator.CreateInstance(response.ApprovalPage, item);
                            else
                            {
                                page = (Page)Activator.CreateInstance(response.ApprovalPage);
                                page.Title = response.Title;
                            }

                            await NavigationService.PushPageAsync(page);
                        }

                        FormSession.MyApprovalSelectedItem = JsonConvert.SerializeObject(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
        }

        private bool CanLoadMoreItems(object obj)
        {
            if (Holder.MyApprovalList.Count >= myApprovalDataService_.TotalListItem)
                return false;

            return true;
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
            var endDate = Holder.EndDate.GetValueOrDefault(Constants.NullDate);

            if (endDate > Constants.NullDate)
                endDate = endDate.AddDays(1);

            var obj = new ListParam()
            {
                ListCount = Holder.MyApprovalList.Count,
                Count = TotalItems,
                IsAscending = Ascending,
                KeyWord = KeyWord,
                FilterTypes = (SelectedTransactionTypes.Count == 0 ? "" : string.Join(",", SelectedTransactionTypes.Select(p => p.Id))),
                Status = (Holder.SelectedStatus.Count == 0 ? RequestStatusValue.ForApproval.ToString() : string.Join(",", Holder.SelectedStatus.Select(p => p.Id))),
                StartDate = Holder.StartDate.GetValueOrDefault(Constants.NullDate).ToString(Constants.DateFormatMMDDYYYY),
                EndDate = endDate.ToString(Constants.DateFormatMMDDYYYY),
            };

            Holder.MyApprovalList = await myApprovalDataService_.RetrieveApprovalList(Holder.MyApprovalList, obj);

            ShowList = (Holder.MyApprovalList.Count != 0 || !string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count != 0);
            NoItems = (Holder.MyApprovalList.Count == 0 && (!string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count > 0));
            Ascending = Ascending;
        }

        private void ExecutePageAppearingCommand()
        {
            CheckListUpdate();
        }

        private void ExecutePageDisappearingCommand()
        {
        }
    }
}