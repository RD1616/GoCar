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

        public async Task<IEnumerable<Categoria>> ListarTodosAsync()
        {
            return await _context.Categorias
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Categoria?> ObterPorIdAsync(int id)
        {
            return await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Categoria> CriarAsync(Categoria categoria)
        {
            _context.Categorias.Add(categoria);

            await _context.SaveChangesAsync();

            return categoria;
        }

        public async Task<bool> AtualizarAsync(Categoria categoria)
        {
            var categoriaExistente = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == categoria.Id);

            if (categoriaExistente == null)
                return false;

            categoriaExistente.Nome = categoria.Nome;
            categoriaExistente.Descricao = categoria.Descricao;
            categoriaExistente.DiariaBase = categoria.DiariaBase;
            categoriaExistente.KmLivre = categoria.KmLivre;
            categoriaExistente.IsAtivo = categoria.IsAtivo;

            await _context.SaveChangesAsync();

            return true;
        }

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
    }
}