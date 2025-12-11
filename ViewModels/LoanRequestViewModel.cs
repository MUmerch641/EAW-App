using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels
{
    public class LoanRequestViewModel : BaseViewModel
    {
        private readonly IFinancialDataService _financialService;
        private readonly NavigationManager _navigationManager;

        public LoanRequestViewModel(IFinancialDataService financialService, NavigationManager navigationManager)
        {
            _financialService = financialService;
            _navigationManager = navigationManager;
            
            SubmitCommand = new Command(async () => await SubmitAsync());
            CancelCommand = new Command(GoBack);
        }

        private decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        private string _purpose;
        public string Purpose
        {
            get => _purpose;
            set => SetProperty(ref _purpose, value);
        }

        private LoanTypeModel _selectedLoanType;
        public LoanTypeModel SelectedLoanType
        {
            get => _selectedLoanType;
            set => SetProperty(ref _selectedLoanType, value);
        }

        public ObservableCollection<LoanTypeModel> LoanTypes { get; private set; } = new();

        public ICommand SubmitCommand { get; }
        public ICommand CancelCommand { get; }

        public override async Task InitializeAsync()
        {
            await ExecuteBusyAsync(async () =>
            {
                var list = await _financialService.GetLoanTypesAsync();
                LoanTypes = new ObservableCollection<LoanTypeModel>(list);
            }, "Loading...");
        }

        private async Task SubmitAsync()
        {
            if (SelectedLoanType == null)
            {
                ErrorMessage = "Please select a loan type.";
                return;
            }

            if (Amount <= 0)
            {
                ErrorMessage = "Amount must be greater than 0.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Purpose))
            {
                ErrorMessage = "Purpose is required.";
                return;
            }

            await ExecuteBusyAsync(async () =>
            {
                var request = new LoanRequestModel
                {
                    LoanTypeSetupId = SelectedLoanType.LoanTypeSetupId,
                    RequestedAmount = Amount,
                    Purpose = Purpose
                };

                var success = await _financialService.SubmitLoanAsync(request);

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
