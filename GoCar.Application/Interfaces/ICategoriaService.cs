using GoCar.Application.DTOs.Categoria;

namespace GoCar.Application.Interfaces
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaDto>> ListarTodosAsync();

        Task<CategoriaDto?> ObterPorIdAsync(int id);

        Task<CategoriaDto> CriarAsync(CriarCategoriaDto dto);

        Task<bool> AtualizarAsync(
            int id,
            AtualizarCategoriaDto dto);

        Task<bool> ExcluirAsync(int id);
    }
}