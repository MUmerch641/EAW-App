using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models;
using EatWork.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Approvals
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChangeRestdaySchedApprovalPage : ContentPage
    {
        public ChangeRestdaySchedApprovalPage(MyApprovalListModel item)
        {
            InitializeComponent();
            var viewModel = AppContainer.Resolve<ChangeRestdayScheduleApprovalViewModel>();
            viewModel.Init(Navigation, item);
            BindingContext = viewModel;
        }
    }
}