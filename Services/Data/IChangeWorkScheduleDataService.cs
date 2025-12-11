using MauiHybridApp.Models;
using System;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public interface IChangeWorkScheduleDataService
    {
        Task<ChangeWorkScheduleHolder> InitForm(long recordId, DateTime? selectedDate);

        Task<ChangeWorkScheduleHolder> SubmitRequest(ChangeWorkScheduleHolder form);

        Task<ChangeWorkScheduleHolder> GetEmployeeSchedule(ChangeWorkScheduleHolder form, int option);
        
        Task<ChangeWorkScheduleHolder> WorkflowTransactionRequest(ChangeWorkScheduleHolder form);
    }
}
