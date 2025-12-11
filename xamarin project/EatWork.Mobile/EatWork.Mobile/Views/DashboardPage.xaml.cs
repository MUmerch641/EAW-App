using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DashboardPage : ContentPage
    {
        private DashboardViewModel viewModel;

        public DashboardPage()
        {
            InitializeComponent();

            viewModel = AppContainer.Resolve<DashboardViewModel>();
            viewModel.Init(Navigation);
            BindingContext = viewModel;
        }

        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();
        //    viewModel.Init(NavigationBack);
        //}
    }
}