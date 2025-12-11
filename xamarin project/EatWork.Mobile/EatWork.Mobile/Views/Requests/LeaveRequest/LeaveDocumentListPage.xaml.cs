using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models.FormHolder.Request;
using EatWork.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Requests.LeaveRequest
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LeaveDocumentListPage : ContentPage
    {
        private readonly INavigation navigation_;

        public LeaveDocumentListPage(LeaveRequestHolder holder = null, INavigation navigation = null)
        {
            InitializeComponent();

            navigation_ = navigation;

            var viewModel = AppContainer.Resolve<LeaveRequestViewModel>();
            viewModel.InitLeaveDocuments(Navigation, holder);
            BindingContext = viewModel;
        }
    }
}