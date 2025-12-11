using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.MyProfile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfileCategoryPage : ContentPage
    {
        public ProfileCategoryPage(long profileId = 0, int groupId = 0)
        {
            InitializeComponent();
            var viewModel = AppContainer.Resolve<MyProfileViewModel>();
            viewModel.InitEmployeeSubCategory(Navigation, groupId);
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //TitleView.Title = Title;
            //TitleView.TitleColor = Color.WhiteSmoke;
        }
    }
}