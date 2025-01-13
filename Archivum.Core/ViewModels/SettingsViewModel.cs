using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Options;

namespace Archivum.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    public static readonly string[] BackdropItems = [
        nameof(Controls.SystemBackdrop.Default),
        nameof(Controls.SystemBackdrop.Mica),
        nameof(Controls.SystemBackdrop.MicaAlt),
        nameof(Controls.SystemBackdrop.Acrylic)
    ];

    [ObservableProperty]
    public partial string FolderPath { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ImageExtensions { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string FolderPattern { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string FilePattern { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Backdrop { get; set; } = string.Empty;

    public SettingsViewModel(IOptions<Models.Settings> settings) {
        _setting = settings.Value;
        Cancel();  // Apply model from settings
    }

    public Models.Settings Apply() {
        _setting.FolderPath = FolderPath;
        _setting.ImageExtensions = ImageExtensions;
        _setting.FolderPattern = FolderPattern;
        _setting.FilePattern = FilePattern;
        _setting.Backdrop = Backdrop;
        return _setting;
    }

    public void Cancel() {
        FolderPath = _setting.FolderPath ?? Models.Settings.DefaultFolderPath;
        ImageExtensions = _setting.ImageExtensions ?? Models.Settings.DefaultImageExtensions;
        FolderPattern = _setting.FolderPattern ?? Models.Settings.DefaultFolderPattern;
        FilePattern = _setting.FilePattern ?? Models.Settings.DefaultFilePattern;
        Backdrop = _setting.Backdrop ?? Models.Settings.DefaultBackdrop;
    }

    readonly Models.Settings _setting;
}
