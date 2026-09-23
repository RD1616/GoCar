using GoCar.Application.DTOs.Categoria;
using GoCar.Application.Interfaces;
using GoCar.Domain.Entities;

namespace GoCar.Application.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _repository;
        private readonly IVeiculoRepository _veiculoRepository;

        public CategoriaService(
            ICategoriaRepository repository,
            IVeiculoRepository veiculoRepository)
        {
            _repository = repository;
            _veiculoRepository = veiculoRepository;
        }

        // =====================================================
        // LISTAR TODOS
        // =====================================================

        public async Task<IEnumerable<CategoriaDto>>
            ListarTodosAsync()
        {
            var categorias =
                await _repository.ListarTodosAsync();

            return categorias.Select(MapearParaDto);
        }

        // =====================================================
        // OBTER POR ID
        // =====================================================

        public async Task<CategoriaDto?> ObterPorIdAsync(
            int id)
        {
            var categoria =
                await _repository.ObterPorIdAsync(id);

            if (categoria == null)
                return null;

            return MapearParaDto(categoria);
        }

        // =====================================================
        // CRIAR
        // =====================================================

        public async Task<CategoriaDto> CriarAsync(
            CriarCategoriaDto dto)
        {
            var categoria = new Categoria
            {
                Nome =
                    dto.Nome,

                Descricao =
                    dto.Descricao,

                DiariaBase =
                    dto.DiariaBase,

                KmLivre =
                    dto.KmLivre,

                IsAtivo =
                    true
            };

            var categoriaCriada =
                await _repository
                    .CriarAsync(categoria);

            return MapearParaDto(
                categoriaCriada);
        }

        // =====================================================
        // ATUALIZAR
        // =====================================================

        public async Task<bool> AtualizarAsync(
            int id,
            AtualizarCategoriaDto dto)
        {
            var categoria =
                await _repository.ObterPorIdAsync(id);

            if (categoria == null)
                return false;

            categoria.Nome =
                dto.Nome;

            categoria.Descricao =
                dto.Descricao;

            categoria.DiariaBase =
                dto.DiariaBase;

            categoria.KmLivre =
                dto.KmLivre;

            categoria.IsAtivo =
                dto.IsAtivo;

            return await _repository
                .AtualizarAsync(categoria);
        }

        // =====================================================
        // EXCLUIR / DESATIVAR
        // =====================================================

        public async Task<bool> ExcluirAsync(
            int id)
        {
            var categoria =
                await _repository.ObterPorIdAsync(id);

            if (categoria == null)
                return false;

            var veiculos =
                await _veiculoRepository
                    .ListarTodosAsync();

            var possuiVeiculoAtivo =
                veiculos.Any(v =>
                    v.CategoriaId == id &&
                    v.IsAtivo);

            if (possuiVeiculoAtivo)
            {
                throw new InvalidOperationException(
                    "Não é possível desativar a categoria porque existem veículos ativos vinculados a ela.");
            }

            return await _repository
                .ExcluirAsync(id);
        }

        // =====================================================
        // MAPEAR PARA DTO
        // =====================================================

        private static CategoriaDto MapearParaDto(
            Categoria categoria)
        {
            return new CategoriaDto
            {
                Id =
                    categoria.Id,

                Nome =
                    categoria.Nome,

                Descricao =
                    categoria.Descricao,

                DiariaBase =
                    categoria.DiariaBase,

                KmLivre =
                    categoria.KmLivre,

                IsAtivo =
                    categoria.IsAtivo
            };
        }
    }
}