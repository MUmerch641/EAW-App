using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels.Expenses;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Expenses
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyExpenseDetailPage : ContentPage
    {
        public MyExpenseDetailPage(long id = 0)
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<MyExpenseDetailViewModel>();
            viewModel.Init(Navigation, id);
            BindingContext = viewModel;
        }
    }
}