using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Archivum.Contracts.Repositories;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Options;

namespace Archivum.ViewModels;

public class MangasViewModel : ObservableObject
{
    public ObservableCollection<MangaViewModel> Mangas { get; } = [];

    public MangasViewModel(IMangaRepository repository, IOptions<Models.Settings> settings) {
        _repository = repository;
        _settings = settings.Value!;
        Mangas.CollectionChanged += MangasCollectionChanged;
    }

    public async Task SyncAsync() {
        var mangas = await _repository.GetMangasAsync();
        foreach (var manga in mangas) {
            var viewModel = Mangas.SingleOrDefault(m => m.Path == manga.Path);
            if (viewModel == null) {
                Mangas.Add(new(manga, _repository, _settings));
            }
        }
    }

    void MangasCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.Action == NotifyCollectionChangedAction.Add) {
            foreach (var manga in e.NewItems!.OfType<MangaViewModel>()) {
                manga.LoadCoverAsync();
            }
        }
    }

    readonly IMangaRepository _repository;
    readonly Models.Settings _settings;
}
