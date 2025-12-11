using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels
{
    public class CashAdvanceRequestViewModel : BaseViewModel
    {
        private readonly IFinancialDataService _financialService;
        private readonly NavigationManager _navigationManager;

        public CashAdvanceRequestViewModel(IFinancialDataService financialService, NavigationManager navigationManager)
        {
            _financialService = financialService;
            _navigationManager = navigationManager;
            
            SubmitCommand = new Command(async () => await SubmitAsync());
            CancelCommand = new Command(GoBack);
        }

        private DateTime _dateNeeded = DateTime.Today;
        public DateTime DateNeeded
        {
            get => _dateNeeded;
            set => SetProperty(ref _dateNeeded, value);
        }

        private decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        private string _reason;
        public string Reason
        {
            get => _reason;
            set => SetProperty(ref _reason, value);
        }

        private CostCenterModel _selectedCostCenter;
        public CostCenterModel SelectedCostCenter
        {
            get => _selectedCostCenter;
            set => SetProperty(ref _selectedCostCenter, value);
        }

        public ObservableCollection<CostCenterModel> CostCenters { get; private set; } = new();

        public ICommand SubmitCommand { get; }
        public ICommand CancelCommand { get; }

        public override async Task InitializeAsync()
        {
            await ExecuteBusyAsync(async () =>
            {
                var list = await _financialService.GetCostCentersAsync();
                CostCenters = new ObservableCollection<CostCenterModel>(list);
            }, "Loading...");
        }

        private async Task SubmitAsync()
        {
            if (Amount <= 0)
            {
                ErrorMessage = "Amount must be greater than 0.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Reason))
            {
                ErrorMessage = "Reason is required.";
                return;
            }

            await ExecuteBusyAsync(async () =>
            {
                var request = new CashAdvanceModel
                {
                    DateNeeded = DateNeeded,
                    Amount = Amount,
                    Reason = Reason,
                    CostCenterId = SelectedCostCenter?.CostCenterId
                };

                var success = await _financialService.SubmitCashAdvanceAsync(request);

                if (success)
                {
                    SuccessMessage = "Request submitted successfully.";
                    await Task.Delay(1500);
                    GoBack();
                }
                else
                {
                    ErrorMessage = "Failed to submit request.";
                }
            }, "Submitting...");
        }

        private void GoBack()
        {
            _navigationManager.NavigateTo("/financial");
        }
    }
}
