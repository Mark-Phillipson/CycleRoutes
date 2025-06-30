using CycleRoutes.Shared.Models;

namespace CycleRoutes.Shared.Services;

public interface IRouteService
{
    Task<CycleRoute?> LoadRouteFromGpxAsync(Stream gpxStream, string fileName);
    Task<List<CycleRoute>> GetSavedRoutesAsync();
    Task SaveRouteAsync(CycleRoute route);
    Task DeleteRouteAsync(string routeName);
    CycleRoute? CurrentRoute { get; }
    event Action<CycleRoute?>? CurrentRouteChanged;
    void SetCurrentRoute(CycleRoute? route);
}
