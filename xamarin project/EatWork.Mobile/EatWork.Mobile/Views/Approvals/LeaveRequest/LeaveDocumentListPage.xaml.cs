using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models.FormHolder.Approvals;
using EatWork.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Approvals.LeaveRequest
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LeaveDocumentListPage : ContentPage
    {
        public LeaveDocumentListPage(LeaveApprovalHolder holder = null)
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<LeaveApprovalViewModel>();
            viewModel.InitLeaveDocuments(Navigation, holder);
            BindingContext = viewModel;
        }
    }
}