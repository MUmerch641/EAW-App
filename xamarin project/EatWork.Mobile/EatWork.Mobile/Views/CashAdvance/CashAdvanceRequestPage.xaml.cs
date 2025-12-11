using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels.CashAdvance;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.CashAdvance
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CashAdvanceRequestPage : ContentPage
    {
        public CashAdvanceRequestPage(long id = 0)
        {
            InitializeComponent();
            var viewModel = AppContainer.Resolve<CashAdvanceRequestViewModel>();
            viewModel.Init(Navigation, id);
            BindingContext = viewModel;
        }

        protected override bool OnBackButtonPressed()
        {
            MessagingCenter.Send<CashAdvanceRequestPage>(this, "onback");
            return true;
        }
    }
}