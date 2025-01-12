using System.Collections.Generic;
using System.Threading.Tasks;

namespace Archivum.Contracts.Repositories;

public interface IMangaRepository
{
    Task<IEnumerable<Models.Manga>> GetMangasAsync();
    Task<IEnumerable<Models.Manga>> GetMangasFromAuthorAsync(string author);
    Task<IEnumerable<Models.Manga>> GetMangasFromTitleAsync(string Title);

    Task<IEnumerable<Models.Author>> GetAuthorsAsync();
    Task<IEnumerable<Models.Title>> GetTitlesAsync();
}
