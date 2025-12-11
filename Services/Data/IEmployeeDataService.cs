using MauiHybridApp.Models;
using MauiHybridApp.Models.DataObjects;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public interface IEmployeeDataService
    {
        Task<ObservableCollection<EmployeeListModel>> RetrieveEmployeeList(ObservableCollection<EmployeeListModel> list, ListParam obj);
    }
}
