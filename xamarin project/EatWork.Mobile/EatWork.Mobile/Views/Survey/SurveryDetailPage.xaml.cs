using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels.Survey;
using EAW.API.DataContracts.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Survey
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SurveryDetailPage : ContentPage
    {
        public SurveryDetailPage(PulseSurveyList item)
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<SurveryDetailViewModel>();
            viewModel.Init(Navigation, item);
            BindingContext = viewModel;
        }
    }
}