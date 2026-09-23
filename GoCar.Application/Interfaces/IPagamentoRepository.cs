using GoCar.Domain.Entities;

namespace GoCar.Application.Interfaces
{
    public interface IPagamentoRepository
    {
        Task<IEnumerable<Pagamento>> ListarTodosAsync();

        Task<Pagamento?> ObterPorIdAsync(int id);

        // Pagamento ativo de uma locação,
        // separado pelo tipo:
        // Locação, Multa ou Adicional.
        Task<Pagamento?> ObterAtivoPorLocacaoAsync(
            int locacaoId,
            string tipo);

        // Entrada vinculada diretamente à reserva.
        Task<Pagamento?> ObterAtivoPorReservaAsync(
            int reservaId);

        Task<Pagamento> CriarAsync(
            Pagamento pagamento);

        Task<bool> AtualizarAsync(
            Pagamento pagamento);

        Task<bool> ExcluirAsync(
            int id);
    }
}