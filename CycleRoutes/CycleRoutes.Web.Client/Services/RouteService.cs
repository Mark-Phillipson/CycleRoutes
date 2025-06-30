using CycleRoutes.Shared.Models;
using CycleRoutes.Shared.Services;
using System.Xml.Linq;

namespace CycleRoutes.Web.Client.Services;

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
            var doc = await XDocument.LoadAsync(gpxStream, LoadOptions.None, CancellationToken.None);
            
            // Parse GPX file
            var ns = doc.Root?.GetDefaultNamespace() ?? XNamespace.None;
            var trackPoints = doc.Descendants(ns + "trkpt").ToList();
            
            if (!trackPoints.Any())
                return null;

            var points = new List<RoutePoint>();
            foreach (var point in trackPoints)
            {
                if (double.TryParse(point.Attribute("lat")?.Value, out var lat) &&
                    double.TryParse(point.Attribute("lon")?.Value, out var lon))
                {
                    var elevation = point.Element(ns + "ele");
                    double? elevationValue = null;
                    if (elevation != null && double.TryParse(elevation.Value, out var elev))
                    {
                        elevationValue = elev;
                    }

                    points.Add(new RoutePoint(lat, lon, elevationValue));
                }
            }

            if (!points.Any())
                return null;

            // Calculate total distance using the built-in method
            var route = new CycleRoute
            {
                Name = Path.GetFileNameWithoutExtension(fileName),
                Points = points,
                EstimatedDuration = TimeSpan.FromHours(points.Count * 0.01) // Rough estimate based on points
            };

            // Calculate and set the total distance (keep in meters)
            route.TotalDistance = route.CalculateTotalDistance();

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
