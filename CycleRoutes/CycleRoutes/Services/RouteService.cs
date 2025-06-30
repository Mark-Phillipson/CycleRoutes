using CycleRoutes.Shared.Models;
using CycleRoutes.Shared.Services;

namespace CycleRoutes.Services;

public class RouteService : IRouteService
{
    private CycleRoute? _currentRoute;
    private readonly List<CycleRoute> _savedRoutes = new();

    public CycleRoute? CurrentRoute => _currentRoute;
    
    public event Action<CycleRoute?>? CurrentRouteChanged;

    public async Task<CycleRoute?> LoadRouteFromGpxAsync(Stream gpxStream, string fileName)
    {
        try
        {
            var route = await GpxParser.ParseGpxAsync(gpxStream, fileName);
            return route;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public Task<List<CycleRoute>> GetSavedRoutesAsync()
    {
        return Task.FromResult(new List<CycleRoute>(_savedRoutes));
    }

    public Task SaveRouteAsync(CycleRoute route)
    {
        var existing = _savedRoutes.FirstOrDefault(r => r.Name == route.Name);
        if (existing != null)
        {
            _savedRoutes.Remove(existing);
        }
        _savedRoutes.Add(route);
        return Task.CompletedTask;
    }

    public Task DeleteRouteAsync(string routeName)
    {
        var route = _savedRoutes.FirstOrDefault(r => r.Name == routeName);
        if (route != null)
        {
            _savedRoutes.Remove(route);
        }
        return Task.CompletedTask;
    }

    public void SetCurrentRoute(CycleRoute? route)
    {
        _currentRoute = route;
        CurrentRouteChanged?.Invoke(route);
    }
}
