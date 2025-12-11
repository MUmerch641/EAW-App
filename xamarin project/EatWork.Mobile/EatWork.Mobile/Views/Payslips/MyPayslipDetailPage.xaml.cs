using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels.Payslip;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Payslips
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyPayslipDetailPage : ContentPage
    {
        public MyPayslipDetailPage(long paysheetHeaderId, long profileId)
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<MyPayslipDetailViewModel>();
            viewModel.Init(Navigation, paysheetHeaderId, profileId);
            BindingContext = viewModel;
        }
    }
}