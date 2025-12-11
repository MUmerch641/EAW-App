using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models;
using EatWork.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Approvals
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoanApprovalPage : ContentPage
    {
        public LoanApprovalPage(MyApprovalListModel item)
        {
            InitializeComponent();
            var viewModel = AppContainer.Resolve<LoanApprovalViewModel>();
            viewModel.Init(Navigation, item);
            BindingContext = viewModel;
        }
    }
}