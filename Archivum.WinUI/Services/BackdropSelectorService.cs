using System;
using System.Threading.Tasks;
using Archivum.Contracts.Services;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace Archivum.Services;

class BackdropSelectorService : IBackdropSelectorService
{
    public async Task InitializeAsync(Window window, Controls.SystemBackdrop backdrop) {
        _window = window;
        await SetBackdropAsync(backdrop);
    }

    public async Task SetBackdropAsync(Controls.SystemBackdrop backdrop) {
        if (_window is null) throw new InvalidOperationException("Initialize must be called first");
        if (_backdrop == backdrop) return;
        _window.SystemBackdrop = (_backdrop = backdrop) switch {
            Controls.SystemBackdrop.Default => null /* Clear backdrop */,
            Controls.SystemBackdrop.Mica => new MicaBackdrop() { Kind = MicaKind.Base },
            Controls.SystemBackdrop.MicaAlt => new MicaBackdrop() { Kind = MicaKind.BaseAlt },
            Controls.SystemBackdrop.Acrylic => new DesktopAcrylicBackdrop(),
            _ => throw new ArgumentException("Invalid backdrop", nameof(backdrop))
        };
        await Task.CompletedTask;
    }

    Window? _window;
    Controls.SystemBackdrop _backdrop;
}
