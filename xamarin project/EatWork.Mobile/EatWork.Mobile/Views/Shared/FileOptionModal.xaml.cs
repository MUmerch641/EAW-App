using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Utils;
using EatWork.Mobile.ViewModels;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Shared
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FileOptionModal : FormModal
    {
        public FileOptionModal()
        {
            InitializeComponent();
            var viewModel = AppContainer.Resolve<FileOptionViewModel>();
            BindingContext = viewModel;
        }
    }
}