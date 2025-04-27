
using StoryConnect.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using BooklyNugget.Models;
using StoryConnect_V2.Services;

namespace StoryConnect.Repositories
{
    public class RepositoryLibros : IRepositoryLibros
    {
        private StoryContext context;
        private BooklyService service;

        public RepositoryLibros(StoryContext context, BooklyService service)
        {
            this.context = context;
            this.service = service;
        }

        public async Task<List<LibrosListasPredefinidas>> LibrosEnPredefinidos(int idUsuario)
        {
            var consulta = await this.context.LibrosListasPredefinidas
                           .Where(datos => datos.Id == idUsuario)
                           .AsNoTracking()
                           .ToListAsync();
            return consulta;
        }

        public async Task<List<LibrosListasPredefinidas>> FindLibrosEnPredefinidos(int idUsuario, int idlista)
        {
            var consulta = await this.context.LibrosListasPredefinidas
                           .Where(datos => datos.Id == idUsuario && datos.ListaId == idlista)
                           .AsNoTracking()
                           .ToListAsync();
            return consulta;
        }

        public async Task<bool> UpdateFotoUsuario(int idUsuario, string fileName)
        {
            try
            {
                var usuario = await this.service.GetUsuario(idUsuario);

                if (usuario == null)
                    return false;
                usuario.ImagenPerfil = fileName;
                await this.service.UpdateUsuarioData(usuario);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar foto de perfil: {ex.Message}");
                return false;
            }
        }
    }
}
