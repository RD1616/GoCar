using GoCar.Domain.Enums;

namespace GoCar.Domain.Entities
{
    public class Locacao
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

        public StatusLocacao Status { get; set; }

        public string? Observacoes { get; set; }

        public bool IsAtiva { get; set; }

        public DateTime DataCriacao { get; set; }

        // Relacionamentos
        public Reserva Reserva { get; set; } = null!;

        public ICollection<Pagamento> Pagamentos { get; set; }
            = new List<Pagamento>();
    }
}