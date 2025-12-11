using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Views.Shared;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class IndividualAttendanceViewModel : ListViewModel
    {
        public ICommand ToggleDetailCommand { get; set; }
        public ICommand CreateRequestCommand { get; set; }
        public ICommand DisplayRequestOptionCommand { get; set; }
        public ICommand HideRequestOptionCommand { get; set; }

        private IndividualAttendance TappedItem;

        private IndividualAttendanceHolder holder_;

        public IndividualAttendanceHolder Holder
        {
            get { return holder_; }
            set { holder_ = value; RaisePropertyChanged(() => Holder); }
        }

        private readonly IMyAttendanceDataService service_;

        public IndividualAttendanceViewModel(IMyAttendanceDataService service)
        {
            service_ = service;
        }

        public void Init(INavigation navigation, SfListView listView)
        {
            NavigationBack = navigation;
            ListView = listView;

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
            Holder = new IndividualAttendanceHolder();
            FilterTypeCommand = new Command(() => LoadListItems());
            LoadItemsCommand = new Command<object>(ExecuteLoadItemsCommand, CanLoadMoreItems);

            ToggleDetailCommand = new Command<object>(ExecuteToggleDetailCommand2);
            CreateRequestCommand = new Command(CreateRequestEvent);
            DisplayRequestOptionCommand = new Command<DateTime>(SelectedDateEvent);
            HideRequestOptionCommand = new Command(() => Holder.DisplayRequestNavigator = false);

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
                    Holder.MyAttendanceList = new ObservableCollection<IndividualAttendance>();

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

        private async Task RetrieveList()
        {
            var enddate = Holder.EndDate.GetValueOrDefault(Constants.NullDate);

            if (enddate > Constants.NullDate)
                enddate = enddate.AddDays(1);

            var obj = new ListParam()
            {
                ListCount = Holder.MyAttendanceList.Count,
                Count = TotalItems,
                IsAscending = Ascending,
                KeyWord = KeyWord,
                FilterTypes = "",
                Status = "",
                StartDate = Holder.StartDate.GetValueOrDefault(Constants.NullDate).ToString(Constants.DateFormatMMDDYYYY),
                EndDate = enddate.ToString(Constants.DateFormatMMDDYYYY),
            };

            Holder.MyAttendanceList = await service_.GetIndividualAttendanceAsync(Holder.MyAttendanceList, obj);

            ShowList = (Holder.MyAttendanceList.Count != 0 || !string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count != 0);
            NoItems = (Holder.MyAttendanceList.Count == 0 && (!string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count > 0));
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
            if (Holder.MyAttendanceList.Count >= service_.TotalListItem)
            {
                return false;
            }

            return true;
        }

        private void SelectedDateEvent(DateTime date)
        {
            if (date != null)
            {
                Holder.DisplayRequestNavigator = true;
                FormSession.MyScheduleSelectedDate = date.ToString(Constants.DateFormatMMDDYYYY);
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

                            Holder.DisplayRequestNavigator = false;
                            var item = new MyRequestListModel();
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
        }

        private void ExecuteToggleDetailCommand(object obj)
        {
            var tappedItemData = obj as IndividualAttendance;

            if (tappedItemData != null)
            {
                if (TappedItem == obj)
                {
                    var test = !tappedItemData.IsVisible;
                    tappedItemData.IsVisible = test;
                    UpdateRequest(tappedItemData);
                }
                else
                {
                    if (TappedItem != null)
                    {
                        TappedItem.IsVisible = false;
                        UpdateRequest(TappedItem);
                    }
                    tappedItemData.IsVisible = true;
                    UpdateRequest(tappedItemData);
                }

                TappedItem = tappedItemData;
            }
        }

        private void UpdateRequest(IndividualAttendance item)
        {
            var index = Holder.MyAttendanceList.IndexOf(item);
            Holder.MyAttendanceList.Remove(item);
            Holder.MyAttendanceList.Insert(index, item);

            Holder = Holder;
        }

        private void ExecuteToggleDetailCommand2(object obj)
        {
            var tappedItemData = obj as IndividualAttendance;

            if (tappedItemData != null)
            {
                if (TappedItem != null && TappedItem.IsVisible)
                {
                    TappedItem.IsVisible = false;
                }

                if (TappedItem == tappedItemData)
                {
                    TappedItem = null;
                    return;
                }

                TappedItem = tappedItemData;
                TappedItem.IsVisible = true;
            }
        }
    }
}