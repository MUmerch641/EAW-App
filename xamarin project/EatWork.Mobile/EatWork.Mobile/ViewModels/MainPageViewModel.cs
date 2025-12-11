using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Utils.DataAccess;
using EatWork.Mobile.Views;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        public ICommand SelectMenuCommand
        {
            protected set;
            get;
        }

        public ICommand MyProfileCommand
        {
            protected set;
            get;
        }

        public ICommand ViewNotificationCommand
        {
            protected set;
            get;
        }

        private UserModel userInfo_;

        public UserModel UserInfo
        {
            get { return userInfo_; }
            set { userInfo_ = value; RaisePropertyChanged(() => UserInfo); }
        }

        private ObservableCollection<MenuItemModel> menuItems_;

        public ObservableCollection<MenuItemModel> MenuItems
        {
            get { return menuItems_; }
            set { menuItems_ = value; RaisePropertyChanged(() => MenuItems); }
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

        private Xamarin.Forms.ImageSource profileImage_;

        public Xamarin.Forms.ImageSource ProfileImage
        {
            get { return profileImage_; }
            set { profileImage_ = value; RaisePropertyChanged(() => ProfileImage); }
        }

        private readonly IMainPageDataService mainPageDataService_;
        private readonly IDialogService dialogService_;
        private readonly IEmployeeProfileDataService employeeProfileService_;
        private readonly ClientSetupDataAccess clientSetup_;

        public MainPageViewModel(IMainPageDataService mainPageDataService,
            IDialogService dialogService)
        {
            mainPageDataService_ = mainPageDataService;
            dialogService_ = dialogService;
            clientSetup_ = AppContainer.Resolve<ClientSetupDataAccess>();
            employeeProfileService_ = AppContainer.Resolve<IEmployeeProfileDataService>();
        }

        public void Init(INavigation navigation)
        {
            NavigationBack = navigation;
            UserInfo = PreferenceHelper.UserInfo();

            MenuItems = new ObservableCollection<MenuItemModel>();
            /*SelectMenuCommand = new Command<MenuItemModel>(SelectedMenu);*/
            SelectMenuCommand = new Command<MenuItemModel>(SelectedMenu);
            MyProfileCommand = new Command(async () => await GoToProfilePage());
            ViewNotificationCommand = new Command(async () => await ViewNotifications());
            InitForm();
            InitMenuItems();
        }

        private async void InitMenuItems()
        {
            if (!IsBusy)
            {
                IsBusy = true;
                await Task.Delay(500);
                MenuItems = await mainPageDataService_.InitMenuList();
                IsBusy = false;
            }
        }

        public async void SelectedMenu(MenuItemModel item)
        {
            if (item != null)
            {
                try
                {
                    if (item.Id == MenuItemType.LogOut)
                    {
                        if (await dialogService_.ConfirmDialogAsync(Messages.Logout))
                        {
                            using (Dialogs.Loading("Logging out.."))
                            {
                                await Task.Delay(500);
                                FormSession.ClearEverything();
                                PreferenceHelper.UserId(0);
                                PreferenceHelper.IsFirstLogin(false);

                                Application.Current.MainPage = new NavigationPage(new LoginPage());
                                await NavigationService.PopToRootAsync();
                            }
                        }
                    }
                    else
                    {
                        IsBusy = true;
                        await Task.Delay(500);

                        var page = (Page)Activator.CreateInstance(item.TargetType);
                        page.Title = item.Title;

                        /*var master = (FlyoutPage)(Application.Current.MainPage as NavigationPage).CurrentPage;*/
                        var master = (Application.Current.MainPage as FlyoutPage);

                        if ((master.Detail.Title != page.Title) || !string.IsNullOrEmpty(master.Detail.Title))
                        {
                            master.Detail = new NavigationPage(page);
                        }

                        master.IsPresented = false;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async Task GoToProfilePage()
        {
            if (!IsBusy)
            {
                IsBusy = true;
                await Task.Delay(500);

                var item = new MenuItemModel()
                {
                    Id = MenuItemType.MyProfile,
                    Icon = "iconprofile.png",
                    Title = "My Profile",
                    TargetType = typeof(MyProfilePage)
                };

                SelectedMenu(item);

                IsBusy = false;
            }
        }

        private async Task ViewNotifications()
        {
            if (!IsBusy)
            {
                IsBusy = true;
                await Task.Delay(500);

                var item = new MenuItemModel()
                {
                    Id = MenuItemType.Notifications,
                    Icon = "iconprofile.png",
                    Title = "Notifications",
                    TargetType = typeof(NotificationPage)
                };

                SelectedMenu(item);

                IsBusy = false;
            }
        }

        /*
        private async void ViewItemEvent(object obj)
        {
            try
            {
                if (obj != null)
                {
                    var eventArgs = obj as Syncfusion.ListView.XForms.ItemTappedEventArgs;
                    var item = (eventArgs.ItemData as MenuItemModel);
                    if (item != null)
                        SelectedMenu(item);
                }
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
        }
        */

        private async void InitForm()
        {
            using (Dialogs.Loading())
            {
                await Task.Delay(500);
                var setup = await clientSetup_.RetrieveClientSetup();
                if (setup != null)
                {
                    if (!string.IsNullOrWhiteSpace(setup.HomeScreenImage))
                    {
                        var type = (string.IsNullOrWhiteSpace(setup.HomeScreenImageType) ? "jpeg" : setup.HomeScreenImageType);
                        var url = new UriBuilder(ApiConstants.BaseApiUrl)
                        {
                            Path = string.Format(ApiConstants.GetImageSetup, type, setup.HomeScreenImage)
                        };

                        HasBackgroundImage = true;
                        SourceImage = url.ToString();
                        PreferenceHelper.HomeScreenSetup(url.ToString());
                    }
                }

                ProfileImage = await employeeProfileService_.GetProfileImage();
            }
        }
    }
}