using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models.FormHolder.PerformanceEvaluation;
using EatWork.Mobile.ViewModels.PerformanceEvaluation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.PerformanceEvaluation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NarrativeSectionPage : ContentPage
    {
        public NarrativeSectionPage(PEFormHolder holder)
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<NarrativeSectionViewModel>();
            viewModel.Init(Navigation, holder);
            BindingContext = viewModel;
        }
    }
}