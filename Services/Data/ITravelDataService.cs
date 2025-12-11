using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MauiHybridApp.Models;

namespace MauiHybridApp.Services.Data
{
    public interface ITravelDataService
    {
        Task<List<TravelRequestListModel>> GetTravelRequestsAsync();
        Task<TravelRequestModel> GetTravelRequestByIdAsync(long id);
        Task<bool> SubmitTravelRequestAsync(TravelRequestModel request);
        Task<TravelInitModel> GetInitDataAsync();
        Task<List<TripTypeModel>> GetTripTypesAsync();
    }
}
