using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels.SuggestionCorner;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.SuggestionCorner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SuggestionsPage : ContentPage
    {
        public SuggestionsPage()
        {
            InitializeComponent();

            var viewmodel = AppContainer.Resolve<SuggestionsViewModel>();
            viewmodel.Init(Navigation, SuggestionList);
            BindingContext = viewmodel;
        }
    }
}