using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models.FormHolder.Request;
using EatWork.Mobile.ViewModels;
using Syncfusion.ListView.XForms;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Requests.ChangeRestDay
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmployeeListPage : ContentPage
    {
        public EmployeeListPage(ChangeRestdayHolder form = null)
        {
            InitializeComponent();

            SfListView EmployeeListView = new SfListView();

            var viewModel = AppContainer.Resolve<ChangeRestDayViewModel>();
            viewModel.InitList(EmployeeListView, form, Navigation);
            BindingContext = viewModel;
        }
    }
}