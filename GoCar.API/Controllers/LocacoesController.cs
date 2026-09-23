using GoCar.Application.DTOs.Cliente;
using GoCar.Application.DTOs.Locacao;
using GoCar.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GoCar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LocacoesController : ControllerBase
    {
        private readonly ILocacaoService _locacaoService;
        private readonly IReservaService _reservaService;
        private readonly IClienteService _clienteService;

        public LocacoesController(
            ILocacaoService locacaoService,
            IReservaService reservaService,
            IClienteService clienteService)
        {
            _locacaoService = locacaoService;
            _reservaService = reservaService;
            _clienteService = clienteService;
        }

        // =====================================================
        // LISTAR TODAS
        // =====================================================

        [HttpGet]
        [Authorize(Roles = "Administrador,Gerente,Atendente")]
        public async Task<IActionResult> ListarTodos()
        {
            var locacoes =
                await _locacaoService
                    .ListarTodosAsync();

            return Ok(locacoes);
        }

        // =====================================================
        // MINHAS LOCAÇÕES
        // =====================================================

        [HttpGet("minhas")]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> ListarMinhas()
        {
            var cliente =
                await ObterClienteLogadoAsync();

            if (cliente == null)
            {
                return Unauthorized(new
                {
                    mensagem =
                        "Cliente não identificado para o usuário autenticado."
                });
            }

            var locacoes =
                await _locacaoService
                    .ListarTodosAsync();

            var minhasLocacoes =
                new List<LocacaoDto>();

            foreach (var locacao in locacoes)
            {
                var reserva =
                    await _reservaService
                        .ObterPorIdAsync(
                            locacao.ReservaId);

                if (reserva != null &&
                    reserva.ClienteId ==
                        cliente.Id)
                {
                    minhasLocacoes.Add(
                        locacao);
                }
            }

            return Ok(minhasLocacoes);
        }

        // =====================================================
        // OBTER POR ID
        // =====================================================

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(
            int id)
        {
            var locacao =
                await _locacaoService
                    .ObterPorIdAsync(id);

            if (locacao == null)
            {
                return NotFound(new
                {
                    mensagem =
                        "Locação não encontrada."
                });
            }

            if (UsuarioEhFuncionario())
            {
                return Ok(locacao);
            }

            var cliente =
                await ObterClienteLogadoAsync();

            if (cliente == null)
            {
                return Unauthorized(new
                {
                    mensagem =
                        "Cliente não identificado para o usuário autenticado."
                });
            }

            var reserva =
                await _reservaService
                    .ObterPorIdAsync(
                        locacao.ReservaId);

            if (reserva == null)
            {
                return NotFound(new
                {
                    mensagem =
                        "Reserva vinculada à locação não encontrada."
                });
            }

            if (reserva.ClienteId !=
                cliente.Id)
            {
                return Forbid();
            }

            return Ok(locacao);
        }

        // =====================================================
        // CRIAR / INICIAR LOCAÇÃO
        // =====================================================

        [HttpPost]
        [Authorize(Roles = "Administrador,Gerente,Atendente")]
        public async Task<IActionResult> Criar(
            [FromBody] CriarLocacaoDto dto)
        {
            try
            {
                var locacao =
                    await _locacaoService
                        .CriarAsync(dto);

                return CreatedAtAction(
                    nameof(ObterPorId),
                    new
                    {
                        id = locacao.Id
                    },
                    locacao);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensagem = ex.Message
                });
            }
        }

        // =====================================================
        // ATUALIZAR / FINALIZAR / CANCELAR
        // =====================================================

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Gerente,Atendente")]
        public async Task<IActionResult> Atualizar(
            int id,
            [FromBody] AtualizarLocacaoDto dto)
        {
            try
            {
                var atualizado =
                    await _locacaoService
                        .AtualizarAsync(
                            id,
                            dto);

                if (!atualizado)
                {
                    return NotFound(new
                    {
                        mensagem =
                            "Locação não encontrada."
                    });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensagem = ex.Message
                });
            }
        }

        // =====================================================
        // EXCLUIR / CANCELAR
        // =====================================================

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador,Gerente,Atendente")]
        public async Task<IActionResult> Excluir(
            int id)
        {
            try
            {
                var excluido =
                    await _locacaoService
                        .ExcluirAsync(id);

                if (!excluido)
                {
                    return NotFound(new
                    {
                        mensagem =
                            "Locação não encontrada."
                    });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensagem = ex.Message
                });
            }
        }

        // =====================================================
        // CLIENTE LOGADO
        // =====================================================

        private async Task<ClienteDto?>
            ObterClienteLogadoAsync()
        {
            var usuarioIdClaim =
                User.FindFirst("sub")?.Value
                ?? User.FindFirst(
                    ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(
                usuarioIdClaim,
                out var usuarioId))
            {
                return null;
            }

            return await _clienteService
                .ObterPorUsuarioIdAsync(
                    usuarioId);
        }

        // =====================================================
        // PERFIL FUNCIONÁRIO
        // =====================================================

        private bool UsuarioEhFuncionario()
        {
            return User.IsInRole(
                       "Administrador")
                || User.IsInRole(
                       "Gerente")
                || User.IsInRole(
                       "Atendente");
        }
    }
}