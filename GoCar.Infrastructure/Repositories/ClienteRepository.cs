using GoCar.Application.Interfaces;
using GoCar.Domain.Entities;
using GoCar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoCar.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly GoCarDbContext _context;

        public ClienteRepository(
            GoCarDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // LISTAR TODOS
        // =====================================================

        public async Task<IEnumerable<Cliente>>
            ListarTodosAsync()
        {
            return await _context.Clientes
                .AsNoTracking()
                .ToListAsync();
        }

        // =====================================================
        // OBTER POR ID
        // =====================================================

        public async Task<Cliente?> ObterPorIdAsync(
            int id)
        {
            return await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // =====================================================
        // OBTER POR USUÁRIO
        // =====================================================

        public async Task<Cliente?> ObterPorUsuarioIdAsync(
            int usuarioId)
        {
            return await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    c => c.UsuarioId == usuarioId);
        }

        // =====================================================
        // CRIAR
        // =====================================================

        public async Task<Cliente> CriarAsync(
            Cliente cliente)
        {
            _context.Clientes.Add(cliente);

            await _context.SaveChangesAsync();

            return cliente;
        }

        // =====================================================
        // ATUALIZAR
        // =====================================================

        public async Task<bool> AtualizarAsync(
            Cliente cliente)
        {
            var existe = await _context.Clientes
                .AnyAsync(c => c.Id == cliente.Id);

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
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
                return false;

            cliente.IsAtivo = false;

            await _context.SaveChangesAsync();

            return true;
        }

        // =====================================================
        // EXCLUIR PERMANENTEMENTE
        // =====================================================

        public async Task<bool>
            ExcluirPermanentementeAsync(int id)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
                return false;

            _context.Clientes.Remove(cliente);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}