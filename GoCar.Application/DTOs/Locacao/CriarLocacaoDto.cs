using GoCar.Domain.Enums;

namespace GoCar.Application.DTOs.Locacao
{
    public class CriarLocacaoDto
    {
        public int ReservaId { get; set; }

        public DateTime DataRetirada { get; set; }

        public int KmSaida { get; set; }

        public decimal CombustivelSaidaPercentual { get; set; }

        public decimal ValorTotal { get; set; }

        public StatusLocacao Status { get; set; }

        public string? Observacoes { get; set; }
    }
}