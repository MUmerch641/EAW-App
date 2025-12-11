using CommunityToolkit.Mvvm.ComponentModel;
using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Services.Navigation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiHybridApp.ViewModels
{
    public partial class WorkflowViewModel : BaseViewModel
    {
        private readonly IWorkflowDataService _dataService;
        private readonly INavigationService _navigationService;

        private List<TransactionHistory> _historyList;
        public List<TransactionHistory> HistoryList
        {
            get => _historyList;
            set => SetProperty(ref _historyList, value);
        }

        public WorkflowViewModel(
            IWorkflowDataService dataService,
            INavigationService navigationService)
        {
            _dataService = dataService;
            _navigationService = navigationService;
            HistoryList = new List<TransactionHistory>();
        }

        public async Task LoadHistoryAsync(long transactionTypeId, long transactionId)
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                HistoryList = await _dataService.GetTransactionHistoryAsync(transactionTypeId, transactionId);
            }
            catch (Exception ex)
            {
                HandleError(ex, "Failed to load history");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
