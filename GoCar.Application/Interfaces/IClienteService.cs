using GoCar.Application.DTOs.Cliente;

namespace GoCar.Application.Interfaces
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteDto>> ListarTodosAsync();

        Task<ClienteDto?> ObterPorIdAsync(int id);

        Task<ClienteDto?> ObterPorUsuarioIdAsync(int usuarioId);

        Task<ClienteDto> CriarAsync(CriarClienteDto dto);

        Task<bool> AtualizarAsync(
            int id,
            AtualizarClienteDto dto);

        Task<bool> ExcluirAsync(int id);

        Task<bool> AtivarAsync(int id);
    }
}