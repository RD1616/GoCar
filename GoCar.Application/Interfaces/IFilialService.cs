using GoCar.Application.DTOs.Filial;

namespace GoCar.Application.Interfaces
{
    public interface IFilialService
    {
        Task<IEnumerable<FilialDto>> ListarTodosAsync();

        Task<FilialDto?> ObterPorIdAsync(int id);

        Task<FilialDto> CriarAsync(CriarFilialDto dto);

        Task<bool> AtualizarAsync(int id, AtualizarFilialDto dto);

        Task<bool> ExcluirAsync(int id);
    }
}