using System.Collections.ObjectModel;
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

    }

    public async Task LoadAsync() {
        var mangas = await _repository.GetAllAsync();
        foreach (var manga in mangas) {
            var viewModel = Mangas.SingleOrDefault(m => m.Path == manga.Path);
            if (viewModel == null) {
                Mangas.Add(new(manga, _settings.ImageExtensions!));
            }
        }
    }

    readonly IMangaRepository _repository;
    readonly Models.Settings _settings;
}
