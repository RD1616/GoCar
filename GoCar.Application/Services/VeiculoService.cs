using GoCar.Application.DTOs.Veiculo;
using GoCar.Application.Interfaces;
using GoCar.Domain.Entities;
using GoCar.Domain.Enums;

namespace GoCar.Application.Services
{
    public class VeiculoService : IVeiculoService
    {
        private readonly IVeiculoRepository _repository;

        public VeiculoService(
            IVeiculoRepository repository)
        {
            _repository = repository;
        }

        // =====================================================
        // LISTAR TODOS
        // =====================================================

        public async Task<IEnumerable<VeiculoDto>>
            ListarTodosAsync()
        {
            var veiculos =
                await _repository.ListarTodosAsync();

            return veiculos.Select(
                v => MapearParaDto(v));
        }

        // =====================================================
        // OBTER POR ID
        // =====================================================

        public async Task<VeiculoDto?> ObterPorIdAsync(
            int id)
        {
            var veiculo =
                await _repository.ObterPorIdAsync(id);

            if (veiculo == null)
                return null;

            return MapearParaDto(veiculo);
        }

        // =====================================================
        // CRIAR
        // =====================================================

        public async Task<VeiculoDto> CriarAsync(
            CriarVeiculoDto dto)
        {
            var veiculo =
                new Veiculo
                {
                    Placa =
                        dto.Placa,

                    Chassi =
                        dto.Chassi,

                    Renavam =
                        dto.Renavam,

                    Modelo =
                        dto.Modelo,

                    Marca =
                        dto.Marca,

                    AnoFabricacao =
                        dto.AnoFabricacao,

                    AnoModelo =
                        dto.AnoModelo,

                    Cor =
                        dto.Cor,

                    Combustivel =
                        dto.Combustivel,

                    Cambio =
                        dto.Cambio,

                    Status =
                        dto.Status,

                    KmAtual =
                        dto.KmAtual,

                    ValorDiaria =
                        dto.ValorDiaria,

                    CategoriaId =
                        dto.CategoriaId,

                    FilialId =
                        dto.FilialId,

                    IsAtivo =
                        true
                };

            var veiculoCriado =
                await _repository
                    .CriarAsync(veiculo);

            return MapearParaDto(
                veiculoCriado);
        }

        // =====================================================
        // ATUALIZAR
        // =====================================================

        public async Task<bool> AtualizarAsync(
            int id,
            AtualizarVeiculoDto dto)
        {
            var veiculo =
                await _repository
                    .ObterPorIdAsync(id);

            if (veiculo == null)
                return false;

            veiculo.Placa =
                dto.Placa;

            veiculo.Chassi =
                dto.Chassi;

            veiculo.Renavam =
                dto.Renavam;

            veiculo.Modelo =
                dto.Modelo;

            veiculo.Marca =
                dto.Marca;

            veiculo.AnoFabricacao =
                dto.AnoFabricacao;

            veiculo.AnoModelo =
                dto.AnoModelo;

            veiculo.Cor =
                dto.Cor;

            veiculo.Combustivel =
                dto.Combustivel;

            veiculo.Cambio =
                dto.Cambio;

            veiculo.Status =
                dto.Status;

            veiculo.KmAtual =
                dto.KmAtual;

            veiculo.ValorDiaria =
                dto.ValorDiaria;

            veiculo.CategoriaId =
                dto.CategoriaId;

            veiculo.FilialId =
                dto.FilialId;

            return await _repository
                .AtualizarAsync(veiculo);
        }

        // =====================================================
        // DESATIVAR
        // =====================================================

        public async Task<bool> ExcluirAsync(
            int id)
        {
            var veiculo =
                await _repository
                    .ObterPorIdAsync(id);

            if (veiculo == null)
                return false;

            // =================================================
            // NÃO PERMITIR DESATIVAR VEÍCULO RESERVADO
            // =================================================

            if (veiculo.Status ==
                StatusVeiculo.Reservado)
            {
                throw new Exception(
                    "Não é possível desativar um veículo " +
                    "que possui uma reserva ativa.");
            }

            // =================================================
            // NÃO PERMITIR DESATIVAR VEÍCULO ALUGADO
            // =================================================

            if (veiculo.Status ==
                StatusVeiculo.Alugado)
            {
                throw new Exception(
                    "Não é possível desativar um veículo " +
                    "que está em uma locação ativa.");
            }

            // =================================================
            // DESATIVAR
            // =================================================

            return await _repository
                .ExcluirAsync(id);
        }

        // =====================================================
        // ATIVAR
        // =====================================================

        public async Task<bool> AtivarAsync(
            int id)
        {
            var veiculo =
                await _repository
                    .ObterPorIdAsync(id);

            if (veiculo == null)
                return false;

            veiculo.IsAtivo =
                true;

            if (veiculo.Status ==
                StatusVeiculo.Inativo)
            {
                veiculo.Status =
                    StatusVeiculo.Disponivel;
            }

            return await _repository
                .AtualizarAsync(veiculo);
        }

        // =====================================================
        // MAPEAR PARA DTO
        // =====================================================

        private static VeiculoDto MapearParaDto(
            Veiculo veiculo)
        {
            return new VeiculoDto
            {
                Id =
                    veiculo.Id,

                Placa =
                    veiculo.Placa,

                Chassi =
                    veiculo.Chassi,

                Renavam =
                    veiculo.Renavam,

                Modelo =
                    veiculo.Modelo,

                Marca =
                    veiculo.Marca,

                AnoFabricacao =
                    veiculo.AnoFabricacao,

                AnoModelo =
                    veiculo.AnoModelo,

                Cor =
                    veiculo.Cor,

                Combustivel =
                    veiculo.Combustivel,

                Cambio =
                    veiculo.Cambio,

                Status =
                    veiculo.Status,

                KmAtual =
                    veiculo.KmAtual,

                ValorDiaria =
                    veiculo.ValorDiaria,

                IsAtivo =
                    veiculo.IsAtivo,

                CategoriaId =
                    veiculo.CategoriaId,

                FilialId =
                    veiculo.FilialId
            };
        }
    }
}