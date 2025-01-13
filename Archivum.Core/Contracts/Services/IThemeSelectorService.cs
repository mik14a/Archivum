using System.Threading.Tasks;
using Archivum.Controls;

namespace Archivum.Contracts.Services;

public interface IThemeSelectorService
{
    Task SetThemeAsync(SystemTheme theme);
}
