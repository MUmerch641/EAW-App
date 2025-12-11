using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.Attendance;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.Attendance;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.AttendanceViewTemplate2
{
    public class AttendanceTemplate2DetailViewModel : ListViewModel
    {
        public ICommand ToggleDetailCommand { get; set; }

        private AttendanceTemplate2DetailHolder holder_;

        public AttendanceTemplate2DetailHolder Holder
        {
            get { return holder_; }
            set { holder_ = value; RaisePropertyChanged(() => Holder); }
        }

        private long recordId_;

        private readonly IAttendanceViewTemplate2DataService service_;

        public AttendanceTemplate2DetailViewModel()
        {
            service_ = AppContainer.Resolve<IAttendanceViewTemplate2DataService>();
        }

        public void Init(INavigation navigation, DetailedAttendanceListModel item)
        {
            NavigationBack = navigation;
            recordId_ = item.TimeEntryHeaderId;

            InitHelpers(item);
            LoadListItems();
        }

        private void InitHelpers(DetailedAttendanceListModel item)
        {
            Holder = new AttendanceTemplate2DetailHolder()
            {
                Status = item.Status,
                ReferenceNumber = item.ReferenceNumber,
                CutOffDate = item.CutoffDate
            };

            LoadItemsCommand = new Command<object>(ExecuteLoadItemsCommand, CanLoadMoreItems);

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
            if (Holder.MyAttendanceList.Count >= service_.TotalListItemDetail)
            {
                return false;
            }

            return true;
        }

        private async Task RetrieveList()
        {
            var endDate = Constants.NullDate;//Holder.EndDate.GetValueOrDefault(Constants.NullDate);
            var startDate = Constants.NullDate;//Holder.EndDate.GetValueOrDefault(Constants.NullDate);

            if (endDate > Constants.NullDate)
                endDate = endDate.AddDays(1);

            var param = new ListParamsRecordId()
            {
                ListCount = Holder.MyAttendanceList.Count,
                Count = TotalItems,
                IsAscending = Ascending,
                KeyWord = KeyWord,
                FilterTypes = "",
                Status = "",
                StartDate = startDate.ToString(Constants.DateFormatMMDDYYYY),
                EndDate = endDate.ToString(Constants.DateFormatMMDDYYYY),
                RecordId = recordId_,
            };

            Holder.MyAttendanceList = await service_.InitFormAsync(Holder.MyAttendanceList, param);

            ShowList = (Holder.MyAttendanceList.Any() || !string.IsNullOrWhiteSpace(KeyWord));
            NoItems = (!Holder.MyAttendanceList.Any() && (!string.IsNullOrWhiteSpace(KeyWord)));
        }
    }
}