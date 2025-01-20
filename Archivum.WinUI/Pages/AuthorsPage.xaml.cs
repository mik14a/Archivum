using System.Threading.Tasks;
using Archivum.Contracts.Services;
using Archivum.ViewModels;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Archivum.Pages;

/// <summary>
/// An Authors page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AuthorsPage : Page
{
    public AuthorsViewModel Model { get; }

    public AuthorsPage() {
        Model = App.GetService<AuthorsViewModel>();
        _navigationService = App.GetService<INavigationService>();
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e) {
        base.OnNavigatedTo(e);
        Model.SyncAsync().ConfigureAwait(false);
    }

    [RelayCommand]
    async Task RefreshAsync() {
        await Model.SyncAsync(true);
    }

    void ItemClick(object sender, ItemClickEventArgs e) {
        var model = (AuthorViewModel)e.ClickedItem;
        _navigationService.PushAsync(typeof(AuthorPage), model);
    }

    readonly INavigationService _navigationService;
}
