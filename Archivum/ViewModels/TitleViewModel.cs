using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;

namespace Archivum.ViewModels;

public partial class TitleViewModel : ObservableObject
{
    [ObservableProperty]
    public partial ImageSource? Image { get; set; }
    [ObservableProperty]
    public partial string Name { get; set; }
    [ObservableProperty]
    public partial string Author { get; set; }
    [ObservableProperty]
    public partial int Count { get; set; }
    [ObservableProperty]
    public partial DateTime LastModified { get; set; }

    public TitleViewModel(Models.Title title) {
        Name = title.Name;
        Author = title.Author;
        Count = title.Count;
        LastModified = title.LastModified;
    }
}
