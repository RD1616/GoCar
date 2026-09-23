namespace GoCar.Domain.Entities
{
    public class MovimentacaoVeiculo
    {
        public int Id { get; set; }

        public int VeiculoId { get; set; }

        public int FilialOrigemId { get; set; }

        public int FilialDestinoId { get; set; }

        public DateTime DataMovimentacao { get; set; }

        public string? Motivo { get; set; }

        public int KmVeiculo { get; set; }

        public string? Observacoes { get; set; }

        // Relacionamentos
        public Veiculo Veiculo { get; set; } = null!;

        public Filial FilialOrigem { get; set; } = null!;

        public Filial FilialDestino { get; set; } = null!;
    }
}