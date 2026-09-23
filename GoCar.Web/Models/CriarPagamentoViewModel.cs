using System.ComponentModel.DataAnnotations;

namespace GoCar.Web.Models
{
    public class CriarPagamentoViewModel
    {
        [Required]
        public int LocacaoId { get; set; }

        [Required]
        public string Tipo { get; set; } = string.Empty;

        [Required]
        public int FormaPagamento { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Valor { get; set; }

        public DateTime? DataPagamento { get; set; }

        public string? Observacoes { get; set; }
    }
}