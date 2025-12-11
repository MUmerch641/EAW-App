using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.MyProfile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FamilyBackgroundPage : ContentPage
    {
        public FamilyBackgroundPage(long recordId = 0)
        {
            InitializeComponent();
            var viewModel = AppContainer.Resolve<EmployeeProfileViewModel>();
            viewModel.InitFamilyBackground(FamilyBackgroundListView, recordId, Navigation);
            BindingContext = viewModel;
        }
    }
}