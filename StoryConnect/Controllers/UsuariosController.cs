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
            List<ObjetivosUsuarios> objetivos = request;
            return View(objetivos);
        }

        [HttpPost]
        public async Task<IActionResult> InsertObjetivo(ObjetivosUsuarios objetivo)
        {
            var idUser =int.Parse( User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);

            ObjetivosUsuarios obj = new ObjetivosUsuarios
            {
                idObjetivo = objetivo.idObjetivo,
                IdUsuario = idUser,
                NombreObjetivo = objetivo.NombreObjetivo,
                Inicio = DateTime.Now,
                Fin = objetivo.Fin,
                TipoObjetivo = objetivo.TipoObjetivo,
                Meta = objetivo.Meta,
                ProgresoActual = objetivo.ProgresoActual,
                estado = "activo"
            };
            await this.service.InsertarObjetivo(obj);
            return RedirectToAction("MisObjetivos", objetivo.IdUsuario);
        }


        public async Task<IActionResult> DeleteObjetivo(int idObjetivo)
        {
            var idUser = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            await this.service.DeleteObjetivoUsuario(idObjetivo);
            return RedirectToAction("MisObjetivos");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProgresoObjetivo(ObjetivosUsuarios objetivos)
        {
            var idUser = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
            ObjetivosUsuarios objt = new ObjetivosUsuarios
            {
                idObjetivo = objetivos.idObjetivo,
                IdUsuario = idUser,
                NombreObjetivo = objetivos.NombreObjetivo,
                Inicio = objetivos.Inicio,
                Fin = objetivos.Fin,
                TipoObjetivo = objetivos.TipoObjetivo,
                Meta = objetivos.Meta,
                ProgresoActual = objetivos.ProgresoActual,
                estado = objetivos.estado
            };
            await this.service.UpdateProgresoObjetivo(objt);
            return RedirectToAction("MisObjetivos");
        }

        public async Task<IActionResult> UpdateUsuario()
        {
            var idUser = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
            var usuario = await this.service.GetUsuario(idUser);
            return View(usuario);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateUsuario(Usuarios usuario)
        {
            var idUser = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
            var data = await this.service.GetUsuario(idUser);
            var usu = new Usuarios
            {
                Id = idUser,
                Nombre = usuario.Nombre,
                email = usuario.email,
                Password_hash = data.Password_hash,
                ImagenPerfil = usuario.ImagenPerfil,
                FechaRegistro = usuario.FechaRegistro,
                TipoUsuario = usuario.TipoUsuario,
                Password = usuario.Password,
                Salt = usuario.Salt,
            };
            await this.service.UpdateUsuarioData(usu);
            return RedirectToAction("Perfil");
        }
    }


        //public IActionResult SubirFichero()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> SubirFichero(IFormFile fichero)
        //{
        //    int idusuario = (int)HttpContext.Session.GetInt32("id");

        //    if (fichero == null || fichero.Length == 0)
        //    {
        //        return BadRequest("No se envió un archivo.");
        //    }

        //    string[] permittedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        //    string extension = Path.GetExtension(fichero.FileName).ToLowerInvariant();

        //    if (!permittedExtensions.Contains(extension))
        //    {
        //        return BadRequest("Extensión de archivo no permitida.");
        //    }

        //    if (fichero.Length > 2 * 1024 * 1024) // 2 MB
        //    {
        //        return BadRequest("El archivo excede el tamaño máximo permitido.");
        //    }

        //    // Generar un nombre único para el archivo
        //    string fileName = $"usuario_{idusuario}{extension}";

        //    // Ruta física donde guardar el archivo
        //    string path = this.helperImages.MapPath(fileName, Folders.Users);

        //    // Asegurarse de que la carpeta existe
        //    string directory = Path.GetDirectoryName(path);
        //    if (!Directory.Exists(directory))
        //    {
        //        Directory.CreateDirectory(directory);
        //    }

        //    // Guardar el archivo
        //    using (var stream = new FileStream(path, FileMode.Create))
        //    {
        //        await fichero.CopyToAsync(stream);
        //    }

        //    // Guardar solo el nombre del archivo en la base de datos
        //    await this.repo.UpdateFotoUsuario(idusuario, fileName);
        //    var perfil = await this.repo.GetUsuario(idusuario);

        //    return RedirectToAction("Perfil", perfil);
        //}

}
