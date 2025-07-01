using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CycleRoutes.Shared.Services;
using CycleRoutes.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add HttpClient for the client-side services
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add device-specific services used by the CycleRoutes.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

// Add route and street view services for client-side with appropriate lifetimes
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<IStreetViewService, StreetViewService>();
builder.Services.AddScoped<IGoogleMapsService, GoogleMapsService>();
builder.Services.AddScoped<IThemeService, ThemeService>();

await builder.Build().RunAsync();
