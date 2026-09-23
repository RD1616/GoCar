using GoCar.Domain.Entities;

namespace GoCar.Application.Interfaces
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> ListarTodosAsync();

        Task<Categoria?> ObterPorIdAsync(int id);

        Task<Categoria> CriarAsync(Categoria categoria);

        Task<bool> AtualizarAsync(Categoria categoria);

        Task<bool> ExcluirAsync(int id);
    }
}