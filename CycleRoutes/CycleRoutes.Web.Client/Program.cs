using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CycleRoutes.Shared.Services;
using CycleRoutes.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add HttpClient for the client-side services
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add device-specific services used by the CycleRoutes.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

// Add route and street view services for client-side
builder.Services.AddSingleton<IRouteService, RouteService>();
builder.Services.AddSingleton<IStreetViewService, StreetViewService>();

await builder.Build().RunAsync();
