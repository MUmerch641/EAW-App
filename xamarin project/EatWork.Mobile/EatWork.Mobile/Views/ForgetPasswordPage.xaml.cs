using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ForgetPasswordPage : ContentPage
    {
        public ForgetPasswordPage()
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<ForgotPasswordViewModel>();
            viewModel.Init(Navigation);
            BindingContext = viewModel;
        }
    }
}