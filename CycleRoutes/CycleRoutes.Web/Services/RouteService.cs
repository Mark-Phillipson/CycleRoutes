using CycleRoutes.Shared.Models;
using CycleRoutes.Shared.Services;
using System.Text.Json;

namespace CycleRoutes.Web.Services;

public class RouteService : IRouteService
{
    private readonly ILogger<RouteService> _logger;
    private CycleRoute? _currentRoute;
    private readonly List<CycleRoute> _savedRoutes = new();

    public CycleRoute? CurrentRoute => _currentRoute;
    public event Action<CycleRoute?>? CurrentRouteChanged;

    public RouteService(ILogger<RouteService> logger)
    {
        _logger = logger;
    }

    public async Task<CycleRoute?> LoadRouteFromGpxAsync(Stream gpxStream, string fileName)
    {
        try
        {
            var route = await GpxParser.ParseGpxAsync(gpxStream, fileName);
            
            if (route != null)
            {
                _logger.LogInformation("Loaded route {RouteName} with {PointCount} points, distance: {Distance:F1}km", 
                    route.Name, route.Points.Count, route.TotalDistance / 1000);
                
                return route;
            }

            _logger.LogWarning("No route points found in file {FileName}", fileName);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading GPX file {FileName}", fileName);
            return null;
        }
    }

    public Task<List<CycleRoute>> GetSavedRoutesAsync()
    {
        return Task.FromResult(_savedRoutes.ToList());
    }

    public Task SaveRouteAsync(CycleRoute route)
    {
        var existing = _savedRoutes.FirstOrDefault(r => r.Name == route.Name);
        if (existing != null)
        {
            _savedRoutes.Remove(existing);
        }
        
        _savedRoutes.Add(route);
        _logger.LogInformation("Saved route {RouteName}", route.Name);
        return Task.CompletedTask;
    }

    public Task DeleteRouteAsync(string routeName)
    {
        var route = _savedRoutes.FirstOrDefault(r => r.Name == routeName);
        if (route != null)
        {
            _savedRoutes.Remove(route);
            if (_currentRoute?.Name == routeName)
            {
                SetCurrentRoute(null);
            }
            _logger.LogInformation("Deleted route {RouteName}", routeName);
        }
        return Task.CompletedTask;
    }

    public void SetCurrentRoute(CycleRoute? route)
    {
        _currentRoute = route;
        CurrentRouteChanged?.Invoke(_currentRoute);
        if (route != null)
        {
            _logger.LogInformation("Set current route to {RouteName}", route.Name);
        }
    }
}
