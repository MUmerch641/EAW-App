using Newtonsoft.Json;
using Newtonsoft.Json.Serialization; // Add this namespace
using MauiHybridApp.Utils;
using Microsoft.Maui.Storage;
using System.Net.Http.Headers;

namespace MauiHybridApp.Services;

public interface IGenericRepository : IDisposable
{
    Task<T?> GetAsync<T>(string uri, string authToken = "");
    Task<T?> PostAsync<T>(string uri, T data, string authToken = "");
    Task<T?> PutAsync<T>(string uri, T data, string authToken = "");
    Task<bool> DeleteAsync(string uri, string authToken = "");
    Task<T?> DeleteAsync<T>(string uri, string authToken = "");
    Task<R?> PostAsync<T, R>(string uri, T data, string authToken = "");
    Task<R?> PutAsync<T, R>(string path, T data, string authToken = "");
    Task<T?> PostMultipartAsync<T>(string uri, MultipartFormDataContent content, string authToken = "");
    Task<Stream> GetStreamAsync(string uri, string authToken = "");
}

public class GenericRepository : IGenericRepository
{
    private readonly HttpClient _httpClient;
    private bool _disposed = false;

    public GenericRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(ApiEndpoints.BaseUrl);
    }

    // 🔥 MAIN JADOO: Ye helper method har request par Token lagayega
    private async Task PrepareHeaderAsync(HttpRequestMessage request, string authToken)
    {
        // Agar caller ne token nahi diya, to Storage se uthao
        if (string.IsNullOrEmpty(authToken))
        {
            authToken = await SecureStorage.GetAsync("auth_token") ?? "";
        }

        if (!string.IsNullOrEmpty(authToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        }
    }

    public async Task<T?> GetAsync<T>(string uri, string authToken = "")
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            await PrepareHeaderAsync(request, authToken); // ✅ Helper Call

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GET Error: {ex.Message}");
            return default;
        }
    }

    public async Task<T?> PostAsync<T>(string uri, T data, string authToken = "")
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            await PrepareHeaderAsync(request, authToken); // ✅ Helper Call

            var json = JsonConvert.SerializeObject(data);
            request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"POST Error: {ex.Message}");
            return default;
        }
    }

    public async Task<R?> PostAsync<T, R>(string uri, T data, string authToken = "")
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            await PrepareHeaderAsync(request, authToken); // ✅ Helper Call

            // 🔥 FIX: Use CamelCase Settings
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            // Serialize with settings
            var json = JsonConvert.SerializeObject(data, settings);
            
            Console.WriteLine($"[REPO] Payload: {json}"); // Verify logs show "profileId" (small p)
            request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            Console.WriteLine($"Making POST request to: {_httpClient.BaseAddress}{uri}");

            var response = await _httpClient.SendAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API ERROR: {response.StatusCode} - {errorContent}");
            }
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<R>(content);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"POST Error: {ex.Message}");
            return default;
        }
    }

    public async Task<T?> PutAsync<T>(string uri, T data, string authToken = "")
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Put, uri);
            await PrepareHeaderAsync(request, authToken); // ✅ Helper Call

            var json = JsonConvert.SerializeObject(data);
            request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"PUT Error: {ex.Message}");
            return default;
        }
    }

    public async Task<R?> PutAsync<T, R>(string path, T data, string authToken = "")
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Put, path);
            await PrepareHeaderAsync(request, authToken); // ✅ Helper Call

            // 🔥 FIX: Use CamelCase Settings
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            // Serialize with settings
            var json = JsonConvert.SerializeObject(data, settings);
            
            request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<R>(content);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"PUT Error: {ex.Message}");
            return default;
        }
    }

    public async Task<bool> DeleteAsync(string uri, string authToken = "")
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, uri);
            await PrepareHeaderAsync(request, authToken); // ✅ Helper Call

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"DELETE Error: {ex.Message}");
            return false;
        }
    }

    public async Task<T?> DeleteAsync<T>(string uri, string authToken = "")
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, uri);
            await PrepareHeaderAsync(request, authToken); // ✅ Helper Call

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return string.IsNullOrEmpty(content) ? default : JsonConvert.DeserializeObject<T>(content);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Delete Error: {ex.Message}");
            return default;
        }
    }

    public async Task<T?> PostMultipartAsync<T>(string uri, MultipartFormDataContent content, string authToken = "")
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = content
            };
            await PrepareHeaderAsync(request, authToken); // ✅ Helper Call

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return string.IsNullOrEmpty(responseContent) ? default : JsonConvert.DeserializeObject<T>(responseContent);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Multipart Error: {ex.Message}");
            return default;
        }
    }

    public async Task<Stream> GetStreamAsync(string uri, string authToken = "")
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            await PrepareHeaderAsync(request, authToken); // ✅ Helper Call

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStreamAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Stream Error: {ex.Message}");
            return Stream.Null;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _disposed = true;
        }
    }
}