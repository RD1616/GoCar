namespace GoCar.Application.DTOs.Filial
{
    public class FilialDto
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string CNPJ { get; set; } = string.Empty;

        public string Telefone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Endereco { get; set; } = string.Empty;

        public string Numero { get; set; } = string.Empty;

        public string Bairro { get; set; } = string.Empty;

        public string Cidade { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;

        public string CEP { get; set; } = string.Empty;

        public bool IsAtivo { get; set; }
    }
}