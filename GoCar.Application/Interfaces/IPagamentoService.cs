using GoCar.Application.DTOs.Pagamento;
using GoCar.Domain.Enums;

namespace GoCar.Application.Interfaces
{
    public interface IPagamentoService
    {
        Task<IEnumerable<PagamentoDto>> ListarTodosAsync();

        Task<PagamentoDto?> ObterPorIdAsync(int id);

        // Pagamentos relacionados à locação
        Task<PagamentoDto> CriarAsync(
            CriarPagamentoDto dto);

        // Entrada obrigatória de 30% da reserva.
        // O valor é calculado pelo backend.
        Task<PagamentoDto> CriarEntradaAsync(
            int reservaId,
            FormaPagamento formaPagamento);

        Task<bool> AtualizarAsync(
            int id,
            CriarPagamentoDto dto);

        Task<bool> ExcluirAsync(int id);
    }
}