using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models;
using EatWork.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Requests
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocumentRequestPage : ContentPage
    {
        public DocumentRequestPage(MyRequestListModel item = null)
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<DocumentRequestViewModel>();
            viewModel.Init(Navigation, item.TransactionId, item.SelectedDate);
            BindingContext = viewModel;
        }

        protected override bool OnBackButtonPressed()
        {
            MessagingCenter.Send<DocumentRequestPage>(this, "onback");
            return true;
        }
    }
}