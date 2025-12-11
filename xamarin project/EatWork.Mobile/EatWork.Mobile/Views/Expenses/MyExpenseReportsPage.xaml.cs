using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels.Expenses;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Expenses
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyExpenseReportsPage : ContentPage
    {
        private MyExpenseReportsViewModel viewModel;

        public MyExpenseReportsPage()
        {
            InitializeComponent();

            viewModel = AppContainer.Resolve<MyExpenseReportsViewModel>();
            viewModel.Init(Navigation, MyExpensesList);
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.CheckListUpdate();
        }
    }
}