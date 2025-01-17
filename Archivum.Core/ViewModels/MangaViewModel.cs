using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Archivum.Contracts.Repositories;
using Archivum.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Archivum.ViewModels;

public partial class MangaViewModel : ObservableObject
{
    [ObservableProperty]
    public partial ImageSource? Image { get; set; }
    [ObservableProperty]
    public partial bool IsRead { get; set; }
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayTitle))]
    public partial string Author { get; set; }
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayTitle))]
    public partial string Title { get; set; }
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayTitle))]
    public partial string Volume { get; set; }

    [ObservableProperty]
    public partial string Path { get; set; }
    [ObservableProperty]
    public partial int Cover { get; set; }
    [ObservableProperty]
    public partial DateTime Created { get; set; }
    [ObservableProperty]
    public partial DateTime Modified { get; set; }
    [ObservableProperty]
    public partial long Size { get; set; }

    public string DisplayTitle => _settings.FilePattern!
        .Replace(Models.Manga.AuthorPattern, Author)
        .Replace(Models.Manga.TitlePattern, Title)
        .Replace(Models.Manga.VolumePattern, Volume);

    [ObservableProperty()]
    [NotifyPropertyChangedFor(nameof(ViewSingleFrame))]
    [NotifyPropertyChangedFor(nameof(ViewSpreadFrame))]
    [NotifyPropertyChangedFor(nameof(Images))]
    public partial int Frame { get; protected set; } = 0;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Images))]
    public partial int Index { get; protected set; } = -1;

    public bool ViewSingleFrame => 1 == Frame;
    public bool ViewSpreadFrame => 2 == Frame;
    public ImageSource?[] Images => _images;
    public int Pages => _imageSources.Count;

    public MangaViewModel(Models.Manga model, IMangaRepository repository, Models.Settings settings) {
        Author = model.Author;
        Title = model.Title;
        Volume = model.Volume;
        Path = model.Path;
        Cover = model.Cover;
        Created = model.Created;
        Modified = model.Modified;
        Size = model.Size;
        IsRead = DateTime.MinValue < model.LastRead;

        _model = model;
        _repository = repository;
        _settings = settings;
        _images = new ImageSource[2];
    }

    public async Task LoadCoverAsync() {
        if (Image != null) return;

        try {
            using var archive = ZipFile.OpenRead(Path);
            var imageFile = archive.Entries.Where(_settings.IsImageEntry).ElementAtOrDefault(Cover);
            if (imageFile != null) {
                using var stream = imageFile.Open();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                Image = new ImageSource(memoryStream.ToArray());
            }
        } catch (Exception ex) {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }

    public async Task LoadAsync() {
        try {
            using var archive = ZipFile.OpenRead(Path);
            var imageFiles = archive.Entries.Where(_settings.IsImageEntry);
            foreach (var entry in imageFiles) {
                using var stream = entry.Open();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                _imageSources.Add(memoryStream.ToArray());
            }
            SetSingleFrameView();
            MoveToPreviousFrame();
            OnPropertyChanged(nameof(Pages));
        } catch { }
    }

    public void Unload() {
        _imageSources.Clear();
    }

    public void ApplyEdit() {
        _model.Author = Author;
        _model.Title = Title;
        _model.Volume = Volume;
        _model.Cover = Cover;
    }

    public void CancelEdit() {
        Author = _model.Author;
        Title = _model.Title;
        Volume = _model.Volume;
        Cover = _model.Cover;
    }

    public void SetFrameIndex(int value) {
        Index = Math.Clamp(value, 0, _imageSources.Count - Frame);
    }

    public void UpdateLastRead() {
        _model.LastRead = DateTime.Now;
    }

    [RelayCommand]
    void SetSingleFrameView() {
        Frame = 1;
    }

    [RelayCommand]
    void SetSpreadFrameView() {
        Frame = 2;
    }

    [RelayCommand]
    void MoveToPreviousFrame() {
        Index = Math.Max(0, Index - 1);
    }

    [RelayCommand]
    void MoveToNextFrame() {
        Index = Math.Min(_imageSources.Count - 1, Index + 1);
    }

    [RelayCommand]
    void MoveToPreviousView() {
        Index = Math.Max(0, Index - Frame);
    }

    [RelayCommand]
    void MoveToNextView() {
        Index = Math.Min(_imageSources.Count - Frame, Index + Frame);
    }

    partial void OnFrameChanged(int value) {
        if (value < 1 || 2 < value) return;
        if (Index < 0 || _imageSources.Count <= Index) return;
        for (var i = 0; i < value; i++) {
            _images[i] = new ImageSource(_imageSources[Index + i]);
        }
    }

    partial void OnIndexChanged(int value) {
        if (value < 0 || _imageSources.Count <= value) return;
        for (var i = 0; i < Frame; i++) {
            _images[i] = new ImageSource(_imageSources[value + i]);
        }
    }

    readonly Models.Manga _model;
    readonly IMangaRepository _repository;
    readonly Models.Settings _settings;

    readonly ImageSource?[] _images = [];
    readonly List<byte[]> _imageSources = [];
}
