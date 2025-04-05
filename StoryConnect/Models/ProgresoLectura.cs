using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoryConnect.Models
{
    [Table("PROGRESO_LECTURA")]
    public class ProgresoLectura
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }
        [Column("USUARIO_ID")]
        public int idUsuario { get; set; }
        [Column("LIBRO_ID")]
        public int idLibro { get; set; }
        [Column("PORCENTAJE_LEIDO")]
        public decimal Porcetaje { get; set; }
        [Column("PAGINA_ACTUAL")]
        public int Pagina { get; set; }
        [Column("ESTADO_LECTURA")]
        public string estado { get; set; }
        [Column("FECHA_INICIO")]
        public DateTime Inicio { get; set; }
        [Column("FECHA_ULTIMA_ACTUALIZACION")]
        public DateTime Actualizacion { get; set; }


    }
}
