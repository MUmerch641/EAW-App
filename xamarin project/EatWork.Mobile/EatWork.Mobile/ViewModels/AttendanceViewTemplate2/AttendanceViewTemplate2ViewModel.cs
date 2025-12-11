using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.Attendance;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Views.AttendanceViewTemplate2;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.AttendanceViewTemplate2
{
    public class AttendanceViewTemplate2ViewModel : ListViewModel
    {
        private ObservableCollection<DetailedAttendanceListModel> listSource_;

        public ObservableCollection<DetailedAttendanceListModel> ListSource
        {
            get { return listSource_; }
            set { listSource_ = value; RaisePropertyChanged(() => ListSource); }
        }

        private readonly IAttendanceViewTemplate2DataService service_;

        public AttendanceViewTemplate2ViewModel()
        {
            service_ = AppContainer.Resolve<IAttendanceViewTemplate2DataService>();
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
            ListSource = new ObservableCollection<DetailedAttendanceListModel>();

            LoadItemsCommand = new Command<object>(ExecuteLoadItemsCommand, CanLoadMoreItems);
            ViewItemCommand = new Command<object>(ExecuteViewDetailCommand);

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

            ResetFilteredTypeCommand = new Command(() =>
            {
                Ascending = !Ascending;
                SelectedTransactionTypes = new ObservableCollection<SelectableListModel>();
                LoadListItems();
            });

            FilterTypeCommand = new Command(() => LoadListItems());
        }

        private async void LoadListItems()
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);
                    ListSource = new ObservableCollection<DetailedAttendanceListModel>();

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
                    await Task.Delay(500);

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
            if (ListSource.Count >= service_.TotalListItem)
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
                    var item = (eventArgs.ItemData as DetailedAttendanceListModel);
                    if (item != null)
                    {
                        using (Dialogs.Loading())
                        {
                            await Task.Delay(500);
                            await NavigationService.PushPageAsync(new AttendanceTemplate2DetailPage(item));
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
            var endDate = Constants.NullDate;//Holder.EndDate.GetValueOrDefault(Constants.NullDate);
            var startDate = Constants.NullDate;//Holder.EndDate.GetValueOrDefault(Constants.NullDate);

            if (endDate > Constants.NullDate)
                endDate = endDate.AddDays(1);

            var obj = new ListParam()
            {
                ListCount = ListSource.Count,
                Count = TotalItems,
                IsAscending = Ascending,
                KeyWord = KeyWord,
                FilterTypes = "",
                Status = "",
                StartDate = startDate.ToString(Constants.DateFormatMMDDYYYY),
                EndDate = endDate.ToString(Constants.DateFormatMMDDYYYY),
            };

            ListSource = await service_.GetListAsync(ListSource, obj);

            ShowList = (ListSource.Any() || !string.IsNullOrWhiteSpace(KeyWord));
            NoItems = (!ListSource.Any() && (!string.IsNullOrWhiteSpace(KeyWord)));
        }
    }
}