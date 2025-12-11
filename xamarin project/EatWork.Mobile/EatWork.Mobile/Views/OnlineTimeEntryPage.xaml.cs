using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OnlineTimeEntryPage : ContentPage
    {
        public OnlineTimeEntryPage()
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<OnlineTimeEntryViewModel>();
            viewModel.Init(Navigation);
            BindingContext = viewModel;
        }
    }
}