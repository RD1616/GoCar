using GoCar.Application.DTOs.Veiculo;

namespace GoCar.Application.Interfaces
{
    public interface IVeiculoService
    {
        Task<IEnumerable<VeiculoDto>> ListarTodosAsync();

        Task<VeiculoDto?> ObterPorIdAsync(int id);

        Task<VeiculoDto> CriarAsync(
            CriarVeiculoDto dto);

        Task<bool> AtualizarAsync(
            int id,
            AtualizarVeiculoDto dto);

        Task<bool> ExcluirAsync(int id);

        Task<bool> AtivarAsync(int id);
    }
}