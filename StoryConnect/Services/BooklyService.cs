using System.Net.Http.Headers;
using System.Text;
using BooklyNugget.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

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


    }
}
