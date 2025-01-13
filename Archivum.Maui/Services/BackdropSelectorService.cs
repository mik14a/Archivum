using System.Threading.Tasks;
using Archivum.Contracts.Services;

#if WINDOWS10_0_17763_0_OR_GREATER
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Composition.SystemBackdrops;
#endif

namespace Archivum.Services;

class BackdropSelectorService : IBackdropSelectorService
{
#if WINDOWS10_0_17763_0_OR_GREATER
    public async Task InitializeAsync(Window window, Controls.SystemBackdrop backdrop) {
        _window = window;
        await SetBackdropAsync(backdrop);
    }
#endif

#if WINDOWS10_0_17763_0_OR_GREATER
    public async Task SetBackdropAsync(Controls.SystemBackdrop backdrop) {
        if (_window == null) throw new InvalidOperationException("InitializeAsync must be called first.");
        if (_backdrop == backdrop) return;
        _window.SystemBackdrop = (_backdrop = backdrop) switch {
            Controls.SystemBackdrop.Default => null,
            Controls.SystemBackdrop.Mica => new MicaBackdrop { Kind = MicaKind.Base },
            Controls.SystemBackdrop.MicaAlt => new MicaBackdrop { Kind = MicaKind.BaseAlt },
            Controls.SystemBackdrop.Acrylic => new DesktopAcrylicBackdrop(),
            _ => throw new System.NotImplementedException(),
        };
        await Task.CompletedTask;
    }

    Window? _window;
    Controls.SystemBackdrop _backdrop;
#else
    public async Task SetBackdropAsync(Controls.SystemBackdrop backdrop) {
        await Task.CompletedTask;
    }
#endif
}
