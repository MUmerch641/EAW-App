using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiHybridApp.Models.DataObjects;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private readonly ISettingsDataService _settingsService;
    private readonly NavigationManager _navigationManager;
    private ObservableCollection<SelectableListModel> _filterList = new();

    public SettingsViewModel(
        ISettingsDataService settingsService,
        NavigationManager navigationManager)
    {
        _settingsService = settingsService;
        _navigationManager = navigationManager;

        ToggleFilterCommand = new Command<SelectableListModel>(async (item) => await ToggleFilterAsync(item));
        NavigateToProfileCommand = new Command(() => _navigationManager.NavigateTo("/profile"));
    }

    public ObservableCollection<SelectableListModel> FilterList
    {
        get => _filterList;
        set => SetProperty(ref _filterList, value);
    }

    public string VersionNumber => "1.0.0";
    public string BuildNumber => "26";

    public ICommand ToggleFilterCommand { get; }
    public ICommand NavigateToProfileCommand { get; }

    public override async Task InitializeAsync()
    {
        await LoadSettingsAsync();
    }

    private async Task LoadSettingsAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            FilterList = await _settingsService.EmployeeFilterConfig();
        }, "Loading settings...");
    }

    private async Task ToggleFilterAsync(SelectableListModel item)
    {
        if (item == null) return;
        
        // Toggle the value (it might be bound, but let's ensure logic)
        // item.IsChecked is likely bound to the switch, but we need to call service
        
        await _settingsService.UpdateEmployeeFilterSetup(item);
    }
}
