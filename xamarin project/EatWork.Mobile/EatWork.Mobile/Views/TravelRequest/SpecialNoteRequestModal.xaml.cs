using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Utils;
using EatWork.Mobile.ViewModels;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.TravelRequest
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SpecialNoteRequestModal : FormModal
    {
        public SpecialNoteRequestModal(string note = "")
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<SpecialNoteRequestViewModel>();
            viewModel.Init(note);
            BindingContext = viewModel;
        }
    }
}