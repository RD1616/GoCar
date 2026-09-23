using GoCar.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace GoCar.Web.Controllers
{
    public class ReservasController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ReservasController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // =========================
        // LISTAR RESERVAS
        // =========================

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

            var client =
                CriarClienteAutenticado(token);

            string endpoint;

            var funcionario =
                EhFuncionario(perfil);

            if (funcionario)
            {
                endpoint = "api/Reservas";
                ViewBag.ModoAdministrador = true;
            }
            else if (perfil == "Cliente")
            {
                endpoint = "api/Reservas/minhas";
                ViewBag.ModoAdministrador = false;
            }
            else
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            // =========================
            // RESERVAS
            // =========================

            var response =
                await client.GetAsync(endpoint);

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
                    "Não foi possível carregar as reservas.";

                return View(
                    new List<ReservaViewModel>());
            }

            var reservas =
                await response.Content
                    .ReadFromJsonAsync<
                        List<ReservaViewModel>>()
                ?? new List<ReservaViewModel>();

            // =========================
            // PAGAMENTOS
            // =========================

            var endpointPagamentos =
                funcionario
                    ? "api/Pagamentos"
                    : "api/Pagamentos/minhas";

            var pagamentosResponse =
                await client.GetAsync(
                    endpointPagamentos);

            var pagamentos =
                new List<PagamentoViewModel>();

            if (pagamentosResponse.IsSuccessStatusCode)
            {
                pagamentos =
                    await pagamentosResponse.Content
                        .ReadFromJsonAsync<
                            List<PagamentoViewModel>>()
                    ?? new List<PagamentoViewModel>();
            }

            // =========================
            // ENTRADAS PAGAS
            // =========================

            var reservasComEntradaPaga =
                pagamentos
                    .Where(p =>
                        p.ReservaId.HasValue &&
                        p.IsAtivo &&
                        p.Status == 2 &&
                        string.Equals(
                            p.Tipo,
                            "Entrada",
                            StringComparison.OrdinalIgnoreCase))
                    .Select(p =>
                        p.ReservaId!.Value)
                    .ToHashSet();

            ViewBag.ReservasComEntradaPaga =
                reservasComEntradaPaga;

            // =========================
            // LOCAÇÕES
            // =========================

            var endpointLocacoes =
                funcionario
                    ? "api/Locacoes"
                    : "api/Locacoes/minhas";

            var locacoesResponse =
                await client.GetAsync(
                    endpointLocacoes);

            var locacoes =
                new List<LocacaoViewModel>();

            if (locacoesResponse.IsSuccessStatusCode)
            {
                locacoes =
                    await locacoesResponse.Content
                        .ReadFromJsonAsync<
                            List<LocacaoViewModel>>()
                    ?? new List<LocacaoViewModel>();
            }

            // =========================
            // RESERVA -> LOCAÇÃO
            // =========================

            var locacaoPorReserva =
                locacoes
                    .GroupBy(l =>
                        l.ReservaId)
                    .ToDictionary(
                        g => g.Key,
                        g => g
                            .OrderByDescending(l =>
                                l.Id)
                            .First());

            // =========================
            // SALDOS PAGOS
            // =========================

            var reservasComSaldoPago =
                new HashSet<int>();

            var valorSaldoPagoPorReserva =
                new Dictionary<int, decimal>();

            foreach (var reserva in reservas)
            {
                if (!locacaoPorReserva.TryGetValue(
                    reserva.Id,
                    out var locacao))
                {
                    continue;
                }

                var pagamentoSaldo =
                    pagamentos
                        .Where(p =>
                            p.LocacaoId ==
                                locacao.Id &&
                            p.IsAtivo &&
                            p.Status == 2 &&
                            string.Equals(
                                p.Tipo,
                                "Locação",
                                StringComparison.OrdinalIgnoreCase))
                        .OrderByDescending(p =>
                            p.DataPagamento ??
                            p.DataCriacao)
                        .FirstOrDefault();

                if (pagamentoSaldo != null)
                {
                    reservasComSaldoPago.Add(
                        reserva.Id);

                    valorSaldoPagoPorReserva[
                        reserva.Id] =
                        pagamentoSaldo.Valor;
                }
            }

            ViewBag.ReservasComSaldoPago =
                reservasComSaldoPago;

            ViewBag.ValorSaldoPagoPorReserva =
                valorSaldoPagoPorReserva;

            return View(reservas);
        }

        // =========================
        // NOVA RESERVA
        // =========================

        [HttpGet]
        public async Task<IActionResult> Criar(
            int? veiculoId = null,
            int? filialId = null,
            DateTime? dataRetirada = null,
            DateTime? dataDevolucao = null)
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

            var funcionario =
                EhFuncionario(perfil);

            var cliente =
                perfil == "Cliente";

            if (!funcionario && !cliente)
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            var carregado =
                await CarregarDadosCriacaoAsync(
                    token,
                    funcionario,
                    veiculoId);

            if (!carregado)
            {
                TempData["Erro"] =
                    "Não foi possível carregar os dados para criar a reserva.";

                return RedirectToAction(
                    nameof(Index));
            }

            ViewBag.ModoAdministrador =
                funcionario;

            ViewBag.VeiculoSelecionadoId =
                veiculoId;

            ViewBag.FilialSelecionadaId =
                filialId;

            ViewBag.DataRetiradaSelecionada =
                dataRetirada;

            ViewBag.DataDevolucaoSelecionada =
                dataDevolucao;

            return View();
        }

        // =========================
        // CONSULTAR DISPONIBILIDADE
        // =========================

        [HttpGet]
        public async Task<IActionResult> ConsultarDisponibilidade(
            int veiculoId,
            int filialRetiradaId,
            DateTime dataRetirada,
            DateTime dataDevolucaoPrevista)
        {
            var token =
                HttpContext.Session.GetString("JWT");

            var perfil =
                HttpContext.Session.GetString(
                    "UsuarioPerfil");

            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized(new
                {
                    mensagem =
                        "Sua sessão expirou."
                });
            }

            if (!EhFuncionario(perfil) &&
                perfil != "Cliente")
            {
                return Forbid();
            }

            if (veiculoId <= 0 ||
                filialRetiradaId <= 0)
            {
                return BadRequest(new
                {
                    mensagem =
                        "Informe o veículo e a filial de retirada."
                });
            }

            if (dataDevolucaoPrevista <=
                dataRetirada)
            {
                return BadRequest(new
                {
                    mensagem =
                        "A data de devolução deve ser posterior à data de retirada."
                });
            }

            var client =
                CriarClienteAutenticado(token);

            var endpoint =
                "api/Reservas/disponibilidade" +
                $"?veiculoId={veiculoId}" +
                $"&filialRetiradaId={filialRetiradaId}" +
                $"&dataRetirada={Uri.EscapeDataString(dataRetirada.ToString("O"))}" +
                $"&dataDevolucaoPrevista={Uri.EscapeDataString(dataDevolucaoPrevista.ToString("O"))}";

            var response =
                await client.GetAsync(endpoint);

            if (response.StatusCode ==
                HttpStatusCode.Unauthorized)
            {
                return Unauthorized(new
                {
                    mensagem =
                        "Sua sessão expirou."
                });
            }

            if (response.StatusCode ==
                HttpStatusCode.Forbidden)
            {
                return Forbid();
            }

            if (!response.IsSuccessStatusCode)
            {
                var mensagem =
                    "Não foi possível verificar a disponibilidade.";

                try
                {
                    var erro =
                        await response.Content
                            .ReadFromJsonAsync<
                                MensagemApi>();

                    if (!string.IsNullOrWhiteSpace(
                        erro?.Mensagem))
                    {
                        mensagem =
                            erro.Mensagem;
                    }
                }
                catch
                {
                    // Mantém a mensagem padrão.
                }

                return StatusCode(
                    (int)response.StatusCode,
                    new
                    {
                        mensagem
                    });
            }

            var disponibilidade =
                await response.Content
                    .ReadFromJsonAsync<
                        DisponibilidadeApi>();

            if (disponibilidade == null)
            {
                return StatusCode(
                    StatusCodes.Status502BadGateway,
                    new
                    {
                        mensagem =
                            "A API não retornou os dados de disponibilidade."
                    });
            }

            return Json(new
            {
                disponivel =
                    disponibilidade.Disponivel,

                quantidadeDisponivel =
                    disponibilidade.QuantidadeDisponivel
            });
        }

        // =========================
        // CRIAR RESERVA - POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(
            int clienteId,
            int veiculoId,
            int filialRetiradaId,
            int filialDevolucaoId,
            DateTime dataRetirada,
            DateTime dataDevolucaoPrevista,
            decimal valorTotalPrevisto,
            string? observacoes)
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

            var funcionario =
                EhFuncionario(perfil);

            var cliente =
                perfil == "Cliente";

            if (!funcionario && !cliente)
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            var parametrosRetorno =
                new
                {
                    veiculoId,
                    filialId =
                        filialRetiradaId,

                    dataRetirada,

                    dataDevolucao =
                        dataDevolucaoPrevista
                };

            if (funcionario &&
                clienteId <= 0)
            {
                TempData["Erro"] =
                    "Selecione um cliente.";

                return RedirectToAction(
                    nameof(Criar),
                    parametrosRetorno);
            }

            if (veiculoId <= 0)
            {
                TempData["Erro"] =
                    "Selecione um veículo.";

                return RedirectToAction(
                    nameof(Criar),
                    new
                    {
                        filialId =
                            filialRetiradaId,

                        dataRetirada,

                        dataDevolucao =
                            dataDevolucaoPrevista
                    });
            }

            if (filialRetiradaId <= 0 ||
                filialDevolucaoId <= 0)
            {
                TempData["Erro"] =
                    "Selecione as filiais de retirada e devolução.";

                return RedirectToAction(
                    nameof(Criar),
                    parametrosRetorno);
            }

            if (dataRetirada <= DateTime.Now)
            {
                TempData["Erro"] =
                    "A data de retirada deve ser futura.";

                return RedirectToAction(
                    nameof(Criar),
                    parametrosRetorno);
            }

            if (dataDevolucaoPrevista <=
                dataRetirada)
            {
                TempData["Erro"] =
                    "A data de devolução deve ser posterior à data de retirada.";

                return RedirectToAction(
                    nameof(Criar),
                    parametrosRetorno);
            }

            var dto = new
            {
                clienteId =
                    funcionario
                        ? clienteId
                        : 0,

                veiculoId,

                filialRetiradaId,

                filialDevolucaoId,

                dataRetirada,

                dataDevolucaoPrevista,

                status = 1,

                valorTotalPrevisto,

                observacoes
            };

            var client =
                CriarClienteAutenticado(token);

            var response =
                await client.PostAsJsonAsync(
                    "api/Reservas",
                    dto);

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
                TempData["Erro"] =
                    "Você não possui permissão para criar esta reserva.";

                return RedirectToAction(
                    nameof(Index));
            }

            if (!response.IsSuccessStatusCode)
            {
                await DefinirErroDaApiAsync(
                    response,
                    "Não foi possível criar a reserva.");

                return RedirectToAction(
                    nameof(Criar),
                    parametrosRetorno);
            }

            var reserva =
                await response.Content
                    .ReadFromJsonAsync<
                        ReservaViewModel>();

            TempData["Sucesso"] =
                "Reserva criada com sucesso. Aguardando pagamento da entrada de 30%.";

            if (reserva != null)
            {
                return RedirectToAction(
                    nameof(Detalhes),
                    new { id = reserva.Id });
            }

            return RedirectToAction(
                nameof(Index));
        }

        // =========================
        // DETALHES
        // =========================

        [HttpGet]
        public async Task<IActionResult> Detalhes(
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

            var funcionario =
                EhFuncionario(perfil);

            var cliente =
                perfil == "Cliente";

            if (!funcionario && !cliente)
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            var client =
                CriarClienteAutenticado(token);

            var response =
                await client.GetAsync(
                    $"api/Reservas/{id}");

            if (response.StatusCode ==
                HttpStatusCode.NotFound)
            {
                TempData["Erro"] =
                    "Reserva não encontrada.";

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

            if (response.StatusCode ==
                HttpStatusCode.Forbidden)
            {
                TempData["Erro"] =
                    "Você não possui permissão para visualizar esta reserva.";

                return RedirectToAction(
                    nameof(Index));
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    "Não foi possível carregar a reserva.";

                return RedirectToAction(
                    nameof(Index));
            }

            var reserva =
                await response.Content
                    .ReadFromJsonAsync<
                        ReservaViewModel>();

            if (reserva == null)
            {
                TempData["Erro"] =
                    "Reserva não encontrada.";

                return RedirectToAction(
                    nameof(Index));
            }

            ViewBag.ModoAdministrador =
                funcionario;

            ViewBag.ValorEntrada =
                Math.Round(
                    reserva.ValorPrevisto * 0.30m,
                    2,
                    MidpointRounding.AwayFromZero);

            ViewBag.PagamentoEntrada = null;
            ViewBag.EntradaPaga = false;

            var endpointPagamentos =
                funcionario
                    ? "api/Pagamentos"
                    : "api/Pagamentos/minhas";

            var pagamentosResponse =
                await client.GetAsync(
                    endpointPagamentos);

            if (pagamentosResponse.IsSuccessStatusCode)
            {
                var pagamentos =
                    await pagamentosResponse.Content
                        .ReadFromJsonAsync<
                            List<PagamentoViewModel>>();

                var entrada =
                    (pagamentos ??
                        new List<PagamentoViewModel>())
                    .Where(p =>
                        p.ReservaId == reserva.Id &&
                        p.IsAtivo &&
                        (p.Status == 1 ||
                         p.Status == 2) &&
                        string.Equals(
                            p.Tipo,
                            "Entrada",
                            StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(p =>
                        p.DataCriacao)
                    .FirstOrDefault();

                if (entrada != null)
                {
                    ViewBag.PagamentoEntrada =
                        entrada;

                    ViewBag.EntradaPaga =
                        entrada.Status == 2;
                }
            }

            return View(reserva);
        }

        // =========================
        // PAGAR ENTRADA
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PagarEntrada(
            int id,
            int formaPagamento)
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

            if (perfil != "Cliente")
            {
                TempData["Erro"] =
                    "Somente clientes podem realizar o pagamento da entrada.";

                return RedirectToAction(
                    nameof(Detalhes),
                    new { id });
            }

            if (formaPagamento < 1 ||
                formaPagamento > 4)
            {
                TempData["Erro"] =
                    "Selecione uma forma de pagamento válida.";

                return RedirectToAction(
                    nameof(Detalhes),
                    new { id });
            }

            var client =
                CriarClienteAutenticado(token);

            var dto = new
            {
                reservaId = id,
                formaPagamento
            };

            var response =
                await client.PostAsJsonAsync(
                    "api/Pagamentos/entrada",
                    dto);

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
                TempData["Erro"] =
                    "Você não possui permissão para pagar a entrada desta reserva.";

                return RedirectToAction(
                    nameof(Index));
            }

            if (!response.IsSuccessStatusCode)
            {
                await DefinirErroDaApiAsync(
                    response,
                    "Não foi possível realizar o pagamento da entrada.");

                return RedirectToAction(
                    nameof(Detalhes),
                    new { id });
            }

            TempData["Sucesso"] =
                "Entrada de 30% paga com sucesso. Sua reserva foi confirmada.";

            return RedirectToAction(
                nameof(Detalhes),
                new { id });
        }

        // =========================
        // CANCELAR - FUNCIONÁRIO
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(
            int id)
        {
            var resultado =
                await ExecutarAcaoAdministrativa(
                    id,
                    "cancelar");

            if (resultado)
            {
                TempData["Sucesso"] =
                    "Reserva cancelada com sucesso.";
            }

            return RedirectToAction(
                nameof(Detalhes),
                new { id });
        }

        // =========================
        // CANCELAR - CLIENTE
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            CancelarMinhaReserva(int id)
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

            if (perfil != "Cliente")
            {
                return RedirectToAction(
                    nameof(Index));
            }

            var client =
                CriarClienteAutenticado(token);

            var response =
                await client.DeleteAsync(
                    $"api/Reservas/{id}");

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
                TempData["Erro"] =
                    "Você não possui permissão para cancelar esta reserva.";

                return RedirectToAction(
                    nameof(Index));
            }

            if (response.StatusCode ==
                HttpStatusCode.NotFound)
            {
                TempData["Erro"] =
                    "Reserva não encontrada.";

                return RedirectToAction(
                    nameof(Index));
            }

            if (!response.IsSuccessStatusCode)
            {
                await DefinirErroDaApiAsync(
                    response,
                    "Não foi possível cancelar a reserva.");

                return RedirectToAction(
                    nameof(Detalhes),
                    new { id });
            }

            TempData["Sucesso"] =
                "Reserva cancelada com sucesso.";

            return RedirectToAction(
                nameof(Detalhes),
                new { id });
        }

        // =========================
        // INICIAR LOCAÇÃO
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IniciarLocacao(
            int id,
            int kmSaida,
            decimal combustivelSaidaPercentual,
            DateTime? dataRetirada,
            string? observacoes)
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

            if (kmSaida < 0)
            {
                TempData["Erro"] =
                    "A quilometragem de saída não pode ser negativa.";

                return RedirectToAction(
                    nameof(Detalhes),
                    new { id });
            }

            if (combustivelSaidaPercentual < 0 ||
                combustivelSaidaPercentual > 100)
            {
                TempData["Erro"] =
                    "O combustível deve estar entre 0% e 100%.";

                return RedirectToAction(
                    nameof(Detalhes),
                    new { id });
            }

            var client =
                CriarClienteAutenticado(token);

            var reservaResponse =
                await client.GetAsync(
                    $"api/Reservas/{id}");

            if (reservaResponse.StatusCode ==
                HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();

                return RedirectToAction(
                    "Login",
                    "Account");
            }

            if (!reservaResponse.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    "Não foi possível localizar a reserva.";

                return RedirectToAction(
                    nameof(Detalhes),
                    new { id });
            }

            var reserva =
                await reservaResponse.Content
                    .ReadFromJsonAsync<
                        ReservaViewModel>();

            if (reserva == null)
            {
                TempData["Erro"] =
                    "Reserva não encontrada.";

                return RedirectToAction(
                    nameof(Index));
            }

            if (reserva.Status != 2)
            {
                TempData["Erro"] =
                    "A locação só pode ser iniciada após o pagamento da entrada de 30%.";

                return RedirectToAction(
                    nameof(Detalhes),
                    new { id });
            }

            if (!reserva.IsAtiva)
            {
                TempData["Erro"] =
                    "Esta reserva não está ativa.";

                return RedirectToAction(
                    nameof(Detalhes),
                    new { id });
            }

            var pagamentosResponse =
                await client.GetAsync(
                    "api/Pagamentos");

            if (!pagamentosResponse.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    "Não foi possível verificar o pagamento da entrada.";

                return RedirectToAction(
                    nameof(Detalhes),
                    new { id });
            }

            var pagamentos =
                await pagamentosResponse.Content
                    .ReadFromJsonAsync<
                        List<PagamentoViewModel>>();

            var entradaPaga =
                (pagamentos ??
                    new List<PagamentoViewModel>())
                .Any(p =>
                    p.ReservaId == reserva.Id &&
                    p.IsAtivo &&
                    p.Status == 2 &&
                    string.Equals(
                        p.Tipo,
                        "Entrada",
                        StringComparison.OrdinalIgnoreCase));

            if (!entradaPaga)
            {
                TempData["Erro"] =
                    "Não é possível iniciar a locação. A entrada de 30% ainda não foi paga.";

                return RedirectToAction(
                    nameof(Detalhes),
                    new { id });
            }

            var dto = new
            {
                reservaId =
                    reserva.Id,

                dataRetirada =
                    dataRetirada ??
                    DateTime.Now,

                kmSaida,

                combustivelSaidaPercentual,

                valorTotal =
                    reserva.ValorPrevisto,

                status = 1,

                observacoes =
                    string.IsNullOrWhiteSpace(
                        observacoes)
                        ? reserva.Observacoes
                        : observacoes
            };

            var locacaoResponse =
                await client.PostAsJsonAsync(
                    "api/Locacoes",
                    dto);

            if (locacaoResponse.StatusCode ==
                HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();

                return RedirectToAction(
                    "Login",
                    "Account");
            }

            if (!locacaoResponse.IsSuccessStatusCode)
            {
                await DefinirErroDaApiAsync(
                    locacaoResponse,
                    "Não foi possível iniciar a locação.");

                return RedirectToAction(
                    nameof(Detalhes),
                    new { id });
            }

            var locacao =
                await locacaoResponse.Content
                    .ReadFromJsonAsync<
                        LocacaoViewModel>();

            TempData["Sucesso"] =
                "Locação iniciada com sucesso.";

            if (locacao != null)
            {
                return RedirectToAction(
                    "Detalhes",
                    "Locacoes",
                    new { id = locacao.Id });
            }

            return RedirectToAction(
                "Index",
                "Locacoes");
        }

        // =========================
        // CARREGAR DADOS
        // =========================

        private async Task<bool>
            CarregarDadosCriacaoAsync(
                string token,
                bool funcionario,
                int? veiculoSelecionadoId)
        {
            var client =
                CriarClienteAutenticado(token);

            var veiculosResponse =
                await client.GetAsync(
                    "api/Veiculos");

            var filiaisResponse =
                await client.GetAsync(
                    "api/Filiais");

            var categoriasResponse =
                await client.GetAsync(
                    "api/Categorias");

            if (!veiculosResponse.IsSuccessStatusCode ||
                !filiaisResponse.IsSuccessStatusCode ||
                !categoriasResponse.IsSuccessStatusCode)
            {
                return false;
            }

            var veiculos =
                await veiculosResponse.Content
                    .ReadFromJsonAsync<
                        List<VeiculoViewModel>>()
                ?? new List<VeiculoViewModel>();

            var filiais =
                await filiaisResponse.Content
                    .ReadFromJsonAsync<
                        List<FilialViewModel>>()
                ?? new List<FilialViewModel>();

            var categorias =
                await categoriasResponse.Content
                    .ReadFromJsonAsync<
                        List<CategoriaViewModel>>()
                ?? new List<CategoriaViewModel>();

            var categoriasPorId =
                categorias
                    .Where(c =>
                        c.IsAtivo)
                    .ToDictionary(
                        c => c.Id,
                        c => c.Nome);

            var veiculosDisponiveis =
                veiculos
                    .Where(v =>
                        v.IsAtivo &&
                        (
                            v.Status == 1 ||
                            v.Status == 2
                        ))
                    .ToList();

            var gruposVeiculos =
                veiculosDisponiveis
                    .GroupBy(v => new
                    {
                        v.CategoriaId,
                        v.Marca,
                        v.Modelo,
                        v.AnoFabricacao,
                        v.AnoModelo,
                        v.ValorDiaria
                    })
                    .Select(g =>
                    {
                        var representante =
                            veiculoSelecionadoId.HasValue
                                ? g.FirstOrDefault(v =>
                                    v.Id ==
                                    veiculoSelecionadoId.Value)
                                : null;

                        representante ??=
                            g.OrderBy(v =>
                                v.Id)
                                .First();

                        var categoriaNome =
                            categoriasPorId.TryGetValue(
                                g.Key.CategoriaId,
                                out var nomeCategoria)
                                ? nomeCategoria
                                : $"Categoria #{g.Key.CategoriaId}";

                        return new VeiculoGrupoViewModel
                        {
                            VeiculoId =
                                representante.Id,

                            CategoriaId =
                                g.Key.CategoriaId,

                            CategoriaNome =
                                categoriaNome,

                            Marca =
                                g.Key.Marca,

                            Modelo =
                                g.Key.Modelo,

                            AnoFabricacao =
                                g.Key.AnoFabricacao,

                            AnoModelo =
                                g.Key.AnoModelo,

                            Combustivel =
                                representante.Combustivel,

                            Cambio =
                                representante.Cambio,

                            ValorDiaria =
                                g.Key.ValorDiaria,

                            QuantidadeDisponivel =
                                g.Count()
                        };
                    })
                    .OrderBy(g =>
                        g.CategoriaNome)
                    .ThenBy(g =>
                        g.Marca)
                    .ThenBy(g =>
                        g.Modelo)
                    .ToList();

            ViewBag.GruposVeiculos =
                gruposVeiculos;

            ViewBag.Veiculos =
                veiculosDisponiveis
                    .OrderBy(v =>
                        v.Marca)
                    .ThenBy(v =>
                        v.Modelo)
                    .ToList();

            ViewBag.Filiais =
                filiais
                    .Where(f =>
                        f.IsAtivo)
                    .OrderBy(f =>
                        f.Nome)
                    .ToList();

            if (funcionario)
            {
                var clientesResponse =
                    await client.GetAsync(
                        "api/Clientes");

                if (!clientesResponse.IsSuccessStatusCode)
                {
                    return false;
                }

                var clientes =
                    await clientesResponse.Content
                        .ReadFromJsonAsync<
                            List<ClienteViewModel>>();

                ViewBag.Clientes =
                    (clientes ??
                        new List<ClienteViewModel>())
                    .Where(c =>
                        c.IsAtivo)
                    .OrderBy(c =>
                        c.Nome)
                    .ToList();
            }
            else
            {
                ViewBag.Clientes =
                    new List<ClienteViewModel>();
            }

            return true;
        }

        // =========================
        // AÇÃO ADMINISTRATIVA
        // =========================

        private async Task<bool>
            ExecutarAcaoAdministrativa(
                int id,
                string acao)
        {
            var token =
                HttpContext.Session.GetString("JWT");

            var perfil =
                HttpContext.Session.GetString(
                    "UsuarioPerfil");

            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["Erro"] =
                    "Sua sessão expirou.";

                return false;
            }

            if (!EhFuncionario(perfil))
            {
                TempData["Erro"] =
                    "Você não possui permissão para realizar esta ação.";

                return false;
            }

            var client =
                CriarClienteAutenticado(token);

            var response =
                await client.PutAsync(
                    $"api/Reservas/{id}/{acao}",
                    null);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            await DefinirErroDaApiAsync(
                response,
                "Não foi possível alterar a reserva.");

            return false;
        }

        // =========================
        // ERRO DA API
        // =========================

        private async Task DefinirErroDaApiAsync(
            HttpResponseMessage response,
            string mensagemPadrao)
        {
            try
            {
                var erro =
                    await response.Content
                        .ReadFromJsonAsync<
                            MensagemApi>();

                TempData["Erro"] =
                    erro?.Mensagem ??
                    mensagemPadrao;
            }
            catch
            {
                TempData["Erro"] =
                    mensagemPadrao;
            }
        }

        // =========================
        // HTTP CLIENT
        // =========================

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

        private static bool EhFuncionario(
            string? perfil)
        {
            return perfil == "Administrador"
                || perfil == "Gerente"
                || perfil == "Atendente";
        }

        // =========================
        // VIEW MODEL DO AGRUPAMENTO
        // =========================

        public class VeiculoGrupoViewModel
        {
            public int VeiculoId { get; set; }

            public int CategoriaId { get; set; }

            public string CategoriaNome { get; set; }
                = string.Empty;

            public string Marca { get; set; }
                = string.Empty;

            public string Modelo { get; set; }
                = string.Empty;

            public short AnoFabricacao { get; set; }

            public short AnoModelo { get; set; }

            public int Combustivel { get; set; }

            public int Cambio { get; set; }

            public decimal ValorDiaria { get; set; }

            public int QuantidadeDisponivel { get; set; }
        }

        // =========================
        // RETORNO DISPONIBILIDADE API
        // =========================

        private class DisponibilidadeApi
        {
            public bool Disponivel { get; set; }

            public int QuantidadeDisponivel { get; set; }
        }

        private class MensagemApi
        {
            public string Mensagem { get; set; }
                = string.Empty;
        }
    }
}