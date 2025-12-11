using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.Payslip;
using EatWork.Mobile.Models.Payslip;
using EatWork.Mobile.Views.Payslips;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.Payslip
{
    public class MyPayslipViewModel : ListViewModel
    {
        public ICommand ViewDetailCommand { get; set; }

        private MyPayslipListHolder holder_;

        public MyPayslipListHolder Holder
        {
            get { return holder_; }
            set { holder_ = value; RaisePropertyChanged(() => Holder); }
        }

        private readonly IPayslipDataService service_;

        public MyPayslipViewModel(IPayslipDataService service)
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

        private void InitForm()
        {
            Holder = new MyPayslipListHolder();

            LoadItemsCommand = new Command<object>(ExecuteLoadItemsCommand, CanLoadMoreItems);
            ViewDetailCommand = new Command(ExecuteViewDetailCommand);

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
                    Holder.MyPayslipList = new ObservableCollection<MyPayslipListModel>();
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
            if (Holder.MyPayslipList.Count >= service_.TotalListItem)
            {
                return false;
            }

            return true;
        }

        private async void ExecuteViewDetailCommand(object obj)
        {
            try
            {
                if (obj != null)
                {
                    var eventArgs = obj as Syncfusion.ListView.XForms.ItemTappedEventArgs;
                    var item = (eventArgs.ItemData as MyPayslipListModel);

                    if (item != null)
                    {
                        using (Dialogs.Loading())
                        {
                            await Task.Delay(500);
                            await NavigationService.PushPageAsync(new MyPayslipDetailPage(item.PaysheetHeaderId, item.ProfileId));
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
                ListCount = Holder.MyPayslipList.Count,
                Count = TotalItems,
                IsAscending = Ascending,
                KeyWord = KeyWord,
                FilterTypes = "",
                Status = "",
                StartDate = "",
                EndDate = "",
            };

            Holder.MyPayslipList = await service_.GetMyPayslipListAsync(Holder.MyPayslipList, obj);

            ShowList = (Holder.MyPayslipList.Count != 0 || !string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count != 0);
            NoItems = (Holder.MyPayslipList.Count == 0 && (!string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count > 0));
        }
    }
}