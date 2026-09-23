using GoCar.Domain.Entities;

namespace GoCar.Application.Interfaces
{
    public interface ILocacaoRepository
    {
        // =====================================================
        // LISTAR TODOS
        // =====================================================

        Task<IEnumerable<Locacao>> ListarTodosAsync();

        // =====================================================
        // OBTER POR ID
        // =====================================================

        Task<Locacao?> ObterPorIdAsync(
            int id);

        // =====================================================
        // OBTER POR RESERVA
        // =====================================================

        Task<Locacao?> ObterPorReservaIdAsync(
            int reservaId);

        // =====================================================
        // CRIAR
        // =====================================================

        Task<Locacao> CriarAsync(
            Locacao locacao);

        // =====================================================
        // ATUALIZAR
        // =====================================================

        Task<bool> AtualizarAsync(
            Locacao locacao);

        // =====================================================
        // EXCLUIR
        // =====================================================

        Task<bool> ExcluirAsync(
            int id);
    }
}