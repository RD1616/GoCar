using GoCar.Domain.Enums;

namespace GoCar.Application.DTOs.Reserva
{
    public class ReservaDto
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

        public DateTime DataReserva { get; set; }

        public DateTime DataRetirada { get; set; }

        public DateTime DataDevolucaoPrevista { get; set; }

        public StatusReserva Status { get; set; }

        public decimal ValorTotalPrevisto { get; set; }

        public string? Observacoes { get; set; }

        public bool IsAtiva { get; set; }

        public DateTime? DataCancelamento { get; set; }
    }
}