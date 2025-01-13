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

    public async Task SyncAsync(bool sortItems = false) {
        var mangas = await _repository.GetMangasAsync();
        foreach (var manga in mangas) {
            var viewModel = Mangas.SingleOrDefault(m => m.Path == manga.Path);
            if (viewModel == null) {
                Mangas.Add(new(manga, _repository, _settings));
            }
        }

        if (sortItems) {
            for (var i = 0; i < Mangas.Count - 1; i++) {
                for (var j = 0; j < Mangas.Count - i - 1; j++) {
                    var compareAuthor = string.Compare(Mangas[j].Author, Mangas[j + 1].Author);
                    if (0 < compareAuthor) {
                        (Mangas[j + 1], Mangas[j]) = (Mangas[j], Mangas[j + 1]);
                    } else if (compareAuthor == 0) {
                        var compareTitle = string.Compare(Mangas[j].Title, Mangas[j + 1].Title);
                        if (0 < compareTitle) {
                            (Mangas[j + 1], Mangas[j]) = (Mangas[j], Mangas[j + 1]);
                        } else if (compareTitle == 0) {
                            var compareVolume = string.Compare(Mangas[j].Volume, Mangas[j + 1].Volume);
                            if (0 < compareVolume) {
                                (Mangas[j + 1], Mangas[j]) = (Mangas[j], Mangas[j + 1]);
                            }
                        }
                    }
                }
            }
        }
    }

    async void MangasCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.Action == NotifyCollectionChangedAction.Add) {
            foreach (var manga in e.NewItems!.OfType<MangaViewModel>()) {
                await manga.LoadCoverAsync().ConfigureAwait(false);
            }
        }
    }

    readonly IMangaRepository _repository;
    readonly Models.Settings _settings;
}
