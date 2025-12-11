using MauiHybridApp.Models;
using System;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public interface IChangeRestdayScheduleDataService
    {
        Task<ChangeRestdayHolder> InitForm(long recordId, DateTime? selectedDate);

        Task<ChangeRestdayHolder> SubmitRequest(ChangeRestdayHolder form);

        Task<ChangeRestdayHolder> GetEmployeeSchedule(ChangeRestdayHolder form);
        
        Task<ChangeRestdayHolder> WorkflowTransactionRequest(ChangeRestdayHolder form);
    }
}
