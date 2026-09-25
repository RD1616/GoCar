using GoCar.Application.Interfaces;
using GoCar.Domain.Entities;
using GoCar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoCar.Infrastructure.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly GoCarDbContext _context;

        public CategoriaRepository(GoCarDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // LISTAR TODOS
        // =====================================================

        public async Task<IEnumerable<Categoria>> ListarTodosAsync()
        {
            return await _context.Categorias
                .AsNoTracking()
                .ToListAsync();
        }

        // =====================================================
        // OBTER POR ID
        // =====================================================

        public async Task<Categoria?> ObterPorIdAsync(int id)
        {
            return await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // =====================================================
        // CRIAR
        // =====================================================

        public async Task<Categoria> CriarAsync(
            Categoria categoria)
        {
            _context.Categorias.Add(categoria);

            await _context.SaveChangesAsync();

            return categoria;
        }

        // =====================================================
        // ATUALIZAR
        // =====================================================

        public async Task<bool> AtualizarAsync(
            Categoria categoria)
        {
            var existe = await _context.Categorias
                .AnyAsync(c => c.Id == categoria.Id);

            if (!existe)
                return false;

            // A entidade já está sendo rastreada pelo DbContext
            // porque foi obtida anteriormente por ObterPorIdAsync.
            await _context.SaveChangesAsync();

            return true;
        }

        // =====================================================
        // DESATIVAR
        // =====================================================

        public async Task<bool> ExcluirAsync(int id)
        {
            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
                return false;

            categoria.IsAtivo = false;

            await _context.SaveChangesAsync();

            return true;
        }

        // =====================================================
        // REATIVAR
        // =====================================================

        public async Task<bool> AtivarAsync(int id)
        {
            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
                return false;

            categoria.IsAtivo = true;

            await _context.SaveChangesAsync();

            return true;
        }

        // =====================================================
        // EXCLUIR PERMANENTEMENTE
        // =====================================================

        public async Task<bool> ExcluirPermanentementeAsync(
            int id)
        {
            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
                return false;

            _context.Categorias.Remove(categoria);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}