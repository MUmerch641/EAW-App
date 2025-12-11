using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.ViewModels.Expenses;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Expenses
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewExpensePage : ContentPage
    {
        public NewExpensePage()
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<NewExpenseViewModel>();
            viewModel.Init(Navigation);
            BindingContext = viewModel;

            //this.dashedBorder.DashArray = new double[2] { 5, 5 };
        }

        protected override bool OnBackButtonPressed()
        {
            MessagingCenter.Send<NewExpensePage>(this, "onback");
            return true;
        }
    }
}