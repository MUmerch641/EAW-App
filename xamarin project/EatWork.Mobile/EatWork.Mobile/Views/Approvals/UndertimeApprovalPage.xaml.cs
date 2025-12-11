using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models;
using EatWork.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Approvals
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UndertimeApprovalPage : ContentPage
    {
        public UndertimeApprovalPage(MyApprovalListModel item)
        {
            InitializeComponent();
            var viewModel = AppContainer.Resolve<UndertimeApprovalViewModel>();
            viewModel.Init(Navigation, item);
            BindingContext = viewModel;
        }
    }
}