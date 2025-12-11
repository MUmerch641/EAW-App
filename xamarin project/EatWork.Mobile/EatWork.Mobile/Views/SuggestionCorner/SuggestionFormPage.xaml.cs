using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels.SuggestionCorner;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using R = EAW.API.DataContracts.Models;

namespace EatWork.Mobile.Views.SuggestionCorner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SuggestionFormPage : ContentPage
    {
        public SuggestionFormPage(R.SuggestionListDto item = null)
        {
            InitializeComponent();

            var viewmodel = AppContainer.Resolve<SuggestionFormViewModel>();
            viewmodel.Init(Navigation, item);
            BindingContext = viewmodel;
        }

        protected override bool OnBackButtonPressed()
        {
            MessagingCenter.Send<SuggestionFormPage>(this, "onback");
            return true;
        }
    }
}