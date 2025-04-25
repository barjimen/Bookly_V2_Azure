using Microsoft.AspNetCore.Mvc;
using BooklyNugget.Models;
using StoryConnect_V2.Services;

namespace StoryConnect.Controllers
{
    public class AutoresController : Controller
    {
        private BooklyService service;
        public AutoresController(BooklyService service)
        {
            this.service = service;
        }
        public async Task<IActionResult> Index()
        {
            List<Autores> autores =
                await this.service.GetAutoresAsync();
            return View(autores);
        }

        public async Task<IActionResult> Details(int idAutor)
        {
            DetallesAutor detalles = await this.service.FindAutorAsync(idAutor);
            return View(detalles);
        }

    }
}
