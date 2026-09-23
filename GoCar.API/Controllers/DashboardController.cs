using GoCar.Domain.Enums;
using GoCar.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoCar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class DashboardController : ControllerBase
    {
        private readonly GoCarDbContext _context;

        public DashboardController(GoCarDbContext context)
        {
            _context = context;
        }

        [HttpGet("resumo")]
        public async Task<IActionResult> ObterResumo()
        {
            // ==========================================
            // TOTAL DE VEÍCULOS
            // ==========================================

            var totalVeiculos =
                await _context.Veiculos.CountAsync();


            // ==========================================
            // TOTAL DE CLIENTES
            // ==========================================

            var totalClientes =
                await _context.Clientes.CountAsync();


            // ==========================================
            // LOCAÇÕES ATIVAS
            // ==========================================

            var locacoesAtivas =
                await _context.Locacoes
                    .CountAsync(l => l.IsAtiva);


            // ==========================================
            // RECEITA DO MÊS
            // ==========================================

            var hoje = DateTime.Now;

            var inicioMes =
                new DateTime(
                    hoje.Year,
                    hoje.Month,
                    1
                );

            var inicioProximoMes =
                inicioMes.AddMonths(1);


            var receitaMes =
                await _context.Pagamentos
                    .Where(p =>
                        p.IsAtivo &&
                        p.Status == StatusPagamento.Pago &&
                        p.DataPagamento.HasValue &&
                        p.DataPagamento.Value >= inicioMes &&
                        p.DataPagamento.Value < inicioProximoMes
                    )
                    .SumAsync(p => (decimal?)p.Valor)
                    ?? 0m;


            // ==========================================
            // RETORNO
            // ==========================================

            return Ok(new
            {
                totalVeiculos,
                locacoesAtivas,
                totalClientes,
                receitaMes
            });
        }
    }
}