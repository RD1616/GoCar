using System.ComponentModel.DataAnnotations;

namespace GoCar.Web.Models
{
    public class FilialViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da filial é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CNPJ é obrigatório.")]
        public string CNPJ { get; set; } = string.Empty;

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        public string Telefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O endereço é obrigatório.")]
        public string Endereco { get; set; } = string.Empty;

        [Required(ErrorMessage = "O número é obrigatório.")]
        public string Numero { get; set; } = string.Empty;

        [Required(ErrorMessage = "O bairro é obrigatório.")]
        public string Bairro { get; set; } = string.Empty;

        [Required(ErrorMessage = "A cidade é obrigatória.")]
        public string Cidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "O estado é obrigatório.")]
        public string Estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CEP é obrigatório.")]
        public string CEP { get; set; } = string.Empty;

        public bool IsAtivo { get; set; }
    }
}