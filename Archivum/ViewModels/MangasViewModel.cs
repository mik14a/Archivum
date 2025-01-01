using System.Collections.ObjectModel;
using Archivum.Contracts.Repositories;
using Archivum.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Archivum.ViewModels;

public class MangasViewModel : ObservableObject
{
    public ObservableCollection<MangaViewModel> Mangas { get; } = [];

    public MangasViewModel(IMangaRepository repository) {
        _repository = repository;
    }

    public async Task LoadAsync() {
        var mangas = await _repository.GetAllAsync();
        foreach (var manga in mangas) {
            Mangas.Add(ToMangaViewModel(manga));
        }

        static MangaViewModel ToMangaViewModel(Manga model) => new(model);
    }

    readonly IMangaRepository _repository;
}
