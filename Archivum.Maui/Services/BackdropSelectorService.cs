using System.Threading.Tasks;
using Archivum.Contracts.Services;
using Archivum.Controls;

namespace Archivum.Services;

class BackdropSelectorService : IBackdropSelectorService
{
    public async Task SetBackdropAsync(SystemBackdrop backdrop) {
        await Task.CompletedTask;
    }
}
