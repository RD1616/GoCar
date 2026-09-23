namespace GoCar.Domain.Entities
{
    public class Cliente
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string CPF { get; set; } = string.Empty;

        public DateTime DataNascimento { get; set; }

        public string Telefone { get; set; } = string.Empty;

        public string CNH { get; set; } = string.Empty;

        public string CategoriaCNH { get; set; } = string.Empty;

        public DateTime DataValidadeCNH { get; set; }

        public string Endereco { get; set; } = string.Empty;

        public string Numero { get; set; } = string.Empty;

        public string Bairro { get; set; } = string.Empty;

        public string Cidade { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;

        public string CEP { get; set; } = string.Empty;

        public bool IsAtivo { get; set; }

        public DateTime DataCadastro { get; set; }

        public Usuario Usuario { get; set; } = null!;

        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}