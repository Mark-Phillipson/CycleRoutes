using Microsoft.JSInterop;

namespace CycleRoutes.Shared.Services;

public class ThemeService : IThemeService
{
    private readonly IJSRuntime _jsRuntime;
    private bool _isDarkMode;

    public event Action? ThemeChanged;

    public bool IsDarkMode => _isDarkMode;

    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        try
        {
            // Try to get the saved theme from localStorage
            var savedTheme = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "theme");
            
            if (savedTheme == "dark")
            {
                _isDarkMode = true;
            }
            else if (savedTheme == "light")
            {
                _isDarkMode = false;
            }
            else
            {
                // If no saved theme, default to light mode
                // User can manually switch to dark if preferred
                _isDarkMode = false;
            }

            await ApplyThemeAsync();
        }
        catch
        {
            // Fallback to light theme if anything fails
            _isDarkMode = false;
            await ApplyThemeAsync();
        }
    }

    public async Task ToggleThemeAsync()
    {
        _isDarkMode = !_isDarkMode;
        await ApplyThemeAsync();
        await SaveThemeAsync();
        ThemeChanged?.Invoke();
    }

    public async Task SetThemeAsync(bool isDarkMode)
    {
        if (_isDarkMode != isDarkMode)
        {
            _isDarkMode = isDarkMode;
            await ApplyThemeAsync();
            await SaveThemeAsync();
            ThemeChanged?.Invoke();
        }
    }

    private async Task ApplyThemeAsync()
    {
        try
        {
            var theme = _isDarkMode ? "dark" : "light";
            
            // Set data-theme attribute on both html and body
            await _jsRuntime.InvokeVoidAsync("document.documentElement.setAttribute", "data-theme", theme);
            await _jsRuntime.InvokeVoidAsync("document.body.setAttribute", "data-theme", theme);
            
            // Also add/remove class as fallback
            if (_isDarkMode)
            {
                await _jsRuntime.InvokeVoidAsync("document.body.classList.add", "dark-theme");
                await _jsRuntime.InvokeVoidAsync("document.body.classList.remove", "light-theme");
            }
            else
            {
                await _jsRuntime.InvokeVoidAsync("document.body.classList.add", "light-theme");
                await _jsRuntime.InvokeVoidAsync("document.body.classList.remove", "dark-theme");
            }
            
            // Force a style recalculation
            await _jsRuntime.InvokeVoidAsync("eval", "document.body.offsetHeight");
        }
        catch
        {
            // Ignore JS errors in case JSRuntime isn't available yet
        }
    }

    private async Task SaveThemeAsync()
    {
        try
        {
            var theme = _isDarkMode ? "dark" : "light";
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "theme", theme);
        }
        catch
        {
            // Ignore JS errors
        }
    }
}
