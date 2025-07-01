using CycleRoutes.Web.Components;
using CycleRoutes.Shared.Services;
using CycleRoutes.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Add controllers for API endpoints
builder.Services.AddControllers();

// Add HTTP client for API validation
builder.Services.AddHttpClient();

// Add device-specific services used by the CycleRoutes.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

// Add route and street view services with appropriate lifetimes
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<IStreetViewService, StreetViewService>();
builder.Services.AddScoped<IGoogleMapsService, GoogleMapsService>();
builder.Services.AddScoped<IThemeService, ThemeService>();
builder.Services.AddSingleton<ApiKeyTester>();

// Add detailed circuit errors for Blazor Server in development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// Add controllers for API endpoints
app.MapControllers();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(CycleRoutes.Shared.Services.IFormFactor).Assembly,
        typeof(CycleRoutes.Web.Client._Imports).Assembly);

// Test endpoint for API key validation
app.MapGet("/test-api-key", async (ApiKeyTester tester, IConfiguration config) =>
{
    var apiKey = config["GoogleMaps:ApiKey"] ?? string.Empty;
    var allConfig = config.AsEnumerable().Where(x => x.Key.Contains("GoogleMaps")).ToList();
    var result = await tester.TestApiKeyAsync(apiKey);
    return Results.Json(new { 
        apiKey = string.IsNullOrEmpty(apiKey) ? "Not configured" : "***", 
        result,
        debugConfig = allConfig.ToDictionary(x => x.Key, x => x.Value),
        configPath = "appsettings.json should be in: " + Directory.GetCurrentDirectory()
    });
});

app.Run();
