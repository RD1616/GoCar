using GoCar.Application.DTOs.Locacao;

namespace GoCar.Application.Interfaces
{
    public interface ILocacaoService
    {
        Task<IEnumerable<LocacaoDto>> ListarTodosAsync();

        Task<LocacaoDto?> ObterPorIdAsync(int id);

        Task<LocacaoDto> CriarAsync(CriarLocacaoDto dto);

        Task<bool> AtualizarAsync(int id, AtualizarLocacaoDto dto);

        Task<bool> ExcluirAsync(int id);
    }
}