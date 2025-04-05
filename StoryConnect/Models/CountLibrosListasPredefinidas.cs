using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoryConnect.Models
{
    [Table("V_COUNT_PREDEFINIDAS")]
    public class CountLibrosListasPredefinidas
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
        [Column("CANTIDAD_LIBROS")]
        public int NumLibros { get; set; }
    }
}
