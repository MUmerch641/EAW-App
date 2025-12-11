using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiHybridApp.ViewModels
{
    public class ExpensesViewModel : BaseViewModel
    {
        private readonly IExpenseDataService _expenseService;
        
        public ExpensesViewModel(IExpenseDataService expenseService)
        {
            _expenseService = expenseService;
            NewExpense = new ExpenseModel();
        }

        // --- Properties ---
        private List<ExpenseModel> _expenses = new();
        public List<ExpenseModel> Expenses
        {
            get => _expenses;
            set => SetProperty(ref _expenses, value);
        }

        private ExpenseModel _newExpense;
        public ExpenseModel NewExpense
        {
            get => _newExpense;
            set => SetProperty(ref _newExpense, value);
        }

        // --- Methods (Logic) ---
        public async Task LoadExpensesAsync()
        {
            await ExecuteBusyAsync(async () =>
            {
                Expenses = await _expenseService.GetExpensesAsync();
            }, "Loading expenses...");
        }

        public async Task SubmitExpenseAsync()
        {
            await ExecuteBusyAsync(async () =>
            {
                var result = await _expenseService.SubmitExpenseAsync(NewExpense);
                
                if (result.Success)
                {
                    SuccessMessage = "Expense Submitted!";
                    NewExpense = new ExpenseModel(); // Reset form
                    await LoadExpensesAsync(); // Refresh list
                }
                else
                {
                    ErrorMessage = $"Error: {result.ErrorMessage}";
                }
            }, "Submitting expense...");
        }
    }
}
