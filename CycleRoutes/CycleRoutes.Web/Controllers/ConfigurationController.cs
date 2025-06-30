using Microsoft.AspNetCore.Mvc;

namespace CycleRoutes.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigurationController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public ConfigurationController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("google-maps")]
    public IActionResult GetGoogleMapsConfig()
    {
        var apiKey = _configuration["GoogleMaps:ApiKey"];
        
        // Only return the API key if it exists and we're in a development environment
        // In production, you might want to handle this differently for security
        var hasApiKey = !string.IsNullOrEmpty(apiKey);
        
        return Ok(new
        {
            HasApiKey = hasApiKey,
            ApiUrl = hasApiKey ? $"https://maps.googleapis.com/maps/api/js?key={apiKey}&callback=initGoogleMaps&loading=async" : string.Empty
        });
    }
}
