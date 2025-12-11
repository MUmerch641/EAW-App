using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels.AttendanceViewTemplate2;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.AttendanceViewTemplate2
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AttendanceTemplate2ListPage : ContentPage
    {
        public AttendanceTemplate2ListPage()
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<AttendanceViewTemplate2ViewModel>();
            viewModel.Init(Navigation, AttendanceListPage);
            BindingContext = viewModel;
        }
    }
}