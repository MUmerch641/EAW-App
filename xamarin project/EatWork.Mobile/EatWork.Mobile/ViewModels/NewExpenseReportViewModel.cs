using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    internal class NewExpenseReportViewModel : BaseViewModel
    {
        #region commands

        public ICommand NavigateCommand { get; set; }
        public ICommand CloseModalCommand { get; set; }

        #endregion commands

        private ObservableCollection<ExpenseTypeModel> expenseMenuItems_;

        public ObservableCollection<ExpenseTypeModel> ExpenseMenuItems
        {
            get { return expenseMenuItems_; }
            set { expenseMenuItems_ = value; OnPropertyChanged(nameof(ExpenseMenuItems)); }
        }

        public INavigation Navigation { get; set; }

        private readonly IMyExpenseDataService newExpenseReportService_;

        public NewExpenseReportViewModel(IMyExpenseDataService newExpenseReportService)
        {
            newExpenseReportService_ = newExpenseReportService;
        }

        public void Init(INavigation navigation)
        {
            Navigation = navigation;
            ExpenseMenuItems = new ObservableCollection<ExpenseTypeModel>();

            NavigateCommand = new Command<ExpenseTypeModel>(Navigate);
            CloseModalCommand = new Command(async () => await CloseCurrentModal());

            InitExpenseListMenu();
        }

        private async void InitExpenseListMenu()
        {
            ExpenseMenuItems = await newExpenseReportService_.InitExpenseMenuList();
        }

        private async void Navigate(ExpenseTypeModel item)
        {
            if (item != null)
            {
                IsBusy = true;
                await Task.Delay(500);

                var page = (Page)Activator.CreateInstance(item.TargetType);
                //page.Title = item.Title;
                await Navigation.PushModalAsync(page);

                IsBusy = false;
            }
        }

        private async Task CloseCurrentModal()
        {
            await Navigation.PopModalAsync(true);
        }
    }
}