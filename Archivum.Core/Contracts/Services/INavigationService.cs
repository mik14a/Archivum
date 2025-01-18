using System;
using System.Threading.Tasks;

namespace Archivum.Contracts.Services;

public interface INavigationService
{
    void NavigateTo(string tag);
    Task<bool> PushAsync(Type sourcePageType, object parameter);
    Task PopAsync();
}
