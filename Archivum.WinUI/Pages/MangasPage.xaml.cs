using System.Threading.Tasks;
using Archivum.Contracts.Services;
using Archivum.ViewModels;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Archivum.Pages;

public sealed partial class MangasPage : Page
{
    public MangasViewModel Model { get; }

    public MangasPage() {
        Model = App.GetService<MangasViewModel>();
        _navigationService = App.GetService<INavigationService>();
        InitializeComponent();
        DataContext = this;
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
        var model = (MangaViewModel)e.ClickedItem;
        _navigationService.PushAsync(typeof(MangaPage), model);
    }

    readonly INavigationService _navigationService;
}
