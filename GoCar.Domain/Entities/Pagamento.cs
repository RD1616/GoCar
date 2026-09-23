using GoCar.Domain.Enums;

namespace GoCar.Domain.Entities
{
    public class Pagamento
    {
        public int Id { get; set; }

        // =========================
        // RELACIONAMENTOS
        // =========================

        /*
         * Pagamentos feitos depois que a locação
         * existe:
         *
         * - Locação / saldo restante
         * - Multa
         * - Adicional
         */
        public int? LocacaoId { get; set; }

        /*
         * Pagamento feito antes da locação:
         *
         * - Entrada de 30% da reserva
         */
        public int? ReservaId { get; set; }

        // =========================
        // DADOS DO PAGAMENTO
        // =========================

        public string Tipo { get; set; }
            = string.Empty;

        public FormaPagamento FormaPagamento
        {
            get;
            set;
        }

        public StatusPagamento Status
        {
            get;
            set;
        }

        public decimal Valor { get; set; }

        public DateTime? DataPagamento
        {
            get;
            set;
        }

        public string? Observacoes
        {
            get;
            set;
        }

        public bool IsAtivo
        {
            get;
            set;
        }

        public DateTime DataCriacao
        {
            get;
            set;
        }

        // =========================
        // NAVEGAÇÃO
        // =========================

        public Locacao? Locacao
        {
            get;
            set;
        }

        public Reserva? Reserva
        {
            get;
            set;
        }
    }
}