using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Models;
using EatWork.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Requests
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OfficialBusinessRequestPage : ContentPage
    {
        public OfficialBusinessRequestPage(MyRequestListModel item = null)
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<OfficialBusinessViewModel>();
            viewModel.Init(Navigation, item.TransactionId, Constants.OfficialBusiness, item.SelectedDate);
            BindingContext = viewModel;
        }

        protected override bool OnBackButtonPressed()
        {
            MessagingCenter.Send<OfficialBusinessRequestPage>(this, "onback");
            return true;
        }
    }
}