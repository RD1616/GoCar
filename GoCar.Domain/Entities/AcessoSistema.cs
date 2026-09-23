namespace GoCar.Domain.Entities
{
    public class AcessoSistema
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public string Acao { get; set; } = string.Empty;

        public string? Entidade { get; set; }

        public int? EntidadeId { get; set; }

        public string? Descricao { get; set; }

        public string? EnderecoIP { get; set; }

        public DateTime DataHora { get; set; }

        // Relacionamento
        public Usuario Usuario { get; set; } = null!;
    }
}