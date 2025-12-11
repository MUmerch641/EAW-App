using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MauiHybridApp.Models;

namespace MauiHybridApp.Services.Data
{
    public interface IUndertimeDataService
    {
        Task<List<UndertimeRequestListModel>> GetUndertimeRequestsAsync();
        Task<UndertimeRequestModel> GetUndertimeRequestByIdAsync(long id);
        Task<bool> SubmitUndertimeRequestAsync(UndertimeRequestModel request);
        Task<List<UndertimeTypeModel>> GetUndertimeTypesAsync();
    }
}
