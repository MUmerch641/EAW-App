using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyApprovalPage : ContentPage
    {
        public MyApprovalPage()
        {
            InitializeComponent();
            var viewModel = AppContainer.Resolve<MyApprovalViewModel>();
            viewModel.Init(Navigation, ApprovalListView);
            BindingContext = viewModel;
        }

        /*
        protected override void OnAppearing()
        {
            base.OnAppearing();

            viewModel.CheckListUpdate();
        }
        */
    }
}