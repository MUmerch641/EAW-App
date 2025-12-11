using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Views.SuggestionCorner;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using R = EAW.API.DataContracts.Models;

namespace EatWork.Mobile.ViewModels.SuggestionCorner
{
    public class SuggestionsViewModel : ListViewModel
    {
        public ICommand AddNewItemCommand { get; set; }
        public ICommand PageAppearingCommand { get; set; }

        private readonly ISuggestionListDataService service_;

        private ObservableCollection<R.SuggestionListDto> listSource_;

        public ObservableCollection<R.SuggestionListDto> ListSource
        {
            get { return listSource_; }
            set { listSource_ = value; RaisePropertyChanged(() => ListSource); }
        }

        public SuggestionsViewModel()
        {
            service_ = AppContainer.Resolve<ISuggestionListDataService>();
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
            LoadItemsCommand = new Command<object>(ExecuteLoadItemsCommand, CanLoadMoreItems);
            ViewItemCommand = new Command<R.SuggestionListDto>(ExecuteViewDetailCommand);
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

            PageAppearingCommand = new Command(ExecutePageAppearingCommand);
        }

        private async void LoadListItems()
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);
                    ListSource = new ObservableCollection<R.SuggestionListDto>();

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
            if (ListSource.Count >= service_.TotalListItem)
            {
                return false;
            }

            return true;
        }

        private async void ExecuteViewDetailCommand(R.SuggestionListDto item)
        {
            try
            {
                if (item != null)
                {
                    using (Dialogs.Loading())
                    {
                        await Task.Delay(500);
                        await NavigationService.PushPageAsync(new SuggestionFormPage(item));
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
                await NavigationService.PushPageAsync(new SuggestionFormPage());
            }
        }

        private async Task RetrieveList()
        {
            var obj = new ListParam()
            {
                ListCount = ListSource.Count,
                Count = TotalItems,
                IsAscending = Ascending,
                KeyWord = KeyWord,
            };

            ListSource = await service_.GetListAsync(ListSource, obj);
            ShowList = (ListSource.Count != 0 || !string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count != 0);
            NoItems = (ListSource.Count == 0 && (!string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count > 0));
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