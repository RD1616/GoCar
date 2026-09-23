namespace GoCar.Desktop.Session
{
    public static class DesktopSession
    {
        public static int UsuarioId { get; set; }

        public static string Nome { get; set; } = string.Empty;

        public static string Email { get; set; } = string.Empty;

        public static string Perfil { get; set; } = string.Empty;

        public static string Token { get; set; } = string.Empty;

        public static bool EstaLogado =>
            UsuarioId > 0 &&
            !string.IsNullOrWhiteSpace(Token);

        public static void Limpar()
        {
            UsuarioId = 0;
            Nome = string.Empty;
            Email = string.Empty;
            Perfil = string.Empty;
            Token = string.Empty;
        }
    }
}