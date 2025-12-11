using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels.Survey;
using EAW.API.DataContracts.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Survey
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SurveyChartPage : ContentPage
    {
        public SurveyChartPage(PulseSurveyList item)
        {
            InitializeComponent();
            var viewModel = AppContainer.Resolve<SurveyChartViewModel>();
            viewModel.Init(Navigation, item);
            BindingContext = viewModel;
        }
    }
}