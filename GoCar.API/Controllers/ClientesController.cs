using GoCar.Application.DTOs.Cliente;
using GoCar.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GoCar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _service;

        public ClientesController(
            IClienteService service)
        {
            _service = service;
        }

        // =====================================================
        // LISTAR TODOS
        // =====================================================

        [HttpGet]
        [Authorize(
            Roles = "Administrador,Gerente,Atendente")]
        public async Task<
            ActionResult<IEnumerable<ClienteDto>>>
            ListarTodos()
        {
            var clientes =
                await _service.ListarTodosAsync();

            return Ok(clientes);
        }

        // =====================================================
        // MEU PERFIL
        // =====================================================

        [HttpGet("meu-perfil")]
        [Authorize(Roles = "Cliente")]
        public async Task<ActionResult<ClienteDto>>
            ObterMeuPerfil()
        {
            var cliente =
                await ObterClienteLogadoAsync();

            if (cliente == null)
            {
                return NotFound(new
                {
                    mensagem =
                        "Cliente não encontrado para o usuário autenticado."
                });
            }

            return Ok(cliente);
        }

        // =====================================================
        // OBTER POR ID
        // =====================================================

        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDto>>
            ObterPorId(int id)
        {
            var cliente =
                await _service.ObterPorIdAsync(id);

            if (cliente == null)
            {
                return NotFound(new
                {
                    mensagem =
                        "Cliente não encontrado."
                });
            }

            if (UsuarioEhFuncionario())
            {
                return Ok(cliente);
            }

            var clienteLogado =
                await ObterClienteLogadoAsync();

            if (clienteLogado == null)
            {
                return Unauthorized(new
                {
                    mensagem =
                        "Cliente não identificado para o usuário autenticado."
                });
            }

            if (cliente.Id !=
                clienteLogado.Id)
            {
                return Forbid();
            }

            return Ok(cliente);
        }

        // =====================================================
        // CRIAR
        // =====================================================

        [HttpPost]
        [Authorize(
            Roles = "Administrador,Gerente,Atendente")]
        public async Task<ActionResult<ClienteDto>> Criar(
            [FromBody] CriarClienteDto dto)
        {
            var cliente =
                await _service.CriarAsync(dto);

            return CreatedAtAction(
                nameof(ObterPorId),
                new { id = cliente.Id },
                cliente);
        }

        // =====================================================
        // ATUALIZAR
        // =====================================================

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(
            int id,
            [FromBody] AtualizarClienteDto dto)
        {
            var clienteExistente =
                await _service.ObterPorIdAsync(id);

            if (clienteExistente == null)
            {
                return NotFound(new
                {
                    mensagem =
                        "Cliente não encontrado."
                });
            }

            if (!UsuarioEhFuncionario())
            {
                var clienteLogado =
                    await ObterClienteLogadoAsync();

                if (clienteLogado == null)
                {
                    return Unauthorized(new
                    {
                        mensagem =
                            "Cliente não identificado para o usuário autenticado."
                    });
                }

                if (clienteExistente.Id !=
                    clienteLogado.Id)
                {
                    return Forbid();
                }
            }

            var atualizado =
                await _service.AtualizarAsync(
                    id,
                    dto);

            if (!atualizado)
            {
                return NotFound(new
                {
                    mensagem =
                        "Cliente não encontrado."
                });
            }

            return NoContent();
        }

        // =====================================================
        // DESATIVAR
        // =====================================================

        [HttpDelete("{id}")]
        [Authorize(
            Roles = "Administrador,Gerente,Atendente")]
        public async Task<IActionResult> Excluir(
            int id)
        {
            try
            {
                var excluido =
                    await _service.ExcluirAsync(id);

                if (!excluido)
                {
                    return NotFound(new
                    {
                        mensagem =
                            "Cliente não encontrado."
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
        // ATIVAR
        // =====================================================

        [HttpPut("{id}/ativar")]
        [Authorize(
            Roles = "Administrador,Gerente,Atendente")]
        public async Task<IActionResult> Ativar(
            int id)
        {
            try
            {
                var ativado =
                    await _service.AtivarAsync(id);

                if (!ativado)
                {
                    return NotFound(new
                    {
                        mensagem =
                            "Cliente não encontrado."
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
        // EXCLUIR PERMANENTEMENTE
        // =====================================================

        [HttpDelete("{id}/permanente")]
        [Authorize(
            Roles = "Administrador,Gerente,Atendente")]
        public async Task<IActionResult>
            ExcluirPermanentemente(int id)
        {
            try
            {
                var excluido =
                    await _service
                        .ExcluirPermanentementeAsync(id);

                if (!excluido)
                {
                    return NotFound(new
                    {
                        mensagem =
                            "Cliente não encontrado."
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
        // OBTER CLIENTE LOGADO
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

            return await _service
                .ObterPorUsuarioIdAsync(usuarioId);
        }

        // =====================================================
        // VERIFICAR FUNCIONÁRIO
        // =====================================================

        private bool UsuarioEhFuncionario()
        {
            return User.IsInRole("Administrador")
                || User.IsInRole("Gerente")
                || User.IsInRole("Atendente");
        }
    }
}