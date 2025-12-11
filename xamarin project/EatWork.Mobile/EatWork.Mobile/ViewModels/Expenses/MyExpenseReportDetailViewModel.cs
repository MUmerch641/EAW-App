using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.FormHolder.Expenses;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Views.Expenses;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.Expenses
{
    public class MyExpenseReportDetailViewModel : BaseViewModel
    {
        public ICommand GoBackPageCommand { get; set; }
        public ICommand SubmitExpenseReportCommand { get; set; }
        public ICommand CancelRequestCommand { get; set; }
        public ICommand ViewFileCommand { get; set; }

        private ExpenseReportDetailHolder holder_;

        public ExpenseReportDetailHolder Holder
        {
            get { return holder_; }
            set { holder_ = value; RaisePropertyChanged(() => Holder); }
        }

        private readonly IExpenseReportDataService service_;
        private readonly IDialogService dialogService_;
        private readonly ICommonDataService commonService_;

        public MyExpenseReportDetailViewModel()
        {
            service_ = AppContainer.Resolve<IExpenseReportDataService>();
            dialogService_ = AppContainer.Resolve<IDialogService>();
            commonService_ = AppContainer.Resolve<ICommonDataService>();

            InitHelpers();
        }

        public void Init(INavigation navigation, ObservableCollection<MyExpensesListDto> expenses = null)
        {
            NavigationBack = navigation;

            InitForm(expenses);
        }

        public void Init(INavigation navigation, long id = 0)
        {
            NavigationBack = navigation;
            InitForm(id);
        }

        private void InitHelpers()
        {
            Holder = new ExpenseReportDetailHolder();

            SubmitExpenseReportCommand = new Command(ExecuteSubmitExpenseReportCommand);
            GoBackPageCommand = new Command(() => { base.BackItemPage(); });
            CancelRequestCommand = new Command(ExecuteCancelRequestCommand);
            ViewFileCommand = new Command<ExpenseReportDetailModel>(ExecuteViewFileCommand);
        }

        private async void InitForm(ObservableCollection<MyExpensesListDto> expenses)
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);

                    Holder = await service_.InitForm(expenses);
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

        private async void InitForm(long id)
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);

                    Holder = await service_.RetrieveRecord(id);
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

        private async void ExecuteSubmitExpenseReportCommand()
        {
            try
            {
                Holder = await service_.SubmitRecord(Holder);

                if (Holder.Success)
                {
                    Success(true, Messages.RecordSaved);
                    /*await NavigationService.PopToRootAsync();*/
                    var master = (MasterDetailPage)(Application.Current.MainPage as NavigationPage).CurrentPage;
                    master.Detail = new NavigationPage(new MyExpenseReportsPage());
                    /*
                    if (!string.IsNullOrEmpty(master.Detail.Title))
                    {
                        master.Detail = new NavigationPage(new MyExpenseReportsPage());
                    }
                    */
                    master.IsPresented = false;
                }
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
        }

        private async void ExecuteCancelRequestCommand()
        {
            try
            {
                Holder.ActionTypeId = ActionTypeId.Cancel;
                Holder.Msg = Messages.Cancel;
                Holder = await service_.WorkflowTransactionRequest(Holder);

                if (Holder.Success)
                {
                    Success(true);
                    await NavigationService.PopToRootAsync();
                }
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
        }

        private async void ExecuteViewFileCommand(ExpenseReportDetailModel item)
        {
            if (item != null)
            {
                await commonService_.PreviewFileBase64(item.Attachment, item.FileType, item.FileName);
            }
        }

        protected override async void BackItemPage()
        {
            if (Holder.RecordId == 0)
            {
                if (Holder.Details.Count > 0)
                {
                    if (await dialogService_.ConfirmDialogAsync(Messages.LEAVEPAGE))
                    {
                        base.BackItemPage();
                    }
                }
                else
                {
                    base.BackItemPage();
                }
            }
            else
            {
                base.BackItemPage();
            }
        }
    }
}