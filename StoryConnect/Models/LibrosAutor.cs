using System.ComponentModel.DataAnnotations.Schema;

namespace StoryConnect.Models
{
    [Table("V_AUTORES_LIBROS")]
    public class LibrosAutor
    {
        [Column("AUTORID")]
        public int IdAutor { get; set; }
        [Column("LIBROID")]
        public int Id { get; set; }
        [Column("TITULOLIBRO")]
        public string Titulo { get; set; }
        [Column("SAGALIBRO")]
        public string? Saga { get; set; }
        [Column("POSICIONENSAGA")]
        public int? PosicionSaga { get; set; }
        [Column("FECHAPUBLICACIONLIBRO")]
        public DateTime FechaPublicacion { get; set; }
        [Column("NUMEROPAGINASLIBRO")]
        public int NumeroPaginas { get; set; }
        [Column("SINOPSISLIBRO")]
        public string Sinopsis { get; set; }
        [Column("IMAGENPORTADALIBRO")]
        public string ImagenPortada { get; set; }
        [Column("CALIFICACIONPROMEDIOLIBRO")]
        public decimal CalificacionPromedio { get; set; }
    }
}
