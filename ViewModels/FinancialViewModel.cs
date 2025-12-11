using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels
{
    public class FinancialViewModel : BaseViewModel
    {
        private readonly IFinancialDataService _financialService;
        private readonly NavigationManager _navigationManager;

        public FinancialViewModel(IFinancialDataService financialService, NavigationManager navigationManager)
        {
            _financialService = financialService;
            _navigationManager = navigationManager;
            
            NavigateToCashAdvanceCommand = new Command(NavigateToCashAdvance);
            NavigateToLoanCommand = new Command(NavigateToLoan);
            SwitchTabCommand = new Command<string>(SwitchTab);
        }

        private string _activeTab = "cash-advance";
        public string ActiveTab
        {
            get => _activeTab;
            set => SetProperty(ref _activeTab, value);
        }

        private ObservableCollection<CashAdvanceModel> _cashAdvances;
        public ObservableCollection<CashAdvanceModel> CashAdvances
        {
            get => _cashAdvances;
            set => SetProperty(ref _cashAdvances, value);
        }

        private ObservableCollection<LoanRequestModel> _loans;
        public ObservableCollection<LoanRequestModel> Loans
        {
            get => _loans;
            set => SetProperty(ref _loans, value);
        }

        public ICommand NavigateToCashAdvanceCommand { get; }
        public ICommand NavigateToLoanCommand { get; }
        public ICommand SwitchTabCommand { get; }

        public override async Task InitializeAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            await ExecuteBusyAsync(async () =>
            {
                if (ActiveTab == "cash-advance")
                {
                    var list = await _financialService.GetCashAdvancesAsync();
                    CashAdvances = new ObservableCollection<CashAdvanceModel>(list);
                }
                else
                {
                    var list = await _financialService.GetLoansAsync();
                    Loans = new ObservableCollection<LoanRequestModel>(list);
                }
            }, "Loading data...");
        }

        private void SwitchTab(string tab)
        {
            if (ActiveTab != tab)
            {
                ActiveTab = tab;
                _ = LoadDataAsync();
            }
        }

        private void NavigateToCashAdvance()
        {
            _navigationManager.NavigateTo("/financial/cash-advance/request");
        }

        private void NavigateToLoan()
        {
            _navigationManager.NavigateTo("/financial/loan/request");
        }
    }
}
