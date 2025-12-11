using MauiHybridApp.Models;
using MauiHybridApp.Models.DataObjects;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public interface IMyRequestDataService
    {
        long TotalListItem { get; set; }
        Task<ObservableCollection<MyRequestListModel>> RetrieveMyRequestList(ObservableCollection<MyRequestListModel> list, ListParam obj);
    }
}
