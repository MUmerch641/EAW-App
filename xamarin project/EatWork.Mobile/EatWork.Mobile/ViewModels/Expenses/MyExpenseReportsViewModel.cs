using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Views.Expenses;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.Expenses
{
    public class MyExpenseReportsViewModel : ListViewModel
    {
        private ObservableCollection<MyExpenseReportsList> myExpenses_;

        public ObservableCollection<MyExpenseReportsList> MyExpenses
        {
            get { return myExpenses_; }
            set { myExpenses_ = value; RaisePropertyChanged(() => MyExpenses); }
        }

        private readonly IExpenseReportDataService service_;

        public MyExpenseReportsViewModel()
        {
            service_ = AppContainer.Resolve<IExpenseReportDataService>();
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
            MyExpenses = new ObservableCollection<MyExpenseReportsList>();
            LoadItemsCommand = new Command<object>(ExecuteLoadItemsCommand, CanLoadMoreItems);
            ViewItemCommand = new Command(ExecuteViewDetailCommand);

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
                    MyExpenses = new ObservableCollection<MyExpenseReportsList>();
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
            if (MyExpenses.Count >= service_.TotalListItem)
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
                    var item = (eventArgs.ItemData as MyExpenseReportsList);
                    if (item != null)
                    {
                        using (Dialogs.Loading())
                        {
                            await Task.Delay(500);
                            await NavigationService.PushPageAsync(new MyExpenseReportDetailPage(item.ExpenseReportId));
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
                ListCount = MyExpenses.Count,
                Count = TotalItems,
                IsAscending = Ascending,
                KeyWord = KeyWord,
                FilterTypes = "",
                Status = "",
                StartDate = "",
                EndDate = "",
            };

            MyExpenses = await service_.RetrieveMyExpenses(MyExpenses, obj);

            ShowList = (MyExpenses.Count != 0 || !string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count != 0);
            NoItems = (MyExpenses.Count == 0 && (!string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count > 0));
        }
    }
}