using System.Threading.Tasks;
using Archivum.ViewModels;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace Archivum.Pages.Editor;

public partial class AuthorEditPage : ContentPage
{
    public AuthorViewModel Model { get; }

    public AuthorEditPage(AuthorViewModel model) {
        Model = model;
        InitializeComponent();
        BindingContext = this;
    }

    protected override async void OnAppearing() {
        await Model.SyncAsync();
        base.OnAppearing();
    }

    [RelayCommand]
    async Task SaveAsync() {
        Model.ApplyEdit();
        await Navigation.PopModalAsync();
    }

    [RelayCommand]
    async Task CancelAsync() {
        Model.CancelEdit();
        await Navigation.PopModalAsync();
    }
}
