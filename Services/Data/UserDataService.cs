using MauiHybridApp.Models;
using MauiHybridApp.Models.Employee;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Utils;
using Microsoft.Maui.Storage;
using System;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class UserDataService : IUserDataService
    {
        private readonly IGenericRepository _repository;

        public UserDataService(IGenericRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserProfileModel?> GetUserProfileAsync()
        {
            try
            {
                var profileIdStr = await SecureStorage.GetAsync("profile_id");
                if (string.IsNullOrEmpty(profileIdStr)) return null;

                // Format URL
                var url = string.Format(ApiEndpoints.GetUserProfile, profileIdStr);

                // API se Bara Model (ProfileModel) mangwao
                var response = await _repository.GetAsync<EmployeeProfileResponse>(url);

                if (response != null && response.Model != null)
                {
                    var apiData = response.Model;

                    // 🔥 FIXED MAPPING (Ab koi error nahi ayega)
                    return new UserProfileModel
                    {
                        // 1. Profile Id (Added in Model)
                        ProfileId = apiData.ProfileId,
                        
                        // 2. Employee Info
                        EmployeeId = apiData.EmployeeNo ?? "N/A",
                        
                        // 3. Name (UI model doesn't have First/Last, only FullName)
                        FullName = $"{apiData.FirstName} {apiData.LastName}".Trim(),
                        
                        // 4. Contact
                        Email = apiData.EmailAddress ?? "",
                        PhoneNumber = apiData.MobileNumber ?? apiData.PhoneNumber,

                        // 5. Dates
                        DateOfBirth = apiData.Birthdate ?? DateTime.MinValue,
                        HireDate = apiData.HireDate ?? DateTime.MinValue,
                        
                        // 6. Job
                        JobTitle = apiData?.Position ?? "N/A", 
                        Department = apiData?.Department ?? "N/A",
                        WorkLocation = apiData?.Location ?? apiData?.Branch ?? "N/A",
                        EmploymentStatus = apiData?.EmploymentStatus ?? "N/A",
                        
                        // 7. Address
                        Address = $"{apiData?.CityAddress1} {apiData?.CityAddressCity}".Trim(),
                        
                        // 8. Emergency
                        EmergencyContactName = apiData?.EmergencyContactName,
                        EmergencyContactRelationship = apiData?.EmergencyContactRelationship,
                        EmergencyContactPhone = apiData?.EmergencyContactContactNumber
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Profile Error: {ex.Message}");
                return null;
            }
        }

        public async Task<SaveResult> UpdateUserProfileAsync(UserProfileModel profile)
        {
            // Fake update for now
            await Task.Delay(500);
            return new SaveResult { Success = true };
        }
    }
}
