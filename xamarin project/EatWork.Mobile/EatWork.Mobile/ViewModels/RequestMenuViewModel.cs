using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Views.Shared;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    internal class RequestMenuViewModel : BaseViewModel
    {
        #region commands

        public ICommand NavigateCommand { get; set; }

        #endregion commands

        private ObservableCollection<MenuItemModel> menuItems_;

        public ObservableCollection<MenuItemModel> MenuItems
        {
            get { return menuItems_; }
            set { menuItems_ = value; RaisePropertyChanged(() => MenuItems); }
        }

        private readonly IRequestMenuDataService requestMenuDataService_;

        public RequestMenuViewModel(IRequestMenuDataService requestMenuDataService)
        {
            requestMenuDataService_ = requestMenuDataService;
        }

        public void Init(INavigation navigation)
        {
            NavigationBack = navigation;
            MenuItems = new ObservableCollection<MenuItemModel>();

            NavigateCommand = new Command(Navigate);

            InitListMenu();
        }

        private async void InitListMenu()
        {
            MenuItems = await requestMenuDataService_.InitMenuList();
        }

        private async void Navigate(object obj)
        {
            try
            {
                if (obj != null)
                {
                    IsBusy = true;
                    await Task.Delay(500);

                    var eventArgs = obj as Syncfusion.ListView.XForms.ItemTappedEventArgs;
                    var item = (eventArgs.ItemData as MenuItemModel);
                    var model = new MyRequestListModel() { TransactionId = 0, SelectedDate = null };
                    Page page;

                    if (item.TargetType != typeof(ComingSoonPage))
                        page = (Page)Activator.CreateInstance(item.TargetType, model);
                    else
                        page = (Page)Activator.CreateInstance(item.TargetType);
                    //page.Title = item.Title;
                    /*await NavigationService.PushPageAsync(page);*/

                    await NavigationService.PushPageAsync(page);

                    IsBusy = false;
                }
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
        }
    }
}