using GoCar.Domain.Enums;

namespace GoCar.Application.DTOs.Locacao
{
    public class AtualizarLocacaoDto
    {
        public DateTime DataRetirada { get; set; }

        public DateTime? DataDevolucaoReal { get; set; }

        public int KmSaida { get; set; }

        public int? KmEntrada { get; set; }

        public decimal CombustivelSaidaPercentual { get; set; }

        public decimal? CombustivelEntradaPercentual { get; set; }

        public decimal ValorTotal { get; set; }

        public StatusLocacao Status { get; set; }

        public string? Observacoes { get; set; }

        public bool IsAtiva { get; set; }
    }
}