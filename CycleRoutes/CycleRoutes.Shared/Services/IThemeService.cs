namespace CycleRoutes.Shared.Services;

public interface IThemeService
{
    event Action? ThemeChanged;
    bool IsDarkMode { get; }
    Task ToggleThemeAsync();
    Task SetThemeAsync(bool isDarkMode);
    Task InitializeAsync();
}
