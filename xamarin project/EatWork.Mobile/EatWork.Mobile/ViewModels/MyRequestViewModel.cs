using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Views.Requests;
using EatWork.Mobile.Views.Shared;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class MyRequestViewModel : ListViewModel
    {
        #region commands

        public ICommand AddNewCommand { get; set; }

        public ICommand PageAppearingCommand { get; set; }
        public ICommand PageDisappearingCommand { get; set; }

        #endregion commands

        #region properties

        private ObservableCollection<MyRequestListModel> myRequest_;

        public ObservableCollection<MyRequestListModel> MyRequest
        {
            get { return myRequest_; }
            set { myRequest_ = value; RaisePropertyChanged(() => MyRequest); }
        }

        private UserModel userInfo_;

        public UserModel UserInfo
        {
            get { return userInfo_; }
            set { userInfo_ = value; RaisePropertyChanged(() => UserInfo); }
        }

        private bool hasBackgroundImage_ = false;

        public bool HasBackgroundImage
        {
            get { return hasBackgroundImage_; }
            set { hasBackgroundImage_ = value; RaisePropertyChanged(() => HasBackgroundImage); }
        }

        private string sourceImage_;

        public string SourceImage
        {
            get { return sourceImage_; }
            set { sourceImage_ = value; RaisePropertyChanged(() => SourceImage); }
        }

        #endregion properties

        private readonly IMyRequestDataService myRequestDataService_;

        public MyRequestViewModel(IMyRequestDataService myRequestDataService)
        {
            myRequestDataService_ = myRequestDataService;
        }

        public void Init(SfListView myRequestListView, INavigation navigation)
        {
            NavigationBack = navigation;
            ListView = myRequestListView;
            UserInfo = PreferenceHelper.UserInfo();

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
            SelectableListItemSource = new ObservableCollection<SelectableListModel>();

            LoadItemsCommand = new Command<object>(ExecuteLoadItemsCommand, CanLoadMoreItems);
            AddNewCommand = new Command(async () => await RequestMenu());
            ViewItemCommand = new Command(ViewItemEvent);

            FilterTypeCommand = new Command(() => LoadListItems());

            PageAppearingCommand = new Command(ExecutePageAppearingCommand);
            PageDisappearingCommand = new Command(ExecutePageDisappearingCommand);

            ResetFilteredTypeCommand = new Command(() =>
            {
                Ascending = !Ascending;
                SelectedTransactionTypes = new ObservableCollection<SelectableListModel>();
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

            EditCommand = new Command(() =>
            {
                IsEditMode = !IsEditMode;
            });

            foreach (var item in RequestType.RequetTypeList.Where(p => p.IsVisible == 1))
                SelectableListItemSource.Add(new SelectableListModel() { Id = item.RequestTypeId, DisplayText = item.Title, IsChecked = false });

            if (!string.IsNullOrWhiteSpace(PreferenceHelper.HomeScreenSetup()))
            {
                HasBackgroundImage = true;
                SourceImage = PreferenceHelper.HomeScreenSetup();
            }
        }

        private async void LoadListItems()
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);
                    MyRequest = new ObservableCollection<MyRequestListModel>();
                    ListView = await myRequestDataService_.InitListView(ListView);
                    await RetrieveList();
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

        private async Task RequestMenu()
        {
            IsBusy = true;
            await Task.Delay(500);
            /*await NavigationService.PushPageAsync(new RequestMenuPage());*/
            await NavigationService.PushPageAsync(new RequestMenuPage());
            IsBusy = false;
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

                    /*
                    var param = new ListParam()
                    {
                        ListCount = MyRequest.Count,
                        Count = TotalItems,
                        IsAscending = Ascending,
                        KeyWord = KeyWord,
                        FilterTypes = (SelectedTransactionTypes.Count == 0 ? "" : string.Join(",", SelectedTransactionTypes.Select(p => p.Id)))
                    };

                    MyRequest = await myRequestDataService_.RetrieveMyRequestList(MyRequest, param);

                    ShowList = (MyRequest.Count != 0 || !string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count != 0);
                    NoItems = (MyRequest.Count == 0 && (!string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count > 0));
                    */
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

        private async void ViewItemEvent(object obj)
        {
            try
            {
                if (obj != null)
                {
                    var eventArgs = obj as Syncfusion.ListView.XForms.ItemTappedEventArgs;
                    var item = (eventArgs.ItemData as MyRequestListModel);
                    if (item != null)
                    {
                        using (Dialogs.Loading())
                        {
                            await Task.Delay(500);
                            var response = await RequestType.GetPageByRequestType(item.TransactionTypeId);
                            Page page;
                            if (response.RequestPage != typeof(ComingSoonPage))
                                page = (Page)Activator.CreateInstance(response.RequestPage, item);
                            else
                            {
                                page = (Page)Activator.CreateInstance(response.RequestPage);
                                page.Title = response.Title;
                            }

                            /*await NavigationService.PushPageAsync(page);*/
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

        /*
        private async void FilterListByTransactionType()
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);
                    MyRequest = new ObservableCollection<MyRequestListModel>();

                    var param = new ListParam()
                    {
                        ListCount = MyRequest.Count,
                        Count = TotalItems,
                        IsAscending = Ascending,
                        KeyWord = KeyWord,
                        FilterTypes = (SelectedTransactionTypes.Count == 0 ? "" : string.Join(",", SelectedTransactionTypes.Select(p => p.Id)))
                    };

                    var list = myRequestDataService_.RetrieveMyRequestList(MyRequest, param);
                    var listForm = myRequestDataService_.InitListView(ListView);

                    await Task.WhenAll(list, listForm);
                    MyRequest = list.Result;
                    ListView = listForm.Result;

                    ShowList = (MyRequest.Count != 0 || !string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count != 0);
                    NoItems = (MyRequest.Count == 0 && (!string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count > 0));
                    Ascending = Ascending;
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
        */

        private bool CanLoadMoreItems(object obj)
        {
            if (MyRequest.Count >= myRequestDataService_.TotalListItem)
                return false;
            return true;
        }

        private async Task RetrieveList()
        {
            var obj = new ListParam()
            {
                ListCount = MyRequest.Count,
                Count = TotalItems,
                IsAscending = Ascending,
                KeyWord = KeyWord,
                FilterTypes = (SelectedTransactionTypes.Count == 0 ? "" : string.Join(",", SelectedTransactionTypes.Select(p => p.Id)))
            };

            MyRequest = await myRequestDataService_.RetrieveMyRequestList(MyRequest, obj);

            ShowList = (MyRequest.Count != 0 || !string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count != 0);
            NoItems = (MyRequest.Count == 0 && (!string.IsNullOrWhiteSpace(KeyWord) || SelectedTransactionTypes.Count > 0));
        }

        private void ExecutePageAppearingCommand()
        {
            CheckListUpdate();
        }

        private void ExecutePageDisappearingCommand()
        {
        }
    }
}