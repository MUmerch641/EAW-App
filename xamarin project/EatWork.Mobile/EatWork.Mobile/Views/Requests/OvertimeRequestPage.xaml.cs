using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models;
using EatWork.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Requests
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OvertimeRequestPage : ContentPage
    {
        public OvertimeRequestPage(MyRequestListModel item = null)
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<OvertimeViewModel>();
            viewModel.Init(Navigation, item.TransactionId, item.SelectedDate);
            BindingContext = viewModel;
        }

        protected override bool OnBackButtonPressed()
        {
            MessagingCenter.Send<OvertimeRequestPage>(this, "onback");
            return true;
        }

    }
}