using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels.Payslip;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Payslips
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class YTDOTPaymentBreakdownListPage : ContentPage
    {
        public YTDOTPaymentBreakdownListPage()
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<YTDOTPaymentBreakdownListViewModel>();
            viewModel.Init(Navigation, MyPayslipListView);
            BindingContext = viewModel;
        }
    }
}