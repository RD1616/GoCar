using GoCar.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace GoCar.Web.Controllers
{
    public class ClientesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ClientesController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // =====================================================
        // INDEX
        // =====================================================

        public async Task<IActionResult> Index()
        {
            if (!UsuarioPodeAcessar())
            {
                return RedirecionarAcesso();
            }

            var client =
                CriarClienteAutenticado();

            var response =
                await client.GetAsync(
                    "api/Clientes");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.MensagemErro =
                    "Não foi possível carregar os clientes.";

                return View(
                    new List<ClienteViewModel>());
            }

            var clientes =
                await response.Content
                    .ReadFromJsonAsync<
                        List<ClienteViewModel>>();

            return View(
                clientes ??
                new List<ClienteViewModel>());
        }

        // =====================================================
        // DETALHES
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Detalhes(
            int id)
        {
            if (!UsuarioPodeAcessar())
            {
                return RedirecionarAcesso();
            }

            var client =
                CriarClienteAutenticado();

            var response =
                await client.GetAsync(
                    $"api/Clientes/{id}");

            if (response.StatusCode ==
                HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    "Não foi possível carregar o cliente.";

                return RedirectToAction(
                    nameof(Index));
            }

            var cliente =
                await response.Content
                    .ReadFromJsonAsync<
                        ClienteViewModel>();

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // =====================================================
        // EDITAR - GET
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Editar(
            int id)
        {
            if (!UsuarioPodeAcessar())
            {
                return RedirecionarAcesso();
            }

            var client =
                CriarClienteAutenticado();

            var response =
                await client.GetAsync(
                    $"api/Clientes/{id}");

            if (response.StatusCode ==
                HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    "Não foi possível carregar o cliente.";

                return RedirectToAction(
                    nameof(Index));
            }

            var cliente =
                await response.Content
                    .ReadFromJsonAsync<
                        ClienteViewModel>();

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // =====================================================
        // EDITAR - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            int id,
            ClienteViewModel model)
        {
            if (!UsuarioPodeAcessar())
            {
                return RedirecionarAcesso();
            }

            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client =
                CriarClienteAutenticado();

            var dto = new
            {
                nome =
                    model.Nome,

                cpf =
                    model.CPF,

                dataNascimento =
                    model.DataNascimento,

                telefone =
                    model.Telefone,

                cnh =
                    model.CNH,

                categoriaCNH =
                    model.CategoriaCNH,

                dataValidadeCNH =
                    model.DataValidadeCNH,

                endereco =
                    model.Endereco,

                numero =
                    model.Numero,

                bairro =
                    model.Bairro,

                cidade =
                    model.Cidade,

                estado =
                    model.Estado,

                cep =
                    model.CEP,

                isAtivo =
                    model.IsAtivo
            };

            var response =
                await client.PutAsJsonAsync(
                    $"api/Clientes/{id}",
                    dto);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível atualizar o cliente.");

                return View(model);
            }

            TempData["Sucesso"] =
                "Cliente atualizado com sucesso.";

            return RedirectToAction(
                nameof(Index));
        }

        // =====================================================
        // DESATIVAR
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desativar(
            int id)
        {
            if (!UsuarioPodeAcessar())
            {
                return RedirecionarAcesso();
            }

            var client =
                CriarClienteAutenticado();

            var response =
                await client.DeleteAsync(
                    $"api/Clientes/{id}");

            if (response.StatusCode ==
                HttpStatusCode.NotFound)
            {
                TempData["Erro"] =
                    "Cliente não encontrado.";

                return RedirectToAction(
                    nameof(Index));
            }

            if (!response.IsSuccessStatusCode)
            {
                var erro =
                    await response.Content
                        .ReadFromJsonAsync<
                            ApiMensagemViewModel>();

                TempData["Erro"] =
                    erro?.Mensagem
                    ?? "Não foi possível desativar o cliente.";

                return RedirectToAction(
                    nameof(Index));
            }

            TempData["Sucesso"] =
                "Cliente desativado com sucesso.";

            return RedirectToAction(
                nameof(Index));
        }

        // =====================================================
        // ATIVAR
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ativar(
            int id)
        {
            if (!UsuarioPodeAcessar())
            {
                return RedirecionarAcesso();
            }

            var client =
                CriarClienteAutenticado();

            var response =
                await client.PutAsync(
                    $"api/Clientes/{id}/ativar",
                    null);

            if (response.StatusCode ==
                HttpStatusCode.NotFound)
            {
                TempData["Erro"] =
                    "Cliente não encontrado.";

                return RedirectToAction(
                    nameof(Index));
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    "Não foi possível ativar o cliente.";

                return RedirectToAction(
                    nameof(Index));
            }

            TempData["Sucesso"] =
                "Cliente ativado com sucesso.";

            return RedirectToAction(
                nameof(Index));
        }

        // =====================================================
        // HTTP CLIENT
        // =====================================================

        private HttpClient CriarClienteAutenticado()
        {
            var token =
                HttpContext.Session
                    .GetString("JWT");

            var client =
                _httpClientFactory
                    .CreateClient("GoCarApi");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);

            return client;
        }

        // =====================================================
        // PERMISSÃO
        // =====================================================

        private bool UsuarioPodeAcessar()
        {
            var token =
                HttpContext.Session
                    .GetString("JWT");

            var perfil =
                HttpContext.Session
                    .GetString("UsuarioPerfil");

            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            return perfil == "Administrador"
                || perfil == "Gerente"
                || perfil == "Atendente";
        }

        // =====================================================
        // REDIRECIONAMENTO
        // =====================================================

        private IActionResult RedirecionarAcesso()
        {
            var token =
                HttpContext.Session
                    .GetString("JWT");

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            return RedirectToAction(
                "Index",
                "Home");
        }

        // =====================================================
        // RESPOSTA DE ERRO DA API
        // =====================================================

        private class ApiMensagemViewModel
        {
            public string Mensagem { get; set; } =
                string.Empty;
        }
    }
}