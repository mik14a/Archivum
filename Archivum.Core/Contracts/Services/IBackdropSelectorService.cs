using System.Threading.Tasks;
using Archivum.Controls;

namespace Archivum.Contracts.Services;

public interface IBackdropSelectorService
{
    Task SetBackdropAsync(SystemBackdrop backdrop);
}
