using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using BooklyNugget.Models;

namespace StoryConnect.Repositories
{
    public interface IRepositoryLibros
    {
        Task<List<LibrosListasPredefinidas>> LibrosEnPredefinidos(int idUsuario);
        Task<List<LibrosListasPredefinidas>> FindLibrosEnPredefinidos(int idUsuario, int idlista);
        Task<bool> UpdateFotoUsuario(int idUsuario, string fileName);
    }
}
