using GoCar.Application.DTOs.Usuario;
using GoCar.Application.Interfaces;
using GoCar.Domain.Entities;
using GoCar.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GoCar.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;
        private readonly IConfiguration _configuration;

        public UsuarioService(
            IUsuarioRepository repository,
            IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        // =====================================================
        // LOGIN
        // =====================================================

        public async Task<LoginResponseDto?> LoginAsync(
            LoginDto dto)
        {
            var email =
                dto.Email.Trim()
                    .ToLowerInvariant();

            var usuario =
                await _repository.ObterPorEmailAsync(email);

            if (usuario == null)
                return null;

            if (!usuario.IsAtivo)
                return null;

            var senhaValida =
                VerificarSenha(
                    dto.Senha,
                    usuario.SenhaHash);

            if (!senhaValida)
                return null;

            // =================================================
            // MIGRAÇÃO AUTOMÁTICA DE SENHA ANTIGA
            //
            // Se a senha ainda estiver salva em texto puro,
            // converte para hash no primeiro login correto.
            // =================================================

            if (!EhSenhaComHash(usuario.SenhaHash))
            {
                usuario.SenhaHash =
                    GerarHashSenha(dto.Senha);

                usuario.DataAtualizacao =
                    DateTime.Now;

                await _repository.AtualizarAsync(usuario);
            }

            var token =
                GerarToken(usuario);

            return new LoginResponseDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfil = usuario.Perfil.ToString(),
                Token = token
            };
        }

        // =====================================================
        // CADASTRO
        // =====================================================

        public async Task<LoginResponseDto?> CadastrarAsync(
            CriarUsuarioDto dto)
        {
            // =================================================
            // NORMALIZAR DADOS
            // =================================================

            var email =
                dto.Email.Trim()
                    .ToLowerInvariant();

            var cpf =
                dto.CPF.Trim();

            // =================================================
            // VERIFICAR E-MAIL
            // =================================================

            var usuarioExistente =
                await _repository.ObterPorEmailAsync(email);

            if (usuarioExistente != null)
                return null;

            // =================================================
            // VERIFICAR CPF
            // =================================================

            var usuarioPorCpf =
                await _repository.ObterPorCpfAsync(cpf);

            if (usuarioPorCpf != null)
                return null;

            // =================================================
            // CRIAR USUÁRIO
            // =================================================

            var usuario =
                new Usuario
                {
                    Nome =
                        dto.Nome.Trim(),

                    Email =
                        email,

                    // Agora a senha nunca é salva em texto puro.
                    SenhaHash =
                        GerarHashSenha(dto.Senha),

                    CPF =
                        cpf,

                    Telefone =
                        dto.Telefone.Trim(),

                    Perfil =
                        PerfilUsuario.Cliente,

                    IsAtivo =
                        true,

                    DataCriacao =
                        DateTime.Now
                };

            // =================================================
            // PREPARAR CLIENTE
            // =================================================

            var cliente =
                new Cliente
                {
                    Nome =
                        usuario.Nome,

                    CPF =
                        usuario.CPF,

                    DataNascimento =
                        dto.DataNascimento,

                    Telefone =
                        usuario.Telefone,

                    CNH =
                        dto.CNH.Trim(),

                    CategoriaCNH =
                        dto.CategoriaCNH.Trim(),

                    DataValidadeCNH =
                        dto.DataValidadeCNH,

                    Endereco =
                        dto.Endereco.Trim(),

                    Numero =
                        dto.Numero.Trim(),

                    Bairro =
                        dto.Bairro.Trim(),

                    Cidade =
                        dto.Cidade.Trim(),

                    Estado =
                        dto.Estado.Trim()
                            .ToUpperInvariant(),

                    CEP =
                        dto.CEP.Trim(),

                    IsAtivo =
                        true,

                    DataCadastro =
                        DateTime.Now
                };

            // =================================================
            // USUÁRIO + CLIENTE NA MESMA TRANSAÇÃO
            // =================================================

            var resultado =
                await _repository
                    .CriarUsuarioComClienteAsync(
                        usuario,
                        cliente);

            usuario =
                resultado.Usuario;

            // =================================================
            // LOGIN AUTOMÁTICO
            // =================================================

            var token =
                GerarToken(usuario);

            return new LoginResponseDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfil = usuario.Perfil.ToString(),
                Token = token
            };
        }

        // =====================================================
        // GERAR HASH DA SENHA
        // =====================================================

        private static string GerarHashSenha(
            string senha)
        {
            const int iteracoes = 100_000;
            const int tamanhoSalt = 16;
            const int tamanhoHash = 32;

            var salt =
                RandomNumberGenerator
                    .GetBytes(tamanhoSalt);

            var hash =
                Rfc2898DeriveBytes.Pbkdf2(
                    senha,
                    salt,
                    iteracoes,
                    HashAlgorithmName.SHA256,
                    tamanhoHash);

            return
                $"PBKDF2${iteracoes}$" +
                $"{Convert.ToBase64String(salt)}$" +
                $"{Convert.ToBase64String(hash)}";
        }

        // =====================================================
        // VERIFICAR SENHA
        // =====================================================

        private static bool VerificarSenha(
            string senhaInformada,
            string senhaArmazenada)
        {
            if (string.IsNullOrWhiteSpace(
                senhaArmazenada))
            {
                return false;
            }

            // =================================================
            // USUÁRIO ANTIGO
            //
            // Mantém compatibilidade temporária com as contas
            // criadas antes da implementação do hash.
            // =================================================

            if (!EhSenhaComHash(senhaArmazenada))
            {
                return senhaArmazenada ==
                    senhaInformada;
            }

            try
            {
                var partes =
                    senhaArmazenada.Split('$');

                if (partes.Length != 4)
                    return false;

                var iteracoes =
                    int.Parse(partes[1]);

                var salt =
                    Convert.FromBase64String(
                        partes[2]);

                var hashArmazenado =
                    Convert.FromBase64String(
                        partes[3]);

                var hashInformado =
                    Rfc2898DeriveBytes.Pbkdf2(
                        senhaInformada,
                        salt,
                        iteracoes,
                        HashAlgorithmName.SHA256,
                        hashArmazenado.Length);

                return CryptographicOperations
                    .FixedTimeEquals(
                        hashInformado,
                        hashArmazenado);
            }
            catch
            {
                return false;
            }
        }

        // =====================================================
        // IDENTIFICAR SENHA COM HASH
        // =====================================================

        private static bool EhSenhaComHash(
            string senha)
        {
            return
                !string.IsNullOrWhiteSpace(senha) &&
                senha.StartsWith(
                    "PBKDF2$",
                    StringComparison.Ordinal);
        }

        // =====================================================
        // GERAR TOKEN JWT
        // =====================================================

        private string GerarToken(
            Usuario usuario)
        {
            var key =
                _configuration["Jwt:Key"];

            var issuer =
                _configuration["Jwt:Issuer"];

            var audience =
                _configuration["Jwt:Audience"];

            var expirationInMinutes =
                int.Parse(
                    _configuration[
                        "Jwt:ExpirationInMinutes"]!);

            var claims =
                new List<Claim>
                {
                    new Claim(
                        JwtRegisteredClaimNames.Sub,
                        usuario.Id.ToString()),

                    new Claim(
                        JwtRegisteredClaimNames.Email,
                        usuario.Email),

                    new Claim(
                        ClaimTypes.Name,
                        usuario.Nome),

                    new Claim(
                        ClaimTypes.Role,
                        usuario.Perfil.ToString())
                };

            var securityKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(key!));

            var credentials =
                new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.HmacSha256);

            var token =
                new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires:
                        DateTime.UtcNow.AddMinutes(
                            expirationInMinutes),
                    signingCredentials:
                        credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}