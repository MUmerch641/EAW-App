using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models.FormHolder.IndividualObjectives;
using EatWork.Mobile.ViewModels.IndividualObjectives;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.IndividualObjectives
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ObjectivesListPage : ContentPage
    {
        public ObjectivesListPage(ObservableCollection<MainObjectiveDto> list)
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<ObjectivesListViewModel>();
            viewModel.Init(Navigation,list);
            BindingContext = viewModel;
        }
    }
}