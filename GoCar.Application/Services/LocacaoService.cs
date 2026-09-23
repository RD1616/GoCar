using GoCar.Application.DTOs.Locacao;
using GoCar.Application.Interfaces;
using GoCar.Domain.Entities;
using GoCar.Domain.Enums;

namespace GoCar.Application.Services
{
    public class LocacaoService : ILocacaoService
    {
        private readonly ILocacaoRepository _repository;
        private readonly IReservaRepository _reservaRepository;
        private readonly IVeiculoRepository _veiculoRepository;
        private readonly IPagamentoRepository _pagamentoRepository;

        public LocacaoService(
            ILocacaoRepository repository,
            IReservaRepository reservaRepository,
            IVeiculoRepository veiculoRepository,
            IPagamentoRepository pagamentoRepository)
        {
            _repository = repository;
            _reservaRepository = reservaRepository;
            _veiculoRepository = veiculoRepository;
            _pagamentoRepository = pagamentoRepository;
        }

        // =====================================================
        // LISTAR TODOS
        // =====================================================

        public async Task<IEnumerable<LocacaoDto>> ListarTodosAsync()
        {
            var locacoes =
                await _repository.ListarTodosAsync();

            return locacoes.Select(MapearParaDto);
        }

        // =====================================================
        // OBTER POR ID
        // =====================================================

        public async Task<LocacaoDto?> ObterPorIdAsync(
            int id)
        {
            var locacao =
                await _repository.ObterPorIdAsync(id);

            if (locacao == null)
                return null;

            return MapearParaDto(locacao);
        }

        // =====================================================
        // CRIAR LOCAÇÃO
        // =====================================================

        public async Task<LocacaoDto> CriarAsync(
            CriarLocacaoDto dto)
        {
            var reserva =
                await _reservaRepository
                    .ObterPorIdAsync(dto.ReservaId);

            if (reserva == null)
            {
                throw new Exception(
                    "Reserva não encontrada.");
            }

            // =================================================
            // IMPEDIR LOCAÇÃO DUPLICADA
            // =================================================

            var locacaoExistente =
                await _repository
                    .ObterPorReservaIdAsync(
                        reserva.Id);

            if (locacaoExistente != null)
            {
                throw new Exception(
                    $"A reserva #{reserva.Id} já possui " +
                    $"a locação #{locacaoExistente.Id}.");
            }

            // =================================================
            // VALIDAR RESERVA
            // =================================================

            if (!reserva.IsAtiva ||
                reserva.Status !=
                    StatusReserva.Confirmada)
            {
                throw new Exception(
                    "Somente uma reserva confirmada e ativa " +
                    "pode iniciar uma locação.");
            }

            // =================================================
            // VALIDAR PERÍODO DA RESERVA
            // =================================================

            var agora =
                DateTime.Now;

            if (agora.Date <
                reserva.DataRetirada.Date)
            {
                throw new InvalidOperationException(
                    $"Esta locação só poderá ser iniciada a partir de " +
                    $"{reserva.DataRetirada:dd/MM/yyyy}.");
            }

            if (agora >
                reserva.DataDevolucaoPrevista)
            {
                throw new InvalidOperationException(
                    "O período desta reserva já terminou. " +
                    "Não é possível iniciar a locação.");
            }

            // =================================================
            // VERIFICAR ENTRADA DE 30%
            // =================================================

            var pagamentoEntrada =
                await _pagamentoRepository
                    .ObterAtivoPorReservaAsync(
                        reserva.Id);

            if (pagamentoEntrada == null)
            {
                throw new Exception(
                    "Não é possível iniciar a locação. " +
                    "A entrada de 30% da reserva ainda não foi gerada.");
            }

            if (!string.Equals(
                    pagamentoEntrada.Tipo,
                    "Entrada",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Não foi encontrado um pagamento de entrada " +
                    "válido para esta reserva.");
            }

            if (pagamentoEntrada.Status !=
                StatusPagamento.Pago)
            {
                throw new Exception(
                    "Não é possível iniciar a locação. " +
                    "A entrada de 30% ainda não foi paga.");
            }

            if (!pagamentoEntrada.IsAtivo)
            {
                throw new Exception(
                    "O pagamento da entrada não está ativo.");
            }

            // =================================================
            // VERIFICAR VEÍCULO
            // =================================================

            var veiculo =
                await _veiculoRepository
                    .ObterPorIdAsync(
                        reserva.VeiculoId);

            if (veiculo == null)
            {
                throw new Exception(
                    "Veículo não encontrado.");
            }

            if (!veiculo.IsAtivo)
            {
                throw new Exception(
                    "Veículo não está ativo.");
            }

            if (veiculo.Status !=
                StatusVeiculo.Reservado)
            {
                throw new Exception(
                    "O veículo precisa estar reservado " +
                    "para iniciar a locação.");
            }

            // =================================================
            // VALIDAÇÕES DA RETIRADA
            // =================================================

            if (dto.KmSaida < 0)
            {
                throw new Exception(
                    "A quilometragem de saída não pode ser negativa.");
            }

            if (dto.CombustivelSaidaPercentual < 0 ||
                dto.CombustivelSaidaPercentual > 100)
            {
                throw new Exception(
                    "O combustível de saída deve estar entre 0% e 100%.");
            }

            // =================================================
            // CRIAR LOCAÇÃO
            // =================================================

            var locacao =
                new Locacao
                {
                    ReservaId =
                        dto.ReservaId,

                    // A data real de retirada deve ser definida
                    // pelo servidor no momento da entrega.
                    DataRetirada =
                        DateTime.Now,

                    KmSaida =
                        dto.KmSaida,

                    CombustivelSaidaPercentual =
                        dto.CombustivelSaidaPercentual,

                    ValorTotal =
                        reserva.ValorTotalPrevisto,

                    Status =
                        StatusLocacao.Ativa,

                    Observacoes =
                        dto.Observacoes,

                    IsAtiva =
                        true,

                    DataCriacao =
                        DateTime.Now
                };

            var locacaoCriada =
                await _repository
                    .CriarAsync(locacao);

            // =================================================
            // VEÍCULO: RESERVADO -> ALUGADO
            // =================================================

            veiculo.Status =
                StatusVeiculo.Alugado;

            var veiculoAtualizado =
                await _veiculoRepository
                    .AtualizarAsync(veiculo);

            if (!veiculoAtualizado)
            {
                throw new Exception(
                    "A locação foi criada, mas não foi possível " +
                    "alterar o veículo para alugado.");
            }

            // =================================================
            // RESERVA: CONFIRMADA -> CONCLUÍDA
            // =================================================

            reserva.Status =
                StatusReserva.Concluida;

            reserva.IsAtiva =
                false;

            var reservaAtualizada =
                await _reservaRepository
                    .AtualizarAsync(reserva);

            if (!reservaAtualizada)
            {
                throw new Exception(
                    "A locação foi criada, mas não foi possível " +
                    "concluir a reserva.");
            }

            return MapearParaDto(
                locacaoCriada);
        }

        // =====================================================
        // ATUALIZAR LOCAÇÃO
        // =====================================================

        public async Task<bool> AtualizarAsync(
            int id,
            AtualizarLocacaoDto dto)
        {
            var locacao =
                await _repository
                    .ObterPorIdAsync(id);

            if (locacao == null)
                return false;

            var reserva =
                await _reservaRepository
                    .ObterPorIdAsync(
                        locacao.ReservaId);

            Veiculo? veiculo = null;

            if (reserva != null)
            {
                veiculo =
                    await _veiculoRepository
                        .ObterPorIdAsync(
                            reserva.VeiculoId);
            }

            // =================================================
            // VALIDAÇÕES
            // =================================================

            if (dto.KmSaida < 0)
            {
                throw new Exception(
                    "A quilometragem de saída não pode ser negativa.");
            }

            if (dto.KmEntrada.HasValue &&
                dto.KmEntrada.Value < dto.KmSaida)
            {
                throw new Exception(
                    "A quilometragem de entrada não pode ser menor " +
                    "que a quilometragem de saída.");
            }

            if (dto.CombustivelSaidaPercentual < 0 ||
                dto.CombustivelSaidaPercentual > 100)
            {
                throw new Exception(
                    "O combustível de saída deve estar entre 0% e 100%.");
            }

            if (dto.CombustivelEntradaPercentual.HasValue &&
                (dto.CombustivelEntradaPercentual.Value < 0 ||
                 dto.CombustivelEntradaPercentual.Value > 100))
            {
                throw new Exception(
                    "O combustível de entrada deve estar entre 0% e 100%.");
            }

            // =================================================
            // FINALIZAÇÃO EXIGE KM DE ENTRADA
            // =================================================

            if (dto.Status ==
                    StatusLocacao.Finalizada &&
                !dto.KmEntrada.HasValue)
            {
                throw new Exception(
                    "Informe a quilometragem de entrada para finalizar a locação.");
            }

            // =================================================
            // ATUALIZAR DADOS DA LOCAÇÃO
            // =================================================

            locacao.DataRetirada =
                dto.DataRetirada;

            locacao.DataDevolucaoReal =
                dto.DataDevolucaoReal;

            locacao.KmSaida =
                dto.KmSaida;

            locacao.KmEntrada =
                dto.KmEntrada;

            locacao.CombustivelSaidaPercentual =
                dto.CombustivelSaidaPercentual;

            locacao.CombustivelEntradaPercentual =
                dto.CombustivelEntradaPercentual;

            if (reserva != null)
            {
                locacao.ValorTotal =
                    reserva.ValorTotalPrevisto;
            }

            locacao.Status =
                dto.Status;

            locacao.Observacoes =
                dto.Observacoes;

            locacao.IsAtiva =
                dto.Status ==
                    StatusLocacao.Ativa;

            var atualizado =
                await _repository
                    .AtualizarAsync(locacao);

            if (!atualizado)
                return false;

            // =================================================
            // LOCAÇÃO ATIVA
            // =================================================

            if (dto.Status ==
                StatusLocacao.Ativa)
            {
                if (veiculo != null)
                {
                    veiculo.Status =
                        StatusVeiculo.Alugado;

                    await _veiculoRepository
                        .AtualizarAsync(veiculo);
                }
            }

            // =================================================
            // LOCAÇÃO FINALIZADA
            // =================================================

            if (dto.Status ==
                    StatusLocacao.Finalizada &&
                veiculo != null &&
                reserva != null)
            {
                if (dto.KmEntrada.HasValue)
                {
                    veiculo.KmAtual =
                        dto.KmEntrada.Value;
                }

                veiculo.Status =
                    await DeterminarStatusVeiculoAposLocacaoAsync(
                        veiculo.Id,
                        locacao.Id);

                await _veiculoRepository
                    .AtualizarAsync(veiculo);

                reserva.Status =
                    StatusReserva.Concluida;

                reserva.IsAtiva =
                    false;

                await _reservaRepository
                    .AtualizarAsync(reserva);
            }

            // =================================================
            // LOCAÇÃO CANCELADA
            // =================================================

            if (dto.Status ==
                    StatusLocacao.Cancelada &&
                veiculo != null &&
                reserva != null)
            {
                veiculo.Status =
                    await DeterminarStatusVeiculoAposLocacaoAsync(
                        veiculo.Id,
                        locacao.Id);

                await _veiculoRepository
                    .AtualizarAsync(veiculo);

                reserva.Status =
                    StatusReserva.Cancelada;

                reserva.IsAtiva =
                    false;

                reserva.DataCancelamento =
                    DateTime.Now;

                await _reservaRepository
                    .AtualizarAsync(reserva);
            }

            return true;
        }

        // =====================================================
        // EXCLUIR / CANCELAR LOCAÇÃO
        // =====================================================

        public async Task<bool> ExcluirAsync(
            int id)
        {
            var locacao =
                await _repository
                    .ObterPorIdAsync(id);

            if (locacao == null)
                return false;

            var reserva =
                await _reservaRepository
                    .ObterPorIdAsync(
                        locacao.ReservaId);

            Veiculo? veiculo = null;

            if (reserva != null)
            {
                veiculo =
                    await _veiculoRepository
                        .ObterPorIdAsync(
                            reserva.VeiculoId);
            }

            locacao.Status =
                StatusLocacao.Cancelada;

            locacao.IsAtiva =
                false;

            var atualizado =
                await _repository
                    .AtualizarAsync(locacao);

            if (!atualizado)
                return false;

            if (veiculo != null)
            {
                veiculo.Status =
                    await DeterminarStatusVeiculoAposLocacaoAsync(
                        veiculo.Id,
                        locacao.Id);

                await _veiculoRepository
                    .AtualizarAsync(veiculo);
            }

            if (reserva != null)
            {
                reserva.Status =
                    StatusReserva.Cancelada;

                reserva.IsAtiva =
                    false;

                reserva.DataCancelamento =
                    DateTime.Now;

                await _reservaRepository
                    .AtualizarAsync(reserva);
            }

            return true;
        }

        // =====================================================
        // DETERMINAR STATUS DO VEÍCULO
        // =====================================================

        private async Task<StatusVeiculo>
            DeterminarStatusVeiculoAposLocacaoAsync(
                int veiculoId,
                int locacaoAtualId)
        {
            var reservas =
                await _reservaRepository
                    .ListarTodosAsync();

            var agora =
                DateTime.Now;

            var possuiReservaFutura =
                reservas.Any(r =>
                    r.Id != 0 &&
                    r.VeiculoId == veiculoId &&
                    r.IsAtiva &&
                    r.Status ==
                        StatusReserva.Confirmada &&
                    r.DataDevolucaoPrevista > agora);

            if (possuiReservaFutura)
            {
                return StatusVeiculo.Reservado;
            }

            return StatusVeiculo.Disponivel;
        }

        // =====================================================
        // MAPEAR PARA DTO
        // =====================================================

        private static LocacaoDto MapearParaDto(
            Locacao locacao)
        {
            return new LocacaoDto
            {
                Id =
                    locacao.Id,

                ReservaId =
                    locacao.ReservaId,

                DataRetirada =
                    locacao.DataRetirada,

                DataDevolucaoReal =
                    locacao.DataDevolucaoReal,

                KmSaida =
                    locacao.KmSaida,

                KmEntrada =
                    locacao.KmEntrada,

                CombustivelSaidaPercentual =
                    locacao.CombustivelSaidaPercentual,

                CombustivelEntradaPercentual =
                    locacao.CombustivelEntradaPercentual,

                ValorTotal =
                    locacao.ValorTotal,

                Status =
                    locacao.Status,

                Observacoes =
                    locacao.Observacoes,

                IsAtiva =
                    locacao.IsAtiva,

                DataCriacao =
                    locacao.DataCriacao
            };
        }
    }
}