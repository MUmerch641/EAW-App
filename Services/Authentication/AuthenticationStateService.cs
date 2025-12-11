using MauiHybridApp.Models;
using MauiHybridApp.Utils;
using Newtonsoft.Json;
using MauiHybridApp.Services.Data;

namespace MauiHybridApp.Services.Authentication;

public class AuthenticationStateService
{
    private UserModel? _currentUser;
    
    private readonly IAuthenticationDataService _authService;

    public AuthenticationStateService(IAuthenticationDataService authService)
    {
        _authService = authService;
    }

    public event Action? OnAuthenticationStateChanged;

    public bool IsAuthenticated => FormSession.IsLoggedIn;

    public bool IsDemoMode => FormSession.TokenBearer == "demo-token-123";

    public UserModel? CurrentUser
    {
        get
        {
            if (_currentUser == null && IsAuthenticated)
            {
                var userJson = FormSession.UserInfo;
                _currentUser = JsonConvert.DeserializeObject<UserModel>(userJson);
            }
            return _currentUser;
        }
        private set
        {
            _currentUser = value;
            OnAuthenticationStateChanged?.Invoke();
        }
    }

    public async Task LoginAsync(UserModel user, string token)
    {
        FormSession.UserInfo = JsonConvert.SerializeObject(user);
        FormSession.TokenBearer = token;
        FormSession.IsLoggedIn = true;
        CurrentUser = user;
        
        await Task.CompletedTask;
    }

    public async Task LogoutAsync()
    {
        // Call API logout and clear secure storage
        await _authService.LogoutAsync();
        
        // Clear local session
        FormSession.ClearEverything();
        CurrentUser = null;
    }

    public async Task InitializeAsync()
    {
        try 
        {
            var token = await SecureStorage.GetAsync("auth_token");
            if (!string.IsNullOrEmpty(token))
            {
                FormSession.TokenBearer = token;
                FormSession.IsLoggedIn = true;
                
                // Try restore user info if we saved it (we didn't explicitly save UserInfo to SecureStorage in Login, only AuthToken/ProfileId)
                // But we can construct a basic user model or fetch it.
                // For now, at least set IsLoggedIn to true so MainLayout renders correctly.
                
                var pid = await SecureStorage.GetAsync("profile_id");
                if (!string.IsNullOrEmpty(pid) && long.TryParse(pid, out var profileId))
                {
                     // Create a minimal user to satisfy checks
                     if (_currentUser == null)
                     {
                         _currentUser = new UserModel 
                         { 
                             ProfileId = profileId,
                             Username = "Restored User" 
                         };
                     }
                }
                
                OnAuthenticationStateChanged?.Invoke();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Auth Init Error: {ex.Message}");
        }
    }

    public string GetAuthToken()
    {
        return FormSession.TokenBearer;
    }
}

