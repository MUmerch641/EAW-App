using Acr.UserDialogs;
using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.Expense;
using EatWork.Mobile.Models.FormHolder.Expenses;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Views.Expenses;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.Expenses
{
    public class MyExpensesListViewModel : ListViewModel
    {
        public ICommand ViewFileCommand { get; set; }
        public ICommand AddNewItemCommand { get; set; }
        public ICommand MultiSelectModeCommand { get; set; }
        public ICommand RegularSelectModeCommand { get; set; }
        public ICommand SelectAllCommand { get; set; }
        public ICommand SelectedItemCommand { get; set; }
        public ICommand DeleteSelectedItemCommand { get; set; }
        public ICommand CreateExpenseReportCommand { get; set; }
        public ICommand SelectedTypeCommand { get; set; }
        public ICommand FilterCommand { get; set; }

        private MyExpensesListHolder holder_;

        public MyExpensesListHolder Holder
        {
            get { return holder_; }
            set { holder_ = value; RaisePropertyChanged(() => Holder); }
        }

        private bool isChecked_ { get; set; }

        private readonly IExpenseDataService service_;
        private readonly ICommonDataService commonService_;

        public MyExpensesListViewModel()
        {
            service_ = AppContainer.Resolve<IExpenseDataService>();
            commonService_ = AppContainer.Resolve<ICommonDataService>();
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
                ListView.SelectionMode = Syncfusion.ListView.XForms.SelectionMode.Single;
                Holder.IsMultipleMode = false;
                IsBusy = false;
            }
        }

        private async void InitForm()
        {
            Holder = new MyExpensesListHolder();
            LoadItemsCommand = new Command<object>(ExecuteLoadItemsCommand, CanLoadMoreItems);
            ViewItemCommand = new Command<MyExpensesList>(ExecuteViewDetailCommand);
            AddNewItemCommand = new Command(ExecuteAddNewItemCommand);
            SelectedItemCommand = new Command<MyExpensesListDto>(ExecuteSelectedItemCommand);
            DeleteSelectedItemCommand = new Command(ExecuteDeleteSelectedItemCommand);
            CreateExpenseReportCommand = new Command(ExecuteCreateExpenseReportCommand);
            ViewFileCommand = new Command<MyExpensesListDto>(ExecuteViewFileCommand);
            isChecked_ = false;

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

            MultiSelectModeCommand = new Command(() =>
            {
                if (Holder.MyExpenses.Count > 0)
                {
                    Holder.IsMultipleMode = true;
                    ExecuteMultiSelectModeCommand();
                }
            });

            RegularSelectModeCommand = new Command(() =>
            {
                Holder.IsMultipleMode = false;
                isChecked_ = !isChecked_;
                ExecuteRegularSelectModeCommand();
                ExecuteSelectAllCommand();
            });

            ResetFilteredTypeCommand = new Command(() =>
            {
                Ascending = !Ascending;
                SelectedTransactionTypes = new ObservableCollection<SelectableListModel>();
                LoadListItems();
            });

            SelectAllCommand = new Command(ExecuteSelectAllCommand);

            SelectedTypeCommand = new Command<SelectableListModel>(ExecuteSelectedTypeCommand);
            FilterTypeCommand = new Command(() => LoadListItems());

            SelectableListItemSource = await service_.GetExpenseTypes();
        }

        private async void LoadListItems()
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);
                    Holder.MyExpenses = new ObservableCollection<MyExpensesListDto>();

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
            if (Holder.MyExpenses.Count >= service_.TotalListItem)
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

        private async void ExecuteViewDetailCommand(MyExpensesList item)
        {
            try
            {
                if (item != null)
                {
                    using (Dialogs.Loading())
                    {
                        await Task.Delay(500);
                        await NavigationService.PushPageAsync(new MyExpenseDetailPage(item.ExpenseReportDetailId));
                    }
                }
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
        }

        private async void ExecuteAddNewItemCommand()
        {
            using (Dialogs.Loading())
            {
                await Task.Delay(500);
                await NavigationService.PushPageAsync(new NewExpensePage());
            }
        }

        private async void ExecuteSelectAllCommand()
        {
            using (UserDialogs.Instance.Loading())
            {
                await Task.Delay(1);

                isChecked_ = !isChecked_;

                var list = Holder.MyExpenses.ToList();
                foreach (var item in list)
                {
                    item.IsChecked = isChecked_;

                    var exist = (Holder.SelectedDetail.Where(p => p.ExpenseReportDetailId == item.ExpenseReportDetailId)).FirstOrDefault();

                    if (item.IsChecked && exist == null)
                    {
                        Holder.SelectedDetail.Add(item);
                    }

                    if (!item.IsChecked && exist != null)
                    {
                        Holder.SelectedDetail.Remove(item);
                    }

                    var index = Holder.MyExpenses.IndexOf(item);
                    Holder.MyExpenses.RemoveAt(index);
                    Holder.MyExpenses.Insert(index, item);
                }

                Holder.MyExpenses = new ObservableCollection<MyExpensesListDto>(list);
                Holder.SelectedTaskCount = Holder.SelectedDetail.Count;

                Holder.SelectAllText = (Holder.SelectedTaskCount == (Holder.MyExpenses.Count()) ? "Deselect All" : "Select All");
            }
        }

        private async Task RetrieveList()
        {
            var enddate = Holder.EndDate.GetValueOrDefault(Constants.NullDate);

            //if (enddate > Constants.NullDate)
            //    enddate = enddate.AddDays(1);

            var obj = new ListParam()
            {
                ListCount = Holder.MyExpenses.Count,
                Count = TotalItems,
                IsAscending = Ascending,
                KeyWord = KeyWord,
                FilterTypes = (SelectedTransactionTypes.Count == 0 ? "" : string.Join(",", SelectedTransactionTypes.Select(p => p.Id))),
                Status = "",
                StartDate = Holder.StartDate.GetValueOrDefault(Constants.NullDate).ToString(Constants.DateFormatMMDDYYYY),
                EndDate = enddate.ToString(Constants.DateFormatMMDDYYYY),
            };

            Holder.MyExpenses = await service_.GetLisyAsync(Holder.MyExpenses, obj);

            ShowList = (Holder.MyExpenses.Count != 0 || !string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count != 0);
            NoItems = (Holder.MyExpenses.Count == 0 && (!string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count > 0));
            Ascending = Ascending;
        }

        private void ExecuteMultiSelectModeCommand()
        {
            UpdateSelectionTempate();
        }

        private void ExecuteRegularSelectModeCommand()
        {
            Holder.SelectedDetail = new ObservableCollection<MyExpensesListDto>();
            UpdateSelectionTempate();
        }

        private void UpdateSelectionTempate()
        {
            if (Holder.IsMultipleMode)
            {
                ListView.SelectionMode = Syncfusion.ListView.XForms.SelectionMode.Multiple;
                Holder.IsMultipleMode = true;
            }
            else
            {
                ListView.SelectionMode = Syncfusion.ListView.XForms.SelectionMode.Single;
                Holder.IsMultipleMode = false;
            }
        }

        private void ExecuteSelectedItemCommand(MyExpensesListDto item)
        {
            if (item != null)
            {
                var exist = (Holder.SelectedDetail.Where(p => p.ExpenseReportDetailId == item.ExpenseReportDetailId)).FirstOrDefault();

                if (item.IsChecked && exist == null)
                {
                    Holder.SelectedDetail.Add(item);
                }

                if (!item.IsChecked && exist != null)
                {
                    Holder.SelectedDetail.Remove(item);
                }

                Holder.SelectedTaskCount = Holder.SelectedDetail.Count;

                Holder.SelectAllText = (Holder.SelectedTaskCount == (Holder.MyExpenses.Count()) ? "Deselect All" : "Select All");
            }
        }

        private async void ExecuteDeleteSelectedItemCommand()
        {
            try
            {
                if (Holder.SelectedDetail.Count > 0)
                {
                    var count = await service_.DeleteAsync(Holder.SelectedDetail);

                    if (count > 0)
                    {
                        Success(true, $"{count} item/s deleted.");
                        LoadListItems();
                    }
                }
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
        }

        private async void ExecuteCreateExpenseReportCommand()
        {
            if (Holder.SelectedDetail.Count > 0)
            {
                using (Dialogs.Loading())
                {
                    await Task.Delay(500);
                    await NavigationService.PushPageAsync(new MyExpenseReportDetailPage(Holder.SelectedDetail));
                }
            }
        }

        private async void ExecuteViewFileCommand(MyExpensesListDto item)
        {
            if (item != null)
            {
                await commonService_.PreviewFileBase64(item.FileAttachment, item.FileType, item.FileName);
            }
        }

        private void ExecuteSelectedTypeCommand(SelectableListModel obj)
        {
            if (obj != null)
            {
                if (!obj.IsChecked)
                    SelectedTransactionTypes.Remove(obj);
                else
                    SelectedTransactionTypes.Add(obj);
            }
        }
    }
}