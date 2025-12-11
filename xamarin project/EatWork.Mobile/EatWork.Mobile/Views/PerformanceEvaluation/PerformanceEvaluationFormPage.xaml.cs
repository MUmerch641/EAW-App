using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels.PerformanceEvaluation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.PerformanceEvaluation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PerformanceEvaluationFormPage : ContentPage
    {
        public PerformanceEvaluationFormPage(long id = 0)
        {
            InitializeComponent();

            var viewmodel = AppContainer.Resolve<PEFormViewModel>();
            viewmodel.Init(Navigation, id);
            BindingContext = viewmodel;
        }
    }
}