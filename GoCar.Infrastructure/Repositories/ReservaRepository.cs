using GoCar.Application.Interfaces;
using GoCar.Domain.Entities;
using GoCar.Domain.Enums;
using GoCar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoCar.Infrastructure.Repositories
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly GoCarDbContext _context;

        public ReservaRepository(
            GoCarDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reserva>> ListarTodosAsync()
        {
            return await _context.Reservas
                .AsNoTracking()
                .Include(r => r.Cliente)
                .Include(r => r.Veiculo)
                .Include(r => r.FilialRetirada)
                .Include(r => r.FilialDevolucao)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reserva>> ListarPorClienteIdAsync(
            int clienteId)
        {
            return await _context.Reservas
                .AsNoTracking()
                .Include(r => r.Cliente)
                .Include(r => r.Veiculo)
                .Include(r => r.FilialRetirada)
                .Include(r => r.FilialDevolucao)
                .Where(r => r.ClienteId == clienteId)
                .ToListAsync();
        }

        public async Task<Reserva?> ObterPorIdAsync(
            int id)
        {
            return await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Veiculo)
                .Include(r => r.FilialRetirada)
                .Include(r => r.FilialDevolucao)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Reserva> CriarAsync(
            Reserva reserva)
        {
            _context.Reservas.Add(reserva);

            await _context.SaveChangesAsync();

            return reserva;
        }

        public async Task<bool> AtualizarAsync(
            Reserva reserva)
        {
            var reservaExistente =
                await _context.Reservas
                    .FirstOrDefaultAsync(
                        r => r.Id == reserva.Id);

            if (reservaExistente == null)
                return false;

            reservaExistente.ClienteId =
                reserva.ClienteId;

            reservaExistente.VeiculoId =
                reserva.VeiculoId;

            reservaExistente.FilialRetiradaId =
                reserva.FilialRetiradaId;

            reservaExistente.FilialDevolucaoId =
                reserva.FilialDevolucaoId;

            reservaExistente.DataReserva =
                reserva.DataReserva;

            reservaExistente.DataRetirada =
                reserva.DataRetirada;

            reservaExistente.DataDevolucaoPrevista =
                reserva.DataDevolucaoPrevista;

            reservaExistente.Status =
                reserva.Status;

            reservaExistente.ValorTotalPrevisto =
                reserva.ValorTotalPrevisto;

            reservaExistente.Observacoes =
                reserva.Observacoes;

            reservaExistente.IsAtiva =
                reserva.IsAtiva;

            reservaExistente.DataCancelamento =
                reserva.DataCancelamento;

            await _context.SaveChangesAsync();

            return true;
        }

        // =========================================
        // VERIFICAR CONFLITO DE PERÍODO
        // =========================================

        public async Task<bool> ExisteConflitoAsync(
            int veiculoId,
            DateTime dataRetirada,
            DateTime dataDevolucao,
            int? reservaId = null)
        {
            return await _context.Reservas
                .AsNoTracking()
                .AnyAsync(r =>
                    r.VeiculoId == veiculoId &&

                    // Somente reservas realmente ativas
                    // podem bloquear o período.
                    r.IsAtiva &&

                    // Somente reserva confirmada bloqueia
                    // fisicamente a unidade.
                    r.Status ==
                        StatusReserva.Confirmada &&

                    // Ignora a própria reserva durante edição.
                    (!reservaId.HasValue ||
                     r.Id != reservaId.Value) &&

                    // Interseção real entre os períodos.
                    dataRetirada <
                        r.DataDevolucaoPrevista &&

                    dataDevolucao >
                        r.DataRetirada);
        }

        // =========================================
        // VERIFICAR OUTRA RESERVA CONFIRMADA
        // =========================================

        public async Task<bool>
            ExisteOutraReservaConfirmadaAsync(
                int veiculoId,
                int reservaIgnorarId)
        {
            return await _context.Reservas
                .AsNoTracking()
                .AnyAsync(r =>
                    r.VeiculoId == veiculoId &&

                    // Não considerar a reserva que
                    // acabou de ser cancelada/concluída.
                    r.Id != reservaIgnorarId &&

                    // Precisa continuar ativa.
                    r.IsAtiva &&

                    // Precisa estar efetivamente confirmada.
                    r.Status ==
                        StatusReserva.Confirmada);
        }

        public async Task<bool> ExcluirAsync(
            int id)
        {
            var reserva =
                await _context.Reservas
                    .FirstOrDefaultAsync(
                        r => r.Id == id);

            if (reserva == null)
                return false;

            reserva.IsAtiva =
                false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}