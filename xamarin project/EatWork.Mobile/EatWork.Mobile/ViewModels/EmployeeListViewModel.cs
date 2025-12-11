using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Views.Requests;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class EmployeeListViewModel : BaseViewModel
    {
        #region commands

        public ICommand LoadItemsCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand SelectEmployeeCommand { get; set; }

        #endregion commands

        public INavigation Navigation { get; set; }

        #region properties

        private ObservableCollection<EmployeeListModel> employee_;

        public ObservableCollection<EmployeeListModel> Employee
        {
            get { return employee_; }
            set { employee_ = value; OnPropertyChanged(nameof(Employee)); }
        }

        private readonly int totalItems_ = 100;

        private SfListView employeeList_;

        public SfListView EmployeeList
        {
            get { return employeeList_; }
            set { employeeList_ = value; OnPropertyChanged(nameof(EmployeeList)); }
        }

        #endregion properties

        private readonly IEmployeeListDataService employeeListDataService_;

        public EmployeeListViewModel(IEmployeeListDataService employeeListDataService)
        {
            employeeListDataService_ = employeeListDataService;
        }

        public void Init(SfListView employeeListView, INavigation navigation)
        {
            Navigation = navigation;
            Employee = new ObservableCollection<EmployeeListModel>();
            EmployeeList = employeeListView;

            LoadItemsCommand = new Command<object>(ExecuteLoadItemsCommand);
            CloseCommand = new Command(async () => await CloseCurrentPage());
            SelectEmployeeCommand = new Command<EmployeeListModel>(SelectEmployee);

            LoadListItems();
        }

        public async void LoadListItems()
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(1000);
                    var myEmployee = employeeListDataService_.RetrieveEmployeeList(Employee.Count, totalItems_, Employee);
                    var EmployeeeList = employeeListDataService_.InitListView(EmployeeList);

                    await Task.WhenAll(myEmployee, EmployeeeList);
                    Employee = myEmployee.Result;
                    EmployeeList = EmployeeeList.Result;
                    IsBusy = false;
                }
                catch (Exception ex)
                {
                    await Dialogs.AlertAsync(ex.Message, "", "Close");
                }
            }
        }

        private async void ExecuteLoadItemsCommand(object obj)
        {
            var listview = obj as SfListView;
            if (!listview.IsBusy)
            {
                try
                {
                    listview.IsBusy = true;
                    await Task.Delay(1000);
                    Employee = await employeeListDataService_.RetrieveEmployeeList(Employee.Count, totalItems_, Employee);
                }
                catch (Exception ex)
                {
                    await Dialogs.AlertAsync(ex.Message, "", "Close");
                }
                finally
                {
                    listview.IsBusy = false;
                }
            }

        }
        private async Task CloseCurrentPage()
        {
            await Navigation.PopToRootAsync(true);
        }
        private async void SelectEmployee(EmployeeListModel item)
        {
            await Navigation.PopModalAsync(true);
        }
    }
}