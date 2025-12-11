using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MauiHybridApp.Models;

namespace MauiHybridApp.Services.Data
{
    public interface IFinancialDataService
    {
        // Cash Advance
        Task<List<CashAdvanceModel>> GetCashAdvancesAsync();
        Task<CashAdvanceModel> GetCashAdvanceByIdAsync(long id);
        Task<bool> SubmitCashAdvanceAsync(CashAdvanceModel request);
        Task<List<CostCenterModel>> GetCostCentersAsync();

        // Loans
        Task<List<LoanRequestModel>> GetLoansAsync();
        Task<LoanRequestModel> GetLoanByIdAsync(long id);
        Task<bool> SubmitLoanAsync(LoanRequestModel request);
        Task<List<LoanTypeModel>> GetLoanTypesAsync();
    }
}
