using GoCar.Application.Interfaces;
using GoCar.Domain.Entities;
using GoCar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoCar.Infrastructure.Repositories
{
    public class FilialRepository : IFilialRepository
    {
        private readonly GoCarDbContext _context;

        public FilialRepository(
            GoCarDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // LISTAR TODOS
        // =====================================================

        public async Task<IEnumerable<Filial>> ListarTodosAsync()
        {
            return await _context.Filiais
                .AsNoTracking()
                .ToListAsync();
        }

        // =====================================================
        // OBTER POR ID
        // =====================================================

        public async Task<Filial?> ObterPorIdAsync(
            int id)
        {
            return await _context.Filiais
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        // =====================================================
        // CRIAR
        // =====================================================

        public async Task<Filial> CriarAsync(
            Filial filial)
        {
            _context.Filiais.Add(filial);

            await _context.SaveChangesAsync();

            return filial;
        }

        // =====================================================
        // ATUALIZAR
        // =====================================================

        public async Task<bool> AtualizarAsync(
            Filial filial)
        {
            var existe = await _context.Filiais
                .AnyAsync(f => f.Id == filial.Id);

            if (!existe)
                return false;

            await _context.SaveChangesAsync();

            return true;
        }

        // =====================================================
        // DESATIVAR
        // =====================================================

        public async Task<bool> ExcluirAsync(
            int id)
        {
            var filial = await _context.Filiais
                .FirstOrDefaultAsync(f => f.Id == id);

            if (filial == null)
                return false;

            filial.IsAtivo = false;

            await _context.SaveChangesAsync();

            return true;
        }

        // =====================================================
        // REATIVAR
        // =====================================================

        public async Task<bool> AtivarAsync(
            int id)
        {
            var filial = await _context.Filiais
                .FirstOrDefaultAsync(f => f.Id == id);

            if (filial == null)
                return false;

            filial.IsAtivo = true;

            await _context.SaveChangesAsync();

            return true;
        }

        // =====================================================
        // EXCLUIR PERMANENTEMENTE
        // =====================================================

        public async Task<bool> ExcluirPermanentementeAsync(
            int id)
        {
            var filial = await _context.Filiais
                .FirstOrDefaultAsync(f => f.Id == id);

            if (filial == null)
                return false;

            _context.Filiais.Remove(filial);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}