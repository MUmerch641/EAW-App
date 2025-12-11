using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.MyProfile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfileMainCategoryPage : ContentPage
    {
        public ProfileMainCategoryPage(long profileId)
        {
            InitializeComponent();
            var viewModel = AppContainer.Resolve<MyProfileViewModel>();
            viewModel.Init(Navigation, profileId);
            BindingContext = viewModel;
        }
    }
}