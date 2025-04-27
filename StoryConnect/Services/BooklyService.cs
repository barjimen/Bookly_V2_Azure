using System.Net.Http.Headers;
using System.Text;
using BooklyNugget.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Azure;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;

namespace StoryConnect_V2.Services
{
    public class BooklyService
    {
        private string UrlApi;
        private MediaTypeWithQualityHeaderValue header;
        private IHttpContextAccessor contextAccessor;

        public BooklyService(IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            this.header = new
                MediaTypeWithQualityHeaderValue("application/json");
            this.UrlApi = configuration.GetValue<string>
                ("ApiUrls:BooklyApi");
            this.contextAccessor = contextAccessor;
        }
        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                // Recuperar token de la sesión
                string token = this.contextAccessor.HttpContext.User.FindFirst("TOKEN")?.Value;

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }
        private async Task CallPostAsync<T>(string request, T model)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                // Recuperar token de sesión
                string token = this.contextAccessor.HttpContext.User.FindFirst("TOKEN")?.Value;
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                string json = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(request, content);

                // 👇 En lugar de reventar, leemos la respuesta
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error en POST: {response.StatusCode}\n{errorResponse}");
                }
            }
        }

        private async Task CallPutAsync<T>(string request, T model)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                // Recuperar token de sesión
                string token = this.contextAccessor.HttpContext.User.FindFirst("TOKEN")?.Value;
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                string json = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync(request, content);
                response.EnsureSuccessStatusCode();
            }
        }

        private async Task CallDeleteAsync(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                // Recuperar token de sesión
                string token = this.contextAccessor.HttpContext.User.FindFirst("TOKEN")?.Value;
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                HttpResponseMessage response = await client.DeleteAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error en DELETE: {response.StatusCode}\n{errorResponse}");
                }
            }
        }



        public async Task<string> GetTokenAsync
       (string email, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/auth/login";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);
                LogIn model = new LogIn
                {
                    email = email,
                    password = password
                };
                string json = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent
                    (json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content
                        .ReadAsStringAsync();
                    JObject keys = JObject.Parse(data);
                    string token = keys.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<List<Autores>> GetAutoresAsync()
        {
            string request = "/api/Autores";
            List<Autores> autores = await CallApiAsync<List<Autores>>(request);
            return autores;
        }

        public async Task<DetallesAutor> FindAutorAsync(int idAutor)
        {
            string request = "/api/Autores/" + idAutor;
            DetallesAutor detalles = await CallApiAsync<DetallesAutor>(request);
            return detalles;
        }

        public async Task<Biblioteca> GetBibliotecaAsync()
        {
            string request = "/api/Libros/GetIndex";
            Biblioteca biblioteca = await CallApiAsync<Biblioteca>(request);
            return biblioteca;
        }

        public async Task<LibrosDetalles> FindLibroAsync(int idLibro)
        {
            string request = "/api/Libros/" + idLibro;
            LibrosDetalles detalles = await CallApiAsync<LibrosDetalles>(request);
            return detalles;
        }
        public async Task<GenerosDTO> GetGeneros()
        {
            string request = "/api/Libros/GetGeneros";
            GenerosDTO generos = await CallApiAsync<GenerosDTO>(request);
            return generos;
        }

        public async Task<Generos> GetDetalleGenero(int idGenero)
        {
            string request = "/api/Libros/GetDetalleGenero/" + idGenero;
            Generos generos = await CallApiAsync<Generos>(request);
            return generos;
        }

        public async Task<List<LibrosBusquedaDTO>> BuscarLibros(string query)
        {
            string request = $"/api/Libros/BuscarLibros?query={query}";
            List<LibrosBusquedaDTO> resultados = await this.CallApiAsync<List<LibrosBusquedaDTO>>(request);
            return resultados;
        }

        public async Task<LibrosLeyendoProgreso> Home()
        {
            string request = "/api/Libros/Home";
            LibrosLeyendoProgreso home = await CallApiAsync<LibrosLeyendoProgreso>(request);
            return home;
        }

        public async Task MoverLibrosListas(int idLibro, int origen, int destino)
        {
            string request = $"/api/Libros/MoverLibrosEntreListas?idlibro={idLibro}&origen={origen}&destino={destino}";
            await this.CallPostAsync<object>(request, null);
        }

        public async Task ActualizarReseña(ReseñaDTO model)
        {
            string request = "/api/Libros/ActualizarReseña";
            await this.CallPutAsync(request, model);
        }

        public async Task InsertReseña(ReseñaDTO model)
        {
            string request = "api/Libros/InsertReseña";
            await CallPostAsync(request, model);
        }

        public async Task UpdateProgreso(ProgresoLectura progreso)
        {
            string request = "/api/Libros/UpdateProgreso";
            await CallPutAsync(request, progreso);
        }

        //Usuarios

        public async Task Register(Register user)
        {
            string request = "api/Usuarios/Register";
            await CallPostAsync<object>(request, user);
        }

        public async Task<HomeUsuario> Perfil()
        {
            string request = "/api/Usuarios/Perfil";
            HomeUsuario home = await CallApiAsync<HomeUsuario>(request);
            return home;
        }

        public async Task<MisLibros> MisLibros()
        {
            string request = "/api/Usuarios/MisLibros";
            MisLibros mislibros = await CallApiAsync<MisLibros>(request);
            return mislibros;
        }

        public async Task<List<ObjetivosUsuarios>> MisObjetivos()
        {
            string request = "/api/Usuarios/MisObjetivos";
            var objetivos = await CallApiAsync<List<ObjetivosUsuarios>>(request);
            return objetivos;
        }

        public async Task InsertarObjetivo(ObjetivosUsuarios objetivo)
        {
            string request = "/api/Usuarios/InsertObjetivo";
            await CallPostAsync<object>(request, objetivo);
        }

        public async Task DeleteObjetivoUsuario(int idObjetivo)
        {
            string request = "/api/Usuarios/DeleteObjetivo/" + idObjetivo;
            await CallDeleteAsync(request);
        }

        public async Task UpdateProgresoObjetivo(ObjetivosUsuarios objetivo)
        {
            string request = "/api/Usuarios/UpdateProgreso";
            await this.CallPutAsync(request, objetivo);
        }

        public async Task<Usuarios> GetUsuario(int idUsuario)
        {
            string request = "/api/Usuarios/GetUsuario?idUsuario=" + idUsuario;
            var usuario = await this.CallApiAsync<Usuarios>(request);
            return usuario;
        }

        public async Task UpdateUsuarioData(Usuarios usuario)
        {
            string request = "/api/Usuarios/UpdateUsuario";
            await this.CallPutAsync(request, usuario);
        }
    }
}
