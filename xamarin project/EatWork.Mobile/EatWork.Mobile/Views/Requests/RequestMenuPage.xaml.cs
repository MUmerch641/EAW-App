using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Requests
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RequestMenuPage : ContentPage
    {
        public RequestMenuPage()
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<RequestMenuViewModel>();
            viewModel.Init(Navigation);
            BindingContext = viewModel;
        }
    }
}