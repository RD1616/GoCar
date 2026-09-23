namespace GoCar.Web.Models
{
    public class LocacaoViewModel
    {
        public int Id { get; set; }

        public int ReservaId { get; set; }

        public DateTime DataRetirada { get; set; }

        public DateTime? DataDevolucaoReal { get; set; }

        public int KmSaida { get; set; }

        public int? KmEntrada { get; set; }

        public decimal CombustivelSaidaPercentual { get; set; }

        public decimal? CombustivelEntradaPercentual { get; set; }

        public decimal ValorTotal { get; set; }

        public int Status { get; set; }

        public string? Observacoes { get; set; }

        public bool IsAtiva { get; set; }

        public DateTime DataCriacao { get; set; }
    }
}