using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using StoryConnect.Models;
using StoryConnect.Repositories;
using StoryConnect_V2.Helper;
using StoryConnect_V2.Models;

namespace StoryConnect.Controllers
{
    public class LibrosController : Controller
    {
        private IRepositoryLibros repo;
        public LibrosController(IRepositoryLibros repo)
        {
            this.repo = repo;
        }
        public async Task<IActionResult> Index()
        {
            int? idUsuario = HttpContext.Session.GetInt32("id");

            var libros = await this.repo.GetLibrosAsync(idUsuario);

            var etiquetas = await this.repo.GetEtiquetas();
            var autores = await this.repo.GetAutoresAsync();
            var librosetiquetas = await this.repo.GetEtiquetasLibroByUsuario(idUsuario);

            var datos = new Biblioteca
            {
                Libros = libros,
                Etiquetas = etiquetas,
                Autores = autores,
                LibroEtiquetas = librosetiquetas
            };

            return View(datos);
        }

        public async Task<IActionResult> Detalles(int id)
        {

            int? idUsuario = HttpContext.Session.GetInt32("id");

            Libros libro = await this.repo.FindLibros(id);
            var etiquetas = await this.repo.ObtenerEtiquetasLibro(id);
            List<Resenas> Reseñas = await this.repo.Reseñas(id);

            int listaId = 0;
            if (idUsuario.HasValue)
            {
                listaId = await this.repo.LibrosListaDetalle(id, idUsuario.Value);
                if (listaId == 0)
                    listaId = 0;
            }
            var detallesLibro = new LibrosDetalles
            {
                Libro = libro,
                Etiquetas = etiquetas,
                Resenas = Reseñas,
                ListaLibro = listaId
            };

            return View(detallesLibro);
        }

        [HttpGet]
        public async Task<JsonResult> BuscarLibros(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Json(new { results = new List<object>() });
            }

            var libros = await this.repo.BuscarLibrosAsync(query);

            var resultado = libros.Select(l => new
            {
                id = l.Id,
                titulo = l.Titulo,
                autor = l.NombreAutor != null ? l.NombreAutor : "Autor desconocido"
            }).ToList();

            return Json(new { results = resultado });
        }

        public async Task<IActionResult> Home()
        {
            int? idUsuario = HttpContext.Session.GetInt32("id");

            List<LibrosLeyendo> libro = await this.repo.LibrosLeyendo(idUsuario.Value);
            List<ProgresoLectura> progresoLectura = new List<ProgresoLectura>();

            foreach (var lib in libro)
            {
                var progresosLectura = await this.repo.GetProgresoLectura(idUsuario.Value, lib.Id);
                progresoLectura.Add(progresosLectura);
            }

            var libros = new LibrosLeyendoProgreso
            {
                Leyendos = libro,
                ProgresoLectura = progresoLectura
            };

            return View(libros);
        }

        [HttpPost]
        public async Task<IActionResult> MoverLibrosEntreListas(int idlibro, int origen, int destino)
        {
            int idusuario = (int)HttpContext.Session.GetInt32("id");
            Console.WriteLine(idusuario.ToString(), idlibro, origen, destino);
            await this.repo.MoverLibrosLista(idusuario, idlibro, origen, destino);
            if (destino == 1)
                await this.repo.InsertProgreso(idusuario, idlibro);
            else if (destino != 1)
            {
                int id = (int)await this.repo.FindProgreso(idusuario, idlibro);
                await this.repo.DeleteProgreso(id, idusuario);
            }
            return RedirectToAction("Home");
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarReseña(Resenas model)
        {
            try
            {
                int idusuario = (int)HttpContext.Session.GetInt32("id");
                int idreseña = model.Id;
                var reseña = await this.repo.UpdateReseña(model.Id, idusuario, model.calificacion, model.texto);

                if (reseña == null)
                {
                    return BadRequest(new { message = "No se encontró la reseña o no tienes permisos para editarla." });
                }

                return RedirectToAction("Detalles", new {id = model.idLibro});
            }
            catch (Exception ex)
            {
                // Log del error
                return StatusCode(500, new { message = ex.Message });
            }

        }


        [HttpPost]
        public async Task<IActionResult> InsertReseña(Resenas model)
        {
            int idusuario = (int)HttpContext.Session.GetInt32("id");

            await this.repo.InsertReseña(idusuario, model.idLibro, model.calificacion, model.texto);
            return RedirectToAction("Detalles", new { id = model.idLibro });

        }

        [HttpPost]
        public async Task<IActionResult> UpdateProgreso(ProgresoLectura progreso)
        {
            int idusuario = (int)HttpContext.Session.GetInt32("id");
            int idprogreso = (int)await this.repo.FindProgreso(idusuario, progreso.idLibro);
            await this.repo.UpdateProgreso(idprogreso, idusuario, progreso.Pagina);

            return RedirectToAction("Home", new { id = progreso.idLibro });
        }

        public async Task<IActionResult> Generos()
        {
            int? idUsuario = HttpContext.Session.GetInt32("id");

            var etiquetas = await this.repo.GetEtiquetas();
            List<LibrosDTO> libros = await this.repo.GetLibrosAsync(idUsuario);
            var generosConLibros = etiquetas.Select(e => new Generos
            {
                Genero = e,
                Libros = libros.Where(l => l.EtiquetaId == e.Id).ToList()
            }).Where(x => x.Libros.Any()).ToList();


            generosConLibros.Shuffle();

            var generarAleatorios = generosConLibros.Take(2).ToList();

            var VistaGeneros = new GenerosDTO
            {
                GenerosDestacados = generarAleatorios,
                TodosLosGeneros = etiquetas
            };

            return View(VistaGeneros);
        }

        public async Task<IActionResult> Genero(int id)
        {
            int? idUsuario = HttpContext.Session.GetInt32("id");

            List<Libros> libros = await this.repo.FiltrarPorEtiquetas(id);
            return View(libros);
        }
    }
}
