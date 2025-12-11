using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Utils;
using EatWork.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MySchedulePage : ContentPage
    {
        private MyScheduleViewModel viewModel;

        public MySchedulePage()
        {
            InitializeComponent();

            viewModel = AppContainer.Resolve<MyScheduleViewModel>();
            viewModel.Init(Navigation, MyScheduleListView);
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.CheckListUpdate();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            FormSession.IsMySchedule = false;
        }
    }
}