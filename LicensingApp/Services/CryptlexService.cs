using LicensingApp.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LicensingApp.Services;

public class CryptlexService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CryptlexService> _logger;

    public CryptlexService(HttpClient httpClient, ILogger<CryptlexService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<string?> LoginAsync(string accountId, string email, string password)
    {
        var loginModel = new CryptlexLoginModel
        {
            AccountId = accountId,
            Email = email,
            Password = password
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync("v3/accounts/login", loginModel);
            var content = await response.Content.ReadAsStringAsync();

            // Check if the response has a success status code.
            if (response.IsSuccessStatusCode)
            {
                // Attempt to deserialize the response to the expected model.
                var tokenResponse = JsonSerializer.Deserialize<TokenResponseModel>(content);
                if (tokenResponse?.AccessToken != null)
                {
                    _logger.LogError($"Access token from cryptlex api: {content}");
                    return tokenResponse.AccessToken;
                }
                else
                {
                    // If there's no access token, log the content and return null.
                    _logger.LogError($"Received 200 status but no access token: {content}");
                    return null;
                }
            }
            else
            {
                // Log the unsuccessful status and the error content.
                _logger.LogError($"Login failed: {response.StatusCode}");
                _logger.LogError($"Error content: {content}");
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception occurred when calling Cryptlex: {ex.Message}");
            return null;
        }
    }

}

public class TokenResponseModel
{
    [JsonPropertyName("accessToken")]  // Ensure this matches the JSON property name exactly as it appears in the response
    public string AccessToken { get; set; }
}