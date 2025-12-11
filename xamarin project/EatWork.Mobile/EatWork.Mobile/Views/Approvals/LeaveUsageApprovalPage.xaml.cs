using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models.FormHolder.Approvals;
using EatWork.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Approvals
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LeaveUsageApprovalPage : ContentPage
    {
        public LeaveUsageApprovalPage(LeaveApprovalHolder form)
        {
            InitializeComponent();
            var viewModel = AppContainer.Resolve<LeaveApprovalViewModel>();
            viewModel.InitLeaveUsage(Navigation, form);
            BindingContext = viewModel;
        }
    }
}