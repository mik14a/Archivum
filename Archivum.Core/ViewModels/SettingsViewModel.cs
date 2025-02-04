using System;
using System.Collections.Generic;
using System.Linq;
using Archivum.Contracts.Services;
using Archivum.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Options;

namespace Archivum.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    public static readonly KeyValuePair<string, SystemBackdrop>[] BackdropItems = [
        new("デフォルト", SystemBackdrop.Default),
        new("マイカ", SystemBackdrop.Mica),
        new("マイカオルタナティブ", SystemBackdrop.MicaAlt),
        new("アクリル", SystemBackdrop.Acrylic)
    ];

    public static readonly KeyValuePair<string, SystemTheme>[] ThemeItems = [
        new("システム", SystemTheme.System),
        new("ライト", SystemTheme.Light),
        new("ダーク", SystemTheme.Dark)
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
    public partial KeyValuePair<string, SystemBackdrop> Backdrop { get; set; }

    [ObservableProperty]
    public partial KeyValuePair<string, SystemTheme> Theme { get; set; }

    public bool FilePatternChanged => _setting.FilePattern != FilePattern;

    public SettingsViewModel(IOptions<Models.Settings> settings,
                             IBackdropSelectorService backdropSelectorService,
                             IThemeSelectorService themeSelectorService) {
        _setting = settings.Value;
        _backdropSelectorService = backdropSelectorService;
        _themeSelectorService = themeSelectorService;
        Cancel();  // Apply model from settings
    }

    public Models.Settings Apply() {
        _setting.FolderPath = FolderPath;
        _setting.ImageExtensions = ImageExtensions;
        _setting.FolderPattern = FolderPattern;
        _setting.FilePattern = FilePattern;
        _setting.Backdrop = Backdrop.Value.ToString();
        _setting.Theme = Theme.Value.ToString();
        return _setting;
    }

    public void Cancel() {
        FolderPath = _setting.FolderPath ?? Models.Settings.DefaultFolderPath;
        ImageExtensions = _setting.ImageExtensions ?? Models.Settings.DefaultImageExtensions;
        FolderPattern = _setting.FolderPattern ?? Models.Settings.DefaultFolderPattern;
        FilePattern = _setting.FilePattern ?? Models.Settings.DefaultFilePattern;
        var backdropText = _setting.Backdrop ?? Models.Settings.DefaultBackdrop;
        var backdrop = Enum.TryParse(backdropText, out SystemBackdrop backdropType) ? backdropType : SystemBackdrop.Default;
        Backdrop = BackdropItems.Single(item => item.Value == backdrop);
        var themeText = _setting.Theme ?? Models.Settings.DefaultTheme;
        var theme = Enum.TryParse(themeText, out SystemTheme themeType) ? themeType : SystemTheme.System;
        Theme = ThemeItems.Single(item => item.Value == theme);
    }

    partial void OnBackdropChanged(KeyValuePair<string, SystemBackdrop> value) {
        _backdropSelectorService.SetBackdropAsync(value.Value);
    }

    partial void OnThemeChanged(KeyValuePair<string, SystemTheme> value) {
        _themeSelectorService.SetThemeAsync(value.Value);
    }

    readonly Models.Settings _setting;
    readonly IBackdropSelectorService _backdropSelectorService;
    readonly IThemeSelectorService _themeSelectorService;
}
