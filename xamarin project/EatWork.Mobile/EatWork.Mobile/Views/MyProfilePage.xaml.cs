using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyProfilePage : ContentPage
    {
        private readonly MyProfileViewModel viewModel;

        public MyProfilePage(long recordId = 0)
        {
            InitializeComponent();
            Title = (recordId > 0 ? "Employee Profile" : "My Profile");
            viewModel = AppContainer.Resolve<MyProfileViewModel>();
            viewModel.Init(Navigation, recordId);
            BindingContext = viewModel;
        }

        public MyProfilePage()
        {
            InitializeComponent();
            viewModel = AppContainer.Resolve<MyProfileViewModel>();
            viewModel.Init(Navigation);
            BindingContext = viewModel;
        }
    }
}