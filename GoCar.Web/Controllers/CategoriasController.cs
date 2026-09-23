using GoCar.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace GoCar.Web.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CategoriasController(
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
                "api/Categorias");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.MensagemErro =
                    "Não foi possível carregar as categorias.";

                return View(
                    new List<CategoriaViewModel>());
            }

            var categorias = await response.Content
                .ReadFromJsonAsync<List<CategoriaViewModel>>();

            return View(
                categorias ?? new List<CategoriaViewModel>());
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
                $"api/Categorias/{id}");

            if (response.StatusCode ==
                System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    "Não foi possível carregar a categoria.";

                return RedirectToAction(nameof(Index));
            }

            var categoria = await response.Content
                .ReadFromJsonAsync<CategoriaViewModel>();

            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        [HttpGet]
        public IActionResult Criar()
        {
            if (!UsuarioPodeAcessar())
            {
                return RedirecionarAcesso();
            }

            var model = new CategoriaViewModel
            {
                IsAtivo = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(
            CategoriaViewModel model)
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
                descricao = model.Descricao,
                diariaBase = model.DiariaBase,
                kmLivre = model.KmLivre
            };

            var response = await client.PostAsJsonAsync(
                "api/Categorias",
                dto);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível cadastrar a categoria.");

                return View(model);
            }

            TempData["Sucesso"] =
                "Categoria cadastrada com sucesso.";

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
                $"api/Categorias/{id}");

            if (response.StatusCode ==
                System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    "Não foi possível carregar a categoria.";

                return RedirectToAction(nameof(Index));
            }

            var categoria = await response.Content
                .ReadFromJsonAsync<CategoriaViewModel>();

            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            int id,
            CategoriaViewModel model)
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
                descricao = model.Descricao,
                diariaBase = model.DiariaBase,
                kmLivre = model.KmLivre,
                isAtivo = model.IsAtivo
            };

            var response = await client.PutAsJsonAsync(
                $"api/Categorias/{id}",
                dto);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível atualizar a categoria.");

                return View(model);
            }

            TempData["Sucesso"] =
                "Categoria atualizada com sucesso.";

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
                $"api/Categorias/{id}");

            if (response.StatusCode ==
                System.Net.HttpStatusCode.NotFound)
            {
                TempData["Erro"] =
                    "Categoria não encontrada.";

                return RedirectToAction(nameof(Index));
            }

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content
                    .ReadFromJsonAsync<ApiMensagemViewModel>();

                TempData["Erro"] =
                    erro?.Mensagem
                    ?? "Não foi possível desativar a categoria.";

                return RedirectToAction(nameof(Index));
            }

            TempData["Sucesso"] =
                "Categoria desativada com sucesso.";

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

            var responseCategoria = await client.GetAsync(
                $"api/Categorias/{id}");

            if (responseCategoria.StatusCode ==
                System.Net.HttpStatusCode.NotFound)
            {
                TempData["Erro"] =
                    "Categoria não encontrada.";

                return RedirectToAction(nameof(Index));
            }

            if (!responseCategoria.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    "Não foi possível carregar a categoria.";

                return RedirectToAction(nameof(Index));
            }

            var categoria = await responseCategoria.Content
                .ReadFromJsonAsync<CategoriaViewModel>();

            if (categoria == null)
            {
                TempData["Erro"] =
                    "Categoria não encontrada.";

                return RedirectToAction(nameof(Index));
            }

            var dto = new
            {
                nome = categoria.Nome,
                descricao = categoria.Descricao,
                diariaBase = categoria.DiariaBase,
                kmLivre = categoria.KmLivre,
                isAtivo = true
            };

            var response = await client.PutAsJsonAsync(
                $"api/Categorias/{id}",
                dto);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    "Não foi possível reativar a categoria.";

                return RedirectToAction(nameof(Index));
            }

            TempData["Sucesso"] =
                "Categoria reativada com sucesso.";

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