using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels.Payslip;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Payslips
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyPayslipPage : ContentPage
    {
        public MyPayslipPage()
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<MyPayslipViewModel>();
            viewModel.Init(Navigation, MyPayslipListView);
            BindingContext = viewModel;
        }
    }
}