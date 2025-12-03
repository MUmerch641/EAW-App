using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiHybridApp.Commands;
using MauiHybridApp.Models;
using MauiHybridApp.Models.Attendance;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels;

/// <summary>
/// ViewModel for Time Entry page (Clock In/Out functionality)
/// </summary>
public class TimeEntryViewModel : BaseViewModel
{
    private readonly ITimeEntryDataService _timeEntryService;
    private readonly NavigationManager _navigationManager;
    private System.Threading.Timer? _timeUpdateTimer;
    
    private ObservableCollection<TimeEntryLogItem> _timeEntries;
    private TimeEntryLogItem? _currentTimeEntry;
    private bool _isCurrentlyClockedIn;
    private DateTime _currentTime;
    private string _currentLocation;
    private string _successMessage = string.Empty;

    public TimeEntryViewModel(
        ITimeEntryDataService timeEntryService,
        NavigationManager navigationManager)
    {
        _timeEntryService = timeEntryService ?? throw new ArgumentNullException(nameof(timeEntryService));
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        
        _timeEntries = new ObservableCollection<TimeEntryLogItem>();
        _currentTime = DateTime.Now;
        _currentLocation = "Getting location...";
        
        ClockInCommand = new AsyncRelayCommand(ClockInAsync, () => !IsCurrentlyClockedIn && !IsBusy);
        ClockOutCommand = new AsyncRelayCommand(ClockOutAsync, () => IsCurrentlyClockedIn && !IsBusy);
        RefreshCommand = new AsyncRelayCommand(LoadTimeEntriesAsync);
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

    public string SuccessMessage
    {
        get => _successMessage;
        private set => SetProperty(ref _successMessage, value);
    }

    public string CurrentTimeFormatted => CurrentTime.ToString("hh:mm:ss tt");
    public string CurrentDateFormatted => CurrentTime.ToString("dddd, MMMM dd, yyyy");
    
    public string StatusText => IsCurrentlyClockedIn ? "Clocked In" : "Clocked Out";
    public string StatusClass => IsCurrentlyClockedIn ? "status-active" : "status-inactive";

    #endregion

    #region Commands

    public ICommand ClockInCommand { get; }
    public ICommand ClockOutCommand { get; }
    public ICommand RefreshCommand { get; }

    #endregion

    #region Methods

    public async Task InitializeAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            await LoadTimeEntriesAsync();
            StartTimeUpdater();
            await GetCurrentLocationAsync();
        }, "Loading time entries...");
    }

    private async Task LoadTimeEntriesAsync()
    {
        try
        {
            var entries = await _timeEntryService.GetTimeEntriesAsync();
            TimeEntries = new ObservableCollection<TimeEntryLogItem>(entries ?? new List<TimeEntryLogItem>());
            
            // Find current active entry
            var today = DateTime.Today;
            var todayInEntries = TimeEntries.Where(e => e.TimeEntry.Date == today && e.Type == "Time-In").ToList();
            var todayOutEntries = TimeEntries.Where(e => e.TimeEntry.Date == today && e.Type == "Time-Out").ToList();
            
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
            await Task.Delay(1000); // Simulate location fetch
            CurrentLocation = "Office Location - Main Building";
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

            // TODO: Get actual GPS coordinates
            double lat = 0.0;
            double lng = 0.0;

            var result = await _timeEntryService.CreateTimeEntryAsync("Time-In", lat, lng);

            if (result.Success)
            {
                SuccessMessage = "Successfully clocked in!";
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

            // TODO: Get actual GPS coordinates
            double lat = 0.0;
            double lng = 0.0;

            var result = await _timeEntryService.CreateTimeEntryAsync("Time-Out", lat, lng);

            if (result.Success)
            {
                SuccessMessage = "Successfully clocked out!";
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
