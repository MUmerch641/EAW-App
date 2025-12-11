using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models;
using EatWork.Mobile.ViewModels.TravelRequest;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Approvals
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TravelRequestApprovalPage : ContentPage
    {
        public TravelRequestApprovalPage(MyApprovalListModel item)
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<TravelRequestApprovalViewModel>();
            viewModel.Init(Navigation, item);
            BindingContext = viewModel;
        }
    }
}