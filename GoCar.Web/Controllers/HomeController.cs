using GoCar.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace GoCar.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory =
                httpClientFactory;
        }

        public async Task<IActionResult> Index(
            int? filialId = null,
            DateTime? dataRetirada = null,
            DateTime? dataDevolucao = null)
        {
            var client =
                _httpClientFactory
                    .CreateClient("GoCarApi");

            // =========================================
            // JWT
            // =========================================

            var token =
                HttpContext.Session
                    .GetString("JWT");

            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        token);
            }

            // =========================================
            // FILIAIS
            // =========================================

            var filiais =
                await client.GetFromJsonAsync<
                    List<FilialViewModel>>(
                    "api/Filiais");

            // =========================================
            // VEÍCULOS
            // =========================================

            var veiculos =
                await client.GetFromJsonAsync<
                    List<VeiculoViewModel>>(
                    "api/Veiculos");

            // =========================================
            // CATEGORIAS
            // =========================================

            var categorias =
                await client.GetFromJsonAsync<
                    List<CategoriaViewModel>>(
                    "api/Categorias");

            filiais ??=
                new List<FilialViewModel>();

            veiculos ??=
                new List<VeiculoViewModel>();

            categorias ??=
                new List<CategoriaViewModel>();

            // =========================================
            // FILIAIS ATIVAS
            // =========================================

            var filiaisAtivas =
                filiais
                    .Where(f => f.IsAtivo)
                    .OrderBy(f => f.Nome)
                    .ToList();

            // =========================================
            // CATEGORIAS ATIVAS
            // =========================================

            var categoriasPorId =
                categorias
                    .Where(c => c.IsAtivo)
                    .ToDictionary(
                        c => c.Id,
                        c => c.Nome);

            // =========================================
            // FILTROS RECEBIDOS
            // =========================================

            ViewBag.FilialSelecionadaId =
                filialId;

            ViewBag.DataRetirada =
                dataRetirada;

            ViewBag.DataDevolucao =
                dataDevolucao;

            var buscaRealizada =
                filialId.HasValue ||
                dataRetirada.HasValue ||
                dataDevolucao.HasValue;

            ViewBag.BuscaRealizada =
                buscaRealizada;

            // =========================================
            // VALIDAR BUSCA
            // =========================================

            var buscaValida = true;

            if (buscaRealizada)
            {
                if (!filialId.HasValue ||
                    filialId.Value <= 0)
                {
                    ViewBag.ErroBusca =
                        "Selecione a filial de retirada.";

                    buscaValida = false;
                }
                else if (!filiaisAtivas.Any(
                    f => f.Id == filialId.Value))
                {
                    ViewBag.ErroBusca =
                        "A filial selecionada não está disponível.";

                    buscaValida = false;
                }
                else if (!dataRetirada.HasValue)
                {
                    ViewBag.ErroBusca =
                        "Informe a data de retirada.";

                    buscaValida = false;
                }
                else if (!dataDevolucao.HasValue)
                {
                    ViewBag.ErroBusca =
                        "Informe a data de devolução.";

                    buscaValida = false;
                }
                else if (
                    dataRetirada.Value.Date <
                    DateTime.Today)
                {
                    ViewBag.ErroBusca =
                        "A data de retirada não pode ser anterior a hoje.";

                    buscaValida = false;
                }
                else if (
                    dataDevolucao.Value.Date <=
                    dataRetirada.Value.Date)
                {
                    ViewBag.ErroBusca =
                        "A data de devolução deve ser posterior à data de retirada.";

                    buscaValida = false;
                }
            }

            ViewBag.BuscaValida =
                buscaValida;

            // =========================================
            // VEÍCULOS CANDIDATOS
            //
            // 1 = Disponível
            // 2 = Reservado
            //
            // Reservado também entra porque pode
            // possuir reserva em outro período.
            // =========================================

            var veiculosDisponiveis =
                veiculos
                    .Where(v =>
                        v.IsAtivo &&
                        (
                            v.Status == 1 ||
                            v.Status == 2
                        ))
                    .ToList();

            // =========================================
            // FILTRAR PELA FILIAL
            // =========================================

            if (buscaRealizada &&
                buscaValida &&
                filialId.HasValue)
            {
                veiculosDisponiveis =
                    veiculosDisponiveis
                        .Where(v =>
                            v.FilialId ==
                            filialId.Value)
                        .ToList();
            }

            // =========================================
            // AGRUPAR UNIDADES FÍSICAS
            // =========================================

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
                            g.OrderBy(v => v.Id)
                                .First();

                        var categoriaNome =
                            categoriasPorId
                                .TryGetValue(
                                    g.Key.CategoriaId,
                                    out var nomeCategoria)
                                ? nomeCategoria
                                : "Categoria não encontrada";

                        return new HomeVeiculoGrupoViewModel
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

                            Cor =
                                representante.Cor,

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
                    .OrderBy(v =>
                        v.CategoriaNome)
                    .ThenBy(v =>
                        v.Marca)
                    .ThenBy(v =>
                        v.Modelo)
                    .ToList();

            // =========================================
            // DISPONIBILIDADE REAL
            // =========================================

            if (buscaRealizada &&
                buscaValida &&
                filialId.HasValue &&
                dataRetirada.HasValue &&
                dataDevolucao.HasValue)
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    ViewBag.ErroBusca =
                        "Faça login para consultar a disponibilidade dos veículos.";

                    ViewBag.BuscaValida =
                        false;

                    gruposVeiculos =
                        new List<HomeVeiculoGrupoViewModel>();
                }
                else
                {
                    var gruposDisponiveis =
                        new List<HomeVeiculoGrupoViewModel>();

                    // =====================================
                    // HORÁRIO DE RETIRADA
                    //
                    // Data futura:
                    // mantém o padrão das 09:00.
                    //
                    // Hoje:
                    // usa o próximo horário válido,
                    // evitando consultar 09:00 quando
                    // esse horário já passou.
                    // =====================================

                    DateTime retirada;

                    if (dataRetirada.Value.Date ==
                        DateTime.Today)
                    {
                        retirada =
                            DateTime.Now
                                .AddMinutes(30);

                        // Remove segundos/milisegundos
                        // para deixar o horário mais limpo.
                        retirada =
                            new DateTime(
                                retirada.Year,
                                retirada.Month,
                                retirada.Day,
                                retirada.Hour,
                                retirada.Minute,
                                0);
                    }
                    else
                    {
                        retirada =
                            dataRetirada.Value.Date
                                .AddHours(9);
                    }

                    // =====================================
                    // DEVOLUÇÃO
                    //
                    // Continua às 09:00 da data escolhida.
                    // =====================================

                    var devolucao =
                        dataDevolucao.Value.Date
                            .AddHours(9);

                    // Segurança adicional.
                    if (devolucao <= retirada)
                    {
                        ViewBag.ErroBusca =
                            "A devolução precisa ocorrer depois da retirada.";

                        ViewBag.BuscaValida =
                            false;

                        gruposVeiculos =
                            new List<HomeVeiculoGrupoViewModel>();
                    }
                    else
                    {
                        foreach (
                            var grupo in gruposVeiculos)
                        {
                            var url =
                                "api/Reservas/disponibilidade" +
                                $"?veiculoId={grupo.VeiculoId}" +
                                $"&filialRetiradaId={filialId.Value}" +
                                $"&dataRetirada={Uri.EscapeDataString(
                                    retirada.ToString("O"))}" +
                                $"&dataDevolucaoPrevista={Uri.EscapeDataString(
                                    devolucao.ToString("O"))}";

                            var response =
                                await client.GetAsync(url);

                            if (!response.IsSuccessStatusCode)
                            {
                                continue;
                            }

                            var resultado =
                                await response.Content
                                    .ReadFromJsonAsync<
                                        DisponibilidadeResponse>();

                            if (resultado == null)
                            {
                                continue;
                            }

                            grupo.QuantidadeDisponivel =
                                resultado.QuantidadeDisponivel;

                            if (resultado.Disponivel &&
                                resultado.QuantidadeDisponivel > 0)
                            {
                                gruposDisponiveis.Add(
                                    grupo);
                            }
                        }

                        gruposVeiculos =
                            gruposDisponiveis;
                    }
                }
            }

            // =========================================
            // HOME
            // =========================================

            var viewModel =
                new HomeViewModel
                {
                    Filiais =
                        filiaisAtivas,

                    Veiculos =
                        veiculosDisponiveis
                };

            ViewBag.GruposVeiculos =
                gruposVeiculos;

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId =
                        Activity.Current?.Id
                        ?? HttpContext.TraceIdentifier
                });
        }

        // =========================================
        // RESPOSTA DA API
        // =========================================

        private class DisponibilidadeResponse
        {
            public bool Disponivel { get; set; }

            public int QuantidadeDisponivel { get; set; }
        }

        // =========================================
        // MODELO USADO SOMENTE NA HOME
        // =========================================

        public class HomeVeiculoGrupoViewModel
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

            public string Cor { get; set; }
                = string.Empty;

            public int Combustivel { get; set; }

            public int Cambio { get; set; }

            public decimal ValorDiaria { get; set; }

            public int QuantidadeDisponivel { get; set; }
        }
    }
}