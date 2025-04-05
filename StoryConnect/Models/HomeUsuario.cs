namespace StoryConnect.Models
{
    public class HomeUsuario
    {
        public Usuarios Usuarios { get; set; }
        public List<CountLibrosListasPredefinidas> CountLibrosPred {  get; set; }
        public List<LibrosListasPredefinidas> LibrosListasPred { get; set; }    
       public List<ObjetivosUsuarios> ObjetivosUsuarios { get; set;}
        public ProgresoLectura ProgresoLectura { get; set; }


    }
}
