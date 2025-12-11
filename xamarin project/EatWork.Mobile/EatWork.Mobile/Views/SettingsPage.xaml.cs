using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<SettingsViewModel>();
            viewModel.Init(Navigation);
            BindingContext = viewModel;
        }
    }
}