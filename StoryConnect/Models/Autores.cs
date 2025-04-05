using System.ComponentModel.DataAnnotations.Schema;

namespace StoryConnect.Models
{
    [Table("AUTORES")]
    public class Autores
    {
        [Column("ID")]
        public int Id { get; set; }
        [Column("NOMBRE")]
        public string Nombre { get; set; }
        [Column("BIOGRAFIA")]
        public string Biografia { get; set; }
        [Column("IMAGEN_PERFIL")]
        public string ImagenPerfil { get; set; }

    }
}
