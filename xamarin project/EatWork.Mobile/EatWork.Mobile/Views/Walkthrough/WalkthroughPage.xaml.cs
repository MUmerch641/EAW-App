using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WalkthroughPage : ContentPage
    {
        public WalkthroughPage()
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<WalkthroughViewModel>();
            viewModel.Init(Navigation);
            BindingContext = viewModel;
        }
    }
}