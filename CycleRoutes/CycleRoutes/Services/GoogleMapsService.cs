using CycleRoutes.Shared.Models;
using CycleRoutes.Shared.Services;

namespace CycleRoutes.Services;

public class GoogleMapsService : IGoogleMapsService
{
    public bool HasApiKey => false;

    public string GetApiKeyStatus() => "Google Maps not available in MAUI app";

    public string GetMapsApiUrl() => string.Empty;

    public string GenerateMapInitializationScript(CycleRoute route, string mapElementId, int currentPointIndex = 0) => string.Empty;

    public string GenerateUpdatePositionScript(RoutePoint currentPoint, string mapElementId) => string.Empty;

    public string GenerateRouteClickScript(CycleRoute route, string callbackFunction) => string.Empty;

    public Task<bool> ValidateApiKeyAsync() => Task.FromResult(false);
}
