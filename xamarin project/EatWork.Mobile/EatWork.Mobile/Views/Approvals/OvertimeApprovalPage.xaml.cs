using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models;
using EatWork.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Approvals
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OvertimeApprovalPage : ContentPage
    {
        public OvertimeApprovalPage(MyApprovalListModel item)
        {
            InitializeComponent();
            var viewModel = AppContainer.Resolve<OvertimeApprovalViewModel>();
            viewModel.Init(Navigation, item);
            BindingContext = viewModel;
        }
    }
}