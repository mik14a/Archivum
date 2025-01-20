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
/// An Title page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class TitlePage : Page
{
    public TitleViewModel? Model => _model;

    public TitlePage() {
        _navigationService = App.GetService<INavigationService>();
        InitializeComponent();
        DataContext = this;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e) {
        switch (e.NavigationMode) {
        case NavigationMode.New:
            _model = (TitleViewModel)e.Parameter;
            _model.LoadCoverAsync().ConfigureAwait(false);
            _model.SyncAsync().ConfigureAwait(false);
            break;
        case NavigationMode.Back:
            _model = (TitleViewModel)e.Parameter;
            _model.SyncAsync().ConfigureAwait(false);
            break;
        }
        base.OnNavigatedTo(e);
    }

    [RelayCommand]
    async Task CloseAsync() {
        await _navigationService.PopAsync();
    }

    void ItemClick(object sender, ItemClickEventArgs e) {
        var model = (MangaViewModel)e.ClickedItem;
        _navigationService.PushAsync(typeof(MangaPage), model);
    }

    readonly INavigationService _navigationService;
    TitleViewModel? _model;
}
