using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MauiHybridApp.Models;

namespace MauiHybridApp.Services.Data
{
    public interface IScheduleDataService
    {
        Task<List<MyScheduleListModel>> RetrieveMyScheduleListAsync(DateTime startDate, DateTime endDate);
        Task<bool> SubmitWorkScheduleChangeAsync(ChangeWorkScheduleRequest request);
        Task<bool> SubmitRestDayChangeAsync(ChangeRestDayRequest request);
    }
}
