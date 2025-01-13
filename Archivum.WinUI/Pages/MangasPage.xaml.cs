using System.Threading.Tasks;
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
        InitializeComponent();
        DataContext = this;
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e) {
        base.OnNavigatedTo(e);
        await Model.SyncAsync();
    }

    [RelayCommand]
    async Task RefreshAsync() {
        await Model.SyncAsync(true);
    }
}
