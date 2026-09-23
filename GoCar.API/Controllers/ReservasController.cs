using GoCar.Application.DTOs.Cliente;
using GoCar.Application.DTOs.Reserva;
using GoCar.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GoCar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReservasController : ControllerBase
    {
        private readonly IReservaService _reservaService;
        private readonly IClienteService _clienteService;

        public ReservasController(
            IReservaService reservaService,
            IClienteService clienteService)
        {
            _reservaService = reservaService;
            _clienteService = clienteService;
        }

        [HttpGet]
        [Authorize(
            Roles = "Administrador,Gerente,Atendente")]
        public async Task<ActionResult<IEnumerable<ReservaDto>>>
            ListarTodos()
        {
            var reservas =
                await _reservaService.ListarTodosAsync();

            return Ok(reservas);
        }

        // =====================================================
        // VERIFICAR DISPONIBILIDADE
        // =====================================================

        [HttpGet("disponibilidade")]
        [Authorize(
            Roles = "Cliente,Administrador,Gerente,Atendente")]
        public async Task<IActionResult>
            VerificarDisponibilidade(
                [FromQuery] int veiculoId,
                [FromQuery] int filialRetiradaId,
                [FromQuery] DateTime dataRetirada,
                [FromQuery] DateTime dataDevolucaoPrevista)
        {
            if (veiculoId <= 0)
            {
                return BadRequest(new
                {
                    mensagem =
                        "Informe um veículo válido."
                });
            }

            if (filialRetiradaId <= 0)
            {
                return BadRequest(new
                {
                    mensagem =
                        "Informe uma filial de retirada válida."
                });
            }

            if (dataDevolucaoPrevista <=
                dataRetirada)
            {
                return BadRequest(new
                {
                    mensagem =
                        "A data de devolução deve ser posterior à data de retirada."
                });
            }

            var quantidadeDisponivel =
                await _reservaService
                    .ObterQuantidadeDisponivelAsync(
                        veiculoId,
                        filialRetiradaId,
                        dataRetirada,
                        dataDevolucaoPrevista);

            return Ok(new
            {
                disponivel =
                    quantidadeDisponivel > 0,

                quantidadeDisponivel
            });
        }

        [HttpGet("minhas")]
        [Authorize(Roles = "Cliente")]
        public async Task<ActionResult<IEnumerable<ReservaDto>>>
            ListarMinhas()
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

            var reservas =
                await _reservaService
                    .ListarPorClienteIdAsync(
                        cliente.Id);

            return Ok(reservas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReservaDto>>
            ObterPorId(int id)
        {
            var reserva =
                await _reservaService
                    .ObterPorIdAsync(id);

            if (reserva == null)
            {
                return NotFound(new
                {
                    mensagem =
                        "Reserva não encontrada."
                });
            }

            if (UsuarioEhFuncionario())
            {
                return Ok(reserva);
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

            if (reserva.ClienteId != cliente.Id)
            {
                return Forbid();
            }

            return Ok(reserva);
        }

        [HttpPost]
        [Authorize(
            Roles = "Cliente,Administrador,Gerente,Atendente")]
        public async Task<ActionResult<ReservaDto>>
            Criar(
                [FromBody] CriarReservaDto dto)
        {
            if (UsuarioEhFuncionario())
            {
                if (dto.ClienteId <= 0)
                {
                    return BadRequest(new
                    {
                        mensagem =
                            "Informe o cliente da reserva."
                    });
                }
            }
            else
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

                dto.ClienteId =
                    cliente.Id;
            }

            try
            {
                var reserva =
                    await _reservaService
                        .CriarAsync(dto);

                return CreatedAtAction(
                    nameof(ObterPorId),
                    new { id = reserva.Id },
                    reserva);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensagem = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Atualizar(
            int id,
            [FromBody] CriarReservaDto dto)
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

            var reservaExistente =
                await _reservaService
                    .ObterPorIdAsync(id);

            if (reservaExistente == null)
            {
                return NotFound(new
                {
                    mensagem =
                        "Reserva não encontrada."
                });
            }

            if (reservaExistente.ClienteId !=
                cliente.Id)
            {
                return Forbid();
            }

            dto.ClienteId =
                cliente.Id;

            try
            {
                var atualizado =
                    await _reservaService
                        .AtualizarAsync(
                            id,
                            dto);

                if (!atualizado)
                {
                    return NotFound(new
                    {
                        mensagem =
                            "Reserva não encontrada."
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Excluir(
            int id)
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

            var reservaExistente =
                await _reservaService
                    .ObterPorIdAsync(id);

            if (reservaExistente == null)
            {
                return NotFound(new
                {
                    mensagem =
                        "Reserva não encontrada."
                });
            }

            if (reservaExistente.ClienteId !=
                cliente.Id)
            {
                return Forbid();
            }

            try
            {
                var cancelada =
                    await _reservaService
                        .CancelarAsync(id);

                if (!cancelada)
                {
                    return NotFound(new
                    {
                        mensagem =
                            "Reserva não encontrada."
                    });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    mensagem = ex.Message
                });
            }
        }

        [HttpPut("{id}/confirmar")]
        [Authorize(
            Roles = "Administrador,Gerente,Atendente")]
        public async Task<IActionResult> Confirmar(
            int id)
        {
            try
            {
                var confirmado =
                    await _reservaService
                        .ConfirmarAsync(id);

                if (!confirmado)
                {
                    return NotFound(new
                    {
                        mensagem =
                            "Reserva não encontrada."
                    });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    mensagem = ex.Message
                });
            }
        }

        [HttpPut("{id}/cancelar")]
        [Authorize(
            Roles = "Administrador,Gerente,Atendente")]
        public async Task<IActionResult> Cancelar(
            int id)
        {
            try
            {
                var cancelada =
                    await _reservaService
                        .CancelarAsync(id);

                if (!cancelada)
                {
                    return NotFound(new
                    {
                        mensagem =
                            "Reserva não encontrada."
                    });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    mensagem = ex.Message
                });
            }
        }

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