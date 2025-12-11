using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models.FormHolder.Request;
using EatWork.Mobile.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Requests.LeaveRequest
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GeneratedLeaveListPage : ContentPage
    {
        private TaskCompletionSource<bool> _taskCompletionSource;
        private readonly INavigation navigation_;
        public GeneratedLeaveListPage(LeaveRequestHolder holder = null, INavigation navigation = null)
        {
            InitializeComponent();

            navigation_ = navigation;

            var viewModel = AppContainer.Resolve<LeaveRequestViewModel>();
            viewModel.InitLeaveDetailsList(Navigation, holder);
            BindingContext = viewModel;
            _taskCompletionSource = new TaskCompletionSource<bool>();
        }

        protected override void OnDisappearing()
        {
            _taskCompletionSource.SetResult(true);
            base.OnDisappearing();
        }

        public Task WaitForModalToCloseAsync()
        {
            return _taskCompletionSource.Task;
        }
    }
}