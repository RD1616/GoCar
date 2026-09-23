using GoCar.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace GoCar.Web.Controllers
{
    public class LocacoesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LocacoesController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // =====================================================
        // INDEX
        // =====================================================

        public async Task<IActionResult> Index()
        {
            var token =
                HttpContext.Session.GetString("JWT");

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var perfil =
                HttpContext.Session.GetString(
                    "UsuarioPerfil");

            var funcionario =
                EhFuncionario(perfil);

            var cliente =
                CriarClienteAutenticado(token);

            var endpoint =
                funcionario
                    ? "api/Locacoes"
                    : "api/Locacoes/minhas";

            var response =
                await cliente.GetAsync(endpoint);

            if (response.StatusCode ==
                HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();

                return RedirectToAction(
                    "Login",
                    "Account");
            }

            if (response.StatusCode ==
                HttpStatusCode.Forbidden)
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    "Não foi possível carregar as locações.";

                return View(
                    new List<LocacaoViewModel>());
            }

            var locacoes =
                await response.Content
                    .ReadFromJsonAsync<
                        List<LocacaoViewModel>>();

            ViewBag.ModoAdministrador =
                funcionario;

            return View(
                locacoes ??
                new List<LocacaoViewModel>());
        }

        // =====================================================
        // DETALHES
        // =====================================================

        public async Task<IActionResult> Detalhes(
            int id)
        {
            var token =
                HttpContext.Session.GetString("JWT");

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var perfil =
                HttpContext.Session.GetString(
                    "UsuarioPerfil");

            var funcionario =
                EhFuncionario(perfil);

            var cliente =
                CriarClienteAutenticado(token);

            var response =
                await cliente.GetAsync(
                    $"api/Locacoes/{id}");

            if (response.StatusCode ==
                HttpStatusCode.NotFound)
            {
                TempData["Erro"] =
                    "Locação não encontrada.";

                return RedirectToAction(
                    nameof(Index));
            }

            if (response.StatusCode ==
                HttpStatusCode.Forbidden)
            {
                TempData["Erro"] =
                    "Você não tem permissão para acessar esta locação.";

                return RedirectToAction(
                    nameof(Index));
            }

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
                TempData["Erro"] =
                    "Não foi possível carregar a locação.";

                return RedirectToAction(
                    nameof(Index));
            }

            var locacao =
                await response.Content
                    .ReadFromJsonAsync<
                        LocacaoViewModel>();

            if (locacao == null)
            {
                TempData["Erro"] =
                    "Locação não encontrada.";

                return RedirectToAction(
                    nameof(Index));
            }

            ViewBag.ModoAdministrador =
                funcionario;

            // =================================================
            // VALORES PADRÃO
            // =================================================

            ViewBag.PagamentoLocacao =
                null;

            ViewBag.PagamentoMulta =
                null;

            ViewBag.PagamentoAdicional =
                null;

            ViewBag.PagamentoEntrada =
                null;

            ViewBag.EntradaPaga =
                false;

            ViewBag.ValorEntrada =
                0m;

            ViewBag.SaldoRestante =
                locacao.ValorTotal;

            ViewBag.ValorMulta =
                0m;

            ViewBag.ValorAdicional =
                0m;

            ViewBag.TotalPagarAgora =
                0m;

            // =================================================
            // CARREGAR PAGAMENTOS
            // =================================================

            var endpointPagamentos =
                funcionario
                    ? "api/Pagamentos"
                    : "api/Pagamentos/minhas";

            var pagamentosResponse =
                await cliente.GetAsync(
                    endpointPagamentos);

            if (pagamentosResponse.StatusCode ==
                HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();

                return RedirectToAction(
                    "Login",
                    "Account");
            }

            if (pagamentosResponse.IsSuccessStatusCode)
            {
                var pagamentos =
                    await pagamentosResponse.Content
                        .ReadFromJsonAsync<
                            List<PagamentoViewModel>>()
                    ?? new List<PagamentoViewModel>();

                // =============================================
                // ENTRADA DE 30%
                // =============================================

                var pagamentoEntrada =
                    pagamentos
                        .Where(p =>
                            p.ReservaId ==
                                locacao.ReservaId &&
                            p.IsAtivo &&
                            string.Equals(
                                p.Tipo,
                                "Entrada",
                                StringComparison.OrdinalIgnoreCase))
                        .OrderByDescending(
                            p => p.Id)
                        .FirstOrDefault();

                var entradaPaga =
                    pagamentoEntrada != null &&
                    pagamentoEntrada.Status == 2;

                var valorEntrada =
                    entradaPaga
                        ? pagamentoEntrada!.Valor
                        : 0m;

                // =============================================
                // SALDO BASE DA LOCAÇÃO
                // =============================================

                var saldoRestante =
                    Math.Round(
                        locacao.ValorTotal -
                        valorEntrada,
                        2,
                        MidpointRounding.AwayFromZero);

                if (saldoRestante < 0)
                {
                    saldoRestante = 0;
                }

                // =============================================
                // PAGAMENTO PRINCIPAL ATUAL
                //
                // Cancelado/Estornado ficam apenas no histórico
                // e não bloqueiam uma nova cobrança.
                // =============================================

                var pagamentoLocacao =
                    pagamentos
                        .Where(p =>
                            p.LocacaoId ==
                                locacao.Id &&
                            p.IsAtivo &&
                            (p.Status == 1 ||
                             p.Status == 2) &&
                            string.Equals(
                                p.Tipo,
                                "Locação",
                                StringComparison.OrdinalIgnoreCase))
                        .OrderByDescending(
                            p => p.Id)
                        .FirstOrDefault();

                // =============================================
                // MULTA ATUAL
                // =============================================

                var pagamentoMulta =
                    pagamentos
                        .Where(p =>
                            p.LocacaoId ==
                                locacao.Id &&
                            p.IsAtivo &&
                            (p.Status == 1 ||
                             p.Status == 2) &&
                            string.Equals(
                                p.Tipo,
                                "Multa",
                                StringComparison.OrdinalIgnoreCase))
                        .OrderByDescending(
                            p => p.Id)
                        .FirstOrDefault();

                // =============================================
                // ADICIONAL ATUAL
                // =============================================

                var pagamentoAdicional =
                    pagamentos
                        .Where(p =>
                            p.LocacaoId ==
                                locacao.Id &&
                            p.IsAtivo &&
                            (p.Status == 1 ||
                             p.Status == 2) &&
                            string.Equals(
                                p.Tipo,
                                "Adicional",
                                StringComparison.OrdinalIgnoreCase))
                        .OrderByDescending(
                            p => p.Id)
                        .FirstOrDefault();

                // =============================================
                // VALORES EXTRAS ATUAIS
                // =============================================

                var valorMulta =
                    pagamentoMulta != null
                        ? pagamentoMulta.Valor
                        : 0m;

                var valorAdicional =
                    pagamentoAdicional != null
                        ? pagamentoAdicional.Valor
                        : 0m;

                // =============================================
                // TOTAL A PAGAR AGORA
                // =============================================

                decimal totalPagarAgora = 0m;

                // O saldo principal só pode ser cobrado
                // depois que a locação for finalizada.
                if (locacao.Status == 2)
                {
                    if (pagamentoLocacao == null ||
                        pagamentoLocacao.Status != 2)
                    {
                        totalPagarAgora +=
                            saldoRestante;
                    }

                    if (pagamentoMulta != null &&
                        pagamentoMulta.Status == 1)
                    {
                        totalPagarAgora +=
                            pagamentoMulta.Valor;
                    }

                    if (pagamentoAdicional != null &&
                        pagamentoAdicional.Status == 1)
                    {
                        totalPagarAgora +=
                            pagamentoAdicional.Valor;
                    }
                }
                // Durante a locação, multas/adicionais
                // pendentes podem existir, mas o saldo
                // principal ainda não é cobrável.
                else if (locacao.Status == 1)
                {
                    if (pagamentoMulta != null &&
                        pagamentoMulta.Status == 1)
                    {
                        totalPagarAgora +=
                            pagamentoMulta.Valor;
                    }

                    if (pagamentoAdicional != null &&
                        pagamentoAdicional.Status == 1)
                    {
                        totalPagarAgora +=
                            pagamentoAdicional.Valor;
                    }
                }

                // Locação cancelada permanece com
                // total a pagar agora igual a zero.

                totalPagarAgora =
                    Math.Round(
                        totalPagarAgora,
                        2,
                        MidpointRounding.AwayFromZero);

                // =============================================
                // VIEWBAGS
                // =============================================

                ViewBag.PagamentoEntrada =
                    pagamentoEntrada;

                ViewBag.EntradaPaga =
                    entradaPaga;

                ViewBag.ValorEntrada =
                    valorEntrada;

                ViewBag.SaldoRestante =
                    saldoRestante;

                ViewBag.PagamentoLocacao =
                    pagamentoLocacao;

                ViewBag.PagamentoMulta =
                    pagamentoMulta;

                ViewBag.PagamentoAdicional =
                    pagamentoAdicional;

                ViewBag.ValorMulta =
                    valorMulta;

                ViewBag.ValorAdicional =
                    valorAdicional;

                ViewBag.TotalPagarAgora =
                    totalPagarAgora;
            }

            return View(locacao);
        }

        // =====================================================
        // FINALIZAR
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalizar(
            int id,
            int kmEntrada,
            decimal combustivelEntradaPercentual,
            DateTime? dataDevolucaoReal)
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

            var cliente =
                CriarClienteAutenticado(token);

            var getResponse =
                await cliente.GetAsync(
                    $"api/Locacoes/{id}");

            if (getResponse.StatusCode ==
                HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();

                return RedirectToAction(
                    "Login",
                    "Account");
            }

            if (!getResponse.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    "Não foi possível localizar a locação.";

                return RedirectToAction(
                    nameof(Detalhes),
                    new { id });
            }

            var locacao =
                await getResponse.Content
                    .ReadFromJsonAsync<
                        LocacaoViewModel>();

            if (locacao == null)
            {
                TempData["Erro"] =
                    "Locação não encontrada.";

                return RedirectToAction(
                    nameof(Index));
            }

            if (locacao.Status != 1)
            {
                TempData["Erro"] =
                    "Somente locações ativas podem ser finalizadas.";

                return RedirectToAction(
                    nameof(Detalhes),
                    new { id });
            }

            if (kmEntrada <
                locacao.KmSaida)
            {
                TempData["Erro"] =
                    "A quilometragem de entrada não pode ser menor que a quilometragem de saída.";

                return RedirectToAction(
                    nameof(Detalhes),
                    new { id });
            }

            if (combustivelEntradaPercentual < 0 ||
                combustivelEntradaPercentual > 100)
            {
                TempData["Erro"] =
                    "O combustível de entrada deve estar entre 0% e 100%.";

                return RedirectToAction(
                    nameof(Detalhes),
                    new { id });
            }

            var dto = new
            {
                dataRetirada =
                    locacao.DataRetirada,

                dataDevolucaoReal =
                    dataDevolucaoReal ??
                    DateTime.Now,

                kmSaida =
                    locacao.KmSaida,

                kmEntrada,

                combustivelSaidaPercentual =
                    locacao.CombustivelSaidaPercentual,

                combustivelEntradaPercentual,

                valorTotal =
                    locacao.ValorTotal,

                status = 2,

                observacoes =
                    locacao.Observacoes,

                isAtiva = false
            };

            var response =
                await cliente.PutAsJsonAsync(
                    $"api/Locacoes/{id}",
                    dto);

            if (response.StatusCode ==
                HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();

                return RedirectToAction(
                    "Login",
                    "Account");
            }

            if (response.IsSuccessStatusCode)
            {
                TempData["Sucesso"] =
                    "Locação finalizada com sucesso.";

                return RedirectToAction(
                    nameof(Detalhes),
                    new { id });
            }

            TempData["Erro"] =
                await ObterMensagemErroAsync(
                    response,
                    "Não foi possível finalizar a locação.");

            return RedirectToAction(
                nameof(Detalhes),
                new { id });
        }

        // =====================================================
        // CANCELAR
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(
            int id)
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

            var cliente =
                CriarClienteAutenticado(token);

            var getResponse =
                await cliente.GetAsync(
                    $"api/Locacoes/{id}");

            if (getResponse.StatusCode ==
                HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();

                return RedirectToAction(
                    "Login",
                    "Account");
            }

            if (!getResponse.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    "Não foi possível localizar a locação.";

                return RedirectToAction(
                    nameof(Detalhes),
                    new { id });
            }

            var locacao =
                await getResponse.Content
                    .ReadFromJsonAsync<
                        LocacaoViewModel>();

            if (locacao == null)
            {
                TempData["Erro"] =
                    "Locação não encontrada.";

                return RedirectToAction(
                    nameof(Index));
            }

            if (locacao.Status != 1)
            {
                TempData["Erro"] =
                    "Somente locações ativas podem ser canceladas.";

                return RedirectToAction(
                    nameof(Detalhes),
                    new { id });
            }

            var dto = new
            {
                dataRetirada =
                    locacao.DataRetirada,

                dataDevolucaoReal =
                    locacao.DataDevolucaoReal,

                kmSaida =
                    locacao.KmSaida,

                kmEntrada =
                    locacao.KmEntrada,

                combustivelSaidaPercentual =
                    locacao.CombustivelSaidaPercentual,

                combustivelEntradaPercentual =
                    locacao.CombustivelEntradaPercentual,

                valorTotal =
                    locacao.ValorTotal,

                status = 3,

                observacoes =
                    locacao.Observacoes,

                isAtiva = false
            };

            var response =
                await cliente.PutAsJsonAsync(
                    $"api/Locacoes/{id}",
                    dto);

            if (response.StatusCode ==
                HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();

                return RedirectToAction(
                    "Login",
                    "Account");
            }

            if (response.IsSuccessStatusCode)
            {
                TempData["Sucesso"] =
                    "Locação cancelada com sucesso.";

                return RedirectToAction(
                    nameof(Detalhes),
                    new { id });
            }

            TempData["Erro"] =
                await ObterMensagemErroAsync(
                    response,
                    "Não foi possível cancelar a locação.");

            return RedirectToAction(
                nameof(Detalhes),
                new { id });
        }

        // =====================================================
        // OBTER MENSAGEM DE ERRO DA API
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
            var cliente =
                _httpClientFactory
                    .CreateClient("GoCarApi");

            cliente.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);

            return cliente;
        }

        // =====================================================
        // PERFIL
        // =====================================================

        private static bool EhFuncionario(
            string? perfil)
        {
            return perfil == "Administrador" ||
                   perfil == "Gerente" ||
                   perfil == "Atendente";
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