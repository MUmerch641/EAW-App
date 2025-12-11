using System.Windows.Input;
using MauiHybridApp.Commands;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels;

public class PersonalDetailsViewModel : BaseViewModel
{
    private readonly IUserDataService _userService;
    private UserProfileModel _userProfile;

    public PersonalDetailsViewModel(IUserDataService userService)
    {
        _userService = userService;
        SaveCommand = new AsyncRelayCommand(SaveAsync);
    }

    public UserProfileModel UserProfile
    {
        get => _userProfile;
        private set => SetProperty(ref _userProfile, value);
    }

    public ICommand SaveCommand { get; }

    public override async Task InitializeAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            UserProfile = await _userService.GetUserProfileAsync();
        }, "Loading details...");
    }

    private async Task SaveAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            var result = await _userService.UpdateUserProfileAsync(UserProfile);
            if (result.Success)
                SuccessMessage = "Personal details updated successfully!";
            else
                ErrorMessage = result.ErrorMessage ?? "Failed to update.";
        }, "Saving...");
    }
}

public class ContactInformationViewModel : BaseViewModel
{
    private readonly IUserDataService _userService;
    private UserProfileModel _userProfile;

    public ContactInformationViewModel(IUserDataService userService)
    {
        _userService = userService;
        SaveCommand = new AsyncRelayCommand(SaveAsync);
    }

    public UserProfileModel UserProfile
    {
        get => _userProfile;
        private set => SetProperty(ref _userProfile, value);
    }

    public ICommand SaveCommand { get; }

    public override async Task InitializeAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            UserProfile = await _userService.GetUserProfileAsync();
        }, "Loading contact info...");
    }

    private async Task SaveAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            var result = await _userService.UpdateUserProfileAsync(UserProfile);
            if (result.Success)
                SuccessMessage = "Contact information updated successfully!";
            else
                ErrorMessage = result.ErrorMessage ?? "Failed to update.";
        }, "Saving...");
    }
}

public class FamilyBackgroundViewModel : BaseViewModel
{
    // Placeholder for now as UserProfileModel might not have these fields yet
    // Will use UserProfileModel for now
    private readonly IUserDataService _userService;
    private UserProfileModel _userProfile;

    public FamilyBackgroundViewModel(IUserDataService userService)
    {
        _userService = userService;
    }

    public UserProfileModel UserProfile
    {
        get => _userProfile;
        private set => SetProperty(ref _userProfile, value);
    }

    public override async Task InitializeAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            UserProfile = await _userService.GetUserProfileAsync();
        }, "Loading family background...");
    }
}

public class EmergencyContactViewModel : BaseViewModel
{
    private readonly IUserDataService _userService;
    private UserProfileModel _userProfile;

    public EmergencyContactViewModel(IUserDataService userService)
    {
        _userService = userService;
        SaveCommand = new AsyncRelayCommand(SaveAsync);
    }

    public UserProfileModel UserProfile
    {
        get => _userProfile;
        private set => SetProperty(ref _userProfile, value);
    }

    public ICommand SaveCommand { get; }

    public override async Task InitializeAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            UserProfile = await _userService.GetUserProfileAsync();
        }, "Loading emergency contact...");
    }

    private async Task SaveAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            var result = await _userService.UpdateUserProfileAsync(UserProfile);
            if (result.Success)
                SuccessMessage = "Emergency contact updated successfully!";
            else
                ErrorMessage = result.ErrorMessage ?? "Failed to update.";
        }, "Saving...");
    }
}

public class CurrentJobInformationViewModel : BaseViewModel
{
    private readonly IUserDataService _userService;
    private UserProfileModel _userProfile;

    public CurrentJobInformationViewModel(IUserDataService userService)
    {
        _userService = userService;
    }

    public UserProfileModel UserProfile
    {
        get => _userProfile;
        private set => SetProperty(ref _userProfile, value);
    }

    public override async Task InitializeAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            UserProfile = await _userService.GetUserProfileAsync();
        }, "Loading job info...");
    }
}

public class AreaOfAssignmentViewModel : BaseViewModel
{
    private readonly IUserDataService _userService;
    private UserProfileModel _userProfile;

    public AreaOfAssignmentViewModel(IUserDataService userService)
    {
        _userService = userService;
    }

    public UserProfileModel UserProfile
    {
        get => _userProfile;
        private set => SetProperty(ref _userProfile, value);
    }

    public override async Task InitializeAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            UserProfile = await _userService.GetUserProfileAsync();
        }, "Loading assignment info...");
    }
}

public class RelevantEmploymentDatesViewModel : BaseViewModel
{
    private readonly IUserDataService _userService;
    private UserProfileModel _userProfile;

    public RelevantEmploymentDatesViewModel(IUserDataService userService)
    {
        _userService = userService;
    }

    public UserProfileModel UserProfile
    {
        get => _userProfile;
        private set => SetProperty(ref _userProfile, value);
    }

    public override async Task InitializeAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            UserProfile = await _userService.GetUserProfileAsync();
        }, "Loading dates...");
    }
}

public class TaxGovernmentRelatedInfoViewModel : BaseViewModel
{
    private readonly IUserDataService _userService;
    private UserProfileModel _userProfile;

    public TaxGovernmentRelatedInfoViewModel(IUserDataService userService)
    {
        _userService = userService;
    }

    public UserProfileModel UserProfile
    {
        get => _userProfile;
        private set => SetProperty(ref _userProfile, value);
    }

    public override async Task InitializeAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            UserProfile = await _userService.GetUserProfileAsync();
        }, "Loading tax info...");
    }
}
