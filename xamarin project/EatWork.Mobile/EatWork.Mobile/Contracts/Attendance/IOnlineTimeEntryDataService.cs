using EatWork.Mobile.Models.FormHolder;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace EatWork.Mobile.Contracts
{
    public interface IOnlineTimeEntryDataService
    {
        Task<OnlineTimeEntryHolder> InitForm();

        Task<OnlineTimeEntryHolder> Transact(OnlineTimeEntryHolder form);

        Task<Location> GetLocation(bool required, int gpsTimeOut = 15);
    }
}