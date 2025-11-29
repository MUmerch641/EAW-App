using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MauiHybridApp.Services.Data;
using UserProfileModel = MauiHybridApp.Services.Data.UserProfileModel;

namespace MauiHybridApp.Components.Pages;

public partial class Profile : ComponentBase
{
    private UserProfileModel? userProfile;
    private UserProfileModel? originalProfile;
    
    private bool isLoading = true;
    private bool isEditMode = false;
    private bool isSaving = false;
    private string errorMessage = string.Empty;
    private string successMessage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadUserProfile();
    }

    private async Task LoadUserProfile()
    {
        try
        {
            isLoading = true;
            StateHasChanged();

            userProfile = await UserService.GetUserProfileAsync();
            
            // Create a copy for edit mode
            if (userProfile != null)
            {
                originalProfile = new UserProfileModel
                {
                    EmployeeId = userProfile.EmployeeId,
                    FullName = userProfile.FullName,
                    Email = userProfile.Email,
                    PhoneNumber = userProfile.PhoneNumber,
                    DateOfBirth = userProfile.DateOfBirth,
                    Address = userProfile.Address,
                    JobTitle = userProfile.JobTitle,
                    Department = userProfile.Department,
                    ManagerName = userProfile.ManagerName,
                    HireDate = userProfile.HireDate,
                    EmploymentStatus = userProfile.EmploymentStatus,
                    WorkLocation = userProfile.WorkLocation,
                    ProfilePhotoUrl = userProfile.ProfilePhotoUrl,
                    EmergencyContactName = userProfile.EmergencyContactName,
                    EmergencyContactRelationship = userProfile.EmergencyContactRelationship,
                    EmergencyContactPhone = userProfile.EmergencyContactPhone
                };
            }

            errorMessage = string.Empty;
            successMessage = string.Empty;
        }
        catch (Exception ex)
        {
            errorMessage = $"Error loading profile: {ex.Message}";
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private void ToggleEditMode()
    {
        if (isEditMode)
        {
            CancelEdit();
        }
        else
        {
            isEditMode = true;
            errorMessage = string.Empty;
            successMessage = string.Empty;
            StateHasChanged();
        }
    }

    private void CancelEdit()
    {
        if (originalProfile != null && userProfile != null)
        {
            // Restore original values
            userProfile.Email = originalProfile.Email;
            userProfile.PhoneNumber = originalProfile.PhoneNumber;
            userProfile.DateOfBirth = originalProfile.DateOfBirth;
            userProfile.Address = originalProfile.Address;
            userProfile.EmergencyContactName = originalProfile.EmergencyContactName;
            userProfile.EmergencyContactRelationship = originalProfile.EmergencyContactRelationship;
            userProfile.EmergencyContactPhone = originalProfile.EmergencyContactPhone;
        }
        
        isEditMode = false;
        errorMessage = string.Empty;
        successMessage = string.Empty;
        StateHasChanged();
    }

    private async Task SaveProfile()
    {
        if (userProfile == null) return;

        try
        {
            isSaving = true;
            errorMessage = string.Empty;
            successMessage = string.Empty;
            StateHasChanged();

            var result = await UserService.UpdateUserProfileAsync(userProfile);
            
            if (result.Success)
            {
                successMessage = "Profile updated successfully.";
                isEditMode = false;
                
                // Update the original profile copy
                if (originalProfile != null)
                {
                    originalProfile.Email = userProfile.Email;
                    originalProfile.PhoneNumber = userProfile.PhoneNumber;
                    originalProfile.DateOfBirth = userProfile.DateOfBirth;
                    originalProfile.Address = userProfile.Address;
                    originalProfile.EmergencyContactName = userProfile.EmergencyContactName;
                    originalProfile.EmergencyContactRelationship = userProfile.EmergencyContactRelationship;
                    originalProfile.EmergencyContactPhone = userProfile.EmergencyContactPhone;
                }
            }
            else
            {
                errorMessage = result.ErrorMessage ?? "Failed to update profile.";
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Error saving profile: {ex.Message}";
        }
        finally
        {
            isSaving = false;
            StateHasChanged();
        }
    }

    private Task ChangePhoto()
    {
        try
        {
            // Feature not implemented yet
            errorMessage = "Photo upload feature is not implemented yet.";
        }
        catch (Exception ex)
        {
            errorMessage = $"Error opening file picker: {ex.Message}";
        }
        return Task.CompletedTask;
    }

    [JSInvokable]
    public static Task HandlePhotoUpload(object fileInfo)
    {
        // This would be called from JavaScript
        // For now, show a message that the feature is in development
        Console.WriteLine("Photo upload triggered - feature in development");
        return Task.CompletedTask;
    }

    private Task GoBack()
    {
        return JSRuntime.InvokeVoidAsync("history.back").AsTask();
    }


}
