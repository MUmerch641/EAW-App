using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.Profile;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Views.MyProfile;
using EatWork.Mobile.Views.Shared;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class MyProfileViewModel : BaseViewModel
    {
        #region commands

        public ICommand ViewItemCommand { get; set; }
        public ICommand ProfileCategoryCommand { get; set; }

        #endregion commands

        private ObservableCollection<MenuItemModel> employeeProfileCategory_;

        public ObservableCollection<MenuItemModel> EmployeeProfileCategory
        {
            get { return employeeProfileCategory_; }
            set { employeeProfileCategory_ = value; RaisePropertyChanged(() => EmployeeProfileCategory); }
        }

        private ObservableCollection<MenuItemModel> employeeProfileSubCategory_;

        public ObservableCollection<MenuItemModel> EmployeeProfileSubCategory
        {
            get { return employeeProfileSubCategory_; }
            set { employeeProfileSubCategory_ = value; RaisePropertyChanged(() => EmployeeProfileSubCategory); }
        }

        private ProfileHolder profile;

        public ProfileHolder Profile
        {
            get { return profile; }
            set { profile = value; RaisePropertyChanged(() => Profile); }
        }

        private bool hasBackgroundImage_ = false;

        public bool HasBackgroundImage
        {
            get { return hasBackgroundImage_; }
            set { hasBackgroundImage_ = value; RaisePropertyChanged(() => HasBackgroundImage); }
        }

        private string sourceImage_ = "HeaderImageProfile.png";

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

        private readonly IEmployeeProfileDataService employeeDataService_;

        public MyProfileViewModel(IEmployeeProfileDataService employeeDataService)
        {
            employeeDataService_ = employeeDataService;
        }

        public void Init(INavigation navigation, long recordId = 0)
        {
            EmployeeProfileCategory = new ObservableCollection<MenuItemModel>();
            Profile = new ProfileHolder();
            NavigationBack = navigation;
            IsBusy = false;

            ViewItemCommand = new Command(ViewItemEvent);
            ProfileCategoryCommand = new Command(async () =>
            {
                IsBusy = true;
                await Task.Delay(500);
                await NavigationService.PushPageAsync(new ProfileMainCategoryPage(recordId));
                IsBusy = false;
            });

            if (!string.IsNullOrWhiteSpace(PreferenceHelper.HomeScreenSetup()))
            {
                HasBackgroundImage = true;
                SourceImage = PreferenceHelper.HomeScreenSetup();
            }

            InitEmployeeProfile(recordId);
        }

        private async void InitEmployeeProfile(long recordId)
        {
            try
            {
                IsBusy = true;
                await Task.Delay(500);
                Profile = await employeeDataService_.InitPersonalDetails(recordId);
                await InitEmployeeCategory();

                ProfileImage = await employeeDataService_.GetProfileImage(recordId);
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

        public void InitEmployeeSubCategory(INavigation navigation, int groupdId = 0)
        {
            EmployeeProfileSubCategory = new ObservableCollection<MenuItemModel>();
            ViewItemCommand = new Command(ViewItemStackLayout);
            NavigationBack = navigation;

            RetrieveEmployeeSubCategories(groupdId);
        }

        private async Task InitEmployeeCategory()
        {
            try
            {
                IsBusy = true;
                await Task.Delay(500);
                EmployeeProfileCategory = await employeeDataService_.InitEmployeeCategory();
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

        private async void RetrieveEmployeeSubCategories(int groupdId = 0)
        {
            try
            {
                IsBusy = true;
                await Task.Delay(500);
                EmployeeProfileSubCategory = await employeeDataService_.InitEmployeeSubCategory(groupdId);
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

        private async void ViewItemEvent(object obj)
        {
            if (obj != null)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);

                    var eventArgs = obj as Syncfusion.ListView.XForms.ItemTappedEventArgs;
                    var item = (eventArgs.ItemData as MenuItemModel);
                    var param = new object[] { 0, item.GroupId };

                    Page page;
                    if (item.TargetType != typeof(ComingSoonPage))
                        page = (Page)Activator.CreateInstance(item.TargetType, param);
                    else
                        page = (Page)Activator.CreateInstance(item.TargetType);

                    page.Title = item.Title;
                    await NavigationService.PushPageAsync(page);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async void ViewItemStackLayout(object obj)
        {
            if (obj != null)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);

                    if (obj is MenuItemModel item)
                    {
                        var param = new object[] { 0 };

                        Page page;
                        if (item.TargetType != typeof(ComingSoonPage))
                            page = (Page)Activator.CreateInstance(item.TargetType, param);
                        else
                            page = (Page)Activator.CreateInstance(item.TargetType);

                        page.Title = item.Title;
                        await NavigationService.PushPageAsync(page);
                    }
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }
    }
}