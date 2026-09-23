using GoCar.Domain.Enums;

namespace GoCar.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;

        public GoCar.Domain.Enums.PerfilUsuario Perfil { get; set; }

        public bool IsAtivo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }

        public Cliente? Cliente { get; set; }
    }
}