using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop; // Added to enable extension methods like InvokeVoidAsync

namespace MauiHybridApp.Services.Navigation;

public interface INavigationService
{
    Task NavigateToAsync(string route);
    Task NavigateToAsync(string route, IDictionary<string, object> parameters);
    Task NavigateBackAsync();
    void SetNavigationManager(NavigationManager navigationManager);
    
    // Helper to get parameters for current navigation
    IDictionary<string, object>? GetAndClearParameters();
}

public class NavigationService : INavigationService
{
    private NavigationManager? _navigationManager;
    private readonly Microsoft.JSInterop.IJSRuntime _jsRuntime;
    private IDictionary<string, object>? _currentParameters;

    public NavigationService(Microsoft.JSInterop.IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public void SetNavigationManager(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    public Task NavigateToAsync(string route)
    {
        return NavigateToAsync(route, null);
    }

    public Task NavigateToAsync(string route, IDictionary<string, object>? parameters)
    {
        if (_navigationManager == null)
        {
            // Fallback precaution if SetNavigationManager wasn't called (though it should be)
            // throw new InvalidOperationException("NavigationManager has not been set.");
            // We can't navigate without it.
             return Task.CompletedTask;
        }

        if (parameters != null)
        {
            _currentParameters = parameters;
        }

        _navigationManager.NavigateTo(route);
        return Task.CompletedTask;
    }

    public async Task NavigateBackAsync()
    {
        // Use JS history.back() for proper navigation
        // If history is empty, this might be a no-op, which is safer than forcing "/"
        try
        {
            await _jsRuntime.InvokeVoidAsync("history.back");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Navigation] Back navigation error: {ex.Message}. Fallback to Dashboard.");
            if (_navigationManager != null)
                _navigationManager.NavigateTo("/dashboard");
        }
    }

    public IDictionary<string, object>? GetAndClearParameters()
    {
        var paramsToReturn = _currentParameters;
        _currentParameters = null;
        return paramsToReturn;
    }
}

