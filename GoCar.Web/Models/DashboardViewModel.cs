namespace GoCar.Web.Models
{
    public class DashboardViewModel
    {
        public int TotalClientes { get; set; }

        public int TotalVeiculos { get; set; }

        public int VeiculosDisponiveis { get; set; }

        public int VeiculosAlugados { get; set; }

        public int ReservasPendentes { get; set; }

        public int LocacoesAtivas { get; set; }

        public int PagamentosPendentes { get; set; }

        public decimal TotalRecebido { get; set; }

        public List<VeiculoViewModel> VeiculosRecentes { get; set; }
            = new();

        public List<OperacaoDiaViewModel> RetiradasHoje { get; set; }
            = new();

        public List<OperacaoDiaViewModel> DevolucoesHoje { get; set; }
            = new();
    }

    public class OperacaoDiaViewModel
    {
        public int ReservaId { get; set; }

        public int? LocacaoId { get; set; }

        public string ClienteNome { get; set; }
            = string.Empty;

        public string VeiculoNome { get; set; }
            = string.Empty;

        public string VeiculoPlaca { get; set; }
            = string.Empty;

        public string FilialNome { get; set; }
            = string.Empty;

        public DateTime DataHora { get; set; }
    }
}