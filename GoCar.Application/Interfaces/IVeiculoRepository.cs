using GoCar.Domain.Entities;

namespace GoCar.Application.Interfaces
{
    public interface IVeiculoRepository
    {
        Task<IEnumerable<Veiculo>> ListarTodosAsync();

        Task<Veiculo?> ObterPorIdAsync(int id);

        Task<Veiculo> CriarAsync(Veiculo veiculo);

        Task<bool> AtualizarAsync(Veiculo veiculo);

        Task<bool> ExcluirAsync(int id);
    }
}