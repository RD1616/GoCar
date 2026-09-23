using GoCar.Application.DTOs.Cliente;
using GoCar.Application.Interfaces;
using GoCar.Domain.Entities;
using GoCar.Domain.Enums;

namespace GoCar.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _repository;
        private readonly IReservaRepository _reservaRepository;
        private readonly ILocacaoRepository _locacaoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public ClienteService(
            IClienteRepository repository,
            IReservaRepository reservaRepository,
            ILocacaoRepository locacaoRepository,
            IUsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _reservaRepository = reservaRepository;
            _locacaoRepository = locacaoRepository;
            _usuarioRepository = usuarioRepository;
        }

        // =====================================================
        // LISTAR TODOS
        // =====================================================

        public async Task<IEnumerable<ClienteDto>>
            ListarTodosAsync()
        {
            var clientes =
                await _repository.ListarTodosAsync();

            return clientes.Select(
                c => MapearParaDto(c));
        }

        // =====================================================
        // OBTER POR ID
        // =====================================================

        public async Task<ClienteDto?> ObterPorIdAsync(
            int id)
        {
            var cliente =
                await _repository.ObterPorIdAsync(id);

            if (cliente == null)
                return null;

            return MapearParaDto(cliente);
        }

        // =====================================================
        // OBTER POR USUÁRIO
        // =====================================================

        public async Task<ClienteDto?> ObterPorUsuarioIdAsync(
            int usuarioId)
        {
            var cliente =
                await _repository
                    .ObterPorUsuarioIdAsync(usuarioId);

            if (cliente == null)
                return null;

            return MapearParaDto(cliente);
        }

        // =====================================================
        // CRIAR
        // =====================================================

        public async Task<ClienteDto> CriarAsync(
            CriarClienteDto dto)
        {
            var cliente =
                new Cliente
                {
                    UsuarioId =
                        dto.UsuarioId,

                    Nome =
                        dto.Nome,

                    CPF =
                        dto.CPF,

                    DataNascimento =
                        dto.DataNascimento,

                    Telefone =
                        dto.Telefone,

                    CNH =
                        dto.CNH,

                    CategoriaCNH =
                        dto.CategoriaCNH,

                    DataValidadeCNH =
                        dto.DataValidadeCNH,

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
                        true,

                    DataCadastro =
                        DateTime.Now
                };

            var clienteCriado =
                await _repository
                    .CriarAsync(cliente);

            return MapearParaDto(
                clienteCriado);
        }

        // =====================================================
        // ATUALIZAR
        // =====================================================

        public async Task<bool> AtualizarAsync(
            int id,
            AtualizarClienteDto dto)
        {
            var cliente =
                await _repository
                    .ObterPorIdAsync(id);

            if (cliente == null)
                return false;

            cliente.Nome =
                dto.Nome;

            cliente.CPF =
                dto.CPF;

            cliente.DataNascimento =
                dto.DataNascimento;

            cliente.Telefone =
                dto.Telefone;

            cliente.CNH =
                dto.CNH;

            cliente.CategoriaCNH =
                dto.CategoriaCNH;

            cliente.DataValidadeCNH =
                dto.DataValidadeCNH;

            cliente.Endereco =
                dto.Endereco;

            cliente.Numero =
                dto.Numero;

            cliente.Bairro =
                dto.Bairro;

            cliente.Cidade =
                dto.Cidade;

            cliente.Estado =
                dto.Estado;

            cliente.CEP =
                dto.CEP;

            return await _repository
                .AtualizarAsync(cliente);
        }

        // =====================================================
        // DESATIVAR
        // =====================================================

        public async Task<bool> ExcluirAsync(
            int id)
        {
            var cliente =
                await _repository
                    .ObterPorIdAsync(id);

            if (cliente == null)
                return false;

            // =================================================
            // BUSCAR RESERVAS DO CLIENTE
            // =================================================

            var reservas =
                (await _reservaRepository
                    .ListarPorClienteIdAsync(id))
                .ToList();

            // =================================================
            // VERIFICAR LOCAÇÃO ATIVA
            // =================================================

            foreach (var reserva in reservas)
            {
                var locacao =
                    await _locacaoRepository
                        .ObterPorReservaIdAsync(
                            reserva.Id);

                if (locacao != null &&
                    locacao.IsAtiva &&
                    locacao.Status ==
                        StatusLocacao.Ativa)
                {
                    throw new Exception(
                        "Não é possível desativar o cliente " +
                        "porque ele possui uma locação ativa.");
                }
            }

            // =================================================
            // VERIFICAR RESERVA ATIVA
            // =================================================

            var possuiReservaAtiva =
                reservas.Any(r =>
                    r.IsAtiva &&
                    (
                        r.Status ==
                            StatusReserva.Pendente
                        ||
                        r.Status ==
                            StatusReserva.Confirmada
                    ));

            if (possuiReservaAtiva)
            {
                throw new Exception(
                    "Não é possível desativar o cliente " +
                    "porque ele possui uma reserva ativa.");
            }

            // =================================================
            // DESATIVAR CLIENTE
            // =================================================

            var clienteDesativado =
                await _repository
                    .ExcluirAsync(id);

            if (!clienteDesativado)
                return false;

            // =================================================
            // DESATIVAR USUÁRIO VINCULADO
            // =================================================

            var usuario =
                await _usuarioRepository
                    .ObterPorIdAsync(
                        cliente.UsuarioId);

            if (usuario != null)
            {
                usuario.IsAtivo =
                    false;

                var usuarioAtualizado =
                    await _usuarioRepository
                        .AtualizarAsync(usuario);

                if (!usuarioAtualizado)
                {
                    throw new Exception(
                        "O cliente foi desativado, mas não foi possível " +
                        "desativar o usuário vinculado.");
                }
            }

            return true;
        }

        // =====================================================
        // ATIVAR
        // =====================================================

        public async Task<bool> AtivarAsync(
            int id)
        {
            var cliente =
                await _repository
                    .ObterPorIdAsync(id);

            if (cliente == null)
                return false;

            // =================================================
            // ATIVAR CLIENTE
            // =====================================================

            cliente.IsAtivo =
                true;

            var clienteAtualizado =
                await _repository
                    .AtualizarAsync(cliente);

            if (!clienteAtualizado)
                return false;

            // =================================================
            // ATIVAR USUÁRIO VINCULADO
            // =====================================================

            var usuario =
                await _usuarioRepository
                    .ObterPorIdAsync(
                        cliente.UsuarioId);

            if (usuario != null)
            {
                usuario.IsAtivo =
                    true;

                var usuarioAtualizado =
                    await _usuarioRepository
                        .AtualizarAsync(usuario);

                if (!usuarioAtualizado)
                {
                    throw new Exception(
                        "O cliente foi ativado, mas não foi possível " +
                        "ativar o usuário vinculado.");
                }
            }

            return true;
        }

        // =====================================================
        // MAPEAR PARA DTO
        // =====================================================

        private static ClienteDto MapearParaDto(
            Cliente cliente)
        {
            return new ClienteDto
            {
                Id =
                    cliente.Id,

                Nome =
                    cliente.Nome,

                CPF =
                    cliente.CPF,

                DataNascimento =
                    cliente.DataNascimento,

                Telefone =
                    cliente.Telefone,

                CNH =
                    cliente.CNH,

                CategoriaCNH =
                    cliente.CategoriaCNH,

                DataValidadeCNH =
                    cliente.DataValidadeCNH,

                Endereco =
                    cliente.Endereco,

                Numero =
                    cliente.Numero,

                Bairro =
                    cliente.Bairro,

                Cidade =
                    cliente.Cidade,

                Estado =
                    cliente.Estado,

                CEP =
                    cliente.CEP,

                IsAtivo =
                    cliente.IsAtivo,

                DataCadastro =
                    cliente.DataCadastro
            };
        }
    }
}