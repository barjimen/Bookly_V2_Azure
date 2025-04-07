using StoryConnect.Models;

namespace StoryConnect_V2.Models
{
    public class Generos
    {
        public Etiquetas Genero { get; set; }
        public List<LibrosDTO> Libros { get; set; }
    }
}
