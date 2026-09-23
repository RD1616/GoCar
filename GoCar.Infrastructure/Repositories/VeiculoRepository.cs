using GoCar.Application.Interfaces;
using GoCar.Domain.Entities;
using GoCar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoCar.Infrastructure.Repositories
{
    public class VeiculoRepository : IVeiculoRepository
    {
        private readonly GoCarDbContext _context;

        public VeiculoRepository(GoCarDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Veiculo>> ListarTodosAsync()
        {
            return await _context.Veiculos
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Veiculo?> ObterPorIdAsync(int id)
        {
            return await _context.Veiculos
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Veiculo> CriarAsync(Veiculo veiculo)
        {
            _context.Veiculos.Add(veiculo);

            await _context.SaveChangesAsync();

            return veiculo;
        }

        public async Task<bool> AtualizarAsync(Veiculo veiculo)
        {
            var veiculoExistente = await _context.Veiculos
                .FirstOrDefaultAsync(v => v.Id == veiculo.Id);

            if (veiculoExistente == null)
                return false;

            veiculoExistente.Placa = veiculo.Placa;
            veiculoExistente.Chassi = veiculo.Chassi;
            veiculoExistente.Renavam = veiculo.Renavam;
            veiculoExistente.Modelo = veiculo.Modelo;
            veiculoExistente.Marca = veiculo.Marca;
            veiculoExistente.AnoFabricacao = veiculo.AnoFabricacao;
            veiculoExistente.AnoModelo = veiculo.AnoModelo;
            veiculoExistente.Cor = veiculo.Cor;
            veiculoExistente.Combustivel = veiculo.Combustivel;
            veiculoExistente.Cambio = veiculo.Cambio;
            veiculoExistente.Status = veiculo.Status;
            veiculoExistente.KmAtual = veiculo.KmAtual;
            veiculoExistente.ValorDiaria = veiculo.ValorDiaria;
            veiculoExistente.CategoriaId = veiculo.CategoriaId;
            veiculoExistente.FilialId = veiculo.FilialId;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExcluirAsync(int id)
        {
            var veiculo = await _context.Veiculos
                .FirstOrDefaultAsync(v => v.Id == id);

            if (veiculo == null)
                return false;

            veiculo.IsAtivo = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}