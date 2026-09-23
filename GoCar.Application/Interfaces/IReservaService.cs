using GoCar.Application.DTOs.Reserva;

namespace GoCar.Application.Interfaces
{
    public interface IReservaService
    {
        Task<IEnumerable<ReservaDto>> ListarTodosAsync();

        Task<IEnumerable<ReservaDto>> ListarPorClienteIdAsync(
            int clienteId);

        Task<ReservaDto?> ObterPorIdAsync(int id);

        Task<ReservaDto> CriarAsync(
            CriarReservaDto dto);

        Task<bool> AtualizarAsync(
            int id,
            CriarReservaDto dto);

        Task<bool> ExcluirAsync(int id);

        Task<bool> ConfirmarAsync(int id);

        Task<bool> CancelarAsync(int id);

        Task<bool> ConcluirAsync(int id);

        // =====================================================
        // EXPIRAR RESERVAS VENCIDAS
        // =====================================================

        Task<int> ExpirarReservasVencidasAsync();

        Task<bool> VerificarDisponibilidadeAsync(
            int veiculoId,
            int filialRetiradaId,
            DateTime dataRetirada,
            DateTime dataDevolucaoPrevista);

        Task<int> ObterQuantidadeDisponivelAsync(
            int veiculoId,
            int filialRetiradaId,
            DateTime dataRetirada,
            DateTime dataDevolucaoPrevista);
    }
}