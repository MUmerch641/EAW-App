using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.Request;
using EatWork.Mobile.ViewModels;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Requests.LeaveRequest
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LeaveTypeListPage : ContentPage
    {
        public LeaveTypeListPage(LeaveRequestHolder holder, ObservableCollection<SelectableListModel> list)
        {
            InitializeComponent();

            var viewModel = AppContainer.Resolve<LeaveRequestViewModel>();
            viewModel.InitLeaveTypeList(Navigation, holder, list);
            BindingContext = viewModel;
        }
    }
}