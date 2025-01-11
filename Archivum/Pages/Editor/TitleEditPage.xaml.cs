using System.Threading.Tasks;
using Archivum.ViewModels;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace Archivum.Pages.Editor;

public partial class TitleEditPage : ContentPage
{
    public TitleViewModel Model { get; }

    public TitleEditPage(TitleViewModel model) {
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
