using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models;
using EatWork.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Shared
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FileAttachmentPage : ContentPage
    {
        public FileAttachmentPage(FileAttachmentParams param = null)
        {
            InitializeComponent();
            var viewModel = AppContainer.Resolve<FileAttachmentViewModel>();
            viewModel.Init(Navigation, param);
            BindingContext = viewModel;
        }
    }
}