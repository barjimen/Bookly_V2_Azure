using System.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using StoryConnect.Context;
using StoryConnect.Repositories;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using StoryConnect.Helper;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using StoryConnect_V2.Helper;
using BooklyNugget.Models;
using StoryConnect_V2.Services;
using System.Security.Claims;

namespace StoryConnect.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private IRepositoryLibros repo;
        private HelperImages helperImages;
        private BooklyService service;
        public UsuariosController(IRepositoryLibros repo, IWebHostEnvironment hostingEnvironment, HelperImages helperImages, BooklyService service)
        {
            _hostingEnvironment = hostingEnvironment;
            this.repo = repo;
            this.helperImages = helperImages;
            this.service = service;
        }

        public IActionResult Login()
        {
            return View();
        }


        public IActionResult Register()
        {
            return View();
        }

        public async Task<IActionResult> Perfil()
        {
            var request = await this.service.Perfil();
            var home = new HomeUsuario
            {
                Usuarios = request.Usuarios,
                CountLibrosPred = request.CountLibrosPred,
                LibrosListasPred = request.LibrosListasPred,
                ObjetivosUsuarios = request.ObjetivosUsuarios,
                ProgresoLectura = request.ProgresoLectura
            };
            return View(home);
        }

        public async Task<IActionResult> MisLibros()
        {
            var response = await this.service.MisLibros();
            var MisLibros = new MisLibros
            {
                CountLibrosPred = response.CountLibrosPred,
                IdUsuario = response.IdUsuario
            };

            return View(MisLibros);
        }

        public async Task<IActionResult> FiltrarMisLibros(int idUsuario, int idLista)
        {
            if (idLista == 0)
            {
                var libros = await this.repo.LibrosEnPredefinidos(idUsuario);
                return PartialView("_LibrosPartial", libros);
            }
            else
            {
                var libros = await this.repo.FindLibrosEnPredefinidos(idUsuario, idLista);
                return PartialView("_LibrosPartial", libros);
            }
        }

        public async Task<IActionResult> MisObjetivos()
        {
            var request = await this.service.MisObjetivos();
            ObjetivosUsuarios objetivos = new ObjetivosUsuarios
            {
                idObjetivo = request.idObjetivo,
                IdUsuario = request.IdUsuario,
                NombreObjetivo = request.NombreObjetivo,
                Inicio = request.Inicio,
                Fin = request.Fin,
                TipoObjetivo = request.TipoObjetivo,
                Meta = request.Meta,
                ProgresoActual = request.ProgresoActual,
                estado = request.estado
            };
            return View(objetivos);
        }

        [HttpPost]
        public async Task<IActionResult> InsertObjetivo(ObjetivosUsuarios objetivo)
        {
            ObjetivosUsuarios obj = new ObjetivosUsuarios
            {
                idObjetivo = objetivo.idObjetivo,
                IdUsuario = objetivo.IdUsuario,
                NombreObjetivo = objetivo.NombreObjetivo,
                Inicio = objetivo.Inicio,
                Fin = objetivo.Fin,
                TipoObjetivo = objetivo.TipoObjetivo,
                Meta = objetivo.Meta,
                ProgresoActual = objetivo.ProgresoActual,
                estado = objetivo.estado
            };
            await this.service.InsertarObjetivo(obj);
            return RedirectToAction("MisObjetivos", objetivo.IdUsuario);
        }


        public async Task<IActionResult> DeleteObjetivo(int idObjetivo)
        {
            int idUser = (int)HttpContext.Session.GetInt32("id");
            await this.repo.DeleteObjetivo(idObjetivo);
            return RedirectToAction("MisObjetivos", idUser);
        }

        public async Task<IActionResult> UpdateProgreso(ObjetivosUsuarios objetivos)
        {
            int idusuario = (int)HttpContext.Session.GetInt32("id");

            await this.repo.UpdateObjetivo(objetivos.idObjetivo, idusuario, objetivos.ProgresoActual);
            return RedirectToAction("MisObjetivos", new { id = objetivos.IdUsuario });
        }

        public async Task<IActionResult>UpdateUsuario()
        {
            int idUser = (int)HttpContext.Session.GetInt32("id");
            var usuario = await this.repo.GetUsuario(idUser);
            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUsuario(Usuarios usuario, IFormFile ProfileImageFile)
        {
            await this.repo.UpdateUsuarios(usuario);
            return View(usuario);
        }

        public IActionResult SubirFichero()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubirFichero(IFormFile fichero)
        {
            int idusuario = (int)HttpContext.Session.GetInt32("id");

            if (fichero == null || fichero.Length == 0)
            {
                return BadRequest("No se envió un archivo.");
            }

            string[] permittedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
            string extension = Path.GetExtension(fichero.FileName).ToLowerInvariant();

            if (!permittedExtensions.Contains(extension))
            {
                return BadRequest("Extensión de archivo no permitida.");
            }

            if (fichero.Length > 2 * 1024 * 1024) // 2 MB
            {
                return BadRequest("El archivo excede el tamaño máximo permitido.");
            }

            // Generar un nombre único para el archivo
            string fileName = $"usuario_{idusuario}{extension}";

            // Ruta física donde guardar el archivo
            string path = this.helperImages.MapPath(fileName, Folders.Users);

            // Asegurarse de que la carpeta existe
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Guardar el archivo
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await fichero.CopyToAsync(stream);
            }

            // Guardar solo el nombre del archivo en la base de datos
            await this.repo.UpdateFotoUsuario(idusuario, fileName);
            var perfil = await this.repo.GetUsuario(idusuario);

            return RedirectToAction("Perfil", perfil);
        }

    }

}
