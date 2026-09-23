using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace GoCar.Desktop.Services
{
    public static class ApiClient
    {
        private static readonly HttpClient _httpClient = new()
        {
            BaseAddress = new Uri("https://localhost:58906/")
        };

        // Configura o JWT recebido no login
        public static void ConfigurarToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        // GET - Buscar dados
        public static async Task<T?> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>();
        }

        // POST - Cadastrar
        public static async Task<HttpResponseMessage> PostAsync<T>(
            string endpoint,
            T objeto)
        {
            return await _httpClient.PostAsJsonAsync(endpoint, objeto);
        }

        // PUT - Editar
        public static async Task<HttpResponseMessage> PutAsync<T>(
            string endpoint,
            T objeto)
        {
            return await _httpClient.PutAsJsonAsync(endpoint, objeto);
        }

        // DELETE - Excluir/desativar
        public static async Task<HttpResponseMessage> DeleteAsync(
            string endpoint)
        {
            return await _httpClient.DeleteAsync(endpoint);
        }
    }
}