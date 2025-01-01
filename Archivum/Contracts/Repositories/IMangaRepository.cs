using Archivum.Models;

namespace Archivum.Contracts.Repositories;

public interface IMangaRepository
{
    Task<IEnumerable<Manga>> GetAllAsync();
}
