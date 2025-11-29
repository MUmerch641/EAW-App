using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.Services.Navigation;

public interface INavigationService
{
    Task NavigateToAsync(string route);
    Task NavigateBackAsync();
    void SetNavigationManager(NavigationManager navigationManager);
}

public class NavigationService : INavigationService
{
    private NavigationManager? _navigationManager;

    public void SetNavigationManager(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    public Task NavigateToAsync(string route)
    {
        if (_navigationManager == null)
        {
            throw new InvalidOperationException("NavigationManager has not been set. Call SetNavigationManager first.");
        }

        _navigationManager.NavigateTo(route);
        return Task.CompletedTask;
    }

    public Task NavigateBackAsync()
    {
        // Blazor doesn't have built-in back navigation
        // This would need to be implemented with a navigation stack
        // For now, navigate to home
        return NavigateToAsync("/");
    }
}

