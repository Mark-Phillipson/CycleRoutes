using Microsoft.AspNetCore.Mvc;

namespace CycleRoutes.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StreetViewController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public StreetViewController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult GetStreetView(double lat, double lng, double heading = 0, int pitch = 0, int fov = 90)
    {
        var apiKey = _configuration["GoogleMaps:ApiKey"];
        
        if (string.IsNullOrEmpty(apiKey))
        {
            return BadRequest("Google Maps API key not configured");
        }

        // Generate the Google Street View Embed URL with the API key
        var streetViewUrl = $"https://www.google.com/maps/embed/v1/streetview?key={apiKey}&location={lat:F6},{lng:F6}&heading={heading:F1}&pitch={pitch}&fov={fov}";
        
        // Redirect to the Google Street View URL
        return Redirect(streetViewUrl);
    }
}
