using GoCar.Application.DTOs.Reserva;
using GoCar.Application.Interfaces;
using GoCar.Domain.Entities;
using GoCar.Domain.Enums;

namespace GoCar.Application.Services
{
    public class ReservaService : IReservaService
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly IVeiculoRepository _veiculoRepository;
        private readonly IPagamentoRepository _pagamentoRepository;

        public ReservaService(
            IReservaRepository reservaRepository,
            IVeiculoRepository veiculoRepository,
            IPagamentoRepository pagamentoRepository)
        {
            _reservaRepository = reservaRepository;
            _veiculoRepository = veiculoRepository;
            _pagamentoRepository = pagamentoRepository;
        }

        // =====================================================
        // LISTAR TODOS
        // =====================================================

        public async Task<IEnumerable<ReservaDto>> ListarTodosAsync()
        {
            await ExpirarReservasVencidasAsync();

            var reservas =
                await _reservaRepository.ListarTodosAsync();

            return reservas.Select(MapearParaDto);
        }

        // =====================================================
        // LISTAR POR CLIENTE
        // =====================================================

        public async Task<IEnumerable<ReservaDto>> ListarPorClienteIdAsync(
            int clienteId)
        {
            await ExpirarReservasVencidasAsync();

            var reservas =
                await _reservaRepository
                    .ListarPorClienteIdAsync(clienteId);

            return reservas.Select(MapearParaDto);
        }

        // =====================================================
        // OBTER POR ID
        // =====================================================

        public async Task<ReservaDto?> ObterPorIdAsync(
            int id)
        {
            await ExpirarReservasVencidasAsync();

            var reserva =
                await _reservaRepository.ObterPorIdAsync(id);

            if (reserva == null)
                return null;

            return MapearParaDto(reserva);
        }

        // =====================================================
        // VERIFICAR DISPONIBILIDADE
        // =====================================================

        public async Task<bool> VerificarDisponibilidadeAsync(
            int veiculoId,
            int filialRetiradaId,
            DateTime dataRetirada,
            DateTime dataDevolucaoPrevista)
        {
            if (dataRetirada <= DateTime.Now)
                return false;

            if (dataDevolucaoPrevista <= dataRetirada)
                return false;

            var veiculoReferencia =
                await _veiculoRepository
                    .ObterPorIdAsync(veiculoId);

            if (veiculoReferencia == null ||
                !veiculoReferencia.IsAtivo)
            {
                return false;
            }

            var veiculoDisponivel =
                await BuscarUnidadeDisponivelAsync(
                    veiculoReferencia,
                    filialRetiradaId,
                    dataRetirada,
                    dataDevolucaoPrevista);

            return veiculoDisponivel != null;
        }

        // =====================================================
        // OBTER QUANTIDADE DISPONÍVEL
        // =====================================================

        public async Task<int> ObterQuantidadeDisponivelAsync(
            int veiculoId,
            int filialRetiradaId,
            DateTime dataRetirada,
            DateTime dataDevolucaoPrevista)
        {
            if (veiculoId <= 0 ||
                filialRetiradaId <= 0 ||
                dataRetirada <= DateTime.Now ||
                dataDevolucaoPrevista <= dataRetirada)
            {
                return 0;
            }

            var veiculoReferencia =
                await _veiculoRepository
                    .ObterPorIdAsync(veiculoId);

            if (veiculoReferencia == null ||
                !veiculoReferencia.IsAtivo)
            {
                return 0;
            }

            var todosVeiculos =
                await _veiculoRepository
                    .ListarTodosAsync();

            var unidades =
                todosVeiculos
                    .Where(v =>
                        v.IsAtivo &&
                        v.FilialId == filialRetiradaId &&
                        (
                            v.Status == StatusVeiculo.Disponivel ||
                            v.Status == StatusVeiculo.Reservado
                        ) &&
                        MesmoGrupo(v, veiculoReferencia))
                    .ToList();

            var quantidadeDisponivel = 0;

            foreach (var unidade in unidades)
            {
                var existeConflito =
                    await _reservaRepository
                        .ExisteConflitoAsync(
                            unidade.Id,
                            dataRetirada,
                            dataDevolucaoPrevista);

                if (!existeConflito)
                {
                    quantidadeDisponivel++;
                }
            }

            return quantidadeDisponivel;
        }

        // =====================================================
        // CRIAR RESERVA
        // =====================================================

        public async Task<ReservaDto> CriarAsync(
            CriarReservaDto dto)
        {
            if (dto.DataDevolucaoPrevista <= dto.DataRetirada)
            {
                throw new Exception(
                    "A data de devolução deve ser posterior à data de retirada.");
            }

            if (dto.DataRetirada <= DateTime.Now)
            {
                throw new Exception(
                    "A data de retirada deve ser futura.");
            }

            var veiculoReferencia =
                await _veiculoRepository
                    .ObterPorIdAsync(dto.VeiculoId);

            if (veiculoReferencia == null)
            {
                throw new Exception(
                    "Veículo não encontrado.");
            }

            if (!veiculoReferencia.IsAtivo)
            {
                throw new Exception(
                    "Veículo não está ativo.");
            }

            var veiculoDisponivel =
                await BuscarUnidadeDisponivelAsync(
                    veiculoReferencia,
                    dto.FilialRetiradaId,
                    dto.DataRetirada,
                    dto.DataDevolucaoPrevista);

            if (veiculoDisponivel == null)
            {
                throw new Exception(
                    $"Não há unidades disponíveis de " +
                    $"{veiculoReferencia.Marca} " +
                    $"{veiculoReferencia.Modelo} " +
                    "na filial selecionada para o período informado.");
            }

            var valorTotalPrevisto =
                CalcularValorPrevisto(
                    veiculoDisponivel.ValorDiaria,
                    dto.DataRetirada,
                    dto.DataDevolucaoPrevista);

            var reserva = new Reserva
            {
                ClienteId = dto.ClienteId,
                VeiculoId = veiculoDisponivel.Id,
                FilialRetiradaId = dto.FilialRetiradaId,
                FilialDevolucaoId = dto.FilialDevolucaoId,
                DataReserva = DateTime.Now,
                DataRetirada = dto.DataRetirada,
                DataDevolucaoPrevista = dto.DataDevolucaoPrevista,
                Status = StatusReserva.Pendente,
                ValorTotalPrevisto = valorTotalPrevisto,
                Observacoes = dto.Observacoes,
                IsAtiva = true,
                DataCancelamento = null
            };

            var reservaCriada =
                await _reservaRepository
                    .CriarAsync(reserva);

            var reservaCompleta =
                await _reservaRepository
                    .ObterPorIdAsync(reservaCriada.Id);

            return MapearParaDto(
                reservaCompleta ?? reservaCriada);
        }

        // =====================================================
        // ATUALIZAR RESERVA
        // =====================================================

        public async Task<bool> AtualizarAsync(
            int id,
            CriarReservaDto dto)
        {
            var reserva =
                await _reservaRepository
                    .ObterPorIdAsync(id);

            if (reserva == null)
                return false;

            if (reserva.Status == StatusReserva.Cancelada ||
                reserva.Status == StatusReserva.Concluida ||
                reserva.Status == StatusReserva.Expirada)
            {
                throw new InvalidOperationException(
                    "Esta reserva não pode mais ser alterada.");
            }

            if (dto.DataDevolucaoPrevista <= dto.DataRetirada)
            {
                throw new Exception(
                    "A data de devolução deve ser posterior à data de retirada.");
            }

            if (dto.DataRetirada <= DateTime.Now)
            {
                throw new Exception(
                    "A data de retirada deve ser futura.");
            }

            var veiculoReferencia =
                await _veiculoRepository
                    .ObterPorIdAsync(dto.VeiculoId);

            if (veiculoReferencia == null)
            {
                throw new Exception(
                    "Veículo não encontrado.");
            }

            if (!veiculoReferencia.IsAtivo)
            {
                throw new Exception(
                    "Veículo não está ativo.");
            }

            var veiculoAtual =
                await _veiculoRepository
                    .ObterPorIdAsync(reserva.VeiculoId);

            Veiculo? veiculoDisponivel = null;

            if (veiculoAtual != null &&
                veiculoAtual.IsAtivo &&
                veiculoAtual.FilialId == dto.FilialRetiradaId &&
                (
                    veiculoAtual.Status == StatusVeiculo.Disponivel ||
                    veiculoAtual.Status == StatusVeiculo.Reservado
                ) &&
                MesmoGrupo(veiculoAtual, veiculoReferencia))
            {
                var conflitoAtual =
                    await _reservaRepository
                        .ExisteConflitoAsync(
                            veiculoAtual.Id,
                            dto.DataRetirada,
                            dto.DataDevolucaoPrevista,
                            id);

                if (!conflitoAtual)
                {
                    veiculoDisponivel =
                        veiculoAtual;
                }
            }

            if (veiculoDisponivel == null)
            {
                veiculoDisponivel =
                    await BuscarUnidadeDisponivelAsync(
                        veiculoReferencia,
                        dto.FilialRetiradaId,
                        dto.DataRetirada,
                        dto.DataDevolucaoPrevista,
                        id);
            }

            if (veiculoDisponivel == null)
            {
                throw new Exception(
                    $"Não há unidades disponíveis de " +
                    $"{veiculoReferencia.Marca} " +
                    $"{veiculoReferencia.Modelo} " +
                    "na filial selecionada para o período informado.");
            }

            var valorTotalPrevisto =
                CalcularValorPrevisto(
                    veiculoDisponivel.ValorDiaria,
                    dto.DataRetirada,
                    dto.DataDevolucaoPrevista);

            reserva.ClienteId = dto.ClienteId;
            reserva.VeiculoId = veiculoDisponivel.Id;
            reserva.FilialRetiradaId = dto.FilialRetiradaId;
            reserva.FilialDevolucaoId = dto.FilialDevolucaoId;
            reserva.DataRetirada = dto.DataRetirada;
            reserva.DataDevolucaoPrevista = dto.DataDevolucaoPrevista;
            reserva.ValorTotalPrevisto = valorTotalPrevisto;
            reserva.Observacoes = dto.Observacoes;

            return await _reservaRepository
                .AtualizarAsync(reserva);
        }

        // =====================================================
        // CONFIRMAR RESERVA
        // =====================================================

        public async Task<bool> ConfirmarAsync(
            int id)
        {
            var reserva =
                await _reservaRepository
                    .ObterPorIdAsync(id);

            if (reserva == null)
                return false;

            if (reserva.Status != StatusReserva.Pendente)
            {
                throw new InvalidOperationException(
                    "Somente reservas pendentes podem ser confirmadas.");
            }

            var pagamentoEntrada =
                await _pagamentoRepository
                    .ObterAtivoPorReservaAsync(reserva.Id);

            if (pagamentoEntrada == null ||
                !pagamentoEntrada.IsAtivo ||
                pagamentoEntrada.Status != StatusPagamento.Pago ||
                !string.Equals(
                    pagamentoEntrada.Tipo,
                    "Entrada",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "A reserva só pode ser confirmada após o pagamento da entrada de 30%.");
            }

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

            if (veiculo.Status != StatusVeiculo.Disponivel &&
                veiculo.Status != StatusVeiculo.Reservado)
            {
                throw new Exception(
                    "O veículo da reserva não está disponível.");
            }

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
                return false;

            if (veiculo.Status == StatusVeiculo.Disponivel)
            {
                veiculo.Status =
                    StatusVeiculo.Reservado;

                var veiculoAtualizado =
                    await _veiculoRepository
                        .AtualizarAsync(veiculo);

                if (!veiculoAtualizado)
                {
                    throw new Exception(
                        "A reserva foi confirmada, mas não foi possível reservar o veículo.");
                }
            }

            return true;
        }

        // =====================================================
        // CANCELAR RESERVA
        // =====================================================

        public async Task<bool> CancelarAsync(
            int id)
        {
            var reserva =
                await _reservaRepository
                    .ObterPorIdAsync(id);

            if (reserva == null)
                return false;

            if (reserva.Status != StatusReserva.Pendente &&
                reserva.Status != StatusReserva.Confirmada)
            {
                throw new InvalidOperationException(
                    "Somente reservas pendentes ou confirmadas podem ser canceladas.");
            }

            var veiculoId =
                reserva.VeiculoId;

            var estavaConfirmada =
                reserva.Status == StatusReserva.Confirmada;

            reserva.Status =
                StatusReserva.Cancelada;

            reserva.IsAtiva =
                false;

            reserva.DataCancelamento =
                DateTime.Now;

            var reservaAtualizada =
                await _reservaRepository
                    .AtualizarAsync(reserva);

            if (!reservaAtualizada)
                return false;

            // =================================================
            // SINCRONIZAR STATUS FÍSICO DA UNIDADE
            // =================================================

            if (estavaConfirmada)
            {
                await SincronizarStatusVeiculoReservasAsync(
                    veiculoId,
                    reserva.Id);
            }

            return true;
        }

        // =====================================================
        // CONCLUIR RESERVA
        // =====================================================

        public async Task<bool> ConcluirAsync(
            int id)
        {
            var reserva =
                await _reservaRepository
                    .ObterPorIdAsync(id);

            if (reserva == null)
                return false;

            if (reserva.Status != StatusReserva.Confirmada)
            {
                throw new InvalidOperationException(
                    "Somente reservas confirmadas podem ser concluídas.");
            }

            reserva.Status =
                StatusReserva.Concluida;

            reserva.IsAtiva =
                false;

            reserva.DataCancelamento =
                null;

            return await _reservaRepository
                .AtualizarAsync(reserva);
        }

        // =====================================================
        // EXPIRAR RESERVAS VENCIDAS
        // =====================================================

        public async Task<int> ExpirarReservasVencidasAsync()
        {
            var reservas =
                await _reservaRepository
                    .ListarTodosAsync();

            var agora =
                DateTime.Now;

            var reservasVencidas =
                reservas
                    .Where(r =>
                        r.IsAtiva &&
                        r.Status == StatusReserva.Confirmada &&
                        r.DataDevolucaoPrevista < agora)
                    .ToList();

            var quantidadeExpirada = 0;

            foreach (var reserva in reservasVencidas)
            {
                var veiculoId =
                    reserva.VeiculoId;

                reserva.Status =
                    StatusReserva.Expirada;

                reserva.IsAtiva =
                    false;

                reserva.DataCancelamento =
                    null;

                var atualizada =
                    await _reservaRepository
                        .AtualizarAsync(reserva);

                if (!atualizada)
                    continue;

                quantidadeExpirada++;

                await SincronizarStatusVeiculoReservasAsync(
                    veiculoId,
                    reserva.Id);
            }

            return quantidadeExpirada;
        }

        // =====================================================
        // SINCRONIZAR STATUS DO VEÍCULO COM AS RESERVAS
        // =====================================================

        private async Task SincronizarStatusVeiculoReservasAsync(
            int veiculoId,
            int reservaIgnorarId)
        {
            var veiculo =
                await _veiculoRepository
                    .ObterPorIdAsync(veiculoId);

            if (veiculo == null ||
                !veiculo.IsAtivo)
            {
                return;
            }

            // Se o carro estiver em locação, uma reserva
            // cancelada/expirada não pode torná-lo disponível.
            if (veiculo.Status == StatusVeiculo.Alugado)
            {
                return;
            }

            var possuiOutraReservaConfirmada =
                await _reservaRepository
                    .ExisteOutraReservaConfirmadaAsync(
                        veiculoId,
                        reservaIgnorarId);

            var novoStatus =
                possuiOutraReservaConfirmada
                    ? StatusVeiculo.Reservado
                    : StatusVeiculo.Disponivel;

            if (veiculo.Status == novoStatus)
            {
                return;
            }

            veiculo.Status =
                novoStatus;

            var atualizado =
                await _veiculoRepository
                    .AtualizarAsync(veiculo);

            if (!atualizado)
            {
                throw new Exception(
                    "Não foi possível sincronizar o status do veículo.");
            }
        }

        // =====================================================
        // EXCLUIR
        // =====================================================

        public async Task<bool> ExcluirAsync(
            int id)
        {
            return await _reservaRepository
                .ExcluirAsync(id);
        }

        // =====================================================
        // BUSCAR UNIDADE DISPONÍVEL
        // =====================================================

        private async Task<Veiculo?> BuscarUnidadeDisponivelAsync(
            Veiculo veiculoReferencia,
            int filialRetiradaId,
            DateTime dataRetirada,
            DateTime dataDevolucaoPrevista,
            int? reservaIgnorarId = null)
        {
            var todosVeiculos =
                await _veiculoRepository
                    .ListarTodosAsync();

            var unidades =
                todosVeiculos
                    .Where(v =>
                        v.IsAtivo &&
                        v.FilialId == filialRetiradaId &&
                        (
                            v.Status == StatusVeiculo.Disponivel ||
                            v.Status == StatusVeiculo.Reservado
                        ) &&
                        MesmoGrupo(v, veiculoReferencia))
                    .OrderBy(v => v.Id)
                    .ToList();

            foreach (var unidade in unidades)
            {
                bool existeConflito;

                if (reservaIgnorarId.HasValue)
                {
                    existeConflito =
                        await _reservaRepository
                            .ExisteConflitoAsync(
                                unidade.Id,
                                dataRetirada,
                                dataDevolucaoPrevista,
                                reservaIgnorarId.Value);
                }
                else
                {
                    existeConflito =
                        await _reservaRepository
                            .ExisteConflitoAsync(
                                unidade.Id,
                                dataRetirada,
                                dataDevolucaoPrevista);
                }

                if (!existeConflito)
                {
                    return unidade;
                }
            }

            return null;
        }

        // =====================================================
        // MESMO GRUPO
        // =====================================================

        private static bool MesmoGrupo(
            Veiculo veiculo,
            Veiculo referencia)
        {
            return
                veiculo.CategoriaId == referencia.CategoriaId &&

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
        // CALCULAR VALOR
        // =====================================================

        private static decimal CalcularValorPrevisto(
            decimal valorDiaria,
            DateTime dataRetirada,
            DateTime dataDevolucaoPrevista)
        {
            var diferenca =
                dataDevolucaoPrevista -
                dataRetirada;

            var quantidadeDiarias =
                Math.Max(
                    1,
                    (int)Math.Ceiling(
                        diferenca.TotalDays));

            return valorDiaria *
                   quantidadeDiarias;
        }

        // =====================================================
        // MAPEAR PARA DTO
        // =====================================================

        private static ReservaDto MapearParaDto(
            Reserva reserva)
        {
            return new ReservaDto
            {
                Id = reserva.Id,
                ClienteId = reserva.ClienteId,
                ClienteNome = reserva.Cliente?.Nome,

                VeiculoId = reserva.VeiculoId,

                VeiculoNome =
                    reserva.Veiculo == null
                        ? null
                        : $"{reserva.Veiculo.Marca} {reserva.Veiculo.Modelo}",

                VeiculoPlaca =
                    reserva.Veiculo?.Placa,

                FilialRetiradaId =
                    reserva.FilialRetiradaId,

                FilialRetiradaNome =
                    reserva.FilialRetirada?.Nome,

                FilialDevolucaoId =
                    reserva.FilialDevolucaoId,

                FilialDevolucaoNome =
                    reserva.FilialDevolucao?.Nome,

                DataReserva =
                    reserva.DataReserva,

                DataRetirada =
                    reserva.DataRetirada,

                DataDevolucaoPrevista =
                    reserva.DataDevolucaoPrevista,

                Status =
                    reserva.Status,

                ValorTotalPrevisto =
                    reserva.ValorTotalPrevisto,

                Observacoes =
                    reserva.Observacoes,

                IsAtiva =
                    reserva.IsAtiva,

                DataCancelamento =
                    reserva.DataCancelamento
            };
        }
    }
}