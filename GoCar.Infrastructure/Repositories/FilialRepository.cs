using GoCar.Application.Interfaces;
using GoCar.Domain.Entities;
using GoCar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoCar.Infrastructure.Repositories
{
    public class FilialRepository : IFilialRepository
    {
        private readonly GoCarDbContext _context;

        public FilialRepository(GoCarDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Filial>> ListarTodosAsync()
        {
            return await _context.Filiais
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Filial?> ObterPorIdAsync(int id)
        {
            return await _context.Filiais
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Filial> CriarAsync(Filial filial)
        {
            _context.Filiais.Add(filial);

            await _context.SaveChangesAsync();

            return filial;
        }

        public async Task<bool> AtualizarAsync(Filial filial)
        {
            var filialExistente = await _context.Filiais
                .FirstOrDefaultAsync(f => f.Id == filial.Id);

            if (filialExistente == null)
                return false;

            filialExistente.Nome = filial.Nome;
            filialExistente.CNPJ = filial.CNPJ;
            filialExistente.Telefone = filial.Telefone;
            filialExistente.Email = filial.Email;
            filialExistente.Endereco = filial.Endereco;
            filialExistente.Numero = filial.Numero;
            filialExistente.Bairro = filial.Bairro;
            filialExistente.Cidade = filial.Cidade;
            filialExistente.Estado = filial.Estado;
            filialExistente.CEP = filial.CEP;
            filialExistente.IsAtivo = filial.IsAtivo;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExcluirAsync(int id)
        {
            var filial = await _context.Filiais
                .FirstOrDefaultAsync(f => f.Id == id);

            if (filial == null)
                return false;

            filial.IsAtivo = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}