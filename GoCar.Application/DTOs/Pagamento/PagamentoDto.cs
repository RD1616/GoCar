using GoCar.Domain.Enums;

namespace GoCar.Application.DTOs.Pagamento
{
    public class PagamentoDto
    {
        public int Id { get; set; }

        // Pagamento relacionado à locação
        // Ex.: saldo, multa ou adicional
        public int? LocacaoId { get; set; }

        // Pagamento relacionado diretamente à reserva
        // Ex.: entrada obrigatória de 30%
        public int? ReservaId { get; set; }

        public string Tipo { get; set; } = string.Empty;

        public FormaPagamento FormaPagamento { get; set; }

        public StatusPagamento Status { get; set; }

        public decimal Valor { get; set; }

        public DateTime? DataPagamento { get; set; }

        public string? Observacoes { get; set; }

        public bool IsAtivo { get; set; }

        public DateTime DataCriacao { get; set; }
    }
}