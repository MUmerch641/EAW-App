using MauiHybridApp.Models;
using MauiHybridApp.Utils;
using Newtonsoft.Json;

namespace MauiHybridApp.Services.Authentication;

public class AuthenticationStateService
{
    private UserModel? _currentUser;
    
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
        FormSession.ClearEverything();
        CurrentUser = null;
        
        await Task.CompletedTask;
    }

    public string GetAuthToken()
    {
        return FormSession.TokenBearer;
    }
}

