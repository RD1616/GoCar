namespace GoCar.Domain.Entities
{
    public class ManutencaoVeiculo
    {
        public int Id { get; set; }

        public int VeiculoId { get; set; }

        public string Tipo { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public string? Oficina { get; set; }

        public DateTime DataEntrada { get; set; }

        public DateTime? DataSaida { get; set; }

        public decimal Custo { get; set; }

        public int KmVeiculo { get; set; }

        public bool Concluida { get; set; }

        public string? Observacoes { get; set; }

        // Relacionamento
        public Veiculo Veiculo { get; set; } = null!;
    }
}