using GoCar.Domain.Entities;

namespace GoCar.Application.Interfaces
{
    public interface IFilialRepository
    {
        Task<IEnumerable<Filial>> ListarTodosAsync();

        Task<Filial?> ObterPorIdAsync(int id);

        Task<Filial> CriarAsync(Filial filial);

        Task<bool> AtualizarAsync(Filial filial);

        Task<bool> ExcluirAsync(int id);
    }
}