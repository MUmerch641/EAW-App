using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TransactionHistoryPage : ContentPage
    {
        public TransactionHistoryPage(long TransactionTypeId, long transactionId)
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<WorkflowViewModel>();
            viewModel.InitTransactionHistory(TransactionTypeId, transactionId, Navigation);
            BindingContext = viewModel;
        }
    }
}