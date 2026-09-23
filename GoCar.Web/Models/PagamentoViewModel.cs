namespace GoCar.Web.Models
{
    public class PagamentoViewModel
    {
        public int Id { get; set; }

        // Pagamento ligado à locação.
        // Ex.: saldo final, multa ou adicional.
        public int? LocacaoId { get; set; }

        // Pagamento ligado diretamente à reserva.
        // Ex.: entrada de 30%.
        public int? ReservaId { get; set; }

        public string Tipo { get; set; } = string.Empty;

        public int FormaPagamento { get; set; }

        public int Status { get; set; }

        public decimal Valor { get; set; }

        public DateTime? DataPagamento { get; set; }

        public string? Observacoes { get; set; }

        public bool IsAtivo { get; set; }

        public DateTime DataCriacao { get; set; }
    }
}