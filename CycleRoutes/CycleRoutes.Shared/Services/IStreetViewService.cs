namespace CycleRoutes.Shared.Services;

public interface IStreetViewService
{
    string GetStreetViewUrl(double latitude, double longitude, double heading = 0, int fov = 90, int pitch = 0);
    string GetStreetViewEmbedUrl(double latitude, double longitude, double heading = 0, int fov = 90, int pitch = 0);
    double CalculateBearing(double lat1, double lon1, double lat2, double lon2);
    bool HasApiKey { get; }
    Task<bool> ValidateApiKeyAsync();
    string GetApiKeyStatus();
    string GetFallbackMapUrl(double latitude, double longitude, int zoom = 18);
}
