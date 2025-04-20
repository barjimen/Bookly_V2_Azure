using Microsoft.AspNetCore.Mvc;
using BooklyNugget.Models;
using StoryConnect.Repositories;

namespace StoryConnect.Controllers
{
    public class AutoresController : Controller
    {
        private IRepositoryLibros repo;
        public AutoresController(IRepositoryLibros repo)
        {
            this.repo = repo;
        }
        public async Task<IActionResult> Index()
        {
            List<Autores> autores =
                await this.repo.GetAutoresAsync();
            return View(autores);
        }

        public async Task<IActionResult> Details(int idAutor)
        {
            DetallesAutor detalles = await this.repo.FindAutorAsync(idAutor);
            return View(detalles);
        }

    }
}
