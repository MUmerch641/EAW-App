using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels.CashAdvance;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.CashAdvance
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CashAdvancesPage : ContentPage
    {
        private CashAdvancesViewModel viewModel;

        public CashAdvancesPage()
        {
            InitializeComponent();
            viewModel = AppContainer.Resolve<CashAdvancesViewModel>();
            viewModel.Init(Navigation, CashAdvanceList);
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.CheckListUpdate();
        }
    }
}