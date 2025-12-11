using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models.FormHolder.Request;
using EatWork.Mobile.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Requests.LeaveRequest
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LeaveDocumentAttachmentPage : PopupPage
    {
        public LeaveDocumentAttachmentPage(LeaveRequestHolder form)
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<LeaveRequestViewModel>();
            viewModel.InitDocumentForm(form);
            BindingContext = viewModel;
        }
    }
}