using System.Windows.Input;
using MauiHybridApp.Commands;
using MauiHybridApp.Services;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;
using UserProfileModel = MauiHybridApp.Services.Data.UserProfileModel;

namespace MauiHybridApp.ViewModels;

/// <summary>
/// ViewModel for Profile page - handles profile viewing and editing
/// </summary>
public class ProfileViewModel : BaseViewModel
{
    private readonly IUserDataService _userService;
    private readonly IFileUploadService _fileUploadService;
    private readonly NavigationManager _navigationManager;
    
    private UserProfileModel? _userProfile;
    private UserProfileModel? _originalProfile;
    private bool _isEditMode;
    private bool _isSaving;
    private string _successMessage = string.Empty;

    public ProfileViewModel(
        IUserDataService userService,
        IFileUploadService fileUploadService,
        NavigationManager navigationManager)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _fileUploadService = fileUploadService ?? throw new ArgumentNullException(nameof(fileUploadService));
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        
        // Initialize commands
        ToggleEditModeCommand = new RelayCommand(ToggleEditMode);
        SaveCommand = new AsyncRelayCommand(SaveProfileAsync, () => !IsSaving);
        CancelCommand = new RelayCommand(CancelEdit);
        ChangePhotoCommand = new AsyncRelayCommand(ChangePhotoAsync);
        GoBackCommand = new RelayCommand(GoBack);
    }

    #region Properties

    public UserProfileModel? UserProfile
    {
        get => _userProfile;
        private set => SetProperty(ref _userProfile, value);
    }

    public bool IsEditMode
    {
        get => _isEditMode;
        private set => SetProperty(ref _isEditMode, value);
    }

    public bool IsSaving
    {
        get => _isSaving;
        private set => SetProperty(ref _isSaving, value);
    }

    public string SuccessMessage
    {
        get => _successMessage;
        private set => SetProperty(ref _successMessage, value);
    }

    public bool HasSuccessMessage => !string.IsNullOrEmpty(SuccessMessage);

    #endregion

    #region Commands

    public ICommand ToggleEditModeCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand ChangePhotoCommand { get; }
    public ICommand GoBackCommand { get; }

    #endregion

    #region Methods

    public override async Task InitializeAsync()
    {
        await LoadUserProfileAsync();
    }

    private async Task LoadUserProfileAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            UserProfile = await _userService.GetUserProfileAsync();
            
            if (UserProfile != null)
            {
                _originalProfile = CloneProfile(UserProfile);
            }
            
            ClearError();
            SuccessMessage = string.Empty;
        }, "Loading profile...");
    }

    private void ToggleEditMode()
    {
        if (IsEditMode)
        {
            CancelEdit();
        }
        else
        {
            IsEditMode = true;
            ClearError();
            SuccessMessage = string.Empty;
        }
    }

    private void CancelEdit()
    {
        if (_originalProfile != null && UserProfile != null)
        {
            RestoreOriginalValues();
        }
        
        IsEditMode = false;
        ClearError();
        SuccessMessage = string.Empty;
    }

    private async Task SaveProfileAsync()
    {
        if (UserProfile == null) return;

        IsSaving = true;
        ClearError();
        SuccessMessage = string.Empty;

        try
        {
            var result = await _userService.UpdateUserProfileAsync(UserProfile);
            
            if (result.Success)
            {
                _originalProfile = CloneProfile(UserProfile);
                IsEditMode = false;
                SuccessMessage = "Profile updated successfully!";
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? "Failed to update profile. Please try again.";
            }
        }
        catch (Exception ex)
        {
            HandleError(ex, "Error updating profile");
        }
        finally
        {
            IsSaving = false;
        }
    }

    private async Task ChangePhotoAsync()
    {
        try
        {
            // File upload not fully implemented - skip for now
            SuccessMessage = "Photo upload feature coming soon";
        }
        catch (Exception ex)
        {
            HandleError(ex, "Error uploading photo");
        }
    }

    private void GoBack()
    {
        _navigationManager.NavigateTo("/dashboard");
    }

    private UserProfileModel CloneProfile(UserProfileModel profile)
    {
        return new UserProfileModel
        {
            EmployeeId = profile.EmployeeId,
            FullName = profile.FullName,
            Email = profile.Email,
            PhoneNumber = profile.PhoneNumber,
            DateOfBirth = profile.DateOfBirth,
            Address = profile.Address,
            JobTitle = profile.JobTitle,
            Department = profile.Department,
            ManagerName = profile.ManagerName,
            HireDate = profile.HireDate,
            EmploymentStatus = profile.EmploymentStatus,
            WorkLocation = profile.WorkLocation,
            ProfilePhotoUrl = profile.ProfilePhotoUrl,
            EmergencyContactName = profile.EmergencyContactName,
            EmergencyContactRelationship = profile.EmergencyContactRelationship,
            EmergencyContactPhone = profile.EmergencyContactPhone
        };
    }

    private void RestoreOriginalValues()
    {
        if (_originalProfile == null || UserProfile == null) return;

        UserProfile.Email = _originalProfile.Email;
        UserProfile.PhoneNumber = _originalProfile.PhoneNumber;
        UserProfile.DateOfBirth = _originalProfile.DateOfBirth;
        UserProfile.Address = _originalProfile.Address;
        UserProfile.EmergencyContactName = _originalProfile.EmergencyContactName;
        UserProfile.EmergencyContactRelationship = _originalProfile.EmergencyContactRelationship;
        UserProfile.EmergencyContactPhone = _originalProfile.EmergencyContactPhone;
    }

    #endregion

    #region Dispose

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Cleanup
        }
        base.Dispose(disposing);
    }

    #endregion
}
