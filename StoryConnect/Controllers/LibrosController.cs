using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using BooklyNugget.Models;
using StoryConnect_V2.Services;
using System.Security.Claims;

namespace StoryConnect.Controllers
{
    public class LibrosController : Controller
    {
        private BooklyService service;
        public LibrosController(BooklyService service)
        {
            this.service = service;
        }
        public async Task<IActionResult> Index()
        {
            var request = await this.service.GetBibliotecaAsync();
            var datos = new Biblioteca
            {
                Libros = request.Libros,
                Etiquetas = request.Etiquetas,
                Autores = request.Autores,
                LibroEtiquetas = request.LibroEtiquetas
            };
            return View(datos);
        }

        public async Task<IActionResult> Detalles(int id)
        {
            var request = await this.service.FindLibroAsync(id);
            var libro = new LibrosDetalles
            {
                Libro = request.Libro,
                Etiquetas = request.Etiquetas,
                Resenas = request.Resenas,
                ListaLibro = request.ListaLibro
            };
            return View(libro);
        }

        [HttpGet]
        public async Task<JsonResult> BuscarLibros(string query)
        {
            var request = await this.service.BuscarLibros(query);
            var resultado = request.Select(l => new
            {
                id = l.Id,
                titulo = l.Titulo,
                autor = l.Autor != null ? l.Autor : "Autor desconocido",
                imagen = l.Imagen,
            }).ToList().Distinct();
            return Json(new { results = resultado });
        }

        public async Task<IActionResult> Home()
        {
            var request = await this.service.Home();
            var libros = new LibrosLeyendoProgreso
            {
                Leyendos = request.Leyendos,
                ProgresoLectura = request.ProgresoLectura ?? new List<ProgresoLectura>()
            };
            return View(libros);
        }

        [HttpPost]
        public async Task<IActionResult> MoverLibrosEntreListas(int idlibro, int origen, int destino)
        {
            await this.service.MoverLibrosListas(idlibro, origen, destino);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarReseña(ReseñaDTO model)
        {
            int idUser = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? "0");
            var nombre = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            var imagenPerdil = User.Claims.FirstOrDefault(x => x.Type == "imagen")?.Value;
            var res = new ReseñaDTO
            {
                Id = model.Id,
                IdLibro = model.IdLibro,
                UsuarioId = idUser,
                NombreUsuario = nombre,
                ImagenPerfil = imagenPerdil,
                Texto = model.Texto,
                Calificacion = model.Calificacion,
                Fecha = model.Fecha,
            };
            await this.service.ActualizarReseña(res);
            return RedirectToAction("Detalles", "Libros", new { id = model.IdLibro });
        }


        [HttpPost]
        public async Task<IActionResult> InsertReseña(ReseñaDTO model)
        {
            int idUser = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? "0");
            var nombre = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            var imagenPerdil = User.Claims.FirstOrDefault(x => x.Type == "imagen")?.Value;
            
            var reseña = new ReseñaDTO
            {
                Id = 10,
                IdLibro = model.IdLibro,
                UsuarioId = idUser,
                NombreUsuario = nombre,
                ImagenPerfil = imagenPerdil,
                Texto = model.Texto,
                Calificacion = model.Calificacion,
                Fecha = model.Fecha,
            };

            await this.service.InsertReseña(reseña);
            return RedirectToAction("Detalles", "Libros", new { id = model.IdLibro });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProgreso(ProgresoLectura progreso)
        {
            int idUser = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? "0");
            var nombre = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            var imagenPerdil = User.Claims.FirstOrDefault(x => x.Type == "imagen")?.Value;
            var prog = new ProgresoLectura
            {
                ID = progreso.ID,
                idUsuario = idUser,
                idLibro = progreso.idLibro,
                Porcetaje = progreso.Porcetaje,
                Pagina = progreso.Pagina,
                estado = progreso.estado,
                Inicio = progreso.Inicio,
                Actualizacion = DateTime.Now
            };
            await this.service.UpdateProgreso(prog);

            return RedirectToAction("Home", "Libros", new { id = progreso.idLibro });
        }

        public async Task<IActionResult> Generos()
        {
            var request = await this.service.GetGeneros();
            var VistaGeneros = new GenerosDTO
            {
                GenerosDestacados = request.GenerosDestacados,
                TodosLosGeneros = request.TodosLosGeneros
            };
            return View(VistaGeneros);
        }

        public async Task<IActionResult> Genero(int id)
        {
            var request = await this.service.GetDetalleGenero(id);
            var VistaGeneros = new Generos
            {
                Genero = request.Genero,
                Libros = request.Libros
            };
            return View(VistaGeneros);
        }
    }
}
