using CycleRoutes.Shared.Models;

namespace CycleRoutes.Shared.Models;

public class CycleRoute
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<RoutePoint> Points { get; set; } = new();
    public double TotalDistance { get; set; }
    public TimeSpan EstimatedDuration { get; set; }
    public DateTime? CreatedDate { get; set; }

    public RoutePoint? StartPoint => Points.FirstOrDefault();
    public RoutePoint? EndPoint => Points.LastOrDefault();

    public IEnumerable<(RoutePoint from, RoutePoint to)> GetSegments()
    {
        for (int i = 0; i < Points.Count - 1; i++)
        {
            yield return (Points[i], Points[i + 1]);
        }
    }

    public double CalculateTotalDistance()
    {
        double totalDistance = 0;
        foreach (var (from, to) in GetSegments())
        {
            totalDistance += CalculateDistance(from, to);
        }
        return totalDistance;
    }

    private static double CalculateDistance(RoutePoint point1, RoutePoint point2)
    {
        const double earthRadius = 6371000; // Earth's radius in meters
        
        var lat1Rad = point1.Latitude * Math.PI / 180;
        var lat2Rad = point2.Latitude * Math.PI / 180;
        var deltaLatRad = (point2.Latitude - point1.Latitude) * Math.PI / 180;
        var deltaLonRad = (point2.Longitude - point1.Longitude) * Math.PI / 180;

        var a = Math.Sin(deltaLatRad / 2) * Math.Sin(deltaLatRad / 2) +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Sin(deltaLonRad / 2) * Math.Sin(deltaLonRad / 2);
        
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        
        return earthRadius * c;
    }
}
