using GoCar.Application.Interfaces;
using GoCar.Domain.Entities;
using GoCar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoCar.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly GoCarDbContext _context;

        public ClienteRepository(GoCarDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cliente>> ListarTodosAsync()
        {
            return await _context.Clientes
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Cliente?> ObterPorIdAsync(int id)
        {
            return await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cliente?> ObterPorUsuarioIdAsync(int usuarioId)
        {
            return await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);
        }

        public async Task<Cliente> CriarAsync(Cliente cliente)
        {
            _context.Clientes.Add(cliente);

            await _context.SaveChangesAsync();

            return cliente;
        }

        public async Task<bool> AtualizarAsync(Cliente cliente)
        {
            var clienteExistente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == cliente.Id);

            if (clienteExistente == null)
                return false;

            clienteExistente.Nome = cliente.Nome;
            clienteExistente.CPF = cliente.CPF;
            clienteExistente.DataNascimento = cliente.DataNascimento;
            clienteExistente.Telefone = cliente.Telefone;
            clienteExistente.CNH = cliente.CNH;
            clienteExistente.CategoriaCNH = cliente.CategoriaCNH;
            clienteExistente.DataValidadeCNH = cliente.DataValidadeCNH;
            clienteExistente.Endereco = cliente.Endereco;
            clienteExistente.Numero = cliente.Numero;
            clienteExistente.Bairro = cliente.Bairro;
            clienteExistente.Cidade = cliente.Cidade;
            clienteExistente.Estado = cliente.Estado;
            clienteExistente.CEP = cliente.CEP;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExcluirAsync(int id)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
                return false;

            cliente.IsAtivo = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}