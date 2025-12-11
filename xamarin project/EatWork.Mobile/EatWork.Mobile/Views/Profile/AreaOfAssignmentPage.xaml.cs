using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.MyProfile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AreaOfAssignmentPage : ContentPage
    {
        public AreaOfAssignmentPage(long recordId = 0)
        {
            InitializeComponent();
            var viewModel = AppContainer.Resolve<EmploymentInformationViewModel>();
            viewModel.InitEmploymentInformation(recordId, Navigation);
            BindingContext = viewModel;
        }
    }
}