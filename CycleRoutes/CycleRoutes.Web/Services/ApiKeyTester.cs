using System.Text.Json;

namespace CycleRoutes.Web.Services;

public class ApiKeyTester
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ApiKeyTester> _logger;

    public ApiKeyTester(IHttpClientFactory httpClientFactory, ILogger<ApiKeyTester> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<string> TestApiKeyAsync(string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            return "❌ No API key provided";
        }

        try
        {
            using var httpClient = _httpClientFactory.CreateClient();
            
            // Test with a simple geocoding request
            var testUrl = $"https://maps.googleapis.com/maps/api/geocode/json?address=London&key={apiKey}";
            
            _logger.LogInformation("Testing API key with URL: {Url}", testUrl.Replace(apiKey, "***"));
            
            var response = await httpClient.GetAsync(testUrl);
            var content = await response.Content.ReadAsStringAsync();
            
            _logger.LogInformation("API Response Status: {Status}", response.StatusCode);
            _logger.LogInformation("API Response Content: {Content}", content);
            
            if (response.IsSuccessStatusCode)
            {
                using var document = JsonDocument.Parse(content);
                var status = document.RootElement.GetProperty("status").GetString();
                
                return status switch
                {
                    "OK" => "✅ API key is valid and working",
                    "REQUEST_DENIED" => "❌ API key is invalid or request was denied",
                    "OVER_DAILY_LIMIT" => "⚠️ API key has exceeded daily quota",
                    "OVER_QUERY_LIMIT" => "⚠️ API key has exceeded query limit",
                    "INVALID_REQUEST" => "⚠️ Invalid request format",
                    _ => $"⚠️ Unexpected status: {status}"
                };
            }
            else
            {
                return $"❌ HTTP Error: {response.StatusCode} - {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing API key");
            return $"❌ Error: {ex.Message}";
        }
    }
}
