using GoCar.Domain.Entities;

namespace GoCar.Application.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorIdAsync(
            int id);

        Task<Usuario?> ObterPorEmailAsync(
            string email);

        Task<Usuario?> ObterPorCpfAsync(
            string cpf);

        Task<Usuario> CriarAsync(
            Usuario usuario);

        Task<bool> AtualizarAsync(
            Usuario usuario);

        // Cadastro atômico:
        // Usuário + Cliente são gravados na mesma transação.
        Task<(Usuario Usuario, Cliente Cliente)> CriarUsuarioComClienteAsync(
            Usuario usuario,
            Cliente cliente);
    }
}