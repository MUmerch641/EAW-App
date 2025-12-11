using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IndividualAttendancePage : ContentPage
    {
        public IndividualAttendancePage()
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<IndividualAttendanceViewModel>();
            viewModel.Init(Navigation, MyAttendanceListView);
            BindingContext = viewModel;
        }
    }
}