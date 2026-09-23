namespace GoCar.Web.Models
{
    public class LoginResponseViewModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Perfil { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;
    }
}