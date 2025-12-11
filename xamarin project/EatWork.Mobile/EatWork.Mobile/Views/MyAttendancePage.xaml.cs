using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Utils;
using EatWork.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyAttendancePage : ContentPage
    {
        public MyAttendancePage()
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<MyAttendanceViewModel>();
            viewModel.Init(Navigation, MyAttendanceListView);
            BindingContext = viewModel;
        }

    }
}