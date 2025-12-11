using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels.IndividualObjectives;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.IndividualObjectives
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IndividualObjectiveItemPage : ContentPage
    {
        public IndividualObjectiveItemPage(long id = 0)
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<IndividualObjectiveItemViewModel>();
            viewModel.Init(Navigation, id);
            BindingContext = viewModel;
        }
    }
}