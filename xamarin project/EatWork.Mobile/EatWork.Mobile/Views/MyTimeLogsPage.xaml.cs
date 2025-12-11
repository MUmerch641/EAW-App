using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyTimeLogsPage : ContentPage
    {
        private MyTimeLogsViewModel viewModel;

        public MyTimeLogsPage()
        {
            InitializeComponent();

            viewModel = AppContainer.Resolve<MyTimeLogsViewModel>();
            viewModel.Init(Navigation, MyTimeLogsListView);
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.CheckListUpdate();
        }
    }
}