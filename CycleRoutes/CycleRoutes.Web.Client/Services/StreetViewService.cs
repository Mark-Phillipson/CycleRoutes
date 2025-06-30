using CycleRoutes.Shared.Services;

namespace CycleRoutes.Web.Client.Services;

public class StreetViewService : IStreetViewService
{
    public bool HasApiKey => false; // Client-side doesn't have access to server config

    public Task<bool> ValidateApiKeyAsync()
    {
        return Task.FromResult(false);
    }

    public string GetApiKeyStatus()
    {
        return "API key validation not available on client-side";
    }

    public string GetStreetViewUrl(double latitude, double longitude, double heading = 0, int fov = 90, int pitch = 0)
    {
        var baseUrl = "https://www.google.com/maps/@";
        return $"{baseUrl}{latitude:F6},{longitude:F6},3a,75y,{heading:F1}h,{pitch}t";
    }

    public string GetStreetViewEmbedUrl(double latitude, double longitude, double heading = 0, int fov = 90, int pitch = 0)
    {
        return string.Empty; // No embedding support on client-side without API key
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
