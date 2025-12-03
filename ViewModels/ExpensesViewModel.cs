using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiHybridApp.ViewModels;

// INotifyPropertyChanged implement karna zaroori hai MVVM ke liye
public class ExpensesViewModel : INotifyPropertyChanged
{
    private readonly IExpenseDataService _expenseService;
    
    public ExpensesViewModel(IExpenseDataService expenseService)
    {
        _expenseService = expenseService;
        // Default Initialize
        NewExpense = new ExpenseModel();
    }

    // --- Properties ---
    private List<ExpenseModel> _expenses = new();
    public List<ExpenseModel> Expenses
    {
        get => _expenses;
        set { _expenses = value; OnPropertyChanged(); }
    }

    private ExpenseModel _newExpense = new ExpenseModel();
    public ExpenseModel NewExpense
    {
        get => _newExpense;
        set { _newExpense = value; OnPropertyChanged(); }
    }

    private bool _isLoading = true;
    public bool IsLoading
    {
        get => _isLoading;
        set { _isLoading = value; OnPropertyChanged(); }
    }

    private string _message = string.Empty;
    public string Message
    {
        get => _message;
        set { _message = value; OnPropertyChanged(); }
    }

    // --- Methods (Logic) ---
    public async Task LoadExpensesAsync()
    {
        IsLoading = true;
        Expenses = await _expenseService.GetExpensesAsync();
        IsLoading = false;
    }

    public async Task SubmitExpenseAsync()
    {
    IsLoading = true;
    var result = await _expenseService.SubmitExpenseAsync(NewExpense);
        
        if (result.Success)
        {
            Message = "Expense Submitted!";
            NewExpense = new ExpenseModel(); // Reset form
            await LoadExpensesAsync(); // Refresh list
        }
        else
        {
            Message = $"Error: {result.ErrorMessage}";
        }
        IsLoading = false;
    }

    // --- MVVM Boilerplate ---
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name ?? string.Empty));
    }
}
