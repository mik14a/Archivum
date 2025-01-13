using System;
using System.Threading.Tasks;
using Archivum.Contracts.Services;
using Microsoft.UI.Xaml;

namespace Archivum.Services;

class ThemeSelectorService : IThemeSelectorService
{
    public async Task InitializeAsync(Window window, Controls.SystemTheme theme) {
        _window = window;
        await SetThemeAsync(theme);
    }

    public async Task SetThemeAsync(Controls.SystemTheme theme) {
        if (_window is null) throw new InvalidOperationException("Initialize must be called first");
        if (_theme == theme) return;
        if (_window.Content is FrameworkElement rootElement) {
            rootElement.RequestedTheme = (_theme = theme) switch {
                Controls.SystemTheme.System => ElementTheme.Default,
                Controls.SystemTheme.Light => ElementTheme.Light,
                Controls.SystemTheme.Dark => ElementTheme.Dark,
                _ => throw new ArgumentException("Invalid theme", nameof(theme))
            };
        }
        await Task.CompletedTask;
    }

    Window? _window;
    Controls.SystemTheme _theme;
}
