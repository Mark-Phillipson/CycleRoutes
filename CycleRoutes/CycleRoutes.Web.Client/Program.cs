using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CycleRoutes.Shared.Services;
using CycleRoutes.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the CycleRoutes.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

// Add route and street view services for client-side
builder.Services.AddSingleton<IRouteService, RouteService>();
builder.Services.AddSingleton<IStreetViewService, StreetViewService>();

await builder.Build().RunAsync();
