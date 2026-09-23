using System.ComponentModel.DataAnnotations;

namespace GoCar.Web.Models
{
    public class ClienteViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public string CPF { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DataNascimento { get; set; }

        [Required]
        public string Telefone { get; set; } = string.Empty;

        [Required]
        public string CNH { get; set; } = string.Empty;

        [Required]
        public string CategoriaCNH { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DataValidadeCNH { get; set; }

        [Required]
        public string Endereco { get; set; } = string.Empty;

        [Required]
        public string Numero { get; set; } = string.Empty;

        [Required]
        public string Bairro { get; set; } = string.Empty;

        [Required]
        public string Cidade { get; set; } = string.Empty;

        [Required]
        public string Estado { get; set; } = string.Empty;

        [Required]
        public string CEP { get; set; } = string.Empty;

        public bool IsAtivo { get; set; }

        public DateTime DataCadastro { get; set; }
    }
}