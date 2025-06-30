using System.Xml;
using CycleRoutes.Shared.Models;

namespace CycleRoutes.Shared.Services;

public static class GpxParser
{
    public static async Task<CycleRoute?> ParseGpxAsync(Stream gpxStream, string fileName)
    {
        try
        {
            var route = new CycleRoute
            {
                Name = Path.GetFileNameWithoutExtension(fileName),
                CreatedDate = DateTime.Now
            };

            using var reader = XmlReader.Create(gpxStream, new XmlReaderSettings { Async = true });
            
            bool inTrack = false;
            bool inTrackSegment = false;

            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName.ToLowerInvariant())
                    {
                        case "name" when !inTrack:
                            route.Name = await reader.ReadElementContentAsStringAsync();
                            break;
                        case "desc" when !inTrack:
                            route.Description = await reader.ReadElementContentAsStringAsync();
                            break;
                        case "trk":
                            inTrack = true;
                            break;
                        case "trkseg":
                            inTrackSegment = true;
                            break;
                        case "trkpt" when inTrack && inTrackSegment:
                            var latStr = reader.GetAttribute("lat");
                            var lonStr = reader.GetAttribute("lon");
                            
                            if (double.TryParse(latStr, out var lat) && double.TryParse(lonStr, out var lon))
                            {
                                double? elevation = null;
                                DateTime? timestamp = null;

                                // Read any child elements (elevation, time, etc.)
                                if (!reader.IsEmptyElement)
                                {
                                    var subtree = reader.ReadSubtree();
                                    while (await subtree.ReadAsync())
                                    {
                                        if (subtree.NodeType == XmlNodeType.Element)
                                        {
                                            switch (subtree.LocalName.ToLowerInvariant())
                                            {
                                                case "ele":
                                                    if (double.TryParse(await subtree.ReadElementContentAsStringAsync(), out var ele))
                                                        elevation = ele;
                                                    break;
                                                case "time":
                                                    if (DateTime.TryParse(await subtree.ReadElementContentAsStringAsync(), out var time))
                                                        timestamp = time;
                                                    break;
                                            }
                                        }
                                    }
                                }

                                route.Points.Add(new RoutePoint(lat, lon, elevation, timestamp));
                            }
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    switch (reader.LocalName.ToLowerInvariant())
                    {
                        case "trk":
                            inTrack = false;
                            break;
                        case "trkseg":
                            inTrackSegment = false;
                            break;
                    }
                }
            }

            if (route.Points.Any())
            {
                route.TotalDistance = route.CalculateTotalDistance();
                // Estimate 15 km/h average cycling speed
                route.EstimatedDuration = TimeSpan.FromHours(route.TotalDistance / 1000 / 15);
                return route;
            }

            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
