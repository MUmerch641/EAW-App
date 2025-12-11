using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Utils;
using EatWork.Mobile.ViewModels.SuggestionCorner;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.SuggestionCorner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CategoryModalPage : FormModal
    {
        public CategoryModalPage()
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<CategoryModalViewModel>();
            viewModel.Init();
            BindingContext = viewModel;
        }
    }
}