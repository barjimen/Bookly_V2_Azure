namespace StoryConnect.Models
{
    public class LibrosDTO
    {
            public int Id { get; set; }
            public string Titulo { get; set; }
            public string? Saga { get; set; }
            public int? PosicionSaga { get; set; }
            public DateTime FechaPublicacion { get; set; }
            public int NumeroPaginas { get; set; }
            public string Sinopsis { get; set; }
            public string ImagenPortada { get; set; }
            public decimal CalificacionPromedio { get; set; }
            public int AutorId { get; set; }
            public string NombreAutor { get; set; }
            public int idLista { get; set; }
            public DateTime FechaAgregado { get; set; }
            public int? EtiquetaId { get; set; }
    }
}
