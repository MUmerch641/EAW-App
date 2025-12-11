using EatWork.Mobile.Models.FormHolder.Request;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Requests.ChangeWorkSchedule
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmployeeListPage : ContentPage
    {
        public EmployeeListPage(ChangeWorkScheduleHolder form = null)
        {
            InitializeComponent();

            //SfListView EmployeeListView = new SfListView();

            //var viewModel = AppContainer.Resolve<ChangeWorkScheduleViewModel>();
            //viewModel.InitList(EmployeeListView, form, NavigationBack);
            //BindingContext = viewModel;
        }
    }
}