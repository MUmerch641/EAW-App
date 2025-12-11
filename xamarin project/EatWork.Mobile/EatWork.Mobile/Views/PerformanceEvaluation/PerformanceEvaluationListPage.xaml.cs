using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels.PerformanceEvaluation;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.PerformanceEvaluation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PerformanceEvaluationListPage : ContentPage
    {
        public PerformanceEvaluationListPage()
        {
            InitializeComponent();
            var viewmodel = AppContainer.Resolve<PEListViewModel>();
            viewmodel.Init(Navigation, ListView);
            BindingContext = viewmodel;
        }
    }
}