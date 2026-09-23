using GoCar.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace GoCar.Web.Controllers
{
    public class VeiculosController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public VeiculosController(
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
                    "api/Veiculos");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.MensagemErro =
                    "Não foi possível carregar os veículos.";

                return View(
                    new List<VeiculoViewModel>());
            }

            var veiculos =
                await response.Content
                    .ReadFromJsonAsync<
                        List<VeiculoViewModel>>();

            return View(
                veiculos ??
                new List<VeiculoViewModel>());
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
                    $"api/Veiculos/{id}");

            if (response.StatusCode ==
                HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    "Não foi possível carregar o veículo.";

                return RedirectToAction(
                    nameof(Index));
            }

            var veiculo =
                await response.Content
                    .ReadFromJsonAsync<
                        VeiculoViewModel>();

            if (veiculo == null)
            {
                return NotFound();
            }

            ViewBag.CategoriaNome =
                $"Categoria #{veiculo.CategoriaId}";

            ViewBag.FilialNome =
                $"Filial #{veiculo.FilialId}";

            var responseCategoria =
                await client.GetAsync(
                    $"api/Categorias/{veiculo.CategoriaId}");

            if (responseCategoria.IsSuccessStatusCode)
            {
                var categoria =
                    await responseCategoria.Content
                        .ReadFromJsonAsync<
                            CategoriaViewModel>();

                if (categoria != null)
                {
                    ViewBag.CategoriaNome =
                        categoria.Nome;
                }
            }

            var responseFilial =
                await client.GetAsync(
                    $"api/Filiais/{veiculo.FilialId}");

            if (responseFilial.IsSuccessStatusCode)
            {
                var filial =
                    await responseFilial.Content
                        .ReadFromJsonAsync<
                            FilialViewModel>();

                if (filial != null)
                {
                    ViewBag.FilialNome =
                        $"{filial.Nome} - " +
                        $"{filial.Cidade}/{filial.Estado}";
                }
            }

            return View(veiculo);
        }

        // =====================================================
        // CRIAR - GET
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Criar()
        {
            if (!UsuarioPodeAcessar())
            {
                return RedirecionarAcesso();
            }

            await CarregarCategoriasEFiliais();

            var model =
                new VeiculoViewModel
                {
                    Combustivel = 3,
                    Cambio = 2,
                    Status = 1,
                    IsAtivo = true
                };

            return View(model);
        }

        // =====================================================
        // CRIAR - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(
            VeiculoViewModel model)
        {
            if (!UsuarioPodeAcessar())
            {
                return RedirecionarAcesso();
            }

            if (!ModelState.IsValid)
            {
                await CarregarCategoriasEFiliais();

                return View(model);
            }

            var client =
                CriarClienteAutenticado();

            var dto = new
            {
                placa = model.Placa,
                chassi = model.Chassi,
                renavam = model.Renavam,
                modelo = model.Modelo,
                marca = model.Marca,
                anoFabricacao = model.AnoFabricacao,
                anoModelo = model.AnoModelo,
                cor = model.Cor,
                combustivel = model.Combustivel,
                cambio = model.Cambio,
                status = model.Status,
                kmAtual = model.KmAtual,
                valorDiaria = model.ValorDiaria,
                categoriaId = model.CategoriaId,
                filialId = model.FilialId
            };

            var response =
                await client.PostAsJsonAsync(
                    "api/Veiculos",
                    dto);

            if (!response.IsSuccessStatusCode)
            {
                await CarregarCategoriasEFiliais();

                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível cadastrar o veículo.");

                return View(model);
            }

            TempData["Sucesso"] =
                "Veículo cadastrado com sucesso.";

            return RedirectToAction(
                nameof(Index));
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
                    $"api/Veiculos/{id}");

            if (response.StatusCode ==
                HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    "Não foi possível carregar o veículo.";

                return RedirectToAction(
                    nameof(Index));
            }

            var veiculo =
                await response.Content
                    .ReadFromJsonAsync<
                        VeiculoViewModel>();

            if (veiculo == null)
            {
                return NotFound();
            }

            await CarregarCategoriasEFiliais();

            return View(veiculo);
        }

        // =====================================================
        // EDITAR - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            int id,
            VeiculoViewModel model)
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
                await CarregarCategoriasEFiliais();

                return View(model);
            }

            var client =
                CriarClienteAutenticado();

            // Status e KmAtual são controlados pelo sistema.
            // Por isso buscamos novamente os valores reais na API.

            var veiculoAtualResponse =
                await client.GetAsync(
                    $"api/Veiculos/{id}");

            if (veiculoAtualResponse.StatusCode ==
                HttpStatusCode.NotFound)
            {
                TempData["Erro"] =
                    "Veículo não encontrado.";

                return RedirectToAction(
                    nameof(Index));
            }

            if (!veiculoAtualResponse.IsSuccessStatusCode)
            {
                await CarregarCategoriasEFiliais();

                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível consultar o estado atual do veículo.");

                return View(model);
            }

            var veiculoAtual =
                await veiculoAtualResponse.Content
                    .ReadFromJsonAsync<
                        VeiculoViewModel>();

            if (veiculoAtual == null)
            {
                await CarregarCategoriasEFiliais();

                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível consultar o estado atual do veículo.");

                return View(model);
            }

            model.Status =
                veiculoAtual.Status;

            model.KmAtual =
                veiculoAtual.KmAtual;

            var dto = new
            {
                placa = model.Placa,
                chassi = model.Chassi,
                renavam = model.Renavam,
                modelo = model.Modelo,
                marca = model.Marca,
                anoFabricacao = model.AnoFabricacao,
                anoModelo = model.AnoModelo,
                cor = model.Cor,
                combustivel = model.Combustivel,
                cambio = model.Cambio,
                status = veiculoAtual.Status,
                kmAtual = veiculoAtual.KmAtual,
                valorDiaria = model.ValorDiaria,
                isAtivo = model.IsAtivo,
                categoriaId = model.CategoriaId,
                filialId = model.FilialId
            };

            var response =
                await client.PutAsJsonAsync(
                    $"api/Veiculos/{id}",
                    dto);

            if (!response.IsSuccessStatusCode)
            {
                await CarregarCategoriasEFiliais();

                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível atualizar o veículo.");

                return View(model);
            }

            TempData["Sucesso"] =
                "Veículo atualizado com sucesso.";

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
                    $"api/Veiculos/{id}");

            if (response.StatusCode ==
                HttpStatusCode.NotFound)
            {
                TempData["Erro"] =
                    "Veículo não encontrado.";

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
                    ?? "Não foi possível desativar o veículo.";

                return RedirectToAction(
                    nameof(Index));
            }

            TempData["Sucesso"] =
                "Veículo desativado com sucesso.";

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
                    $"api/Veiculos/{id}/ativar",
                    null);

            if (response.StatusCode ==
                HttpStatusCode.NotFound)
            {
                TempData["Erro"] =
                    "Veículo não encontrado.";

                return RedirectToAction(
                    nameof(Index));
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    "Não foi possível ativar o veículo.";

                return RedirectToAction(
                    nameof(Index));
            }

            TempData["Sucesso"] =
                "Veículo ativado com sucesso.";

            return RedirectToAction(
                nameof(Index));
        }

        // =====================================================
        // CATEGORIAS E FILIAIS
        // =====================================================

        private async Task CarregarCategoriasEFiliais()
        {
            var client =
                CriarClienteAutenticado();

            var categorias =
                new List<CategoriaViewModel>();

            var filiais =
                new List<FilialViewModel>();

            var responseCategorias =
                await client.GetAsync(
                    "api/Categorias");

            if (responseCategorias.IsSuccessStatusCode)
            {
                categorias =
                    await responseCategorias.Content
                        .ReadFromJsonAsync<
                            List<CategoriaViewModel>>()
                    ?? new List<CategoriaViewModel>();
            }

            var responseFiliais =
                await client.GetAsync(
                    "api/Filiais");

            if (responseFiliais.IsSuccessStatusCode)
            {
                filiais =
                    await responseFiliais.Content
                        .ReadFromJsonAsync<
                            List<FilialViewModel>>()
                    ?? new List<FilialViewModel>();
            }

            ViewBag.Categorias =
                categorias
                    .Where(c => c.IsAtivo)
                    .OrderBy(c => c.Nome)
                    .ToList();

            ViewBag.Filiais =
                filiais
                    .Where(f => f.IsAtivo)
                    .OrderBy(f => f.Nome)
                    .ToList();
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