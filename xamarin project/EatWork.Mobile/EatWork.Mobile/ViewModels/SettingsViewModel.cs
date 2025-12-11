using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Utils.DataAccess;
using EatWork.Mobile.Views;
using EatWork.Mobile.Views.Settings;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public ICommand UpdateThemeCommand { get; set; }
        public ICommand AppInfoCommand { get; set; }
        public ICommand SelectedFilterCommand { get; set; }
        public ICommand GoToProfileCommand { get; set; }

        private ObservableCollection<SelectableListModel> list_;

        public ObservableCollection<SelectableListModel> List
        {
            get { return list_; }
            set { list_ = value; RaisePropertyChanged(() => List); }
        }

        private string versionNumber_;

        public string VersionNumber
        {
            get { return versionNumber_; }
            set { versionNumber_ = value; RaisePropertyChanged(() => VersionNumber); }
        }

        private string builderNumber_;

        public string BuildNumber
        {
            get { return builderNumber_; }
            set { builderNumber_ = value; RaisePropertyChanged(() => BuildNumber); }
        }

        private bool showThemeConfig_;

        public bool ShowThemeConfig
        {
            get { return showThemeConfig_; }
            set { showThemeConfig_ = value; RaisePropertyChanged(() => ShowThemeConfig); }
        }

        private readonly ClientSetupDataAccess clientSetup_;
        private readonly IMainPageDataService mainPageDataService_;
        private readonly IDialogService dialogs_;
        private readonly ISettingsDataService service_;
        private readonly IAuthenticationDataService authenticationDataService_;
        private readonly MainPageViewModel mainPageViewModel_;

        public SettingsViewModel()
        {
            mainPageDataService_ = AppContainer.Resolve<IMainPageDataService>();
            clientSetup_ = AppContainer.Resolve<ClientSetupDataAccess>();
            dialogs_ = AppContainer.Resolve<IDialogService>();
            service_ = AppContainer.Resolve<ISettingsDataService>();
            authenticationDataService_ = AppContainer.Resolve<IAuthenticationDataService>();
            mainPageViewModel_ = AppContainer.Resolve<MainPageViewModel>();
        }

        public void Init(INavigation nav)
        {
            IsBusy = false;
            ShowThemeConfig = false;
            NavigationBack = nav;
            List = new ObservableCollection<SelectableListModel>();

            UpdateThemeCommand = new Command(ExecuteUpdateThemeCommand);
            SelectedFilterCommand = new Command<SelectableListModel>(ExecuteSelectedFilterCommand);
            AppInfoCommand = new Command(ExecuteAppInfoCommand);
            GoToProfileCommand = new Command(async () => await ExecuteGoToProfileCommand());

            RetrieveList();
            InitSetup();
        }

        private async void InitSetup()
        {
            try
            {
                VersionNumber = VersionTracking.CurrentBuild;/*Constants.VersionNumber*/;
                BuildNumber = VersionTracking.CurrentVersion;

                var form = MenuHelper.Forms().FirstOrDefault(x => x.FormCode == MenuItemType.ThemeConfigForm.ToString());
                ShowThemeConfig = form != null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                ShowThemeConfig = false;
            }
        }

        private async void ExecuteUpdateThemeCommand()
        {
            try
            {
                var setup = await clientSetup_.RetrieveClientSetup();

                if (setup != null)
                {
                    if (setup.ThemeConfigId.GetValueOrDefault(0) == 0)
                    {
                        var param = new ConnectionHolder()
                        {
                            ClientSetupModel = new Models.DataAccess.ClientSetupModel()
                            {
                                ClientCode = setup.ClientCode,
                                Passkey = setup.Passkey.Decrypt()
                            }
                        };

                        var holder = await authenticationDataService_.RetrieveClientSetup(param);
                        setup = holder.ClientSetupModel;
                    }

                    ThemeConfigModel response;
                    using (Dialogs.Loading())
                    {
                        await Task.Delay(100);
                        response = await mainPageDataService_.GetThemeSetup(setup.ThemeConfigId.GetValueOrDefault(0));
                    }

                    if (!string.IsNullOrWhiteSpace(response.Code))
                    {
                        AppLayout.Extensions.ApplyColorSet();
                        await dialogs_.AlertAsync("Theme updated.");
                    }
                    else
                    {
                        await dialogs_.AlertAsync("No theme setup found.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async void RetrieveList()
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);
                    List = await service_.EmployeeFilterConfig();
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async void ExecuteSelectedFilterCommand(SelectableListModel obj)
        {
            if (obj != null)
            {
                await service_.UpdateEmployeeFilterSetup(obj);
            }
        }

        private async void ExecuteAppInfoCommand()
        {
            using (Dialogs.Loading())
            {
                await Task.Delay(500);
                await NavigationService.PushPageAsync(new AppInfoPage());
            }
        }

        private async Task ExecuteGoToProfileCommand()
        {
            if (!IsBusy)
            {
                using (Dialogs.Loading())
                {
                    await Task.Delay(500);

                    var item = new MenuItemModel()
                    {
                        Id = MenuItemType.MyProfile,
                        Icon = "iconprofile.png",
                        Title = "My Profile",
                        TargetType = typeof(MyProfilePage)
                    };

                    mainPageViewModel_.SelectedMenu(item);
                }
            }
        }
    }
}