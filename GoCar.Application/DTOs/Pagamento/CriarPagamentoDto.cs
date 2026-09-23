using GoCar.Domain.Enums;

namespace GoCar.Application.DTOs.Pagamento
{
    public class CriarPagamentoDto
    {
        // Usado quando o pagamento pertence a uma locação.
        // Ex.: saldo, multa ou adicional.
        public int? LocacaoId { get; set; }

        // Usado quando o pagamento pertence diretamente
        // a uma reserva.
        // Ex.: entrada obrigatória de 30%.
        public int? ReservaId { get; set; }

        public string Tipo { get; set; } = string.Empty;

        public FormaPagamento FormaPagamento { get; set; }

        public StatusPagamento Status { get; set; }

        public decimal Valor { get; set; }

        public DateTime? DataPagamento { get; set; }

        public string? Observacoes { get; set; }
    }
}