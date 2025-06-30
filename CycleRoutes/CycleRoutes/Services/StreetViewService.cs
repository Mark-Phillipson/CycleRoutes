using CycleRoutes.Shared.Services;

namespace CycleRoutes.Services;

public class StreetViewService : IStreetViewService
{
    public bool HasApiKey => false; // MAUI doesn't handle API keys directly

    public Task<bool> ValidateApiKeyAsync()
    {
        return Task.FromResult(false); // No API key in MAUI
    }

    public string GetApiKeyStatus()
    {
        return "API key management not available in MAUI";
    }

    public string GetStreetViewUrl(double latitude, double longitude, double heading = 0, int fov = 90, int pitch = 0)
    {
        var baseUrl = "https://www.google.com/maps/@";
        return $"{baseUrl}{latitude:F6},{longitude:F6},3a,75y,{heading:F1}h,{pitch}t";
    }

    public string GetStreetViewEmbedUrl(double latitude, double longitude, double heading = 0, int fov = 90, int pitch = 0)
    {
        // Return empty for MAUI - Street View embeds need API keys which are managed server-side
        return string.Empty;
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
        // Return empty for MAUI - fallback maps also need API keys which are managed server-side
        return string.Empty;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;
    private static double ToDegrees(double radians) => radians * 180 / Math.PI;
}
