using CycleRoutes.Shared.Services;

namespace CycleRoutes.Web.Client.Services;

public class StreetViewService : IStreetViewService
{
    private readonly HttpClient _httpClient;
    private string? _apiKey;

    public StreetViewService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = null;
    }

    // Allow setting API key at runtime
    public void SetApiKey(string key)
    {
        _apiKey = key;
    }

    public bool HasApiKey => !string.IsNullOrEmpty(_apiKey);

    public Task<bool> ValidateApiKeyAsync()
    {
        return Task.FromResult(HasApiKey);
    }

    public string GetApiKeyStatus()
    {
        return HasApiKey ? "API key is set" : "No API key configured";
    }

    public string GetStreetViewUrl(double latitude, double longitude, double heading = 0, int fov = 90, int pitch = 0)
    {
        var baseUrl = "https://www.google.com/maps/@";
        return $"{baseUrl}{latitude:F6},{longitude:F6},3a,75y,{heading:F1}h,{pitch}t";
    }

    public string GetStreetViewEmbedUrl(double latitude, double longitude, double heading = 0, int fov = 90, int pitch = 0)
    {
        if (!HasApiKey) return string.Empty;
        // Use Google Maps Embed API directly
        return $"https://www.google.com/maps/embed/v1/streetview?key={_apiKey}&location={latitude:F6},{longitude:F6}&heading={heading:F1}&pitch={pitch}&fov={fov}";
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

    public string GetFallbackMapUrl(double latitude, double longitude, int zoom = 18)
    {
        if (!HasApiKey) return string.Empty;
        return $"https://www.google.com/maps/embed/v1/view?key={_apiKey}&center={latitude:F6},{longitude:F6}&zoom={zoom}&maptype=roadmap";
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;
    private static double ToDegrees(double radians) => radians * 180 / Math.PI;
}
