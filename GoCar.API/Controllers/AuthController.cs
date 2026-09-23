using GoCar.Application.DTOs.Usuario;
using GoCar.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GoCar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioService _service;

        public AuthController(
            IUsuarioService service)
        {
            _service = service;
        }

        // =====================================================
        // LOGIN
        // =====================================================

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(
            LoginDto dto)
        {
            var usuario =
                await _service.LoginAsync(dto);

            if (usuario == null)
            {
                return Unauthorized(new
                {
                    mensagem =
                        "E-mail ou senha inválidos."
                });
            }

            return Ok(usuario);
        }

        // =====================================================
        // CADASTRO
        // =====================================================

        [HttpPost("cadastro")]
        public async Task<ActionResult<LoginResponseDto>> Cadastro(
            CriarUsuarioDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
            {
                return BadRequest(new
                {
                    mensagem =
                        "Informe o nome."
                });
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                return BadRequest(new
                {
                    mensagem =
                        "Informe o e-mail."
                });
            }

            if (string.IsNullOrWhiteSpace(dto.Senha))
            {
                return BadRequest(new
                {
                    mensagem =
                        "Informe a senha."
                });
            }

            if (string.IsNullOrWhiteSpace(dto.CPF))
            {
                return BadRequest(new
                {
                    mensagem =
                        "Informe o CPF."
                });
            }

            if (string.IsNullOrWhiteSpace(dto.Telefone))
            {
                return BadRequest(new
                {
                    mensagem =
                        "Informe o telefone."
                });
            }

            if (string.IsNullOrWhiteSpace(dto.CNH))
            {
                return BadRequest(new
                {
                    mensagem =
                        "Informe a CNH."
                });
            }

            if (string.IsNullOrWhiteSpace(dto.CategoriaCNH))
            {
                return BadRequest(new
                {
                    mensagem =
                        "Informe a categoria da CNH."
                });
            }

            if (dto.DataNascimento == default)
            {
                return BadRequest(new
                {
                    mensagem =
                        "Informe a data de nascimento."
                });
            }

            if (dto.DataValidadeCNH == default)
            {
                return BadRequest(new
                {
                    mensagem =
                        "Informe a validade da CNH."
                });
            }

            if (string.IsNullOrWhiteSpace(dto.Endereco))
            {
                return BadRequest(new
                {
                    mensagem =
                        "Informe o endereço."
                });
            }

            if (string.IsNullOrWhiteSpace(dto.Numero))
            {
                return BadRequest(new
                {
                    mensagem =
                        "Informe o número."
                });
            }

            if (string.IsNullOrWhiteSpace(dto.Bairro))
            {
                return BadRequest(new
                {
                    mensagem =
                        "Informe o bairro."
                });
            }

            if (string.IsNullOrWhiteSpace(dto.Cidade))
            {
                return BadRequest(new
                {
                    mensagem =
                        "Informe a cidade."
                });
            }

            if (string.IsNullOrWhiteSpace(dto.Estado))
            {
                return BadRequest(new
                {
                    mensagem =
                        "Informe o estado."
                });
            }

            if (string.IsNullOrWhiteSpace(dto.CEP))
            {
                return BadRequest(new
                {
                    mensagem =
                        "Informe o CEP."
                });
            }

            var usuario =
                await _service.CadastrarAsync(dto);

            if (usuario == null)
            {
                return Conflict(new
                {
                    mensagem =
                        "Já existe um usuário cadastrado com este e-mail ou CPF."
                });
            }

            return Ok(usuario);
        }
    }
}