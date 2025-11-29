using MauiHybridApp.Services.Data;
using MauiHybridApp.Utils;
using MauiHybridApp.Models;
using System.Collections.Generic;
using Microsoft.Maui.Storage;

namespace MauiHybridApp.Services.Authentication;

public class AuthenticationDataService : IAuthenticationDataService
{
    private readonly IGenericRepository _repository;

    public AuthenticationDataService(IGenericRepository repository)
    {
        _repository = repository;
    }

    public async Task<AuthenticationResult> AuthenticateAsync(LoginRequest request)
    {
        Console.WriteLine("=== AUTHENTICATION START ===");
        try
        {
            Console.WriteLine($"[AUTH] Attempting login with username: {request.username}");
            Console.WriteLine($"[AUTH] Password provided: {(string.IsNullOrEmpty(request.password) ? "NO" : "YES")}");
            Console.WriteLine($"[AUTH] Portal: {request.portal}");
            
            // Make direct HTTP call to handle API error responses properly
            var httpClient = new HttpClient { BaseAddress = new Uri(ApiEndpoints.BaseUrl) };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            Console.WriteLine($"[AUTH] Making POST request to: {ApiEndpoints.BaseUrl}{ApiEndpoints.Login}");
            Console.WriteLine($"[AUTH] Request body: {json}");
            
            var httpResponse = await httpClient.PostAsync(ApiEndpoints.Login, content);
            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            
            Console.WriteLine($"[AUTH] HTTP Status: {httpResponse.StatusCode}");
            Console.WriteLine($"[AUTH] Response content: {responseContent}");
            
            if (httpResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("[AUTH] HTTP request successful, parsing response...");
                
                // Parse successful response
                var apiResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthenticationApiResponse>(responseContent);
                
                Console.WriteLine($"[AUTH] Parsed response - isSuccess: {apiResponse?.isSuccess}, hasUser: {apiResponse?.user != null}");
                
                if (apiResponse != null && apiResponse.isSuccess && apiResponse.user != null)
                {
                    Console.WriteLine($"[AUTH] ✅ LOGIN SUCCESSFUL!");
                    Console.WriteLine($"[AUTH] User: {apiResponse.user.userName} (ID: {apiResponse.user.userSecurityId})");
                    Console.WriteLine($"[AUTH] Token received: {(string.IsNullOrEmpty(apiResponse.token) ? "NO" : "YES")}");
                    Console.WriteLine($"[AUTH] Expiration: {apiResponse.expiration}");
                    
                    // 1. Token Save kar lo (Taake agli request mein use ho sake)
                    if (!string.IsNullOrEmpty(apiResponse.token))
                    {
                        await SecureStorage.SetAsync("auth_token", apiResponse.token);
                    }

                    // =========================================================
                    // 🔥 NEW STEP: Fetch Real ProfileId using UserSecurityId
                    // =========================================================
                    long realProfileId = 0;
                    try
                    {
                        // UserSecurityId uthao login response se
                        var userSecurityId = apiResponse.user.userSecurityId;

                        // URL banao: /api/v1/employee/{id}/employee-details-by-userid
                        // Note: Agar V1 kaam na kare to bina V1 ke try karna
                        var empUrl = $"api/v1/employee/{userSecurityId}/employee-details-by-userid";
                        
                        Console.WriteLine($"[AUTH] Fetching ProfileId from: {empUrl}");

                        // Hum Repository use kar sakte hain kyunke token save ho chuka hai,
                        // ya direct httpClient use karlo same token ke sath
                        httpClient.DefaultRequestHeaders.Authorization = 
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiResponse.token);

                        var empResponse = await httpClient.GetAsync(empUrl);
                        if (empResponse.IsSuccessStatusCode)
                        {
                            var empContent = await empResponse.Content.ReadAsStringAsync();
                            var empDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<EmployeeProfileResponse>(empContent);
                            
                            if (empDetails != null)
                            {
                                realProfileId = empDetails.ProfileId;
                                Console.WriteLine($"[AUTH] ✅ FOUND REAL PROFILE ID: {realProfileId}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"[AUTH] ❌ Failed to fetch ProfileId. Status: {empResponse.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[AUTH] Error fetching profile details: {ex.Message}");
                    }

                    // Agar API fail ho gayi, to fallback ke tor par UserSecurityId use karlo (risk hai)
                    // Lekin client ne kaha ProfileId use karo, to hum wahi save karenge
                    if (realProfileId == 0) 
                    {
                        Console.WriteLine("[AUTH] ⚠️ Warning: Using UserSecurityId as fallback");
                        realProfileId = apiResponse.user.userSecurityId;
                    }

                    // Save Profile ID to Storage
                    await SecureStorage.SetAsync("profile_id", realProfileId.ToString());
                    // =========================================================
                    
                    return new AuthenticationResult
                    {
                        Success = true,
                        Token = apiResponse.token,
                        RefreshToken = null, // API doesn't provide refresh token
                        TokenExpiry = DateTime.TryParse(apiResponse.expiration, out var expiry) ? expiry : null,
                        User = new UserModel
                        {
                            Username = apiResponse.user.userName,
                            EmployeeName = apiResponse.user.userName, // Using username as employee name since API doesn't provide full name
                            ProfileId = realProfileId, // Using real ProfileId from API
                            UserSecurityId = apiResponse.user.userSecurityId,
                            Position = "", // API doesn't provide position
                            Department = "", // API doesn't provide department
                            Company = "", // API doesn't provide company
                            EmployeeNo = null,
                            FirstName = null,
                            LastName = null,
                            EmailAddress = null,
                            CompanyId = 0,
                            BranchId = 0,
                            DepartmentId = 0,
                            OfficeId = 0,
                            TeamId = 0,
                            UserTypeId = apiResponse.user.userTypeId,
                            AccessId = null
                        }
                    };
                }
                else
                {
                    Console.WriteLine($"[AUTH] ❌ LOGIN FAILED - API returned success=false or no user");
                    Console.WriteLine($"[AUTH] Error message: {apiResponse?.errorMessage}");
                    return new AuthenticationResult
                    {
                        Success = false,
                        ErrorMessage = apiResponse?.errorMessage ?? "Authentication failed"
                    };
                }
            }
            else
            {
                Console.WriteLine($"[AUTH] ❌ HTTP ERROR - Status: {httpResponse.StatusCode}");
                
                // Handle API error responses (like 400 Bad Request)
                try
                {
                    // Try to parse as validation error format
                    var errorResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(responseContent);
                    if (errorResponse != null && errorResponse.ContainsKey("") && errorResponse[""].Any())
                    {
                        var errorMsg = string.Join(", ", errorResponse[""]);
                        Console.WriteLine($"[AUTH] Parsed error message: {errorMsg}");
                        return new AuthenticationResult
                        {
                            Success = false,
                            ErrorMessage = errorMsg
                        };
                    }
                }
                catch (Exception parseEx)
                {
                    Console.WriteLine($"[AUTH] Failed to parse error response: {parseEx.Message}");
                }
                
                Console.WriteLine($"[AUTH] Raw error response: {responseContent}");
                return new AuthenticationResult
                {
                    Success = false,
                    ErrorMessage = $"Login failed: {httpResponse.StatusCode} - {responseContent}"
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AUTH] ❌ EXCEPTION during authentication: {ex.Message}");
            Console.WriteLine($"[AUTH] Stack trace: {ex.StackTrace}");
            return new AuthenticationResult
            {
                Success = false,
                ErrorMessage = "Network error. Please check your connection and try again."
            };
        }
        finally
        {
            Console.WriteLine("=== AUTHENTICATION END ===");
        }
    }

    public async Task<ForgotPasswordResult> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        try
        {
            var response = await _repository.PostAsync<ForgotPasswordRequest, ForgotPasswordApiResponse>(
                ApiEndpoints.ForgotPassword, request);

            return new ForgotPasswordResult
            {
                Success = response?.Success ?? false,
                Message = response?.Message,
                ErrorMessage = response?.ErrorMessage
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Forgot password error: {ex.Message}");
            return new ForgotPasswordResult
            {
                Success = false,
                ErrorMessage = "Network error. Please try again later."
            };
        }
    }

    public async Task<RegistrationResult> RegistrationAsync(RegistrationRequest request)
    {
        try
        {
            var response = await _repository.PostAsync<RegistrationRequest, RegistrationApiResponse>(
                ApiEndpoints.Register, request);

            return new RegistrationResult
            {
                Success = response?.Success ?? false,
                Message = response?.Message,
                ErrorMessage = response?.ErrorMessage
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Registration error: {ex.Message}");
            return new RegistrationResult
            {
                Success = false,
                ErrorMessage = "Network error. Please try again later."
            };
        }
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var request = new { Token = token };
            var response = await _repository.PostAsync<object, TokenValidationResponse>(
                ApiEndpoints.ValidateToken, request);

            return response?.IsValid ?? false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token validation error: {ex.Message}");
            return false;
        }
    }

    public async Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var request = new { RefreshToken = refreshToken };
            var response = await _repository.PostAsync<object, RefreshTokenApiResponse>(
                ApiEndpoints.RefreshToken, request);

            if (response?.Success == true)
            {
                return new RefreshTokenResult
                {
                    Success = true,
                    Token = response.Token,
                    RefreshToken = response.RefreshToken,
                    TokenExpiry = response.TokenExpiry
                };
            }

            return new RefreshTokenResult
            {
                Success = false,
                ErrorMessage = response?.ErrorMessage ?? "Token refresh failed"
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token refresh error: {ex.Message}");
            return new RefreshTokenResult
            {
                Success = false,
                ErrorMessage = "Network error. Please login again."
            };
        }
    }

    // API Response Models
    private class AuthenticationApiResponse
    {
        public string? token { get; set; }
        public string? expiration { get; set; }
        public ApiUserModel? user { get; set; }
        public bool isSuccess { get; set; }
        public string? errorMessage { get; set; }
        public string? validationMessage { get; set; }
        public List<string>? validationMessages { get; set; }
        public int responseType { get; set; }
    }

    private class ApiUserModel
    {
        public long userSecurityId { get; set; }
        public string userName { get; set; } = string.Empty;
        public long userTypeId { get; set; }
    }

    private class ForgotPasswordApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
    }

    private class RegistrationApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
    }

    private class TokenValidationResponse
    {
        public bool IsValid { get; set; }
    }

    private class RefreshTokenApiResponse
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? TokenExpiry { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

