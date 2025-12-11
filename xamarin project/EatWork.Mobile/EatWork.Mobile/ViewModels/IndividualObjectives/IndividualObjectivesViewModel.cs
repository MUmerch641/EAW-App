using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.IndividualObjectives;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Views.IndividualObjectives;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.IndividualObjectives
{
    public class IndividualObjectivesViewModel : ListViewModel
    {
        public ICommand AddNewItemCommand { get; set; }
        public ICommand PageAppearingCommand { get; set; }

        private IndividualObjectivesHolder holder_;

        public IndividualObjectivesHolder Holder
        {
            get { return holder_; }
            set { holder_ = value; RaisePropertyChanged(() => Holder); }
        }

        private readonly IIndividualObjectivesDataService service_;

        public IndividualObjectivesViewModel()
        {
            service_ = AppContainer.Resolve<IIndividualObjectivesDataService>();
        }

        public void Init(INavigation navigation, SfListView listView)
        {
            NavigationBack = navigation;
            ListView = listView;

            InitHelpers();
            LoadListItems();
        }

        private void InitHelpers()
        {
            Holder = new IndividualObjectivesHolder();

            AddNewItemCommand = new Command(ExecuteAddNewItemCommand);
            LoadItemsCommand = new Command<object>(ExecuteLoadItemsCommand, CanLoadMoreItems);
            ViewItemCommand = new Command<IndividualObjectivesDto>(ExecuteViewItemCommand);
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
                    Holder.ListItemsSource = new ObservableCollection<IndividualObjectivesDto>();
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
            var obj = new ListParam()
            {
                ListCount = Holder.ListItemsSource.Count,
                Count = TotalItems,
                IsAscending = Ascending,
                KeyWord = KeyWord,
                FilterTypes = "",
                Status = "",
                StartDate = "",
                EndDate = "",
            };

            Holder.ListItemsSource = await service_.GetListAsync(Holder.ListItemsSource, obj);

            ShowList = (Holder.ListItemsSource.Count != 0 || !string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count != 0);
            NoItems = (Holder.ListItemsSource.Count == 0 && (!string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count > 0));

            Ascending = Ascending;
        }

        private async void ExecuteAddNewItemCommand()
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);
                    await NavigationService.PushPageAsync(new IndividualObjectiveItemPage());
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

        private bool CanLoadMoreItems(object obj)
        {
            if (Holder.ListItemsSource.Count >= service_.TotalListItem)
                return false;
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
                    await Dialogs.AlertAsync(ex.Message, "", "Close");
                }
                finally
                {
                    listview.IsBusy = false;
                }
            }
        }

        private async void ExecuteViewItemCommand(IndividualObjectivesDto item)
        {
            if (!IsBusy)
            {
                try
                {
                    using (Dialogs.Loading())
                    {
                        await Task.Delay(500);
                        await NavigationService.PushPageAsync(new IndividualObjectiveItemPage(item.IndividualOjbectiveId)); ;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
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