using Microsoft.Extensions.Logging;
using CycleRoutes.Shared.Services;
using CycleRoutes.Services;

namespace CycleRoutes;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Add device-specific services used by the CycleRoutes.Shared project
        builder.Services.AddSingleton<IFormFactor, FormFactor>();
        
        // Add route and street view services for MAUI
        builder.Services.AddScoped<IRouteService, RouteService>();
        builder.Services.AddScoped<IStreetViewService, StreetViewService>();
        builder.Services.AddScoped<IGoogleMapsService, GoogleMapsService>();
        builder.Services.AddScoped<IThemeService, ThemeService>();

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
