using GoCar.Application.DTOs.Usuario;

namespace GoCar.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<LoginResponseDto?> LoginAsync(
            LoginDto dto);

        Task<LoginResponseDto?> CadastrarAsync(
            CriarUsuarioDto dto);
    }
}