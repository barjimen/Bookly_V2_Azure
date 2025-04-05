using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoryConnect.Models
{
    [Table("libros_etiquetas")]
    public class LibroEtiquetas
    {
        [Key]
        [Column("libro_id")]
        public int LibroId { get; set; }

        [Column("etiqueta_id")]
        public int EtiquetaId { get; set; }

        [ForeignKey(nameof(LibroId))]
        public Libros Libro { get; set; }

        [ForeignKey(nameof(EtiquetaId))]
        public Etiquetas Etiqueta { get; set; }
    }
}
