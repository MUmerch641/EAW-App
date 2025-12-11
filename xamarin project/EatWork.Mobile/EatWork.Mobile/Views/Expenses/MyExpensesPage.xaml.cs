using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels.Expenses;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Expenses
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyExpensesPage : ContentPage
    {
        private MyExpensesListViewModel viewModel;

        public MyExpensesPage()
        {
            InitializeComponent();

            viewModel = AppContainer.Resolve<MyExpensesListViewModel>();
            viewModel.Init(Navigation, MyExpensesListView);
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.CheckListUpdate();
        }
    }
}