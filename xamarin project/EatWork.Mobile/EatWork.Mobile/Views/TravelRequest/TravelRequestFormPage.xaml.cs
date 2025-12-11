using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models;
using EatWork.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.TravelRequest
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TravelRequestFormPage : ContentPage
    {
        public TravelRequestFormPage(MyRequestListModel item = null)
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<TravelRequestFormViewModel>();
            viewModel.Init(Navigation, item.TransactionId, item.SelectedDate);
            BindingContext = viewModel;
        }

        protected override bool OnBackButtonPressed()
        {
            MessagingCenter.Send<TravelRequestFormPage>(this, "onback");
            return true;
        }
    }
}