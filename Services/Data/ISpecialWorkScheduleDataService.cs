using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MauiHybridApp.Models;

namespace MauiHybridApp.Services.Data
{
    public interface ISpecialWorkScheduleDataService
    {
        Task<List<SpecialWorkScheduleListModel>> GetSpecialWorkScheduleRequestsAsync();
        Task<List<ShiftModel>> GetShiftsAsync();
        Task<ShiftModel> GetShiftByIdAsync(long id);
        Task<bool> SubmitSpecialWorkScheduleRequestAsync(SpecialWorkScheduleRequestModel request);
    }
}
