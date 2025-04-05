using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace StoryConnect.Models
{
    [Table("V_LIBROS_LEYENDO")]
    public class LibrosLeyendo
    {
        [Key]
        [Column("UsuarioId")]
        public int IdUsuario { get; set; }
        [Column("LIBROID")]
        public int Id { get; set; }
        [Column("TITULOLIBRO")]
        public string Titulo { get; set; }
        [Column("FECHAPUBLICACION")]
        public DateTime FechaPublicacion { get; set; }
        [Column("IMAGENLIBRO")]
        public string ImagenPortada { get; set; }
        [Column("AUTORID")]
        public int AutorId { get; set; }
        [Column("NOMBREAUTOR")]
        public string NombreAutor { get; set; }
        [Column("LISTAID")]
        public int IdLista {  get; set; }
        [Column("NUMEROPAGINAS")]
        public int paginas { get; set; }
    }
}
