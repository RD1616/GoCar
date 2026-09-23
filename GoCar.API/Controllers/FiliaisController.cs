using GoCar.Application.DTOs.Filial;
using GoCar.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoCar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FiliaisController : ControllerBase
    {
        private readonly IFilialService _service;

        public FiliaisController(
            IFilialService service)
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
            var filiais =
                await _service.ListarTodosAsync();

            return Ok(filiais);
        }

        // =====================================================
        // OBTER POR ID
        // =====================================================

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(
            int id)
        {
            var filial =
                await _service.ObterPorIdAsync(id);

            if (filial == null)
            {
                return NotFound(new
                {
                    mensagem =
                        "Filial não encontrada."
                });
            }

            return Ok(filial);
        }

        // =====================================================
        // CRIAR
        // =====================================================

        [HttpPost]
        [Authorize(
            Roles = "Administrador,Gerente,Atendente")]
        public async Task<IActionResult> Criar(
            [FromBody] CriarFilialDto dto)
        {
            var filial =
                await _service.CriarAsync(dto);

            return CreatedAtAction(
                nameof(ObterPorId),
                new { id = filial.Id },
                filial);
        }

        // =====================================================
        // ATUALIZAR
        // =====================================================

        [HttpPut("{id}")]
        [Authorize(
            Roles = "Administrador,Gerente,Atendente")]
        public async Task<IActionResult> Atualizar(
            int id,
            [FromBody] AtualizarFilialDto dto)
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
                        "Filial não encontrada."
                });
            }

            return NoContent();
        }

        // =====================================================
        // EXCLUIR / DESATIVAR
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
                            "Filial não encontrada."
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
    }
}