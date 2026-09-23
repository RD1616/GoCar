using GoCar.Application.Interfaces;
using GoCar.Domain.Entities;
using GoCar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoCar.Infrastructure.Repositories
{
    public class LocacaoRepository : ILocacaoRepository
    {
        private readonly GoCarDbContext _context;

        public LocacaoRepository(
            GoCarDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // LISTAR TODOS
        // =====================================================

        public async Task<IEnumerable<Locacao>> ListarTodosAsync()
        {
            return await _context.Locacoes
                .AsNoTracking()
                .ToListAsync();
        }

        // =====================================================
        // OBTER POR ID
        // =====================================================

        public async Task<Locacao?> ObterPorIdAsync(
            int id)
        {
            return await _context.Locacoes
                .FirstOrDefaultAsync(
                    l => l.Id == id);
        }

        // =====================================================
        // OBTER POR RESERVA
        // =====================================================

        public async Task<Locacao?> ObterPorReservaIdAsync(
            int reservaId)
        {
            return await _context.Locacoes
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    l => l.ReservaId == reservaId);
        }

        // =====================================================
        // CRIAR
        // =====================================================

        public async Task<Locacao> CriarAsync(
            Locacao locacao)
        {
            _context.Locacoes.Add(locacao);

            await _context.SaveChangesAsync();

            return locacao;
        }

        // =====================================================
        // ATUALIZAR
        // =====================================================

        public async Task<bool> AtualizarAsync(
            Locacao locacao)
        {
            _context.Locacoes.Update(locacao);

            var resultado =
                await _context.SaveChangesAsync();

            return resultado > 0;
        }

        // =====================================================
        // EXCLUIR
        // =====================================================

        public async Task<bool> ExcluirAsync(
            int id)
        {
            var locacao =
                await _context.Locacoes
                    .FirstOrDefaultAsync(
                        l => l.Id == id);

            if (locacao == null)
                return false;

            locacao.IsAtiva = false;

            var resultado =
                await _context.SaveChangesAsync();

            return resultado > 0;
        }
    }
}