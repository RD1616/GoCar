using GoCar.Application.DTOs.Categoria;
using GoCar.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoCar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _service;

        public CategoriasController(
            ICategoriaService service)
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
            var categorias =
                await _service.ListarTodosAsync();

            return Ok(categorias);
        }

        // =====================================================
        // OBTER POR ID
        // =====================================================

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(
            int id)
        {
            var categoria =
                await _service.ObterPorIdAsync(id);

            if (categoria == null)
            {
                return NotFound(new
                {
                    mensagem =
                        "Categoria não encontrada."
                });
            }

            return Ok(categoria);
        }

        // =====================================================
        // CRIAR
        // =====================================================

        [HttpPost]
        [Authorize(
            Roles = "Administrador,Gerente,Atendente")]
        public async Task<IActionResult> Criar(
            [FromBody] CriarCategoriaDto dto)
        {
            var categoria =
                await _service.CriarAsync(dto);

            return CreatedAtAction(
                nameof(ObterPorId),
                new { id = categoria.Id },
                categoria);
        }

        // =====================================================
        // ATUALIZAR
        // =====================================================

        [HttpPut("{id}")]
        [Authorize(
            Roles = "Administrador,Gerente,Atendente")]
        public async Task<IActionResult> Atualizar(
            int id,
            [FromBody] AtualizarCategoriaDto dto)
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
                        "Categoria não encontrada."
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
                            "Categoria não encontrada."
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