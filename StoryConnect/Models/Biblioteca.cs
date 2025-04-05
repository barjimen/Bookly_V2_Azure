namespace StoryConnect.Models
{
    public class Biblioteca
    {
            public List<LibrosDTO> Libros { get; set; }
            public List<Etiquetas> Etiquetas { get; set; }
            public List<Autores> Autores { get; set; }
            public List<LibroEtiquetas> LibroEtiquetas { get; set; }
    }
}
