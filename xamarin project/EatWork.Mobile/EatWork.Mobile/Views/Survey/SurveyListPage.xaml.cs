using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels.Survey;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Survey
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SurveyListPage : ContentPage
    {
        public SurveyListPage()
        {
            InitializeComponent();

            var viewmodel = AppContainer.Resolve<SurveyListViewModel>();
            viewmodel.Init(Navigation);
            BindingContext = viewmodel;
        }
    }
}