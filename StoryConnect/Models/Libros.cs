using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace StoryConnect.Models
{
    [Table("V_LIBROS")]
    public class Libros
    {
        [Key]
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
        [Column("AutorId")]
        public int AutorId { get; set; }
        [Column("NombreAutor")]
        public string NombreAutor { get; set; }
        [Column("EtiquetaId")]
        public int? EtiquetaId { get; set; }
    }
}
