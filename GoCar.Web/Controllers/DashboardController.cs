using GoCar.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace GoCar.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DashboardController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

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

            try
            {
                // CLIENTES

                var clientesResponse =
                    await client.GetAsync("api/Clientes");

                if (clientesResponse.StatusCode ==
                    HttpStatusCode.Unauthorized)
                {
                    HttpContext.Session.Clear();

                    return RedirectToAction(
                        "Login",
                        "Account");
                }

                var clientes =
                    clientesResponse.IsSuccessStatusCode
                        ? await clientesResponse.Content
                            .ReadFromJsonAsync<List<ClienteViewModel>>()
                        : new List<ClienteViewModel>();


                // VEÍCULOS

                var veiculosResponse =
                    await client.GetAsync("api/Veiculos");

                var veiculos =
                    veiculosResponse.IsSuccessStatusCode
                        ? await veiculosResponse.Content
                            .ReadFromJsonAsync<List<VeiculoViewModel>>()
                        : new List<VeiculoViewModel>();


                // RESERVAS

                var reservasResponse =
                    await client.GetAsync("api/Reservas");

                var reservas =
                    reservasResponse.IsSuccessStatusCode
                        ? await reservasResponse.Content
                            .ReadFromJsonAsync<List<ReservaViewModel>>()
                        : new List<ReservaViewModel>();


                // LOCAÇÕES

                var locacoesResponse =
                    await client.GetAsync("api/Locacoes");

                var locacoes =
                    locacoesResponse.IsSuccessStatusCode
                        ? await locacoesResponse.Content
                            .ReadFromJsonAsync<List<LocacaoViewModel>>()
                        : new List<LocacaoViewModel>();


                // PAGAMENTOS

                var pagamentosResponse =
                    await client.GetAsync("api/Pagamentos");

                var pagamentos =
                    pagamentosResponse.IsSuccessStatusCode
                        ? await pagamentosResponse.Content
                            .ReadFromJsonAsync<List<PagamentoViewModel>>()
                        : new List<PagamentoViewModel>();


                clientes ??= new();
                veiculos ??= new();
                reservas ??= new();
                locacoes ??= new();
                pagamentos ??= new();

                var hoje = DateTime.Today;


                // RETIRADAS DE HOJE

                var retiradasHoje =
                    reservas
                        .Where(r =>
                            r.IsAtiva &&
                            r.Status == 2 &&
                            r.DataRetirada.Date == hoje)
                        .OrderBy(r => r.DataRetirada)
                        .Select(r =>
                            new OperacaoDiaViewModel
                            {
                                ReservaId = r.Id,

                                ClienteNome =
                                    r.ClienteNome
                                    ?? $"Cliente #{r.ClienteId}",

                                VeiculoNome =
                                    r.VeiculoNome
                                    ?? $"Veículo #{r.VeiculoId}",

                                VeiculoPlaca =
                                    r.VeiculoPlaca ?? "-",

                                FilialNome =
                                    r.FilialRetiradaNome
                                    ?? $"Filial #{r.FilialRetiradaId}",

                                DataHora =
                                    r.DataRetirada
                            })
                        .ToList();


                // DEVOLUÇÕES DE HOJE

                var devolucoesHoje =
                    (
                        from locacao in locacoes

                        join reserva in reservas
                            on locacao.ReservaId
                            equals reserva.Id

                        where
                            locacao.IsAtiva &&
                            locacao.Status == 1 &&
                            reserva.DataDevolucaoPrevista.Date
                                == hoje

                        orderby
                            reserva.DataDevolucaoPrevista

                        select new OperacaoDiaViewModel
                        {
                            ReservaId =
                                reserva.Id,

                            LocacaoId =
                                locacao.Id,

                            ClienteNome =
                                reserva.ClienteNome
                                ?? $"Cliente #{reserva.ClienteId}",

                            VeiculoNome =
                                reserva.VeiculoNome
                                ?? $"Veículo #{reserva.VeiculoId}",

                            VeiculoPlaca =
                                reserva.VeiculoPlaca ?? "-",

                            FilialNome =
                                reserva.FilialDevolucaoNome
                                ?? $"Filial #{reserva.FilialDevolucaoId}",

                            DataHora =
                                reserva.DataDevolucaoPrevista
                        }
                    )
                    .ToList();


                // DASHBOARD

                var viewModel =
                    new DashboardViewModel
                    {
                        TotalClientes =
                            clientes.Count(c =>
                                c.IsAtivo),

                        TotalVeiculos =
                            veiculos.Count(v =>
                                v.IsAtivo),

                        VeiculosDisponiveis =
                            veiculos.Count(v =>
                                v.IsAtivo &&
                                v.Status == 1),

                        VeiculosAlugados =
                            veiculos.Count(v =>
                                v.IsAtivo &&
                                v.Status == 3),

                        ReservasPendentes =
                            reservas.Count(r =>
                                r.IsAtiva &&
                                r.Status == 2),

                        LocacoesAtivas =
                            locacoes.Count(l =>
                                l.IsAtiva &&
                                l.Status == 1),

                        PagamentosPendentes =
                            pagamentos.Count(p =>
                                p.IsAtivo &&
                                p.Status == 1),

                        TotalRecebido =
                            pagamentos
                                .Where(p =>
                                    p.IsAtivo &&
                                    p.Status == 2)
                                .Sum(p => p.Valor),

                        VeiculosRecentes =
                            veiculos
                                .Where(v => v.IsAtivo)
                                .OrderByDescending(v => v.Id)
                                .Take(5)
                                .ToList(),

                        RetiradasHoje =
                            retiradasHoje,

                        DevolucoesHoje =
                            devolucoesHoje
                    };

                return View(viewModel);
            }
            catch
            {
                TempData["Erro"] =
                    "Não foi possível carregar todos os dados do Dashboard.";

                return View(
                    new DashboardViewModel());
            }
        }

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
    }
}