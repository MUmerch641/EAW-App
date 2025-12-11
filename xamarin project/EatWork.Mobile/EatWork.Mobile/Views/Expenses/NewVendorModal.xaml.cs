using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Utils;
using EatWork.Mobile.ViewModels.Expenses;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.TravelRequest
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewVendorModal : FormModal
    {
        public NewVendorModal()
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<NewVendorViewModel>();
            viewModel.Init();
            BindingContext = viewModel;
        }
    }
}