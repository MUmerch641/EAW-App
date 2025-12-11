using System.Collections.ObjectModel;
using MauiHybridApp.Models.DataObjects;

namespace MauiHybridApp.Services.Data;

public interface ISettingsDataService
{
    Task<ObservableCollection<SelectableListModel>> EmployeeFilterConfig();
    Task UpdateEmployeeFilterSetup(SelectableListModel model);
}
