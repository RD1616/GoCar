using GoCar.Application.Interfaces;
using GoCar.Domain.Entities;
using GoCar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoCar.Infrastructure.Repositories
{
    public class VeiculoRepository : IVeiculoRepository
    {
        private readonly GoCarDbContext _context;

        public VeiculoRepository(
            GoCarDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // LISTAR TODOS
        // =====================================================

        public async Task<IEnumerable<Veiculo>>
            ListarTodosAsync()
        {
            return await _context.Veiculos
                .AsNoTracking()
                .ToListAsync();
        }

        // =====================================================
        // OBTER POR ID
        // =====================================================

        public async Task<Veiculo?> ObterPorIdAsync(
            int id)
        {
            return await _context.Veiculos
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        // =====================================================
        // CRIAR
        // =====================================================

        public async Task<Veiculo> CriarAsync(
            Veiculo veiculo)
        {
            _context.Veiculos.Add(veiculo);

            await _context.SaveChangesAsync();

            return veiculo;
        }

        // =====================================================
        // ATUALIZAR
        // =====================================================

        public async Task<bool> AtualizarAsync(
            Veiculo veiculo)
        {
            var existe = await _context.Veiculos
                .AnyAsync(v => v.Id == veiculo.Id);

            if (!existe)
                return false;

            // A entidade já foi obtida e alterada pelo Service.
            // Como está sendo rastreada pelo mesmo DbContext,
            // basta persistir as alterações.
            await _context.SaveChangesAsync();

            return true;
        }

        // =====================================================
        // DESATIVAR
        // =====================================================

        public async Task<bool> ExcluirAsync(
            int id)
        {
            var veiculo = await _context.Veiculos
                .FirstOrDefaultAsync(v => v.Id == id);

            if (veiculo == null)
                return false;

            veiculo.IsAtivo = false;

            await _context.SaveChangesAsync();

            return true;
        }

        // =====================================================
        // EXCLUIR PERMANENTEMENTE
        // =====================================================

        public async Task<bool>
            ExcluirPermanentementeAsync(int id)
        {
            var veiculo = await _context.Veiculos
                .FirstOrDefaultAsync(v => v.Id == id);

            if (veiculo == null)
                return false;

            _context.Veiculos.Remove(veiculo);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}