using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models.Attendance;
using EatWork.Mobile.ViewModels.AttendanceViewTemplate2;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.AttendanceViewTemplate2
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AttendanceTemplate2DetailPage : ContentPage
    {
        public AttendanceTemplate2DetailPage(DetailedAttendanceListModel item)
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<AttendanceTemplate2DetailViewModel>();
            viewModel.Init(Navigation, item);
            BindingContext = viewModel;
        }
    }
}