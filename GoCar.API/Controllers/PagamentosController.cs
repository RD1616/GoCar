using GoCar.Application.DTOs.Cliente;
using GoCar.Application.DTOs.Pagamento;
using GoCar.Application.Interfaces;
using GoCar.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GoCar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PagamentosController : ControllerBase
    {
        private readonly IPagamentoService _pagamentoService;
        private readonly ILocacaoService _locacaoService;
        private readonly IReservaService _reservaService;
        private readonly IClienteService _clienteService;

        public PagamentosController(
            IPagamentoService pagamentoService,
            ILocacaoService locacaoService,
            IReservaService reservaService,
            IClienteService clienteService)
        {
            _pagamentoService = pagamentoService;
            _locacaoService = locacaoService;
            _reservaService = reservaService;
            _clienteService = clienteService;
        }

        // =====================================================
        // LISTAR TODOS
        // =====================================================

        [HttpGet]
        [Authorize(Roles = "Administrador,Gerente,Atendente")]
        public async Task<ActionResult<IEnumerable<PagamentoDto>>> ListarTodos()
        {
            var pagamentos =
                await _pagamentoService.ListarTodosAsync();

            return Ok(pagamentos);
        }

        // =====================================================
        // LISTAR MEUS PAGAMENTOS
        // =====================================================

        [HttpGet("minhas")]
        [Authorize(Roles = "Cliente")]
        public async Task<ActionResult<IEnumerable<PagamentoDto>>> ListarMinhas()
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

            var pagamentos =
                await _pagamentoService.ListarTodosAsync();

            var meusPagamentos =
                new List<PagamentoDto>();

            foreach (var pagamento in pagamentos)
            {
                // =============================================
                // PAGAMENTO DIRETO DA RESERVA
                // Ex.: entrada de 30%
                // =============================================

                if (pagamento.ReservaId.HasValue)
                {
                    var reserva =
                        await _reservaService.ObterPorIdAsync(
                            pagamento.ReservaId.Value);

                    if (reserva != null &&
                        reserva.ClienteId == cliente.Id)
                    {
                        meusPagamentos.Add(pagamento);
                    }

                    continue;
                }

                // =============================================
                // PAGAMENTO DA LOCAÇÃO
                // =============================================

                if (pagamento.LocacaoId.HasValue)
                {
                    var locacao =
                        await _locacaoService.ObterPorIdAsync(
                            pagamento.LocacaoId.Value);

                    if (locacao == null)
                        continue;

                    var reserva =
                        await _reservaService.ObterPorIdAsync(
                            locacao.ReservaId);

                    if (reserva != null &&
                        reserva.ClienteId == cliente.Id)
                    {
                        meusPagamentos.Add(pagamento);
                    }
                }
            }

            return Ok(meusPagamentos);
        }

        // =====================================================
        // PAGAR ENTRADA DA MINHA RESERVA
        // =====================================================
        // Somente Cliente.
        //
        // FLUXO:
        //
        // 1. A reserva é criada como Pendente.
        // 2. O cliente paga a entrada de 30%.
        // 3. O PagamentoService calcula os 30%.
        // 4. O PagamentoService registra a entrada como paga.
        // 5. O PagamentoService altera a reserva para Confirmada.
        // 6. A reserva fica pronta para retirada.
        //
        // O cliente NÃO precisa confirmar a reserva manualmente.
        // =====================================================

        [HttpPost("entrada")]
        [Authorize(Roles = "Cliente")]
        public async Task<ActionResult<PagamentoDto>> PagarEntrada(
            [FromBody] PagarEntradaRequest request)
        {
            try
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

                var reserva =
                    await _reservaService.ObterPorIdAsync(
                        request.ReservaId);

                if (reserva == null)
                {
                    return NotFound(new
                    {
                        mensagem =
                            "Reserva não encontrada."
                    });
                }

                // =============================================
                // SEGURANÇA
                // O cliente só pode pagar a própria reserva.
                // =============================================

                if (reserva.ClienteId != cliente.Id)
                {
                    return Forbid();
                }

                // =============================================
                // NOVO FLUXO
                //
                // A entrada é paga enquanto a reserva ainda
                // está PENDENTE.
                //
                // Depois do pagamento, o PagamentoService
                // altera a reserva para CONFIRMADA.
                // =============================================

                if (reserva.Status != StatusReserva.Pendente ||
                    !reserva.IsAtiva)
                {
                    return BadRequest(new
                    {
                        mensagem =
                            "A entrada só pode ser paga para uma " +
                            "reserva pendente e ativa."
                    });
                }

                // =============================================
                // VALIDAR FORMA DE PAGAMENTO
                // =============================================

                if (!Enum.IsDefined(
                        typeof(FormaPagamento),
                        request.FormaPagamento))
                {
                    return BadRequest(new
                    {
                        mensagem =
                            "Forma de pagamento inválida."
                    });
                }

                // =============================================
                // CRIAR ENTRADA
                //
                // O cálculo dos 30% e a confirmação da reserva
                // ficam centralizados no PagamentoService.
                // =============================================

                var pagamento =
                    await _pagamentoService
                        .CriarEntradaAsync(
                            request.ReservaId,
                            request.FormaPagamento);

                return CreatedAtAction(
                    nameof(ObterPorId),
                    new { id = pagamento.Id },
                    pagamento);
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
        // OBTER POR ID
        // =====================================================

        [HttpGet("{id}")]
        public async Task<ActionResult<PagamentoDto>> ObterPorId(
            int id)
        {
            var pagamento =
                await _pagamentoService.ObterPorIdAsync(id);

            if (pagamento == null)
            {
                return NotFound(new
                {
                    mensagem =
                        "Pagamento não encontrado."
                });
            }

            if (UsuarioEhFuncionario())
            {
                return Ok(pagamento);
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

            // =============================================
            // PAGAMENTO DIRETO DA RESERVA
            // =============================================

            if (pagamento.ReservaId.HasValue)
            {
                var reserva =
                    await _reservaService.ObterPorIdAsync(
                        pagamento.ReservaId.Value);

                if (reserva == null)
                {
                    return NotFound(new
                    {
                        mensagem =
                            "Reserva vinculada ao pagamento não encontrada."
                    });
                }

                if (reserva.ClienteId != cliente.Id)
                {
                    return Forbid();
                }

                return Ok(pagamento);
            }

            // =============================================
            // PAGAMENTO DA LOCAÇÃO
            // =============================================

            if (pagamento.LocacaoId.HasValue)
            {
                var locacao =
                    await _locacaoService.ObterPorIdAsync(
                        pagamento.LocacaoId.Value);

                if (locacao == null)
                {
                    return NotFound(new
                    {
                        mensagem =
                            "Locação vinculada ao pagamento não encontrada."
                    });
                }

                var reserva =
                    await _reservaService.ObterPorIdAsync(
                        locacao.ReservaId);

                if (reserva == null)
                {
                    return NotFound(new
                    {
                        mensagem =
                            "Reserva vinculada à locação não encontrada."
                    });
                }

                if (reserva.ClienteId != cliente.Id)
                {
                    return Forbid();
                }

                return Ok(pagamento);
            }

            return NotFound(new
            {
                mensagem =
                    "O pagamento não possui reserva ou locação vinculada."
            });
        }

        // =====================================================
        // CRIAR PAGAMENTO ADMINISTRATIVO
        // =====================================================

        [HttpPost]
        [Authorize(Roles = "Administrador,Gerente,Atendente")]
        public async Task<ActionResult<PagamentoDto>> Criar(
            [FromBody] CriarPagamentoDto dto)
        {
            try
            {
                var pagamento =
                    await _pagamentoService.CriarAsync(dto);

                return CreatedAtAction(
                    nameof(ObterPorId),
                    new { id = pagamento.Id },
                    pagamento);
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
        // ATUALIZAR
        // =====================================================

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Gerente,Atendente")]
        public async Task<IActionResult> Atualizar(
            int id,
            [FromBody] CriarPagamentoDto dto)
        {
            try
            {
                var atualizado =
                    await _pagamentoService.AtualizarAsync(
                        id,
                        dto);

                if (!atualizado)
                {
                    return NotFound(new
                    {
                        mensagem =
                            "Pagamento, reserva ou locação não encontrada."
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
        // EXCLUIR
        // =====================================================

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador,Gerente,Atendente")]
        public async Task<IActionResult> Excluir(
            int id)
        {
            var excluido =
                await _pagamentoService.ExcluirAsync(id);

            if (!excluido)
            {
                return NotFound(new
                {
                    mensagem =
                        "Pagamento não encontrado."
                });
            }

            return NoContent();
        }

        // =====================================================
        // CLIENTE LOGADO
        // =====================================================

        private async Task<ClienteDto?> ObterClienteLogadoAsync()
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

    // =========================================================
    // REQUEST DA ENTRADA
    // =========================================================

    public class PagarEntradaRequest
    {
        public int ReservaId { get; set; }

        public FormaPagamento FormaPagamento { get; set; }
    }
}