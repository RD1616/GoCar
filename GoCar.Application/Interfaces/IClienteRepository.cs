using GoCar.Domain.Entities;

namespace GoCar.Application.Interfaces
{
    public interface IClienteRepository
    {
        Task<IEnumerable<Cliente>> ListarTodosAsync();

        Task<Cliente?> ObterPorIdAsync(int id);

        Task<Cliente?> ObterPorUsuarioIdAsync(int usuarioId);

        Task<Cliente> CriarAsync(Cliente cliente);

        Task<bool> AtualizarAsync(Cliente cliente);

        Task<bool> ExcluirAsync(int id);
    }
}