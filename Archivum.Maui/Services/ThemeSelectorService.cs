using System.Threading.Tasks;
using Archivum.Contracts.Services;
using Archivum.Controls;

namespace Archivum.Services;

class ThemeSelectorService : IThemeSelectorService
{
    public async Task SetThemeAsync(SystemTheme theme) {
        await Task.CompletedTask;
    }
}
