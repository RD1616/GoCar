using GoCar.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace GoCar.Web.Controllers
{
    public class PagamentosController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PagamentosController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // =====================================================
        // INDEX - LISTAR PAGAMENTOS
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token =
                HttpContext.Session.GetString("JWT");

            var perfil =
                HttpContext.Session.GetString("UsuarioPerfil");

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            if (!EhFuncionario(perfil))
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            var client =
                CriarClienteAutenticado(token);

            var response =
                await client.GetAsync(
                    "api/Pagamentos");

            if (response.StatusCode ==
                HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();

                return RedirectToAction(
                    "Login",
                    "Account");
            }

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.MensagemErro =
                    "Não foi possível carregar os pagamentos.";

                return View(
                    new List<PagamentoViewModel>());
            }

            var pagamentos =
                await response.Content
                    .ReadFromJsonAsync<
                        List<PagamentoViewModel>>();

            return View(
                pagamentos ??
                new List<PagamentoViewModel>());
        }

        // =====================================================
        // DETALHES
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Detalhes(
            int id)
        {
            var token =
                HttpContext.Session.GetString("JWT");

            var perfil =
                HttpContext.Session.GetString("UsuarioPerfil");

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            if (!EhFuncionario(perfil))
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            var client =
                CriarClienteAutenticado(token);

            var response =
                await client.GetAsync(
                    $"api/Pagamentos/{id}");

            if (response.StatusCode ==
                HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();

                return RedirectToAction(
                    "Login",
                    "Account");
            }

            if (response.StatusCode ==
                HttpStatusCode.NotFound)
            {
                TempData["Erro"] =
                    "Pagamento não encontrado.";

                return RedirectToAction(
                    nameof(Index));
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    "Não foi possível carregar o pagamento.";

                return RedirectToAction(
                    nameof(Index));
            }

            var pagamento =
                await response.Content
                    .ReadFromJsonAsync<
                        PagamentoViewModel>();

            if (pagamento == null)
            {
                TempData["Erro"] =
                    "Pagamento não encontrado.";

                return RedirectToAction(
                    nameof(Index));
            }

            return View(pagamento);
        }

        // =====================================================
        // CRIAR - GET
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Criar(
            int? locacaoId = null,
            decimal? valor = null)
        {
            var token =
                HttpContext.Session.GetString("JWT");

            var perfil =
                HttpContext.Session.GetString(
                    "UsuarioPerfil");

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            if (!EhFuncionario(perfil))
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            // =================================================
            // CARREGAR LOCAÇÕES
            // =================================================

            var locacoes =
                await ObterLocacoesAsync(
                    token);

            if (locacoes == null)
            {
                TempData["Erro"] =
                    "Não foi possível carregar as locações.";

                return RedirectToAction(
                    nameof(Index));
            }

            ViewBag.Locacoes =
                FiltrarLocacoesParaPagamento(
                    locacoes);

            // =================================================
            // DETERMINAR VALOR INICIAL
            // =================================================

            decimal valorInicial =
                valor ?? 0m;

            // Se o valor não veio pela URL,
            // calcular automaticamente.
            if ((!valor.HasValue ||
                 valor.Value <= 0) &&
                locacaoId.HasValue &&
                locacaoId.Value > 0)
            {
                var locacaoSelecionada =
                    locacoes.FirstOrDefault(
                        l => l.Id ==
                             locacaoId.Value);

                if (locacaoSelecionada != null)
                {
                    valorInicial =
                        await CalcularSaldoLocacaoAsync(
                            token,
                            locacaoSelecionada);
                }
            }

            var model =
                new CriarPagamentoViewModel
                {
                    LocacaoId =
                        locacaoId ?? 0,

                    Valor =
                        valorInicial,

                    Status = 1
                };

            return View(model);
        }

        // =====================================================
        // CRIAR - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(
            CriarPagamentoViewModel model)
        {
            var token =
                HttpContext.Session.GetString("JWT");

            var perfil =
                HttpContext.Session.GetString(
                    "UsuarioPerfil");

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            if (!EhFuncionario(perfil))
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            // =================================================
            // VALIDAÇÕES BÁSICAS
            // =================================================

            if (model.LocacaoId <= 0)
            {
                ModelState.AddModelError(
                    nameof(model.LocacaoId),
                    "Selecione uma locação válida.");
            }

            if (string.IsNullOrWhiteSpace(model.Tipo))
            {
                ModelState.AddModelError(
                    nameof(model.Tipo),
                    "Selecione o tipo do pagamento.");
            }
            else
            {
                model.Tipo =
                    NormalizarTipoPagamento(
                        model.Tipo);

                if (model.Tipo != "Locação" &&
                    model.Tipo != "Multa" &&
                    model.Tipo != "Adicional")
                {
                    ModelState.AddModelError(
                        nameof(model.Tipo),
                        "Selecione um tipo de pagamento válido.");
                }
            }

            if (model.FormaPagamento < 1 ||
                model.FormaPagamento > 4)
            {
                ModelState.AddModelError(
                    nameof(model.FormaPagamento),
                    "Selecione uma forma de pagamento válida.");
            }

            if (model.Status != 1 &&
                model.Status != 2)
            {
                ModelState.AddModelError(
                    nameof(model.Status),
                    "O pagamento deve ser criado como Pendente ou Pago.");
            }

            if ((model.Tipo == "Multa" ||
                 model.Tipo == "Adicional") &&
                model.Valor <= 0)
            {
                ModelState.AddModelError(
                    nameof(model.Valor),
                    "O valor deve ser maior que zero.");
            }

            // Para Locação, o backend continua sendo
            // a autoridade para calcular o saldo.
            if (model.Tipo == "Locação")
            {
                model.Valor = 0;
            }

            // =================================================
            // CARREGAR LOCAÇÕES
            // =================================================

            var locacoes =
                await ObterLocacoesAsync(
                    token);

            if (locacoes == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível consultar as locações.");

                ViewBag.Locacoes =
                    new List<LocacaoViewModel>();

                return View(model);
            }

            ViewBag.Locacoes =
                FiltrarLocacoesParaPagamento(
                    locacoes);

            // =================================================
            // VALIDAR LOCAÇÃO
            // =================================================

            LocacaoViewModel? locacao = null;

            if (model.LocacaoId > 0)
            {
                locacao =
                    locacoes.FirstOrDefault(
                        l => l.Id ==
                             model.LocacaoId);

                if (locacao == null)
                {
                    ModelState.AddModelError(
                        nameof(model.LocacaoId),
                        "A locação selecionada não foi encontrada.");
                }
                else if (locacao.Status == 3)
                {
                    ModelState.AddModelError(
                        nameof(model.LocacaoId),
                        "Não é possível criar novas cobranças para uma locação cancelada.");
                }
                else if (locacao.Status != 1 &&
                         locacao.Status != 2)
                {
                    ModelState.AddModelError(
                        nameof(model.LocacaoId),
                        "A locação selecionada não permite novas cobranças.");
                }
            }

            // =================================================
            // REGRAS POR STATUS DA LOCAÇÃO
            // =================================================

            if (locacao != null &&
                !string.IsNullOrWhiteSpace(
                    model.Tipo))
            {
                if (locacao.Status == 1 &&
                    model.Tipo == "Locação")
                {
                    ModelState.AddModelError(
                        nameof(model.Tipo),
                        "O saldo principal da locação só pode ser cobrado após a finalização. Para uma locação ativa, utilize Multa ou Adicional.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // =================================================
            // DATA DO PAGAMENTO
            // =================================================

            DateTime? dataPagamento = null;

            if (model.Status == 2)
            {
                dataPagamento =
                    model.DataPagamento ??
                    DateTime.Now;
            }

            // =================================================
            // DTO
            // =================================================

            var dto = new
            {
                locacaoId =
                    model.LocacaoId,

                tipo =
                    model.Tipo,

                formaPagamento =
                    model.FormaPagamento,

                status =
                    model.Status,

                valor =
                    model.Valor,

                dataPagamento =
                    dataPagamento,

                observacoes =
                    model.Observacoes
            };

            var client =
                CriarClienteAutenticado(token);

            var response =
                await client.PostAsJsonAsync(
                    "api/Pagamentos",
                    dto);

            if (response.StatusCode ==
                HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();

                return RedirectToAction(
                    "Login",
                    "Account");
            }

            if (!response.IsSuccessStatusCode)
            {
                var mensagem =
                    await ObterMensagemErroAsync(
                        response,
                        "Não foi possível criar o pagamento.");

                ModelState.AddModelError(
                    string.Empty,
                    mensagem);

                return View(model);
            }

            var pagamento =
                await response.Content
                    .ReadFromJsonAsync<
                        PagamentoViewModel>();

            TempData["Sucesso"] =
                "Pagamento criado com sucesso.";

            if (pagamento != null)
            {
                return RedirectToAction(
                    nameof(Detalhes),
                    new
                    {
                        id = pagamento.Id
                    });
            }

            return RedirectToAction(
                nameof(Index));
        }

        // =====================================================
        // MARCAR COMO PAGO
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarComoPago(
            int id)
        {
            return await AlterarStatusPagamento(
                id,
                2,
                "Pagamento marcado como pago com sucesso.");
        }

        // =====================================================
        // CANCELAR PAGAMENTO
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(
            int id)
        {
            return await AlterarStatusPagamento(
                id,
                3,
                "Pagamento cancelado com sucesso.");
        }

        // =====================================================
        // ESTORNAR PAGAMENTO
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Estornar(
            int id)
        {
            return await AlterarStatusPagamento(
                id,
                4,
                "Pagamento estornado com sucesso.");
        }

        // =====================================================
        // ALTERAR STATUS DO PAGAMENTO
        // =====================================================

        private async Task<IActionResult>
            AlterarStatusPagamento(
                int id,
                int novoStatus,
                string mensagemSucesso)
        {
            var token =
                HttpContext.Session.GetString("JWT");

            var perfil =
                HttpContext.Session.GetString(
                    "UsuarioPerfil");

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            if (!EhFuncionario(perfil))
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            var client =
                CriarClienteAutenticado(token);

            var buscarResponse =
                await client.GetAsync(
                    $"api/Pagamentos/{id}");

            if (buscarResponse.StatusCode ==
                HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();

                return RedirectToAction(
                    "Login",
                    "Account");
            }

            if (!buscarResponse.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    "Não foi possível localizar o pagamento.";

                return RedirectToAction(
                    nameof(Index));
            }

            var pagamento =
                await buscarResponse.Content
                    .ReadFromJsonAsync<
                        PagamentoViewModel>();

            if (pagamento == null)
            {
                TempData["Erro"] =
                    "Pagamento não encontrado.";

                return RedirectToAction(
                    nameof(Index));
            }

            DateTime? dataPagamento =
                pagamento.DataPagamento;

            if (novoStatus == 2 &&
                dataPagamento == null)
            {
                dataPagamento =
                    DateTime.Now;
            }

            var dto = new
            {
                locacaoId =
                    pagamento.LocacaoId,

                tipo =
                    pagamento.Tipo,

                formaPagamento =
                    pagamento.FormaPagamento,

                status =
                    novoStatus,

                valor =
                    pagamento.Valor,

                dataPagamento =
                    dataPagamento,

                observacoes =
                    pagamento.Observacoes
            };

            var atualizarResponse =
                await client.PutAsJsonAsync(
                    $"api/Pagamentos/{id}",
                    dto);

            if (atualizarResponse.StatusCode ==
                HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();

                return RedirectToAction(
                    "Login",
                    "Account");
            }

            if (!atualizarResponse.IsSuccessStatusCode)
            {
                var mensagem =
                    await ObterMensagemErroAsync(
                        atualizarResponse,
                        "Não foi possível alterar o pagamento.");

                TempData["Erro"] =
                    mensagem;

                return RedirectToAction(
                    nameof(Detalhes),
                    new { id });
            }

            TempData["Sucesso"] =
                mensagemSucesso;

            return RedirectToAction(
                nameof(Detalhes),
                new { id });
        }

        // =====================================================
        // CARREGAR LOCAÇÕES PARA PAGAMENTO
        // =====================================================

        private async Task<bool>
            CarregarLocacoesAsync(
                string token)
        {
            var locacoes =
                await ObterLocacoesAsync(
                    token);

            if (locacoes == null)
            {
                ViewBag.Locacoes =
                    new List<LocacaoViewModel>();

                return false;
            }

            ViewBag.Locacoes =
                FiltrarLocacoesParaPagamento(
                    locacoes);

            return true;
        }

        // =====================================================
        // OBTER LOCAÇÕES
        // =====================================================

        private async Task<List<LocacaoViewModel>?>
            ObterLocacoesAsync(
                string token)
        {
            var client =
                CriarClienteAutenticado(token);

            var response =
                await client.GetAsync(
                    "api/Locacoes");

            if (response.StatusCode ==
                HttpStatusCode.Unauthorized)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content
                .ReadFromJsonAsync<
                    List<LocacaoViewModel>>()
                ?? new List<LocacaoViewModel>();
        }

        // =====================================================
        // CALCULAR SALDO AUTOMÁTICO DA LOCAÇÃO
        // =====================================================

        private async Task<decimal>
            CalcularSaldoLocacaoAsync(
                string token,
                LocacaoViewModel locacao)
        {
            if (locacao.Status != 2)
            {
                return 0m;
            }

            // =============================================
            // ENTRADA DE 30%
            // =============================================

            var entrada =
                Math.Round(
                    locacao.ValorTotal * 0.30m,
                    2,
                    MidpointRounding.AwayFromZero);

            // =============================================
            // SALDO PRINCIPAL
            // =============================================

            var saldo =
                locacao.ValorTotal -
                entrada;

            // =============================================
            // PAGAMENTOS JÁ REALIZADOS DO TIPO LOCAÇÃO
            // =============================================

            var client =
                CriarClienteAutenticado(token);

            var pagamentosResponse =
                await client.GetAsync(
                    "api/Pagamentos");

            if (pagamentosResponse.IsSuccessStatusCode)
            {
                var pagamentos =
                    await pagamentosResponse.Content
                        .ReadFromJsonAsync<
                            List<PagamentoViewModel>>();

                var pagamentosLocacao =
                    (pagamentos ??
                        new List<PagamentoViewModel>())
                    .Where(p =>
                        p.LocacaoId ==
                            locacao.Id &&
                        p.IsAtivo &&
                        p.Status == 2 &&
                        string.Equals(
                            p.Tipo,
                            "Locação",
                            StringComparison.OrdinalIgnoreCase))
                    .Sum(p =>
                        p.Valor);

                saldo -=
                    pagamentosLocacao;
            }

            if (saldo < 0)
            {
                saldo = 0;
            }

            return Math.Round(
                saldo,
                2,
                MidpointRounding.AwayFromZero);
        }

        // =====================================================
        // FILTRAR LOCAÇÕES
        // =====================================================

        private static List<LocacaoViewModel>
            FiltrarLocacoesParaPagamento(
                IEnumerable<LocacaoViewModel> locacoes)
        {
            return locacoes
                .Where(l =>
                    l.Status == 1 ||
                    l.Status == 2)
                .OrderByDescending(
                    l => l.Status == 1)
                .ThenByDescending(
                    l => l.Id)
                .ToList();
        }

        // =====================================================
        // NORMALIZAR TIPO
        // =====================================================

        private static string
            NormalizarTipoPagamento(
                string tipo)
        {
            tipo =
                tipo.Trim();

            if (tipo.Equals(
                "Locacao",
                StringComparison.OrdinalIgnoreCase) ||
                tipo.Equals(
                "Locação",
                StringComparison.OrdinalIgnoreCase))
            {
                return "Locação";
            }

            if (tipo.Equals(
                "Multa",
                StringComparison.OrdinalIgnoreCase))
            {
                return "Multa";
            }

            if (tipo.Equals(
                "Adicional",
                StringComparison.OrdinalIgnoreCase))
            {
                return "Adicional";
            }

            return tipo;
        }

        // =====================================================
        // OBTER MENSAGEM DE ERRO
        // =====================================================

        private static async Task<string>
            ObterMensagemErroAsync(
                HttpResponseMessage response,
                string mensagemPadrao)
        {
            try
            {
                var erro =
                    await response.Content
                        .ReadFromJsonAsync<
                            MensagemApi>();

                if (erro != null &&
                    !string.IsNullOrWhiteSpace(
                        erro.Mensagem))
                {
                    return erro.Mensagem;
                }

                var texto =
                    await response.Content
                        .ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(texto))
                {
                    return texto;
                }
            }
            catch
            {
            }

            return mensagemPadrao;
        }

        // =====================================================
        // HTTP CLIENT
        // =====================================================

        private HttpClient CriarClienteAutenticado(
            string token)
        {
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
        // PERFIL FUNCIONÁRIO
        // =====================================================

        private static bool EhFuncionario(
            string? perfil)
        {
            return perfil == "Administrador"
                || perfil == "Gerente"
                || perfil == "Atendente";
        }

        // =====================================================
        // RESPOSTA DA API
        // =====================================================

        private class MensagemApi
        {
            public string Mensagem { get; set; }
                = string.Empty;
        }
    }
}