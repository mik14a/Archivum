using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Options;

namespace Archivum.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    public static readonly KeyValuePair<string, Controls.SystemBackdrop>[] BackdropItems = [
        new("デフォルト", Controls.SystemBackdrop.Default),
        new("マイカ", Controls.SystemBackdrop.Mica),
        new("マイカオルタナティブ", Controls.SystemBackdrop.MicaAlt),
        new("アクリル", Controls.SystemBackdrop.Acrylic)
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
    public partial KeyValuePair<string, Controls.SystemBackdrop> Backdrop { get; set; }

    public SettingsViewModel(IOptions<Models.Settings> settings) {
        _setting = settings.Value;
        Cancel();  // Apply model from settings
    }

    public Models.Settings Apply() {
        _setting.FolderPath = FolderPath;
        _setting.ImageExtensions = ImageExtensions;
        _setting.FolderPattern = FolderPattern;
        _setting.FilePattern = FilePattern;
        _setting.Backdrop = Backdrop.Value.ToString();
        return _setting;
    }

    public void Cancel() {
        FolderPath = _setting.FolderPath ?? Models.Settings.DefaultFolderPath;
        ImageExtensions = _setting.ImageExtensions ?? Models.Settings.DefaultImageExtensions;
        FolderPattern = _setting.FolderPattern ?? Models.Settings.DefaultFolderPattern;
        FilePattern = _setting.FilePattern ?? Models.Settings.DefaultFilePattern;
        var backdropText = _setting.Backdrop ?? Models.Settings.DefaultBackdrop;
        var backdrop = Enum.TryParse(backdropText, out Controls.SystemBackdrop backdropType) ? backdropType : Controls.SystemBackdrop.Default;
        Backdrop = BackdropItems.Single(item => item.Value == backdrop);
    }

    readonly Models.Settings _setting;
}
