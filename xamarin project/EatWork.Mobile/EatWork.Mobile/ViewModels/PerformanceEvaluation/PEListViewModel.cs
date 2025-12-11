using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.PerformanceEvaluation;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Views.PerformanceEvaluation;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.PerformanceEvaluation
{
    public class PEListViewModel : ListViewModel
    {
        public ICommand AddNewCommand { get; set; }
        public ICommand PageAppearingCommand { get; set; }

        private PEListHolder holder_;

        public PEListHolder Holder
        {
            get { return holder_; }
            set { holder_ = value; RaisePropertyChanged(() => Holder); }
        }

        private readonly IPEListDataService service_;

        public PEListViewModel()
        {
            service_ = AppContainer.Resolve<IPEListDataService>();
        }

        public void Init(INavigation nav, SfListView listview)
        {
            NavigationBack = nav;
            ListView = listview;

            InitHelpers();
            LoadListItems();
        }

        private void InitHelpers()
        {
            Holder = new PEListHolder();

            AddNewCommand = new Command(ExecuteAddNewItemCommand);
            LoadItemsCommand = new Command<object>(ExecuteLoadItemsCommand, CanLoadMoreItems);
            ViewItemCommand = new Command<PEListDto>(ExecuteViewItemCommand);
            PageAppearingCommand = new Command(ExecutePageAppearingCommand);

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
                    Holder.ItemSource = new ObservableCollection<PEListDto>();
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

        private async void ExecuteAddNewItemCommand()
        {
            if (!IsBusy)
            {
                using (Dialogs.Loading())
                {
                    await Task.Delay(500);
                    await NavigationService.PushPageAsync(new PerformanceEvaluationFormPage());
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

        private bool CanLoadMoreItems(object obj)
        {
            if (Holder.ItemSource.Count >= service_.TotalListItem)
                return false;
            return true;
        }

        private async void ExecuteViewItemCommand(PEListDto item)
        {
            if (!IsBusy)
            {
                try
                {
                    using (Dialogs.Loading())
                    {
                        await Task.Delay(500);
                        await NavigationService.PushPageAsync(new PerformanceEvaluationFormPage(item.RecordId));
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private async Task RetrieveList()
        {
            var obj = new ListParam()
            {
                ListCount = Holder.ItemSource.Count,
                Count = TotalItems,
                IsAscending = Ascending,
                KeyWord = KeyWord,
                FilterTypes = "",
                Status = "",
                StartDate = "",
                EndDate = "",
            };

            Holder.ItemSource = await service_.GetListAsync(Holder.ItemSource, obj);

            ShowList = (Holder.ItemSource.Count != 0 || !string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count != 0);
            NoItems = (Holder.ItemSource.Count == 0 && (!string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count > 0));

            Ascending = Ascending;
        }

        private void ExecutePageAppearingCommand()
        {
            if (FormSession.IsSubmitted)
            {
                LoadListItems();
                FormSession.IsSubmitted = false;
                IsBusy = false;
            }
        }
    }
}