using System.ComponentModel.DataAnnotations;

namespace GoCar.Application.DTOs.Usuario
{
    public class CriarUsuarioDto
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(
            150,
            MinimumLength = 3,
            ErrorMessage = "O nome deve possuir entre 3 e 150 caracteres.")]
        public string Nome { get; set; }
            = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        [StringLength(
            150,
            ErrorMessage = "O e-mail deve possuir no máximo 150 caracteres.")]
        public string Email { get; set; }
            = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(
            100,
            MinimumLength = 6,
            ErrorMessage = "A senha deve possuir pelo menos 6 caracteres.")]
        public string Senha { get; set; }
            = string.Empty;

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [StringLength(
            14,
            MinimumLength = 11,
            ErrorMessage = "Informe um CPF válido.")]
        public string CPF { get; set; }
            = string.Empty;

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [StringLength(
            20,
            ErrorMessage = "O telefone deve possuir no máximo 20 caracteres.")]
        public string Telefone { get; set; }
            = string.Empty;

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "A CNH é obrigatória.")]
        [StringLength(
            20,
            ErrorMessage = "A CNH deve possuir no máximo 20 caracteres.")]
        public string CNH { get; set; }
            = string.Empty;

        [Required(ErrorMessage = "A categoria da CNH é obrigatória.")]
        [StringLength(
            5,
            ErrorMessage = "A categoria da CNH deve possuir no máximo 5 caracteres.")]
        public string CategoriaCNH { get; set; }
            = string.Empty;

        [Required(ErrorMessage = "A validade da CNH é obrigatória.")]
        public DateTime DataValidadeCNH { get; set; }

        [Required(ErrorMessage = "O endereço é obrigatório.")]
        [StringLength(
            200,
            ErrorMessage = "O endereço deve possuir no máximo 200 caracteres.")]
        public string Endereco { get; set; }
            = string.Empty;

        [Required(ErrorMessage = "O número é obrigatório.")]
        [StringLength(
            20,
            ErrorMessage = "O número deve possuir no máximo 20 caracteres.")]
        public string Numero { get; set; }
            = string.Empty;

        [Required(ErrorMessage = "O bairro é obrigatório.")]
        [StringLength(
            100,
            ErrorMessage = "O bairro deve possuir no máximo 100 caracteres.")]
        public string Bairro { get; set; }
            = string.Empty;

        [Required(ErrorMessage = "A cidade é obrigatória.")]
        [StringLength(
            100,
            ErrorMessage = "A cidade deve possuir no máximo 100 caracteres.")]
        public string Cidade { get; set; }
            = string.Empty;

        [Required(ErrorMessage = "O estado é obrigatório.")]
        [StringLength(
            2,
            MinimumLength = 2,
            ErrorMessage = "Informe a sigla do estado com 2 letras.")]
        public string Estado { get; set; }
            = string.Empty;

        [Required(ErrorMessage = "O CEP é obrigatório.")]
        [RegularExpression(
            @"^\d{5}-?\d{3}$",
            ErrorMessage = "Informe um CEP válido. Exemplo: 07123-456.")]
        public string CEP { get; set; }
            = string.Empty;
    }
}