using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.Expense;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.Expenses
{
    public class MyExpenseDetailViewModel : BaseViewModel
    {
        public ICommand PrintFileCommand { get; set; }

        private AppExpenseReportDetail model_;

        public AppExpenseReportDetail Model
        {
            get { return model_; }
            set { model_ = value; RaisePropertyChanged(() => Model); }
        }

        private readonly IExpenseDataService service_;

        public MyExpenseDetailViewModel(IExpenseDataService service)
        {
            service_ = service;
        }

        public void Init(INavigation navigation, long id)
        {
            NavigationBack = navigation;
            Model = new AppExpenseReportDetail();

            InitForm(id);
        }

        private async void InitForm(long id)
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);
                    Model = await service_.GetRecordAsync(id);
                }
                catch (Exception ex)
                {
                    Error(false, ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }
    }
}