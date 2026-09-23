using GoCar.Application.DTOs.Filial;
using GoCar.Application.Interfaces;
using GoCar.Domain.Entities;

namespace GoCar.Application.Services
{
    public class FilialService : IFilialService
    {
        private readonly IFilialRepository _repository;
        private readonly IVeiculoRepository _veiculoRepository;

        public FilialService(
            IFilialRepository repository,
            IVeiculoRepository veiculoRepository)
        {
            _repository = repository;
            _veiculoRepository = veiculoRepository;
        }

        // =====================================================
        // LISTAR TODOS
        // =====================================================

        public async Task<IEnumerable<FilialDto>>
            ListarTodosAsync()
        {
            var filiais =
                await _repository.ListarTodosAsync();

            return filiais.Select(MapearParaDto);
        }

        // =====================================================
        // OBTER POR ID
        // =====================================================

        public async Task<FilialDto?> ObterPorIdAsync(
            int id)
        {
            var filial =
                await _repository.ObterPorIdAsync(id);

            if (filial == null)
                return null;

            return MapearParaDto(filial);
        }

        // =====================================================
        // CRIAR
        // =====================================================

        public async Task<FilialDto> CriarAsync(
            CriarFilialDto dto)
        {
            var filial = new Filial
            {
                Nome =
                    dto.Nome,

                CNPJ =
                    dto.CNPJ,

                Telefone =
                    dto.Telefone,

                Email =
                    dto.Email,

                Endereco =
                    dto.Endereco,

                Numero =
                    dto.Numero,

                Bairro =
                    dto.Bairro,

                Cidade =
                    dto.Cidade,

                Estado =
                    dto.Estado,

                CEP =
                    dto.CEP,

                IsAtivo =
                    true
            };

            var filialCriada =
                await _repository.CriarAsync(filial);

            return MapearParaDto(
                filialCriada);
        }

        // =====================================================
        // ATUALIZAR
        // =====================================================

        public async Task<bool> AtualizarAsync(
            int id,
            AtualizarFilialDto dto)
        {
            var filial =
                await _repository.ObterPorIdAsync(id);

            if (filial == null)
                return false;

            filial.Nome =
                dto.Nome;

            filial.CNPJ =
                dto.CNPJ;

            filial.Telefone =
                dto.Telefone;

            filial.Email =
                dto.Email;

            filial.Endereco =
                dto.Endereco;

            filial.Numero =
                dto.Numero;

            filial.Bairro =
                dto.Bairro;

            filial.Cidade =
                dto.Cidade;

            filial.Estado =
                dto.Estado;

            filial.CEP =
                dto.CEP;

            filial.IsAtivo =
                dto.IsAtivo;

            return await _repository
                .AtualizarAsync(filial);
        }

        // =====================================================
        // EXCLUIR / DESATIVAR
        // =====================================================

        public async Task<bool> ExcluirAsync(
            int id)
        {
            var filial =
                await _repository.ObterPorIdAsync(id);

            if (filial == null)
                return false;

            var veiculos =
                await _veiculoRepository
                    .ListarTodosAsync();

            var possuiVeiculoAtivo =
                veiculos.Any(v =>
                    v.FilialId == id &&
                    v.IsAtivo);

            if (possuiVeiculoAtivo)
            {
                throw new InvalidOperationException(
                    "Não é possível desativar a filial porque existem veículos ativos vinculados a ela.");
            }

            return await _repository
                .ExcluirAsync(id);
        }

        // =====================================================
        // MAPEAR PARA DTO
        // =====================================================

        private static FilialDto MapearParaDto(
            Filial filial)
        {
            return new FilialDto
            {
                Id =
                    filial.Id,

                Nome =
                    filial.Nome,

                CNPJ =
                    filial.CNPJ,

                Telefone =
                    filial.Telefone,

                Email =
                    filial.Email,

                Endereco =
                    filial.Endereco,

                Numero =
                    filial.Numero,

                Bairro =
                    filial.Bairro,

                Cidade =
                    filial.Cidade,

                Estado =
                    filial.Estado,

                CEP =
                    filial.CEP,

                IsAtivo =
                    filial.IsAtivo
            };
        }
    }
}