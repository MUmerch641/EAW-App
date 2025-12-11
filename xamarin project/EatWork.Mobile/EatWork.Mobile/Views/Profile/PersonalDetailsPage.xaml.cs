using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.MyProfile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonalDetailsPage : ContentPage
    {
        public PersonalDetailsPage(long recordId = 0)
        {
            InitializeComponent();
            var viewModel = AppContainer.Resolve<PersonalDetailsViewModel>();
            viewModel.InitPersonalDetails(recordId, Navigation);
            BindingContext = viewModel;
        }
    }
}