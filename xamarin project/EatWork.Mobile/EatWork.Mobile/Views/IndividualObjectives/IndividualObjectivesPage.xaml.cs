using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels.IndividualObjectives;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.IndividualObjectives
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IndividualObjectivesPage : ContentPage
    {
        public IndividualObjectivesPage()
        {
            InitializeComponent();
            var vm = AppContainer.Resolve<IndividualObjectivesViewModel>();
            vm.Init(Navigation, ListView);
            BindingContext = vm;
        }
    }
}