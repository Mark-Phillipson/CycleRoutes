using CycleRoutes.Shared.Services;
using System.Text.Json;

namespace CycleRoutes.Web.Services;

public class StreetViewService : IStreetViewService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<StreetViewService> _logger;
    private readonly string? _apiKey;
    private bool? _apiKeyValidated = null;

    public StreetViewService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<StreetViewService> logger)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiKey = _configuration["GoogleMaps:ApiKey"];
    }

    public bool HasApiKey => !string.IsNullOrEmpty(_apiKey);

    public async Task<bool> ValidateApiKeyAsync()
    {
        if (!HasApiKey)
        {
            _apiKeyValidated = false;
            return false;
        }

        if (_apiKeyValidated.HasValue)
        {
            return _apiKeyValidated.Value;
        }

        try
        {
            using var httpClient = _httpClientFactory.CreateClient();
            // Test the API key with a simple geocoding request
            var testUrl = $"https://maps.googleapis.com/maps/api/geocode/json?address=London&key={_apiKey}";
            var response = await httpClient.GetAsync(testUrl);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(content);
                var status = document.RootElement.GetProperty("status").GetString();
                
                _apiKeyValidated = status == "OK";
                if (!_apiKeyValidated.Value)
                {
                    _logger.LogWarning("Google Maps API key validation failed with status: {Status}", status);
                }
            }
            else
            {
                _apiKeyValidated = false;
                _logger.LogWarning("Google Maps API key validation failed with HTTP status: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _apiKeyValidated = false;
            _logger.LogError(ex, "Error validating Google Maps API key");
        }

        return _apiKeyValidated ?? false;
    }

    public string GetApiKeyStatus()
    {
        if (!HasApiKey)
            return "No API key configured";
        
        if (!_apiKeyValidated.HasValue)
            return "API key not yet validated";
            
        return _apiKeyValidated.Value ? "API key is valid" : "API key is invalid or has insufficient permissions";
    }

    public string GetStreetViewUrl(double latitude, double longitude, double heading = 0, int fov = 90, int pitch = 0)
    {
        var baseUrl = "https://www.google.com/maps/@";
        return $"{baseUrl}{latitude:F6},{longitude:F6},3a,75y,{heading:F1}h,{pitch}t";
    }

    public string GetStreetViewEmbedUrl(double latitude, double longitude, double heading = 0, int fov = 90, int pitch = 0)
    {
        if (!HasApiKey)
        {
            _logger.LogWarning("No API key available for Street View");
            return string.Empty;
        }

        // Don't check validation status on client-side - just generate the URL
        var baseUrl = "https://www.google.com/maps/embed/v1/streetview";
        var url = $"{baseUrl}?key={_apiKey}&location={latitude:F6},{longitude:F6}&heading={heading:F1}&pitch={pitch}&fov={fov}";
        
        if (!string.IsNullOrEmpty(_apiKey))
        {
            _logger.LogInformation("Generated Street View URL for location {Lat},{Lng}: {Url}", 
                latitude, longitude, url.Replace(_apiKey, "***API_KEY***"));
        }
        return url;
    }

    public double CalculateBearing(double lat1, double lon1, double lat2, double lon2)
    {
        var dLon = ToRadians(lon2 - lon1);
        var y = Math.Sin(dLon) * Math.Cos(ToRadians(lat2));
        var x = Math.Cos(ToRadians(lat1)) * Math.Sin(ToRadians(lat2)) - 
                Math.Sin(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) * Math.Cos(dLon);
        
        var bearing = ToDegrees(Math.Atan2(y, x));
        return (bearing + 360) % 360; // Normalize to 0-360 degrees
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;
    private static double ToDegrees(double radians) => radians * 180 / Math.PI;
}
