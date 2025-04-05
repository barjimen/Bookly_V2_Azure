using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoryConnect.Models
{
    [Table("RESENAS")]
    public class Resenas
    {
        [Key]

        [Column("ID")]
        public int Id { get; set; }
        [ForeignKey(nameof(Id))]
        public Usuarios Usuario { get; set; }
        [Column("LIBRO_ID")]
        public int idLibro { get; set; }
        [Column("CALIFICACION")]
        public int calificacion { get; set; }
        [Column("TEXTO")]
        public string texto {  get; set; }
        [Column("FECHA_PUBLICACION")]
        public DateTime fecha { get; set; }
        [Column("USUARIO_ID")]
        public int UsuarioId { get; set; }
    }
}
