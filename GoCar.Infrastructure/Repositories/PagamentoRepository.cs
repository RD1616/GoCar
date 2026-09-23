using GoCar.Application.Interfaces;
using GoCar.Domain.Entities;
using GoCar.Domain.Enums;
using GoCar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoCar.Infrastructure.Repositories
{
    public class PagamentoRepository : IPagamentoRepository
    {
        private readonly GoCarDbContext _context;

        public PagamentoRepository(
            GoCarDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // LISTAR TODOS
        // =====================================================

        public async Task<IEnumerable<Pagamento>> ListarTodosAsync()
        {
            return await _context.Pagamentos
                .AsNoTracking()
                .ToListAsync();
        }

        // =====================================================
        // OBTER POR ID
        // =====================================================

        public async Task<Pagamento?> ObterPorIdAsync(int id)
        {
            return await _context.Pagamentos
                .FirstOrDefaultAsync(
                    p => p.Id == id);
        }

        // =====================================================
        // OBTER PAGAMENTO ATIVO POR LOCAÇÃO E TIPO
        // =====================================================
        //
        // Permite que a mesma locação tenha pagamentos
        // diferentes:
        //
        // - Locação
        // - Multa
        // - Adicional
        //
        // Mas impede dois pagamentos ativos do mesmo tipo.
        // =====================================================

        public async Task<Pagamento?> ObterAtivoPorLocacaoAsync(
            int locacaoId,
            string tipo)
        {
            return await _context.Pagamentos
                .AsNoTracking()
                .FirstOrDefaultAsync(p =>
                    p.LocacaoId == locacaoId &&
                    p.Tipo == tipo &&
                    p.IsAtivo &&
                    (
                        p.Status == StatusPagamento.Pendente ||
                        p.Status == StatusPagamento.Pago
                    ));
        }

        // =====================================================
        // OBTER PAGAMENTO ATIVO POR RESERVA
        // =====================================================
        //
        // Utilizado para localizar a entrada de 30%
        // vinculada diretamente à reserva.
        // =====================================================

        public async Task<Pagamento?> ObterAtivoPorReservaAsync(
            int reservaId)
        {
            return await _context.Pagamentos
                .AsNoTracking()
                .FirstOrDefaultAsync(p =>
                    p.ReservaId == reservaId &&
                    p.IsAtivo &&
                    (
                        p.Status == StatusPagamento.Pendente ||
                        p.Status == StatusPagamento.Pago
                    ));
        }

        // =====================================================
        // CRIAR
        // =====================================================

        public async Task<Pagamento> CriarAsync(
            Pagamento pagamento)
        {
            _context.Pagamentos.Add(
                pagamento);

            await _context.SaveChangesAsync();

            return pagamento;
        }

        // =====================================================
        // ATUALIZAR
        // =====================================================

        public async Task<bool> AtualizarAsync(
            Pagamento pagamento)
        {
            var pagamentoExistente =
                await _context.Pagamentos
                    .FirstOrDefaultAsync(
                        p => p.Id == pagamento.Id);

            if (pagamentoExistente == null)
                return false;

            pagamentoExistente.LocacaoId =
                pagamento.LocacaoId;

            pagamentoExistente.ReservaId =
                pagamento.ReservaId;

            pagamentoExistente.Tipo =
                pagamento.Tipo;

            pagamentoExistente.FormaPagamento =
                pagamento.FormaPagamento;

            pagamentoExistente.Status =
                pagamento.Status;

            pagamentoExistente.Valor =
                pagamento.Valor;

            pagamentoExistente.DataPagamento =
                pagamento.DataPagamento;

            pagamentoExistente.Observacoes =
                pagamento.Observacoes;

            pagamentoExistente.IsAtivo =
                pagamento.IsAtivo;

            await _context.SaveChangesAsync();

            return true;
        }

        // =====================================================
        // EXCLUIR
        // =====================================================

        public async Task<bool> ExcluirAsync(
            int id)
        {
            var pagamento =
                await _context.Pagamentos
                    .FirstOrDefaultAsync(
                        p => p.Id == id);

            if (pagamento == null)
                return false;

            pagamento.IsAtivo = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}