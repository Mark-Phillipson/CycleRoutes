using CycleRoutes.Shared.Models;
using CycleRoutes.Shared.Services;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace CycleRoutes.Web.Client.Services;

public class GoogleMapsService : IGoogleMapsService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private string? _apiKey;
    private string? _apiUrl;
    private bool? _apiKeyValidated = null;
    private bool _configurationLoaded = false;

    public GoogleMapsService(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        // If running in development, load API key from config
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        if (environment == "Development")
        {
            _apiKey = _configuration["GoogleMaps:ApiKey"];
        }
        else
        {
            _apiKey = null;
        }
    }

    // Allow setting API key at runtime
    public void SetApiKey(string key)
    {
        _apiKey = key;
        _apiKeyValidated = null;
        _apiUrl = null;
        _configurationLoaded = true;
    }

    private async Task EnsureConfigurationLoadedAsync()
    {
        if (_configurationLoaded)
            return;
        _configurationLoaded = true;
    }

    public bool HasApiKey => !string.IsNullOrEmpty(_apiKey);

    public string GetMapsApiUrl()
    {
        if (!HasApiKey)
            return string.Empty;
        return $"https://maps.googleapis.com/maps/api/js?key={_apiKey}&libraries=geometry&callback=initGoogleMaps&loading=async";
    }

    public async Task<bool> ValidateApiKeyAsync()
    {
        await EnsureConfigurationLoadedAsync();
        if (!HasApiKey)
        {
            _apiKeyValidated = false;
            return false;
        }
        if (_apiKeyValidated.HasValue)
        {
            return _apiKeyValidated.Value;
        }
        try
        {
            var testUrl = $"https://maps.googleapis.com/maps/api/geocode/json?address=London&key={_apiKey}";
            var response = await _httpClient.GetAsync(testUrl);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(content);
                var status = document.RootElement.GetProperty("status").GetString();
                _apiKeyValidated = status == "OK";
                return _apiKeyValidated.Value;
            }
            _apiKeyValidated = false;
            return false;
        }
        catch (Exception)
        {
            _apiKeyValidated = false;
            return false;
        }
    }

    public string GetApiKeyStatus()
    {
        if (!HasApiKey)
            return "No API key configured";
        if (!_apiKeyValidated.HasValue)
            return "API key not yet validated";
        return _apiKeyValidated.Value ? "API key is valid" : "API key is invalid or lacks required permissions";
    }

    public string GenerateMapInitializationScript(CycleRoute route, string mapElementId, int currentPointIndex = 0)
    {
        if (!route.Points.Any())
            return string.Empty;

        var currentPoint = route.Points[Math.Min(currentPointIndex, route.Points.Count - 1)];
        var pathPoints = route.Points.Select(p => $"{{lat: {p.Latitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture)}, lng: {p.Longitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture)}}}");
        var script = new StringBuilder();
        script.AppendLine($@"
function initializeMap_{mapElementId}() {{
    if (typeof google === 'undefined' || !google.maps) {{
        console.error('Google Maps API not loaded');
        return;
    }}

    const mapElement = document.getElementById('{mapElementId}');
    if (!mapElement) {{
        console.error('Map element not found: {mapElementId}');
        return;
    }}

    // Create map centered on current point
    window.cycleRouteMap = new google.maps.Map(mapElement, {{
        zoom: 16,
        center: {{
            lat: {currentPoint.Latitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture)}, 
            lng: {currentPoint.Longitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture)}
        }},
        mapTypeId: 'roadmap'
    }});

    // Create route path
    const routePath = [
        {string.Join(",\n        ", pathPoints)}
    ];

    console.log('Route path points:', routePath.length);
    console.log('First point:', routePath[0]);
    console.log('Last point:', routePath[routePath.length - 1]);

    // Draw the route polyline with direction arrows
    window.cycleRoutePolyline = new google.maps.Polyline({{
        path: routePath,
        geodesic: true,
        strokeColor: '#FF0000',
        strokeOpacity: 1.0,
        strokeWeight: 3,
        icons: [{{
            icon: {{
                path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                scale: 3,
                strokeColor: '#FF0000',
                strokeWeight: 2,
                fillColor: '#FF0000',
                fillOpacity: 1
            }},
            offset: '0%',
            repeat: '100px'
        }}]
    }});
    window.cycleRoutePolyline.setMap(window.cycleRouteMap);

    console.log('Polyline created and added to map');

    // Add start marker
    window.cycleRouteStartMarker = new google.maps.Marker({{
        position: routePath[0],
        map: window.cycleRouteMap,
        title: 'Start',
        icon: {{
            url: 'data:image/svg+xml;charset=UTF-8,' + encodeURIComponent(`
                <svg width=""24"" height=""24"" viewBox=""0 0 24 24"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                    <circle cx=""12"" cy=""12"" r=""10"" fill=""#4CAF50"" stroke=""white"" stroke-width=""2""/>
                    <text x=""12"" y=""16"" text-anchor=""middle"" fill=""white"" font-size=""12"" font-weight=""bold"">S</text>
                </svg>
            `),
            scaledSize: new google.maps.Size(24, 24),
            anchor: new google.maps.Point(12, 12)
        }}
    }});

    // Add end marker
    window.cycleRouteEndMarker = new google.maps.Marker({{
        position: routePath[routePath.length - 1],
        map: window.cycleRouteMap,
        title: 'End',
        icon: {{
            url: 'data:image/svg+xml;charset=UTF-8,' + encodeURIComponent(`
                <svg width=""24"" height=""24"" viewBox=""0 0 24 24"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                    <circle cx=""12"" cy=""12"" r=""10"" fill=""#F44336"" stroke=""white"" stroke-width=""2""/>
                    <text x=""12"" y=""16"" text-anchor=""middle"" fill=""white"" font-size=""12"" font-weight=""bold"">E</text>
                </svg>
            `),
            scaledSize: new google.maps.Size(24, 24),
            anchor: new google.maps.Point(12, 12)
        }}
    }});

    // Add current position marker
    window.cycleRouteCurrentMarker = new google.maps.Marker({{
        position: {{
            lat: {currentPoint.Latitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture)}, 
            lng: {currentPoint.Longitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture)}
        }},
        map: window.cycleRouteMap,
        title: 'Current Position',
        icon: {{
            url: 'data:image/svg+xml;charset=UTF-8,' + encodeURIComponent(`
                <svg width=""20"" height=""20"" viewBox=""0 0 20 20"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                    <circle cx=""10"" cy=""10"" r=""8"" fill=""#2196F3"" stroke=""white"" stroke-width=""2""/>
                    <circle cx=""10"" cy=""10"" r=""3"" fill=""white""/>
                </svg>
            `),
            scaledSize: new google.maps.Size(20, 20),
            anchor: new google.maps.Point(10, 10)
        }},
        zIndex: 1000
    }});

    // Fit map to show the entire route
    const bounds = new google.maps.LatLngBounds();
    routePath.forEach(point => bounds.extend(point));
    window.cycleRouteMap.fitBounds(bounds);
    
    // Set a good zoom level for detailed view while showing context
    google.maps.event.addListenerOnce(window.cycleRouteMap, 'bounds_changed', function() {{
        if (window.cycleRouteMap.getZoom() < 16) {{
            window.cycleRouteMap.setZoom(16);
        }}
        if (window.cycleRouteMap.getZoom() > 17) {{
            window.cycleRouteMap.setZoom(17);
        }}
    }});
}}

// Initialize the map when called
initializeMap_{mapElementId}();");

        return script.ToString();
    }

    public string GenerateUpdatePositionScript(RoutePoint currentPoint, string mapElementId)
    {
        return $@"
if (window.cycleRouteCurrentMarker && window.cycleRouteMap) {{
    const newPosition = {{
        lat: {currentPoint.Latitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture)}, 
        lng: {currentPoint.Longitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture)}
    }};
    
    window.cycleRouteCurrentMarker.setPosition(newPosition);
    window.cycleRouteMap.panTo(newPosition);
}}";
    }

    public string GenerateRouteClickScript(CycleRoute route, string callbackFunction)
    {
        if (!route.Points.Any())
            return string.Empty;

        var routePointsJson = string.Join(",", route.Points.Select((p, index) => 
            $"{{lat: {p.Latitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture)}, lng: {p.Longitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture)}, index: {index}}}"));

        return $@"
if (window.cycleRoutePolyline && window.cycleRouteMap) {{
    const routePoints = [{routePointsJson}];
    
    // Function to find the closest point on the route to the clicked location
    function findClosestRoutePoint(clickedLatLng) {{
        let closestIndex = 0;
        let minDistance = Number.MAX_VALUE;
        
        routePoints.forEach((point, index) => {{
            const distance = google.maps.geometry.spherical.computeDistanceBetween(
                clickedLatLng, 
                new google.maps.LatLng(point.lat, point.lng)
            );
            
            if (distance < minDistance) {{
                minDistance = distance;
                closestIndex = index;
            }}
        }});
        
        return closestIndex;
    }}
    
    // Add click listener to the polyline
    window.cycleRoutePolyline.addListener('click', function(event) {{
        const clickedLatLng = event.latLng;
        const closestPointIndex = findClosestRoutePoint(clickedLatLng);
        
        // Call the C# callback function
        if (window.DotNet && window.{callbackFunction}) {{
            window.{callbackFunction}(closestPointIndex);
        }}
    }});
    
    // Also add click listener to the map for points near the route
    window.cycleRouteMap.addListener('click', function(event) {{
        const clickedLatLng = event.latLng;
        const closestPointIndex = findClosestRoutePoint(clickedLatLng);
        
        // Only trigger if the click is reasonably close to the route (within 100 meters)
        const closestPoint = routePoints[closestPointIndex];
        const distanceToRoute = google.maps.geometry.spherical.computeDistanceBetween(
            clickedLatLng,
            new google.maps.LatLng(closestPoint.lat, closestPoint.lng)
        );
        
        if (distanceToRoute <= 100) {{ // 100 meters tolerance
            if (window.DotNet && window.{callbackFunction}) {{
                window.{callbackFunction}(closestPointIndex);
            }}
        }}
    }});
}}";
    }
}
