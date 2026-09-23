using GoCar.Domain.Enums;

namespace GoCar.Application.DTOs.Reserva
{
    public class CriarReservaDto
    {
        public int ClienteId { get; set; }

        public int VeiculoId { get; set; }

        public int FilialRetiradaId { get; set; }

        public int FilialDevolucaoId { get; set; }

        public DateTime DataRetirada { get; set; }

        public DateTime DataDevolucaoPrevista { get; set; }

        public StatusReserva Status { get; set; }

        public decimal ValorTotalPrevisto { get; set; }

        public string? Observacoes { get; set; }
    }
}