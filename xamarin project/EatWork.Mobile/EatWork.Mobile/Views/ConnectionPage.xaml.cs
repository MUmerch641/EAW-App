using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnectionPage : ContentPage
    {
        public ConnectionPage()
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<ConnectionViewModel>();
            viewModel.Init(Navigation);
            BindingContext = viewModel;

            //DependencyService.Get<IStatusBar>().HideStatusBar();
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await DisplayAlert("Exit Application?", "Are you sure you want to exit?.", "Yes", "No"))
                {
                    base.OnBackButtonPressed();
                    //DependencyService.Get<INativeHelper>().CloseApp();
                }
            });

            return true;
        }
    }
}