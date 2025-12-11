using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models;
using EatWork.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Requests
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LeaveRequestPage : ContentPage
    {
        public LeaveRequestPage(MyRequestListModel item = null)
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<LeaveRequestViewModel>();
            viewModel.Init(Navigation, item.TransactionId, item.SelectedDate);
            BindingContext = viewModel;
        }

        /*
        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.ContinueSubmitRequest();
        }
        */

        protected override bool OnBackButtonPressed()
        {
            MessagingCenter.Send<LeaveRequestPage>(this, "onback");
            return true;
        }
    }
}