using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models.FormHolder.Request;
using EatWork.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Requests.DocumentRequest
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocumentReasonListPage : ContentPage
    {
        public DocumentReasonListPage(DocumentRequestHolder form)
        {
            InitializeComponent();
            //var viewModel = AppContainer.Resolve<DocumentRequestViewModel>();
            //viewModel.InitReason(NavigationBack, form);
            //BindingContext = viewModel;
        }
    }
}