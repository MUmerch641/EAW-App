using System.Windows.Input;
using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.Maui.Storage; // Added

namespace MauiHybridApp.ViewModels;

public class ConnectionViewModel : BaseViewModel
{
    private readonly IAuthenticationDataService _authService;
    private readonly NavigationManager _navigationManager;
    
    private string _clientCode = string.Empty;
    private string _passKey = string.Empty;

    public ConnectionViewModel(
        IAuthenticationDataService authService,
        NavigationManager navigationManager)
    {
        _authService = authService;
        _navigationManager = navigationManager;
        
        SetupCommand = new Command(async () => await ExecuteSetupAsync());
    }

    public string ClientCode
    {
        get => _clientCode;
        set => SetProperty(ref _clientCode, value);
    }

    public string PassKey
    {
        get => _passKey;
        set => SetProperty(ref _passKey, value);
    }

    public ICommand SetupCommand { get; }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        // ---------------------------------------------------------
        // CRITICAL FIX: Redirect if already logged in
        // If navigation falls back to "/", we shouldn't show Setup
        // ---------------------------------------------------------
        var token = await SecureStorage.GetAsync("auth_token");
        if (!string.IsNullOrEmpty(token))
        {
            Console.WriteLine("[CONNECTION] Found active token, redirecting to Dashboard");
            // If we have a token, assume we are logged in or let Dashboard verify it
            _navigationManager.NavigateTo("/dashboard", replace: true);
            return;
        }

        // Check if client setup already exists
        if (await _authService.HasClientSetupAsync())
        {
            Console.WriteLine("[CONNECTION] Client setup already exists. Pre-filling fields.");
            
            // Pre-fill fields so user can see/verify
            var setup = await _authService.GetClientSetupAsync();
            if (setup != null)
            {
                ClientCode = setup.ClientCode;
                PassKey = setup.Passkey;
            }
            
            // DO NOT REDIRECT AUTOMATICALLY (Unless logged in!)
            // User wants to land here on app start/logout
        }
    }

    private async Task ExecuteSetupAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            // Validate input
            if (string.IsNullOrWhiteSpace(ClientCode))
            {
                ErrorMessage = "Please enter your Client Code";
                return;
            }

            if (string.IsNullOrWhiteSpace(PassKey))
            {
                ErrorMessage = "Please enter your Pass Key";
                return;
            }

            Console.WriteLine($"[CONNECTION] Submitting setup for client: {ClientCode}");

            var request = new ClientSetupRequest
            {
                ClientCode = ClientCode,
                PassKey = PassKey
            };

            // Call API through service
            var result = await _authService.SetupClientAsync(request);

            if (result.IsSuccess)
            {
                Console.WriteLine("[CONNECTION] ✅ Setup successful, navigating to login");
                _navigationManager.NavigateTo("/login", replace: true);
            }
            else
            {
                Console.WriteLine($"[CONNECTION] ❌ Setup failed: {result.ErrorMessage}");
                ErrorMessage = result.ErrorMessage ?? "Setup failed. Please try again.";
            }
        }
        catch (Exception ex)
        {
            HandleError(ex, "Setup failed. Please check your connection and try again.");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
