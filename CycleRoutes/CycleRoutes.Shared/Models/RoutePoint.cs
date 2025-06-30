namespace CycleRoutes.Shared.Models;

public record RoutePoint(
    double Latitude,
    double Longitude,
    double? Elevation = null,
    DateTime? Timestamp = null)
{
    public override string ToString() => $"{Latitude:F6}, {Longitude:F6}";
}
