using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models.FormHolder.Expenses;
using EatWork.Mobile.ViewModels.Expenses;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Expenses
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyExpenseReportDetailPage : ContentPage
    {
        private MyExpenseReportDetailViewModel viewModel;

        public MyExpenseReportDetailPage(ObservableCollection<MyExpensesListDto> expenses = null)
        {
            InitializeComponent();

            viewModel = AppContainer.Resolve<MyExpenseReportDetailViewModel>();
            viewModel.Init(Navigation, expenses);
            BindingContext = viewModel;
        }

        public MyExpenseReportDetailPage(long id)
        {
            InitializeComponent();

            viewModel = AppContainer.Resolve<MyExpenseReportDetailViewModel>();
            viewModel.Init(Navigation, id);
            BindingContext = viewModel;
        }
    }
}