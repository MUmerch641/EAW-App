using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MauiHybridApp.Models.Attendance;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Utils;
using MauiHybridApp.Models;

namespace MauiHybridApp.Components.Pages;

public partial class TimeEntry : ComponentBase, IDisposable
{
    private List<TimeEntryLogItem>? todayEntries;
    private TimeEntryLogItem? currentTimeEntry;
    private bool isCurrentlyClockedIn = false;
    
    private bool isLoading = true;
    private bool isSubmitting = false;
    private DateTime currentTime = DateTime.Now;
    private string? currentLocation = "Getting location...";
    private string errorMessage = string.Empty;
    private string successMessage = string.Empty;
    
    private Timer? timeUpdateTimer;

    protected override async Task OnInitializedAsync()
    {
        await LoadTimeEntries();
        StartTimeUpdater();
        await GetCurrentLocation();
    }

    private async Task LoadTimeEntries()
    {
        try
        {
            isLoading = true;
            StateHasChanged();

            // Load time entries (last 30 days)
            todayEntries = await TimeEntryService.GetTimeEntriesAsync();
            
            // Find current active entry (clocked in but not clocked out)
            // Look for today's entries that are "Time-In" but no corresponding "Time-Out"
            var today = DateTime.Today;
            var todayInEntries = todayEntries?.Where(e => e.TimeEntry.Date == today && e.Type == "Time-In").ToList();
            var todayOutEntries = todayEntries?.Where(e => e.TimeEntry.Date == today && e.Type == "Time-Out").ToList();
            
            // If we have more Time-In than Time-Out, user is still clocked in
            isCurrentlyClockedIn = (todayInEntries?.Count ?? 0) > (todayOutEntries?.Count ?? 0);
            
            if (isCurrentlyClockedIn && todayInEntries?.Any() == true)
            {
                currentTimeEntry = todayInEntries.Last(); // Get the last Time-In entry
            }

            errorMessage = string.Empty;
            successMessage = string.Empty;
        }
        catch (Exception ex)
        {
            errorMessage = $"Unable to load time entries. Please check your internet connection and try again.";
            System.Diagnostics.Debug.WriteLine($"LoadTimeEntries error: {ex.Message}");
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private void StartTimeUpdater()
    {
        timeUpdateTimer = new Timer(async _ =>
        {
            currentTime = DateTime.Now;
            await InvokeAsync(StateHasChanged);
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    private async Task GetCurrentLocation()
    {
        try
        {
            // In a real app, you would use geolocation services
            // For now, we'll use a placeholder
            await Task.Delay(1000);
            currentLocation = "Office Location - Main Building";
            StateHasChanged();
        }
        catch (Exception ex)
        {
            currentLocation = "Location unavailable";
            Console.WriteLine($"Error getting location: {ex.Message}");
        }
    }

    private async Task ClockIn()
    {
        if (isSubmitting) return;

        try
        {
            isSubmitting = true;
            errorMessage = string.Empty;
            successMessage = string.Empty;
            StateHasChanged();

            // Get current location coordinates (placeholder for now)
            double lat = 0.0; // TODO: Get actual GPS coordinates
            double lng = 0.0; // TODO: Get actual GPS coordinates

            var result = await TimeEntryService.CreateTimeEntryAsync("Time-In", lat, lng);

            if (result.Success)
            {
                successMessage = "Successfully clocked in!";
                await LoadTimeEntries(); // Refresh the entries
            }
            else
            {
                errorMessage = result.ErrorMessage ?? "Failed to clock in.";
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Error clocking in: {ex.Message}";
        }
        finally
        {
            isSubmitting = false;
            StateHasChanged();
        }
    }

    private async Task ClockOut()
    {
        if (isSubmitting) return;

        try
        {
            isSubmitting = true;
            errorMessage = string.Empty;
            successMessage = string.Empty;
            StateHasChanged();

            // Get current location coordinates (placeholder for now)
            double lat = 0.0; // TODO: Get actual GPS coordinates
            double lng = 0.0; // TODO: Get actual GPS coordinates

            var result = await TimeEntryService.CreateTimeEntryAsync("Time-Out", lat, lng);

            if (result.Success)
            {
                successMessage = "Successfully clocked out!";
                await LoadTimeEntries(); // Refresh the entries
            }
            else
            {
                errorMessage = result.ErrorMessage ?? "Failed to clock out.";
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Error clocking out: {ex.Message}";
        }
        finally
        {
            isSubmitting = false;
            StateHasChanged();
        }
    }

    private string GetWorkDuration()
    {
        if (isCurrentlyClockedIn && currentTimeEntry != null)
        {
            var duration = DateTime.Now - currentTimeEntry.TimeEntry;
            return duration.ToString(@"hh\:mm\:ss");
        }
        return "00:00:00";
    }

    private string GetTotalHoursToday()
    {
        if (todayEntries == null || !todayEntries.Any())
            return "00:00";

        var today = DateTime.Today;
        var todayEntriesOnly = todayEntries.Where(e => e.TimeEntry.Date == today).ToList();
        
        // Group by day and calculate total time
        // For simplicity, we'll pair Time-In with next Time-Out
        var totalMinutes = 0.0;
        TimeEntryLogItem? lastInEntry = null;
        
        foreach (var entry in todayEntriesOnly.OrderBy(e => e.TimeEntry))
        {
            if (entry.Type == "Time-In")
            {
                lastInEntry = entry;
            }
            else if (entry.Type == "Time-Out" && lastInEntry != null)
            {
                var duration = entry.TimeEntry - lastInEntry.TimeEntry;
                totalMinutes += duration.TotalMinutes;
                lastInEntry = null; // Reset for next pair
            }
        }
        
        // If still clocked in, add current session time
        if (isCurrentlyClockedIn && lastInEntry != null)
        {
            var currentDuration = DateTime.Now - lastInEntry.TimeEntry;
            totalMinutes += currentDuration.TotalMinutes;
        }

        var hours = (int)(totalMinutes / 60);
        var minutes = (int)(totalMinutes % 60);
        
        return $"{hours:D2}:{minutes:D2}";
    }

    private async Task GoBack()
    {
        await JSRuntime.InvokeVoidAsync("history.back");
    }

    public void Dispose()
    {
        timeUpdateTimer?.Dispose();
    }
}
