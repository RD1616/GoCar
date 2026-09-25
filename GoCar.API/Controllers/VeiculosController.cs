using GoCar.Application.DTOs.Veiculo;
using GoCar.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoCar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VeiculosController : ControllerBase
    {
        private readonly IVeiculoService _service;

        public VeiculosController(
            IVeiculoService service)
        {
            _service = service;
        }

        // =====================================================
        // LISTAR TODOS
        // =====================================================

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ListarTodos()
        {
            var veiculos =
                await _service.ListarTodosAsync();

            return Ok(veiculos);
        }

        // =====================================================
        // OBTER POR ID
        // =====================================================

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(
            int id)
        {
            var veiculo =
                await _service.ObterPorIdAsync(id);

            if (veiculo == null)
            {
                return NotFound(new
                {
                    mensagem =
                        "Veículo não encontrado."
                });
            }

            return Ok(veiculo);
        }

        // =====================================================
        // CRIAR
        // =====================================================

        [HttpPost]
        [Authorize(
            Roles = "Administrador,Gerente,Atendente")]
        public async Task<IActionResult> Criar(
            [FromBody] CriarVeiculoDto dto)
        {
            var veiculo =
                await _service.CriarAsync(dto);

            return CreatedAtAction(
                nameof(ObterPorId),
                new { id = veiculo.Id },
                veiculo);
        }

        // =====================================================
        // ATUALIZAR
        // =====================================================

        [HttpPut("{id}")]
        [Authorize(
            Roles = "Administrador,Gerente,Atendente")]
        public async Task<IActionResult> Atualizar(
            int id,
            [FromBody] AtualizarVeiculoDto dto)
        {
            var atualizado =
                await _service.AtualizarAsync(
                    id,
                    dto);

            if (!atualizado)
            {
                return NotFound(new
                {
                    mensagem =
                        "Veículo não encontrado."
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
                            "Veículo não encontrado."
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
            var ativado =
                await _service.AtivarAsync(id);

            if (!ativado)
            {
                return NotFound(new
                {
                    mensagem =
                        "Veículo não encontrado."
                });
            }

            return NoContent();
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
                            "Veículo não encontrado."
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
    }
}