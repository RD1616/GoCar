namespace GoCar.Domain.Entities
{
    public class VeiculoImagem
    {
        public int Id { get; set; }

        public int VeiculoId { get; set; }

        public string Url { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        public bool Principal { get; set; }

        public int Ordem { get; set; }

        public DateTime DataCadastro { get; set; }

        // Relacionamento
        public Veiculo Veiculo { get; set; } = null!;
    }
}