using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StoryConnect.Models
{
    [Table("V_LIBROS_PREDEFINIDAS")]
    //[Table("V_LIBROS_PREDEFINIDAS")]
    public class LibrosListasPredefinidas
    {
        [Key]
        [Column("USUARIO_ID")]
        public int Id { get; set; }
        [Column("NOMBRE_USUARIO")]
        public string NombreU { get; set; }
        [Column("LISTA_ID")]
        public int ListaId { get; set; }
        [Column("NOMBRE_LISTA")]
        public string NombreLista { get; set; }
        [Column("LIBRO_ID")]
        public int IdLibro { get; set; }
        [Column("TITULO_LIBRO")]
        public string Titulo { get;set; }
        [Column("AUTOR_ID")]
        public int IdAutor {  get; set; }
        [Column("AUTOR_NOMBRE")]
        public string Autor { get;set; }
        [Column("PORTADA_LIBRO")]
        public string Portada { get; set; }
        [Column("FECHA_AGREGADO")]
        public DateTime Agregado_lista { get; set; }
    }
}
