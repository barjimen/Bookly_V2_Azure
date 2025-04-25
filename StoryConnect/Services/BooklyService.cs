using System.Net.Http.Headers;
using BooklyNugget.Models;

namespace StoryConnect_V2.Services
{
    public class BooklyService
    {
        private string UrlApi;
        private MediaTypeWithQualityHeaderValue header;

        public BooklyService(IConfiguration configuration)
        {
            this.header = new
                MediaTypeWithQualityHeaderValue("application/json");
            this.UrlApi = configuration.GetValue<string>
                ("ApiUrls:BooklyApi");
        }
        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await
                        response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
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

    }
}
