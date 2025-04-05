using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoryConnect.Models
{
    [Table("usuario_lista_predefinida_libro")]
    public class usuario_lista_predefinida_libro
    {
        [Key]
        [Column("USUARIO_ID")]
        public int UsuarioId { get; set; } // INT en SQL

        [Column("LISTA_PREDEFINIDA_ID")]
        public int ListaPredefinidaId { get; set; }

        [Column("LIBRO_ID")]

        public int LibroId { get; set; } 
        
    }
}
