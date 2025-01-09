using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Archivum.Contracts.Repositories;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Options;

namespace Archivum.ViewModels;

public partial class TitlesViewModel : ObservableObject
{
    public ObservableCollection<TitleViewModel> Titles { get; } = [];

    public TitlesViewModel(IMangaRepository repository, IOptions<Models.Settings> settings) {
        _repository = repository;
        _settings = settings.Value!;
        Titles.CollectionChanged += TitlesCollectionChanged;
    }

    public async Task SyncAsync() {
        var titles = await _repository.GetTitlesAsync();
        foreach (var title in titles) {
            var existing = Titles.FirstOrDefault(t => t.Name == title.Name);
            if (existing != null) {
                existing.Author = title.Author;
                existing.Count = title.Count;
                existing.LastModified = title.LastModified;
            } else {
                Titles.Add(new(title, _repository, _settings));
            }
        }
    }

    void TitlesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.Action == NotifyCollectionChangedAction.Add) {
            foreach (var title in e.NewItems!.OfType<TitleViewModel>()) {
                title.LoadCoverAsync();
            }
        }
    }

    readonly IMangaRepository _repository;
    readonly Models.Settings _settings;
}
