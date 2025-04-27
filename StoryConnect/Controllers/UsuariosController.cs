using System.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using StoryConnect.Context;
using StoryConnect.Repositories;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using BooklyNugget.Models;
using StoryConnect_V2.Services;
using System.Security.Claims;
using Azure.Storage.Blobs;

namespace StoryConnect.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private IRepositoryLibros repo;
        private BooklyService service;
        public UsuariosController(IRepositoryLibros repo, IWebHostEnvironment hostingEnvironment, BooklyService service)
        {
            _hostingEnvironment = hostingEnvironment;
            this.repo = repo;
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
            var idUser = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);

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
    


        public IActionResult SubirFichero()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubirFichero(IFormFile fichero)
        {
            var idUser = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);

            if (fichero == null || fichero.Length == 0)
            {
                return BadRequest("No se envió un archivo.");
            }

            // Validación de extensiones permitidas
            string[] permittedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
            string extension = Path.GetExtension(fichero.FileName).ToLowerInvariant();

            if (!permittedExtensions.Contains(extension))
            {
                return BadRequest("Extensión de archivo no permitida.");
            }

            // Validación de tamaño
            if (fichero.Length > 2 * 1024 * 1024) // 2 MB
            {
                return BadRequest("El archivo excede el tamaño máximo permitido.");
            }

            // Generar un nombre único para el archivo
            string fileName = $"usuario_{idUser}{extension}";

            // Configuración del cliente de Azure Blob Storage
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=storagebooklybja;AccountKey=GXewDrOYW2lH5a8ZkgG9SPKSoz+cPW2AQAIHihpC7dIKVgGGDtWuyaOYjEsNUS6DSyk451yzbD/++ASt3BsbQg==;EndpointSuffix=core.windows.net";
            string containerName = "imagesbookly";

            // Crear el cliente del contenedor
            var blobServiceClient = new BlobServiceClient(connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

            string blobPath = $"Users/{fileName}";

            var blobClient = blobContainerClient.GetBlobClient(blobPath);

            using (var stream = fichero.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            
            string fileUrl = blobClient.Uri.ToString();
            await this.repo.UpdateFotoUsuario(idUser, fileName);
            var perfil = await this.service.GetUsuario(idUser);

            return RedirectToAction("Perfil");
        }



    }
}
