using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoryConnect.Models
{
    [Table("V_OBJETIVOS_LECTURA")]
    public class ObjetivosUsuarios
    {
        [Key]
        [Column("OBJETIVOID")]
        public int idObjetivo {  get; set; }
        [Column("USUARIOID")]
        public int IdUsuario { get; set; }
        [Column("NOMBREOBJETIVO")]
        public string NombreObjetivo { get; set; }
        [Column("FECHAINICIO")]
        public DateTime Inicio { get; set; }
        [Column("FECHAFIN")]
        public DateTime Fin {  get; set; }
        [Column("TIPOOBJETIVO")]
        public string TipoObjetivo { get; set; }
        [Column("METATOTAL")]
        public int Meta { get; set; }
        [Column("PROGRESOACTUAL")]
        public int ProgresoActual { get; set; }
        [Column("ESTADOOBJETIVO")]
        public string estado { get; set; }
    }
}
