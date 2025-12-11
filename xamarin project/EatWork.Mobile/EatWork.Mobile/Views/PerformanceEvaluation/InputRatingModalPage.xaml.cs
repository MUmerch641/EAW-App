using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models.FormHolder.IndividualObjectives;
using EatWork.Mobile.Utils;
using EatWork.Mobile.ViewModels.PerformanceEvaluation;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.PerformanceEvaluation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InputRatingModalPage : FormModal
    {
        public InputRatingModalPage(ObjectiveDetailDto item)
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<InputRatingViewModel>();
            viewModel.Init(item);
            BindingContext = viewModel;
        }
    }
}