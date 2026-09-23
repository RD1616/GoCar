using System.ComponentModel.DataAnnotations;

namespace GoCar.Web.Models
{
    public class CadastroViewModel
    {
        // =========================================
        // DADOS DA CONTA
        // =========================================

        [Required(ErrorMessage = "Informe seu nome.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe seu e-mail.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe sua senha.")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirme sua senha.")]
        [DataType(DataType.Password)]
        [Compare(
            nameof(Senha),
            ErrorMessage = "As senhas não conferem.")]
        public string ConfirmarSenha { get; set; } = string.Empty;

        // =========================================
        // DADOS PESSOAIS
        // =========================================

        [Required(ErrorMessage = "Informe o CPF.")]
        public string CPF { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a data de nascimento.")]
        [DataType(DataType.Date)]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "Informe o telefone.")]
        public string Telefone { get; set; } = string.Empty;

        // =========================================
        // CNH
        // =========================================

        [Required(ErrorMessage = "Informe a CNH.")]
        public string CNH { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a categoria da CNH.")]
        public string CategoriaCNH { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a validade da CNH.")]
        [DataType(DataType.Date)]
        public DateTime DataValidadeCNH { get; set; }

        // =========================================
        // ENDEREÇO
        // =========================================

        [Required(ErrorMessage = "Informe o endereço.")]
        public string Endereco { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o número.")]
        public string Numero { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o bairro.")]
        public string Bairro { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a cidade.")]
        public string Cidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o estado.")]
        public string Estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o CEP.")]
        public string CEP { get; set; } = string.Empty;

        // =========================================
        // MENSAGEM
        // =========================================

        public string? MensagemErro { get; set; }

        public string? MensagemSucesso { get; set; }
    }
}