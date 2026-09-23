using System.Text.Json.Serialization;

namespace GoCar.Web.Models
{
    public class ReservaViewModel
    {
        public int Id { get; set; }

        public int ClienteId { get; set; }

        public string? ClienteNome { get; set; }

        public int VeiculoId { get; set; }

        public string? VeiculoNome { get; set; }

        public string? VeiculoPlaca { get; set; }

        public int FilialRetiradaId { get; set; }

        public string? FilialRetiradaNome { get; set; }

        public int FilialDevolucaoId { get; set; }

        public string? FilialDevolucaoNome { get; set; }

        [JsonPropertyName("dataReserva")]
        public DateTime DataCriacao { get; set; }

        public DateTime DataRetirada { get; set; }

        public DateTime DataDevolucaoPrevista { get; set; }

        [JsonPropertyName("valorTotalPrevisto")]
        public decimal ValorPrevisto { get; set; }

        public int Status { get; set; }

        public bool IsAtiva { get; set; }

        public DateTime? DataCancelamento { get; set; }

        public string? Observacoes { get; set; }
    }
}