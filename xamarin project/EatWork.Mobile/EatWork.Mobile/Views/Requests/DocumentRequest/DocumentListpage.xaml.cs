using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models.FormHolder.Request;
using EatWork.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Requests.DocumentRequest
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocumentListpage : ContentPage
    {
        public DocumentListpage(DocumentRequestHolder form)
        {
            InitializeComponent();
            //var viewModel = AppContainer.Resolve<DocumentRequestViewModel>();
            //viewModel.InitDocuments(NavigationBack, form);
            //BindingContext = viewModel;
        }
    }
}