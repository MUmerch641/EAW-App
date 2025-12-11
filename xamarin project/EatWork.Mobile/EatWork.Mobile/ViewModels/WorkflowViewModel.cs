using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class WorkflowViewModel : BaseViewModel
    {
        #region commands

        public ICommand CloseModalCommand { get; set; }

        #endregion commands

        private List<TransactionHistory> transactionHistory_;

        public List<TransactionHistory> TransactionHistory
        {
            get { return transactionHistory_; }
            set { transactionHistory_ = value; RaisePropertyChanged(() => TransactionHistory); }
        }

        private List<NotificationModel> notifList_;

        public List<NotificationModel> NotificationList
        {
            get { return notifList_; }
            set { notifList_ = value; RaisePropertyChanged(() => NotificationList); }
        }

        private readonly IWorkflowDataService workflowDataService_;

        public WorkflowViewModel(IWorkflowDataService workflowDataService)
        {
            workflowDataService_ = workflowDataService;
        }

        public void InitTransactionHistory(long TransactionTypeId, long transactionId, INavigation navigation)
        {
            IsBusy = false;
            NavigationBack = navigation;
            TransactionHistory = new List<TransactionHistory>();

            CloseModalCommand = new Command(async () => await NavigationService.PopModalAsync());

            InitHistoryList(TransactionTypeId, transactionId);
        }

        public void InitNotifications(INavigation navigation)
        {
            NavigationBack = navigation;
            NotificationList = new List<NotificationModel>();
            InitNotificationList();
        }

        private async void InitHistoryList(long TransactionTypeId, long transactionId)
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);

                    TransactionHistory = await workflowDataService_.GetTransactionHistory(TransactionTypeId, transactionId);
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

        private async void InitNotificationList()
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(500);

                    for (int i = 0; i < 5; i++)
                    {
                        NotificationList.Add(new NotificationModel() 
                        {
                            IsRead = true,
                            Message = "Your Bank File has been Approved",
                            ActionDateTime = Convert.ToDateTime(DateTime.Now),
                        });
                    }
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