using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiHybridApp.Commands;
using MauiHybridApp.Models;
using MauiHybridApp.Models.Attendance;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Services.Navigation;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels;

/// <summary>
/// ViewModel for Time Entry page (Clock In/Out functionality)
/// </summary>
public class TimeEntryViewModel : BaseViewModel
{
    private readonly ITimeEntryDataService _timeEntryService;
    private readonly INavigationService _navigationService;
    private System.Threading.Timer? _timeUpdateTimer;
    
    private ObservableCollection<TimeEntryLogItem> _timeEntries;
    private TimeEntryLogItem? _currentTimeEntry;
    private bool _isCurrentlyClockedIn;
    private DateTime _currentTime;
    private string _currentLocation;
    private string _successMessage = string.Empty;

    // Filter Properties
    private bool _showFilter;
    private DateTime _filterStartDate = DateTime.Today.AddDays(-30); // Default to last 30 days
    private DateTime _filterEndDate = DateTime.Today;
    private ObservableCollection<SelectionItem> _filterStatuses;

    public TimeEntryViewModel(
        ITimeEntryDataService timeEntryService,
        INavigationService navigationService)
    {
        _timeEntryService = timeEntryService ?? throw new ArgumentNullException(nameof(timeEntryService));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        
        _timeEntries = new ObservableCollection<TimeEntryLogItem>();
        _currentTime = DateTime.Now;
        _currentLocation = "Getting location...";
        
        ClockInCommand = new AsyncRelayCommand(ClockInAsync, () => !IsCurrentlyClockedIn && !IsBusy);
        ClockOutCommand = new AsyncRelayCommand(ClockOutAsync, () => IsCurrentlyClockedIn && !IsBusy);
        RefreshCommand = new AsyncRelayCommand(LoadTimeEntriesAsync);
        
        ApplyFilterCommand = new AsyncRelayCommand(ApplyFilterAsync);
        ResetFilterCommand = new Command(ResetFilter);
        
        // Initialize Status Filters
        _filterStatuses = new ObservableCollection<SelectionItem>
        {
            new SelectionItem { Name = "Calculated", Value = "Calculated", IsSelected = true },
            new SelectionItem { Name = "Pending", Value = "Pending", IsSelected = true },
            new SelectionItem { Name = "Posted", Value = "Posted", IsSelected = true }
        };
    }

    #region Properties

    public ObservableCollection<TimeEntryLogItem> TimeEntries
    {
        get => _timeEntries;
        private set => SetProperty(ref _timeEntries, value);
    }

    public TimeEntryLogItem? CurrentTimeEntry
    {
        get => _currentTimeEntry;
        private set => SetProperty(ref _currentTimeEntry, value);
    }

    public bool IsCurrentlyClockedIn
    {
        get => _isCurrentlyClockedIn;
        private set
        {
            if (SetProperty(ref _isCurrentlyClockedIn, value))
            {
                RaiseCommandsCanExecuteChanged();
            }
        }
    }

    public DateTime CurrentTime
    {
        get => _currentTime;
        private set => SetProperty(ref _currentTime, value);
    }

    public string CurrentLocation
    {
        get => _currentLocation;
        private set => SetProperty(ref _currentLocation, value);
    }

    public string CurrentTimeFormatted => CurrentTime.ToString("hh:mm:ss tt");
    public string CurrentDateFormatted => CurrentTime.ToString("dddd, MMMM dd, yyyy");
    
    public string StatusText => IsCurrentlyClockedIn ? "Clocked In" : "Clocked Out";
     public string StatusClass => IsCurrentlyClockedIn ? "status-active" : "status-inactive";

    // Filter View Properties
    public bool ShowFilter
    {
        get => _showFilter;
        set => SetProperty(ref _showFilter, value);
    }

    public DateTime FilterStartDate
    {
        get => _filterStartDate;
        set => SetProperty(ref _filterStartDate, value);
    }

    public DateTime FilterEndDate
    {
        get => _filterEndDate;
        set => SetProperty(ref _filterEndDate, value);
    }

    public ObservableCollection<SelectionItem> FilterStatuses
    {
        get => _filterStatuses;
        set => SetProperty(ref _filterStatuses, value);
    }

    // Dynamic UI Properties
    private string _timeInColor = "#27ae60"; // Default Success
    private string _timeOutColor = "#EE3F60"; // Default Danger

    public string TimeInColor
    {
        get => _timeInColor;
        set => SetProperty(ref _timeInColor, value);
    }

    public string TimeOutColor
    {
        get => _timeOutColor;
        set => SetProperty(ref _timeOutColor, value);
    }
    #endregion

    #region Commands

    public ICommand ClockInCommand { get; }
    public ICommand ClockOutCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand ApplyFilterCommand { get; }
    public ICommand ResetFilterCommand { get; }

    #endregion

    #region Methods

    public override async Task InitializeAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            // 1. Initialize Form (Get Server Config)
            var formHolder = await _timeEntryService.InitFormAsync();
            if (formHolder.HasSetup)
            {
                // Apply colors from server config
                if (!string.IsNullOrEmpty(formHolder.TimeInColor)) 
                    TimeInColor = formHolder.TimeInColor;
                
                if (!string.IsNullOrEmpty(formHolder.TimeOutColor)) 
                    TimeOutColor = formHolder.TimeOutColor;
            }
            else if (!string.IsNullOrEmpty(formHolder.UserError))
            {
                ErrorMessage = formHolder.UserError;
            }

            // 2. Load History
            await LoadTimeEntriesAsync();
            
            // 3. Start Timer
            StartTimeUpdater();
            
            // 4. Get Location
            await GetCurrentLocationAsync();
        }, "Loading time entries...");
    }

    private async Task LoadTimeEntriesAsync()
    {
        try
        {
            // Build status string from selected filters
            var selectedStatuses = FilterStatuses.Where(s => s.IsSelected).Select(s => s.Value).ToList();
            string statusFilter = string.Join(",", selectedStatuses);

            var entries = await _timeEntryService.GetTimeEntriesAsync(FilterStartDate, FilterEndDate, statusFilter);
            var allEntries = entries ?? new List<TimeEntryLogItem>();

            // Remove internal filtering since service now handles it (partially client-side, partially API)
            // But we update the ObservableCollection directly
            TimeEntries = new ObservableCollection<TimeEntryLogItem>(allEntries);
            
            // ---------------------------------------------------------
            // CRITICAL FIX: Fetch Unfiltered Status for Today
            // The History List respects filters, but the "Clock In" button state 
            // must respect REALITY. We fetch today's logs without filters to check state.
            // ---------------------------------------------------------
            var today = DateTime.Today;
            var todayEntries = await _timeEntryService.GetTimeEntriesAsync(today, today, null); // Pass null to get ALL statuses
            
            var todayInEntries = todayEntries.Where(e => e.Type == "Time-In").ToList();
            var todayOutEntries = todayEntries.Where(e => e.Type == "Time-Out").ToList();
            
            IsCurrentlyClockedIn = todayInEntries.Count > todayOutEntries.Count;
            
            if (IsCurrentlyClockedIn && todayInEntries.Any())
            {
                CurrentTimeEntry = todayInEntries.Last();
            }
            else
            {
                CurrentTimeEntry = null;
            }
            
            ClearError();
        }
        catch (Exception ex)
        {
            HandleError(ex, "Unable to load time entries. Please check your connection.");
        }
    }

    private async Task ApplyFilterAsync()
    {
        ShowFilter = false;
        await LoadTimeEntriesAsync();
    }

    private void ResetFilter()
    {
        FilterStartDate = DateTime.Today.AddDays(-30);
        FilterEndDate = DateTime.Today;
        
        foreach (var item in FilterStatuses)
        {
            item.IsSelected = true;
        }
        
        // Don't close, just reset values? Or close and reload? 
        // Typically reset just resets values. User hits Filter to apply.
        // But let's reload to be helpful or just let them click Filter.
        // Let's just reset values.
    }

    private void StartTimeUpdater()
    {
        _timeUpdateTimer?.Dispose();
        _timeUpdateTimer = new System.Threading.Timer(_ =>
        {
            CurrentTime = DateTime.Now;
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    private async Task GetCurrentLocationAsync()
    {
        try
        {
            CurrentLocation = "Getting location...";

            // 1. Check and Request Permissions
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (status != PermissionStatus.Granted)
            {
                CurrentLocation = "Location permission denied";
                return;
            }

            // 2. Get Location
            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var location = await Geolocation.Default.GetLocationAsync(request);

            if (location != null)
            {
                CurrentLocation = $"Lat: {location.Latitude:F4}, Lng: {location.Longitude:F4}";
            }
            else
            {
                CurrentLocation = "Location unavailable";
            }
        }
        catch (FeatureNotSupportedException)
        {
            CurrentLocation = "Location not supported";
        }
        catch (FeatureNotEnabledException)
        {
            CurrentLocation = "Location disabled";
        }
        catch (Exception ex)
        {
            CurrentLocation = "Location unavailable";
            System.Diagnostics.Debug.WriteLine($"Error getting location: {ex.Message}");
        }
    }

    private async Task ClockInAsync()
    {
        try
        {
            ClearError();
            SuccessMessage = string.Empty;

            double lat = 0.0;
            double lng = 0.0;

            // 1. Check Permissions
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (status == PermissionStatus.Granted)
            {
                try 
                {
                    // Try to get fresh location
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(5));
                    var location = await Geolocation.Default.GetLocationAsync(request);
                    
                    if (location != null)
                    {
                        lat = location.Latitude;
                        lng = location.Longitude;
                        
                        // Update UI location too
                        CurrentLocation = $"Lat: {lat:F4}, Lng: {lng:F4}";
                    }
                }
                catch 
                { 
                    // Fallback to last known
                    try
                    {
                        var location = await Geolocation.Default.GetLastKnownLocationAsync();
                        if (location != null)
                        {
                            lat = location.Latitude;
                            lng = location.Longitude;
                        }
                    }
                    catch { /* Give up */ }
                }
            }
            else
            {
                CurrentLocation = "Location permission denied";
                // We proceed with 0,0 but maybe we should stop? 
                // For now, let's proceed as the backend seems to allow it, but at least we tried to ask.
            }

            var result = await _timeEntryService.CreateTimeEntryAsync("Time-In", lat, lng);

            if (result.Success)
            {
                SuccessMessage = result.ErrorMessage ?? "Successfully clocked in!"; 
                await LoadTimeEntriesAsync();
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? "Failed to clock in. Please try again.";
            }
        }
        catch (Exception ex)
        {
            HandleError(ex, "Error clocking in");
        }
    }

    private async Task ClockOutAsync()
    {
        try
        {
            ClearError();
            SuccessMessage = string.Empty;

            double lat = 0.0;
            double lng = 0.0;

            // 1. Check Permissions
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (status == PermissionStatus.Granted)
            {
                try 
                {
                    // Try to get fresh location
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(5));
                    var location = await Geolocation.Default.GetLocationAsync(request);
                    
                    if (location != null)
                    {
                        lat = location.Latitude;
                        lng = location.Longitude;
                        
                        // Update UI location too
                        CurrentLocation = $"Lat: {lat:F4}, Lng: {lng:F4}";
                    }
                }
                catch 
                { 
                    // Fallback to last known
                    try
                    {
                        var location = await Geolocation.Default.GetLastKnownLocationAsync();
                        if (location != null)
                        {
                            lat = location.Latitude;
                            lng = location.Longitude;
                        }
                    }
                    catch { /* Give up */ }
                }
            }
            else
            {
                CurrentLocation = "Location permission denied";
            }

            var result = await _timeEntryService.CreateTimeEntryAsync("Time-Out", lat, lng);

            if (result.Success)
            {
                SuccessMessage = result.ErrorMessage ?? "Successfully clocked out!";
                await LoadTimeEntriesAsync();
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? "Failed to clock out. Please try again.";
            }
        }
        catch (Exception ex)
        {
            HandleError(ex, "Error clocking out");
        }
    }

    private void RaiseCommandsCanExecuteChanged()
    {
        (ClockInCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (ClockOutCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _timeUpdateTimer?.Dispose();
            _timeUpdateTimer = null;
        }
        base.Dispose(disposing);
    }

    #endregion
}

public class SelectionItem : BaseViewModel
{
    private string _name;
    private string _value;
    private bool _isSelected;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }
    
    public string Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }
    
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}
