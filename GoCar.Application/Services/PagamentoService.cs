using GoCar.Application.DTOs.Pagamento;
using GoCar.Application.Interfaces;
using GoCar.Domain.Entities;
using GoCar.Domain.Enums;

namespace GoCar.Application.Services
{
    public class PagamentoService : IPagamentoService
    {
        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly ILocacaoRepository _locacaoRepository;
        private readonly IReservaRepository _reservaRepository;
        private readonly IVeiculoRepository _veiculoRepository;

        public PagamentoService(
            IPagamentoRepository pagamentoRepository,
            ILocacaoRepository locacaoRepository,
            IReservaRepository reservaRepository,
            IVeiculoRepository veiculoRepository)
        {
            _pagamentoRepository = pagamentoRepository;
            _locacaoRepository = locacaoRepository;
            _reservaRepository = reservaRepository;
            _veiculoRepository = veiculoRepository;
        }

        // =====================================================
        // LISTAR TODOS
        // =====================================================

        public async Task<IEnumerable<PagamentoDto>> ListarTodosAsync()
        {
            var pagamentos =
                await _pagamentoRepository.ListarTodosAsync();

            return pagamentos.Select(MapearParaDto);
        }

        // =====================================================
        // OBTER POR ID
        // =====================================================

        public async Task<PagamentoDto?> ObterPorIdAsync(int id)
        {
            var pagamento =
                await _pagamentoRepository.ObterPorIdAsync(id);

            if (pagamento == null)
                return null;

            return MapearParaDto(pagamento);
        }

        // =====================================================
        // CRIAR ENTRADA DE 30%
        // =====================================================

        public async Task<PagamentoDto> CriarEntradaAsync(
            int reservaId,
            FormaPagamento formaPagamento)
        {
            var reserva =
                await _reservaRepository
                    .ObterPorIdAsync(reservaId);

            if (reserva == null)
            {
                throw new Exception(
                    "Reserva não encontrada.");
            }

            if (reserva.Status != StatusReserva.Pendente ||
                !reserva.IsAtiva)
            {
                throw new Exception(
                    "A entrada só pode ser paga para uma " +
                    "reserva pendente e ativa.");
            }

            var pagamentoExistente =
                await _pagamentoRepository
                    .ObterAtivoPorReservaAsync(reservaId);

            if (pagamentoExistente != null)
            {
                if (pagamentoExistente.Status ==
                    StatusPagamento.Pago)
                {
                    throw new Exception(
                        "A entrada desta reserva já foi paga.");
                }

                throw new Exception(
                    "Esta reserva já possui uma entrada pendente.");
            }

            await ValidarDisponibilidadeParaConfirmacaoAsync(
                reserva);

            var valorEntrada =
                CalcularValorEntrada(
                    reserva.ValorTotalPrevisto);

            var pagamento = new Pagamento
            {
                LocacaoId = null,

                ReservaId = reservaId,

                Tipo = "Entrada",

                FormaPagamento = formaPagamento,

                Status = StatusPagamento.Pago,

                Valor = valorEntrada,

                DataPagamento = DateTime.Now,

                Observacoes =
                    $"Entrada de 30% da reserva #{reservaId}",

                IsAtivo = true,

                DataCriacao = DateTime.Now
            };

            var pagamentoCriado =
                await _pagamentoRepository
                    .CriarAsync(pagamento);

            // =================================================
            // CONFIRMAR RESERVA AUTOMATICAMENTE
            // =================================================

            reserva.Status =
                StatusReserva.Confirmada;

            reserva.IsAtiva =
                true;

            reserva.DataCancelamento =
                null;

            var reservaAtualizada =
                await _reservaRepository
                    .AtualizarAsync(reserva);

            if (!reservaAtualizada)
            {
                throw new Exception(
                    "A entrada foi registrada, mas não foi possível " +
                    "confirmar a reserva.");
            }

            // =================================================
            // RESERVAR UNIDADE FÍSICA DO VEÍCULO
            // =================================================

            await ReservarVeiculoAsync(reserva);

            return MapearParaDto(
                pagamentoCriado);
        }

        // =====================================================
        // CRIAR PAGAMENTO
        // =====================================================

        public async Task<PagamentoDto> CriarAsync(
            CriarPagamentoDto dto)
        {
            if (dto.ReservaId.HasValue &&
                dto.LocacaoId.HasValue)
            {
                throw new Exception(
                    "O pagamento não pode pertencer a uma reserva " +
                    "e a uma locação ao mesmo tempo.");
            }

            if (!dto.ReservaId.HasValue &&
                !dto.LocacaoId.HasValue)
            {
                throw new Exception(
                    "Informe a reserva ou a locação do pagamento.");
            }

            Pagamento pagamento;

            Reserva? reservaEntrada = null;

            // =================================================
            // ENTRADA DA RESERVA
            // =================================================

            if (dto.ReservaId.HasValue)
            {
                var reservaId =
                    dto.ReservaId.Value;

                var reserva =
                    await _reservaRepository
                        .ObterPorIdAsync(reservaId);

                if (reserva == null)
                {
                    throw new Exception(
                        "Reserva não encontrada.");
                }

                if (reserva.Status != StatusReserva.Pendente ||
                    !reserva.IsAtiva)
                {
                    throw new Exception(
                        "A entrada só pode ser gerada para uma " +
                        "reserva pendente e ativa.");
                }

                var pagamentoExistente =
                    await _pagamentoRepository
                        .ObterAtivoPorReservaAsync(reservaId);

                if (pagamentoExistente != null)
                {
                    var statusAtual =
                        pagamentoExistente.Status ==
                        StatusPagamento.Pago
                            ? "Pago"
                            : "Pendente";

                    throw new Exception(
                        $"A reserva #{reservaId} já possui " +
                        $"o pagamento de entrada " +
                        $"#{pagamentoExistente.Id} " +
                        $"com status {statusAtual}.");
                }

                if (dto.Status == StatusPagamento.Pago)
                {
                    await ValidarDisponibilidadeParaConfirmacaoAsync(
                        reserva);
                }

                var valorEntrada =
                    CalcularValorEntrada(
                        reserva.ValorTotalPrevisto);

                pagamento = new Pagamento
                {
                    ReservaId = reservaId,

                    LocacaoId = null,

                    Tipo = "Entrada",

                    FormaPagamento =
                        dto.FormaPagamento,

                    Status =
                        dto.Status,

                    Valor =
                        valorEntrada,

                    DataPagamento =
                        dto.DataPagamento,

                    Observacoes =
                        dto.Observacoes,

                    IsAtivo = true,

                    DataCriacao =
                        DateTime.Now
                };

                reservaEntrada =
                    reserva;
            }

            // =================================================
            // PAGAMENTO DA LOCAÇÃO
            // =================================================

            else
            {
                var locacaoId =
                    dto.LocacaoId!.Value;

                var locacao =
                    await _locacaoRepository
                        .ObterPorIdAsync(locacaoId);

                if (locacao == null)
                {
                    throw new Exception(
                        "Locação não encontrada.");
                }

                // =============================================
                // LOCAÇÃO CANCELADA NÃO ACEITA NOVAS COBRANÇAS
                // =============================================

                if (locacao.Status ==
                    StatusLocacao.Cancelada)
                {
                    throw new Exception(
                        "Não é possível criar um pagamento " +
                        "para uma locação cancelada.");
                }

                var tipo =
                    NormalizarTipoPagamento(
                        dto.Tipo);

                // =============================================
                // SALDO PRINCIPAL SOMENTE APÓS FINALIZAÇÃO
                // =============================================

                if (tipo == "Locação" &&
                    locacao.Status !=
                        StatusLocacao.Finalizada)
                {
                    throw new Exception(
                        "O saldo principal da locação só pode " +
                        "ser cobrado após a finalização.");
                }

                var pagamentoExistente =
                    await _pagamentoRepository
                        .ObterAtivoPorLocacaoAsync(
                            locacaoId,
                            tipo);

                if (pagamentoExistente != null)
                {
                    var statusAtual =
                        pagamentoExistente.Status ==
                        StatusPagamento.Pago
                            ? "Pago"
                            : "Pendente";

                    throw new Exception(
                        $"A locação #{locacaoId} já possui " +
                        $"um pagamento do tipo {tipo} " +
                        $"#{pagamentoExistente.Id} " +
                        $"com status {statusAtual}.");
                }

                decimal valorPagamento;

                if (tipo == "Locação")
                {
                    valorPagamento =
                        await CalcularSaldoLocacaoAsync(
                            locacao.ReservaId,
                            locacao.ValorTotal);
                }
                else
                {
                    if (dto.Valor <= 0)
                    {
                        throw new Exception(
                            "O valor do pagamento deve ser maior que zero.");
                    }

                    valorPagamento =
                        Math.Round(
                            dto.Valor,
                            2,
                            MidpointRounding.AwayFromZero);
                }

                pagamento = new Pagamento
                {
                    LocacaoId = locacaoId,

                    ReservaId = null,

                    Tipo = tipo,

                    FormaPagamento =
                        dto.FormaPagamento,

                    Status =
                        dto.Status,

                    Valor =
                        valorPagamento,

                    DataPagamento =
                        dto.DataPagamento,

                    Observacoes =
                        dto.Observacoes,

                    IsAtivo = true,

                    DataCriacao =
                        DateTime.Now
                };
            }

            if (pagamento.Status ==
                    StatusPagamento.Pago &&
                pagamento.DataPagamento == null)
            {
                pagamento.DataPagamento =
                    DateTime.Now;
            }

            var pagamentoCriado =
                await _pagamentoRepository
                    .CriarAsync(pagamento);

            // =================================================
            // CONFIRMAR RESERVA E RESERVAR VEÍCULO
            // =================================================

            if (reservaEntrada != null &&
                pagamento.Status ==
                    StatusPagamento.Pago)
            {
                reservaEntrada.Status =
                    StatusReserva.Confirmada;

                reservaEntrada.IsAtiva =
                    true;

                reservaEntrada.DataCancelamento =
                    null;

                var reservaAtualizada =
                    await _reservaRepository
                        .AtualizarAsync(
                            reservaEntrada);

                if (!reservaAtualizada)
                {
                    throw new Exception(
                        "O pagamento foi registrado, mas não foi possível " +
                        "confirmar a reserva.");
                }

                await ReservarVeiculoAsync(
                    reservaEntrada);
            }

            return MapearParaDto(
                pagamentoCriado);
        }

        // =====================================================
        // ATUALIZAR
        // =====================================================

        public async Task<bool> AtualizarAsync(
            int id,
            CriarPagamentoDto dto)
        {
            var pagamento =
                await _pagamentoRepository
                    .ObterPorIdAsync(id);

            if (pagamento == null)
                return false;

            Reserva? reservaEntrada = null;

            if (pagamento.ReservaId.HasValue)
            {
                var reservaId =
                    pagamento.ReservaId.Value;

                var reserva =
                    await _reservaRepository
                        .ObterPorIdAsync(reservaId);

                if (reserva == null)
                    return false;

                pagamento.Valor =
                    CalcularValorEntrada(
                        reserva.ValorTotalPrevisto);

                pagamento.Tipo =
                    "Entrada";

                pagamento.ReservaId =
                    reservaId;

                pagamento.LocacaoId =
                    null;

                reservaEntrada =
                    reserva;
            }
            else if (pagamento.LocacaoId.HasValue)
            {
                var locacaoId =
                    pagamento.LocacaoId.Value;

                var locacao =
                    await _locacaoRepository
                        .ObterPorIdAsync(locacaoId);

                if (locacao == null)
                    return false;

                // =============================================
                // LOCAÇÃO CANCELADA
                //
                // Não permite nova cobrança nem alteração
                // financeira. Permite somente regularizar
                // pagamento existente como Cancelado/Estornado.
                // =============================================

                if (locacao.Status ==
                    StatusLocacao.Cancelada)
                {
                    if (dto.Status !=
                            StatusPagamento.Cancelado &&
                        dto.Status !=
                            StatusPagamento.Estornado)
                    {
                        throw new Exception(
                            "Uma locação cancelada permite apenas " +
                            "cancelar ou estornar pagamentos já existentes.");
                    }

                    var tipoOriginal =
                        NormalizarTipoPagamento(
                            pagamento.Tipo);

                    var tipoRecebido =
                        NormalizarTipoPagamento(
                            dto.Tipo);

                    if (!string.Equals(
                        tipoOriginal,
                        tipoRecebido,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        throw new Exception(
                            "Não é possível alterar o tipo do pagamento " +
                            "de uma locação cancelada.");
                    }

                    // Preserva o tipo e o valor originais.
                    pagamento.Tipo =
                        tipoOriginal;

                    pagamento.LocacaoId =
                        locacaoId;

                    pagamento.ReservaId =
                        null;
                }
                else
                {
                    var tipo =
                        NormalizarTipoPagamento(
                            dto.Tipo);

                    // =========================================
                    // SALDO PRINCIPAL SOMENTE APÓS FINALIZAÇÃO
                    // =========================================

                    if (tipo == "Locação" &&
                        locacao.Status !=
                            StatusLocacao.Finalizada)
                    {
                        throw new Exception(
                            "O saldo principal da locação só pode " +
                            "ser cobrado após a finalização.");
                    }

                    if (!string.Equals(
                        pagamento.Tipo,
                        tipo,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        var pagamentoExistente =
                            await _pagamentoRepository
                                .ObterAtivoPorLocacaoAsync(
                                    locacaoId,
                                    tipo);

                        if (pagamentoExistente != null &&
                            pagamentoExistente.Id != id)
                        {
                            throw new Exception(
                                $"A locação #{locacaoId} já possui " +
                                $"um pagamento ativo do tipo {tipo}.");
                        }
                    }

                    pagamento.Tipo =
                        tipo;

                    if (tipo == "Locação")
                    {
                        pagamento.Valor =
                            await CalcularSaldoLocacaoAsync(
                                locacao.ReservaId,
                                locacao.ValorTotal);
                    }
                    else
                    {
                        if (dto.Valor <= 0)
                        {
                            throw new Exception(
                                "O valor do pagamento deve ser maior que zero.");
                        }

                        pagamento.Valor =
                            Math.Round(
                                dto.Valor,
                                2,
                                MidpointRounding.AwayFromZero);
                    }

                    pagamento.LocacaoId =
                        locacaoId;

                    pagamento.ReservaId =
                        null;
                }
            }
            else
            {
                return false;
            }

            // =================================================
            // ENTRADA SENDO ALTERADA PARA PAGA
            // =================================================

            if (reservaEntrada != null &&
                dto.Status ==
                    StatusPagamento.Pago &&
                reservaEntrada.Status ==
                    StatusReserva.Pendente)
            {
                await ValidarDisponibilidadeParaConfirmacaoAsync(
                    reservaEntrada);
            }

            pagamento.FormaPagamento =
                dto.FormaPagamento;

            pagamento.Status =
                dto.Status;

            pagamento.DataPagamento =
                dto.DataPagamento;

            pagamento.Observacoes =
                dto.Observacoes;

            if (pagamento.Status ==
                    StatusPagamento.Pago &&
                pagamento.DataPagamento == null)
            {
                pagamento.DataPagamento =
                    DateTime.Now;
            }

            var atualizado =
                await _pagamentoRepository
                    .AtualizarAsync(pagamento);

            if (!atualizado)
                return false;

            // =================================================
            // ENTRADA ALTERADA PARA PAGA
            // =================================================

            if (reservaEntrada != null &&
                pagamento.Status ==
                    StatusPagamento.Pago &&
                reservaEntrada.Status ==
                    StatusReserva.Pendente)
            {
                reservaEntrada.Status =
                    StatusReserva.Confirmada;

                reservaEntrada.IsAtiva =
                    true;

                reservaEntrada.DataCancelamento =
                    null;

                var reservaAtualizada =
                    await _reservaRepository
                        .AtualizarAsync(
                            reservaEntrada);

                if (!reservaAtualizada)
                {
                    throw new Exception(
                        "O pagamento foi atualizado, mas não foi possível " +
                        "confirmar a reserva.");
                }

                await ReservarVeiculoAsync(
                    reservaEntrada);
            }

            return true;
        }

        // =====================================================
        // EXCLUIR
        // =====================================================

        public async Task<bool> ExcluirAsync(int id)
        {
            return await _pagamentoRepository
                .ExcluirAsync(id);
        }

        // =====================================================
        // VALIDAR / REALOCAR ANTES DA CONFIRMAÇÃO
        // =====================================================

        private async Task ValidarDisponibilidadeParaConfirmacaoAsync(
            Reserva reserva)
        {
            var veiculoAtual =
                await _veiculoRepository
                    .ObterPorIdAsync(reserva.VeiculoId);

            if (veiculoAtual == null)
            {
                throw new Exception(
                    "Veículo da reserva não encontrado.");
            }

            if (!veiculoAtual.IsAtivo)
            {
                throw new Exception(
                    "O veículo desta reserva está inativo.");
            }

            var unidadeAtualPodeSerUsada =
                veiculoAtual.FilialId ==
                    reserva.FilialRetiradaId &&
                (
                    veiculoAtual.Status ==
                        StatusVeiculo.Disponivel ||
                    veiculoAtual.Status ==
                        StatusVeiculo.Reservado
                );

            if (unidadeAtualPodeSerUsada)
            {
                var existeConflito =
                    await _reservaRepository
                        .ExisteConflitoAsync(
                            veiculoAtual.Id,
                            reserva.DataRetirada,
                            reserva.DataDevolucaoPrevista,
                            reserva.Id);

                if (!existeConflito)
                {
                    return;
                }
            }

            var todosVeiculos =
                await _veiculoRepository
                    .ListarTodosAsync();

            var unidadesCandidatas =
                todosVeiculos
                    .Where(v =>
                        v.Id != veiculoAtual.Id &&
                        v.IsAtivo &&
                        v.FilialId ==
                            reserva.FilialRetiradaId &&
                        (
                            v.Status ==
                                StatusVeiculo.Disponivel ||
                            v.Status ==
                                StatusVeiculo.Reservado
                        ) &&
                        MesmoGrupo(
                            v,
                            veiculoAtual))
                    .OrderBy(v => v.Id)
                    .ToList();

            foreach (var unidade in unidadesCandidatas)
            {
                var existeConflito =
                    await _reservaRepository
                        .ExisteConflitoAsync(
                            unidade.Id,
                            reserva.DataRetirada,
                            reserva.DataDevolucaoPrevista,
                            reserva.Id);

                if (existeConflito)
                    continue;

                reserva.VeiculoId =
                    unidade.Id;

                return;
            }

            throw new Exception(
                $"Não há mais unidades disponíveis de " +
                $"{veiculoAtual.Marca} {veiculoAtual.Modelo} " +
                "na filial selecionada para o período desta reserva.");
        }

        // =====================================================
        // RESERVAR VEÍCULO
        // =====================================================

        private async Task ReservarVeiculoAsync(
            Reserva reserva)
        {
            var veiculo =
                await _veiculoRepository
                    .ObterPorIdAsync(reserva.VeiculoId);

            if (veiculo == null)
            {
                throw new Exception(
                    "Veículo da reserva não encontrado.");
            }

            if (!veiculo.IsAtivo)
            {
                throw new Exception(
                    "O veículo da reserva está inativo.");
            }

            if (veiculo.Status ==
                StatusVeiculo.Reservado)
            {
                return;
            }

            if (veiculo.Status !=
                StatusVeiculo.Disponivel)
            {
                throw new Exception(
                    "O veículo da reserva não está disponível " +
                    "para ser reservado.");
            }

            veiculo.Status =
                StatusVeiculo.Reservado;

            var veiculoAtualizado =
                await _veiculoRepository
                    .AtualizarAsync(veiculo);

            if (!veiculoAtualizado)
            {
                throw new Exception(
                    "A reserva foi confirmada, mas não foi possível " +
                    "alterar o veículo para reservado.");
            }
        }

        // =====================================================
        // MESMO GRUPO DE VEÍCULO
        // =====================================================

        private static bool MesmoGrupo(
            Veiculo veiculo,
            Veiculo referencia)
        {
            return
                veiculo.CategoriaId ==
                    referencia.CategoriaId &&

                string.Equals(
                    veiculo.Marca,
                    referencia.Marca,
                    StringComparison.OrdinalIgnoreCase) &&

                string.Equals(
                    veiculo.Modelo,
                    referencia.Modelo,
                    StringComparison.OrdinalIgnoreCase) &&

                veiculo.AnoFabricacao ==
                    referencia.AnoFabricacao &&

                veiculo.AnoModelo ==
                    referencia.AnoModelo &&

                veiculo.ValorDiaria ==
                    referencia.ValorDiaria;
        }

        // =====================================================
        // NORMALIZAR TIPO
        // =====================================================

        private static string NormalizarTipoPagamento(
            string? tipo)
        {
            tipo =
                tipo?.Trim() ??
                string.Empty;

            if (string.Equals(
                tipo,
                "Locação",
                StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    tipo,
                    "Locacao",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "Locação";
            }

            if (string.Equals(
                tipo,
                "Multa",
                StringComparison.OrdinalIgnoreCase))
            {
                return "Multa";
            }

            if (string.Equals(
                tipo,
                "Adicional",
                StringComparison.OrdinalIgnoreCase))
            {
                return "Adicional";
            }

            throw new Exception(
                "Tipo de pagamento inválido. " +
                "Utilize Locação, Multa ou Adicional.");
        }

        // =====================================================
        // CALCULAR ENTRADA
        // =====================================================

        private static decimal CalcularValorEntrada(
            decimal valorTotal)
        {
            return Math.Round(
                valorTotal * 0.30m,
                2,
                MidpointRounding.AwayFromZero);
        }

        // =====================================================
        // CALCULAR SALDO RESTANTE
        // =====================================================

        private async Task<decimal>
            CalcularSaldoLocacaoAsync(
                int reservaId,
                decimal valorTotalLocacao)
        {
            var entrada =
                await _pagamentoRepository
                    .ObterAtivoPorReservaAsync(
                        reservaId);

            if (entrada == null)
            {
                throw new Exception(
                    "A reserva vinculada à locação " +
                    "não possui pagamento de entrada.");
            }

            if (!entrada.IsAtivo ||
                entrada.Status != StatusPagamento.Pago ||
                !string.Equals(
                    entrada.Tipo,
                    "Entrada",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "A entrada da reserva ainda não foi paga.");
            }

            var saldo =
                valorTotalLocacao -
                entrada.Valor;

            saldo =
                Math.Round(
                    saldo,
                    2,
                    MidpointRounding.AwayFromZero);

            if (saldo < 0)
            {
                throw new Exception(
                    "O valor da entrada é maior que " +
                    "o valor total da locação.");
            }

            if (saldo == 0)
            {
                throw new Exception(
                    "Esta locação não possui saldo restante para pagamento.");
            }

            return saldo;
        }

        // =====================================================
        // MAPEAR PARA DTO
        // =====================================================

        private static PagamentoDto MapearParaDto(
            Pagamento pagamento)
        {
            return new PagamentoDto
            {
                Id =
                    pagamento.Id,

                LocacaoId =
                    pagamento.LocacaoId,

                ReservaId =
                    pagamento.ReservaId,

                Tipo =
                    pagamento.Tipo,

                FormaPagamento =
                    pagamento.FormaPagamento,

                Status =
                    pagamento.Status,

                Valor =
                    pagamento.Valor,

                DataPagamento =
                    pagamento.DataPagamento,

                Observacoes =
                    pagamento.Observacoes,

                IsAtivo =
                    pagamento.IsAtivo,

                DataCriacao =
                    pagamento.DataCriacao
            };
        }
    }
}