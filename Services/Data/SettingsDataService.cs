using System.Collections.ObjectModel;
using MauiHybridApp.Models.DataObjects;

namespace MauiHybridApp.Services.Data;

public class SettingsDataService : ISettingsDataService
{
    private readonly IGenericRepository _repository;

    public SettingsDataService(IGenericRepository repository)
    {
        _repository = repository;
    }

    public async Task<ObservableCollection<SelectableListModel>> EmployeeFilterConfig()
    {
        // Simulate API call
        await Task.Delay(500);

        // Mock Data based on Xamarin app
        return new ObservableCollection<SelectableListModel>
        {
            new SelectableListModel { Id = 1, DisplayText = "Show Active Employees", IsChecked = true },
            new SelectableListModel { Id = 2, DisplayText = "Show Resigned Employees", IsChecked = false },
            new SelectableListModel { Id = 3, DisplayText = "Show On Leave", IsChecked = true },
            new SelectableListModel { Id = 4, DisplayText = "Show Remote Workers", IsChecked = true }
        };
    }

    public async Task UpdateEmployeeFilterSetup(SelectableListModel model)
    {
        // Simulate API call to update filter
        await Task.Delay(200);
        Console.WriteLine($"Updated filter: {model.DisplayText} -> {model.IsChecked}");
    }
}
