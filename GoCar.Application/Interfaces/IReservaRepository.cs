using GoCar.Domain.Entities;

namespace GoCar.Application.Interfaces
{
    public interface IReservaRepository
    {
        Task<IEnumerable<Reserva>> ListarTodosAsync();

        Task<IEnumerable<Reserva>> ListarPorClienteIdAsync(
            int clienteId);

        Task<Reserva?> ObterPorIdAsync(
            int id);

        Task<Reserva> CriarAsync(
            Reserva reserva);

        Task<bool> AtualizarAsync(
            Reserva reserva);

        Task<bool> ExcluirAsync(
            int id);

        Task<bool> ExisteConflitoAsync(
            int veiculoId,
            DateTime dataRetirada,
            DateTime dataDevolucao,
            int? reservaId = null);

        Task<bool> ExisteOutraReservaConfirmadaAsync(
            int veiculoId,
            int reservaIgnorarId);
    }
}