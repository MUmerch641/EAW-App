using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models.FormHolder.PerformanceEvaluation;
using EatWork.Mobile.ViewModels.PerformanceEvaluation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.PerformanceEvaluation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuestionnairePage : ContentPage
    {
        public QuestionnairePage(PEFormHolder holder)
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<QuestionnaireViewModel>();
            viewModel.Init(Navigation, holder);
            BindingContext = viewModel;
        }
    }
}