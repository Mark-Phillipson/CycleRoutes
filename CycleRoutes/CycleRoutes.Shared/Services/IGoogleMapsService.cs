using CycleRoutes.Shared.Models;

namespace CycleRoutes.Shared.Services;

public interface IGoogleMapsService
{
    /// <summary>
    /// Gets the Google Maps JavaScript API URL with the API key
    /// </summary>
    string GetMapsApiUrl();
    
    /// <summary>
    /// Checks if a valid API key is available
    /// </summary>
    bool HasApiKey { get; }
    
    /// <summary>
    /// Validates the API key asynchronously
    /// </summary>
    Task<bool> ValidateApiKeyAsync();
    
    /// <summary>
    /// Gets the current API key status message
    /// </summary>
    string GetApiKeyStatus();
    
    /// <summary>
    /// Generates the JavaScript code to initialize a map with a route
    /// </summary>
    string GenerateMapInitializationScript(CycleRoute route, string mapElementId, int currentPointIndex = 0);
    
    /// <summary>
    /// Generates the JavaScript code to update the current position marker
    /// </summary>
    string GenerateUpdatePositionScript(RoutePoint currentPoint, string mapElementId);
}
