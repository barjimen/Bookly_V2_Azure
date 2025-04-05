using System.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoryConnect.Models;
using System.Security.Cryptography;
using StoryConnect.Context;
using StoryConnect.Repositories;

namespace StoryConnect.Controllers
{
    public class UsuariosController : Controller
    {
        private IRepositoryLibros repo;
        public UsuariosController(IRepositoryLibros repo)
        {
            this.repo = repo;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string nombre, string email, string password)
        {
            await this.repo.Register(nombre, email, password);
            ViewData["Mensaje"] = "Usuario registrado correctamente.";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var usuario = await this.repo.Login(email, password);
            if (usuario != null)
            {
                HttpContext.Session.SetInt32("id", usuario.Id);
                HttpContext.Session.SetString("nombre", usuario.Nombre);
                HttpContext.Session.SetString("email", usuario.email);
                HttpContext.Session.SetString("tipo_usuario", usuario.TipoUsuario);
                HttpContext.Session.SetString("imagen_perfil", usuario.ImagenPerfil);

                return RedirectToAction("Home", "Libros");
            }
            else
            {
                ViewData["MENSAJE"] = "Usuario o contraseña incorrectos.";
                return View();
            }
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }


        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Perfil()
        {
            int idUser = (int)HttpContext.Session.GetInt32("id");
            var usuario = await this.repo.GetUsuario(idUser);
            var CountLibros = await this.repo.ObtenerCountListas(idUser);
            var librosPredefinidos = await this.repo.LibrosEnPredefinidos(idUser);
            var objetivos = await this.repo.ObjetivosUsuarios(idUser);
            ProgresoLectura progreso = null;
            if (librosPredefinidos.Count > 0)
            {
                int idLibro = librosPredefinidos.First().IdLibro;
                progreso = await this.repo.GetProgresoLectura(idUser, idLibro);
            }

            var homeUsuario = new HomeUsuario
            {
                Usuarios = usuario,
                CountLibrosPred = CountLibros,
                LibrosListasPred = librosPredefinidos,
                ObjetivosUsuarios = objetivos,
                ProgresoLectura = progreso
            };

            return View(homeUsuario);
        }

        public async Task<IActionResult> MisLibros()
        {
            int idUser = (int)HttpContext.Session.GetInt32("id");
            var CountLibros = await this.repo.ObtenerCountListas(idUser);


            var MisLibros = new MisLibros
            {
                IdUsuario = idUser,
                CountLibrosPred = CountLibros
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
            int idUser = (int)HttpContext.Session.GetInt32("id");

            var objetivos = await this.repo.ObjetivosUsuarios(idUser);
            return View(objetivos);
        }

        [HttpPost]
        public async Task<IActionResult> InsertObjetivo(ObjetivosUsuarios objetivos)
        {
            int idUser = (int)HttpContext.Session.GetInt32("id");

            await this.repo.InsertObjetivo(idUser, objetivos.NombreObjetivo, objetivos.Fin, objetivos.TipoObjetivo,objetivos.Meta);
            return RedirectToAction("MisObjetivos", idUser);
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
    }
}
