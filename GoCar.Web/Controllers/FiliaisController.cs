using GoCar.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace GoCar.Web.Controllers
{
    public class FiliaisController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public FiliaisController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            if (!UsuarioPodeAcessar())
            {
                return RedirecionarAcesso();
            }

            var client = CriarClienteAutenticado();

            var response = await client.GetAsync(
                "api/Filiais");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.MensagemErro =
                    "Não foi possível carregar as filiais.";

                return View(
                    new List<FilialViewModel>());
            }

            var filiais = await response.Content
                .ReadFromJsonAsync<List<FilialViewModel>>();

            return View(
                filiais ?? new List<FilialViewModel>());
        }

        [HttpGet]
        public async Task<IActionResult> Detalhes(int id)
        {
            if (!UsuarioPodeAcessar())
            {
                return RedirecionarAcesso();
            }

            var client = CriarClienteAutenticado();

            var response = await client.GetAsync(
                $"api/Filiais/{id}");

            if (response.StatusCode ==
                System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    "Não foi possível carregar a filial.";

                return RedirectToAction(nameof(Index));
            }

            var filial = await response.Content
                .ReadFromJsonAsync<FilialViewModel>();

            if (filial == null)
            {
                return NotFound();
            }

            return View(filial);
        }

        [HttpGet]
        public IActionResult Criar()
        {
            if (!UsuarioPodeAcessar())
            {
                return RedirecionarAcesso();
            }

            var model = new FilialViewModel
            {
                IsAtivo = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(
            FilialViewModel model)
        {
            if (!UsuarioPodeAcessar())
            {
                return RedirecionarAcesso();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = CriarClienteAutenticado();

            var dto = new
            {
                nome = model.Nome,
                cnpj = model.CNPJ,
                telefone = model.Telefone,
                email = model.Email,
                endereco = model.Endereco,
                numero = model.Numero,
                bairro = model.Bairro,
                cidade = model.Cidade,
                estado = model.Estado,
                cep = model.CEP
            };

            var response = await client.PostAsJsonAsync(
                "api/Filiais",
                dto);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível cadastrar a filial.");

                return View(model);
            }

            TempData["Sucesso"] =
                "Filial cadastrada com sucesso.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            if (!UsuarioPodeAcessar())
            {
                return RedirecionarAcesso();
            }

            var client = CriarClienteAutenticado();

            var response = await client.GetAsync(
                $"api/Filiais/{id}");

            if (response.StatusCode ==
                System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    "Não foi possível carregar a filial.";

                return RedirectToAction(nameof(Index));
            }

            var filial = await response.Content
                .ReadFromJsonAsync<FilialViewModel>();

            if (filial == null)
            {
                return NotFound();
            }

            return View(filial);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            int id,
            FilialViewModel model)
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

            var client = CriarClienteAutenticado();

            var dto = new
            {
                nome = model.Nome,
                cnpj = model.CNPJ,
                telefone = model.Telefone,
                email = model.Email,
                endereco = model.Endereco,
                numero = model.Numero,
                bairro = model.Bairro,
                cidade = model.Cidade,
                estado = model.Estado,
                cep = model.CEP,
                isAtivo = model.IsAtivo
            };

            var response = await client.PutAsJsonAsync(
                $"api/Filiais/{id}",
                dto);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível atualizar a filial.");

                return View(model);
            }

            TempData["Sucesso"] =
                "Filial atualizada com sucesso.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desativar(int id)
        {
            if (!UsuarioPodeAcessar())
            {
                return RedirecionarAcesso();
            }

            var client = CriarClienteAutenticado();

            var response = await client.DeleteAsync(
                $"api/Filiais/{id}");

            if (response.StatusCode ==
                System.Net.HttpStatusCode.NotFound)
            {
                TempData["Erro"] =
                    "Filial não encontrada.";

                return RedirectToAction(nameof(Index));
            }

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content
                    .ReadFromJsonAsync<ApiMensagemViewModel>();

                TempData["Erro"] =
                    erro?.Mensagem
                    ?? "Não foi possível desativar a filial.";

                return RedirectToAction(nameof(Index));
            }

            TempData["Sucesso"] =
                "Filial desativada com sucesso.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ativar(int id)
        {
            if (!UsuarioPodeAcessar())
            {
                return RedirecionarAcesso();
            }

            var client = CriarClienteAutenticado();

            var responseFilial = await client.GetAsync(
                $"api/Filiais/{id}");

            if (responseFilial.StatusCode ==
                System.Net.HttpStatusCode.NotFound)
            {
                TempData["Erro"] =
                    "Filial não encontrada.";

                return RedirectToAction(nameof(Index));
            }

            if (!responseFilial.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    "Não foi possível carregar a filial.";

                return RedirectToAction(nameof(Index));
            }

            var filial = await responseFilial.Content
                .ReadFromJsonAsync<FilialViewModel>();

            if (filial == null)
            {
                TempData["Erro"] =
                    "Filial não encontrada.";

                return RedirectToAction(nameof(Index));
            }

            var dto = new
            {
                nome = filial.Nome,
                cnpj = filial.CNPJ,
                telefone = filial.Telefone,
                email = filial.Email,
                endereco = filial.Endereco,
                numero = filial.Numero,
                bairro = filial.Bairro,
                cidade = filial.Cidade,
                estado = filial.Estado,
                cep = filial.CEP,
                isAtivo = true
            };

            var response = await client.PutAsJsonAsync(
                $"api/Filiais/{id}",
                dto);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    "Não foi possível reativar a filial.";

                return RedirectToAction(nameof(Index));
            }

            TempData["Sucesso"] =
                "Filial reativada com sucesso.";

            return RedirectToAction(nameof(Index));
        }

        private HttpClient CriarClienteAutenticado()
        {
            var token = HttpContext.Session
                .GetString("JWT");

            var client = _httpClientFactory
                .CreateClient("GoCarApi");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);

            return client;
        }

        private bool UsuarioPodeAcessar()
        {
            var token = HttpContext.Session
                .GetString("JWT");

            var perfil = HttpContext.Session
                .GetString("UsuarioPerfil");

            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            return perfil == "Administrador"
                || perfil == "Gerente"
                || perfil == "Atendente";
        }

        private IActionResult RedirecionarAcesso()
        {
            var token = HttpContext.Session
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

        private class ApiMensagemViewModel
        {
            public string Mensagem { get; set; } =
                string.Empty;
        }
    }
}