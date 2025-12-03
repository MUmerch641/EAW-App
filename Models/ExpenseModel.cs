using MauiHybridApp.Utils;

namespace MauiHybridApp.Models;

public class ExpenseModel
{
    public ExpenseModel()
    {
        // Defaults
        ExpenseId = 0;
        ProfileId = 0;
        StatusId = RequestStatusValue.Submitted;
        
        DateFiled = DateTime.UtcNow;
        ExpenseDate = DateTime.UtcNow.Date;
        
        Amount = 0;
        Remarks = string.Empty; // Details/Description
        Merchant = string.Empty;
        
        // Setup/Type ID (Dropdown se aayega)
        ExpenseSetupId = 0; 
        ExpenseSetupName = string.Empty;
        Status = string.Empty;
    }

    public long ExpenseId { get; set; }
    public long ProfileId { get; set; }
    public DateTime DateFiled { get; set; }
    public DateTime ExpenseDate { get; set; }
    
    public decimal Amount { get; set; }
    public string Remarks { get; set; }
    public string Merchant { get; set; } // Kis dukaan se khareeda
    
    public long ExpenseSetupId { get; set; } // Expense Type (e.g. Taxi, Meal)
    public string ExpenseSetupName { get; set; } // Display Name for List
    
    public long StatusId { get; set; }
    public string Status { get; set; }
}
