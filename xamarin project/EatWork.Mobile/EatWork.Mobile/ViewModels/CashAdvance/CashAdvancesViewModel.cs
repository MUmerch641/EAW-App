using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.Accountability;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Views.CashAdvance;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.CashAdvance
{
    public class CashAdvancesViewModel : ListViewModel
    {
        public ICommand AddNewItemCommand { get; set; }

        private ObservableCollection<CashAdvanceRequestList> cashAdvances_;

        public ObservableCollection<CashAdvanceRequestList> CashAdvances
        {
            get { return cashAdvances_; }
            set { cashAdvances_ = value; RaisePropertyChanged(() => CashAdvances); }
        }

        private readonly ICashAdvanceRequestDataService service_;

        public CashAdvancesViewModel()
        {
            service_ = AppContainer.Resolve<ICashAdvanceRequestDataService>();
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
            CashAdvances = new ObservableCollection<CashAdvanceRequestList>();
            LoadItemsCommand = new Command<object>(ExecuteLoadItemsCommand, CanLoadMoreItems);
            ViewItemCommand = new Command(ExecuteViewDetailCommand);
            AddNewItemCommand = new Command(ExecuteAddNewItemCommand);

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
        }

        private async void LoadListItems()
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);
                    CashAdvances = new ObservableCollection<CashAdvanceRequestList>();
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

        private bool CanLoadMoreItems(object obj)
        {
            if (CashAdvances.Count >= service_.TotalListItem)
            {
                return false;
            }

            return true;
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

        private async void ExecuteViewDetailCommand(object obj)
        {
            try
            {
                if (obj != null)
                {
                    var eventArgs = obj as Syncfusion.ListView.XForms.ItemTappedEventArgs;
                    var item = (eventArgs.ItemData as CashAdvanceRequestList);
                    if (item != null)
                    {
                        using (Dialogs.Loading())
                        {
                            await Task.Delay(500);
                            await NavigationService.PushPageAsync(new CashAdvanceRequestPage(item.CashAdvanceId));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
        }

        private async Task RetrieveList()
        {
            var obj = new ListParam()
            {
                ListCount = CashAdvances.Count,
                Count = TotalItems,
                IsAscending = Ascending,
                KeyWord = KeyWord,
                FilterTypes = "",
                Status = "",
                StartDate = "",
                EndDate = "",
            };

            CashAdvances = await service_.RetrieveList(CashAdvances, obj);

            ShowList = (CashAdvances.Count != 0 || !string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count != 0);
            NoItems = (CashAdvances.Count == 0 && (!string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count > 0));
        }

        private async void ExecuteAddNewItemCommand()
        {
            using (Dialogs.Loading())
            {
                await Task.Delay(500);
                await NavigationService.PushPageAsync(new CashAdvanceRequestPage(0));
            }
        }
    }
}