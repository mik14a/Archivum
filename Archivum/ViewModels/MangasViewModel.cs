using System.Collections.ObjectModel;
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

    }

    public async Task LoadAsync() {
        var mangas = await _repository.GetAllAsync();
        foreach (var manga in mangas) {
            Mangas.Add(ToMangaViewModel(manga));
        }

        MangaViewModel ToMangaViewModel(Models.Manga model) => new(model, _settings.ImageExtensions);
    }

    readonly IMangaRepository _repository;
    readonly Models.Settings _settings;
}
