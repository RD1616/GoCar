using System.Net.Http.Json;

namespace GoCar.Desktop.Forms
{
    public class LoginForm : Form
    {
        private readonly TextBox txtEmail;
        private readonly TextBox txtSenha;
        private readonly Button btnEntrar;
        private readonly Label lblStatus;

        private readonly HttpClient _httpClient;

        public LoginForm()
        {
            // ==========================================
            // API
            // ==========================================

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(
                    "https://localhost:58906/")
            };

            // ==========================================
            // JANELA
            // ==========================================

            Text = "GoCar - Login Administrativo";

            StartPosition =
                FormStartPosition.CenterScreen;

            ClientSize =
                new Size(440, 470);

            FormBorderStyle =
                FormBorderStyle.FixedSingle;

            MaximizeBox = false;

            BackColor =
                Color.WhiteSmoke;

            // ==========================================
            // TÍTULO
            // ==========================================

            var lblTitulo = new Label
            {
                Text = "GoCar",

                Font = new Font(
                    "Segoe UI",
                    30,
                    FontStyle.Bold),

                AutoSize = true,

                Location =
                    new Point(145, 45)
            };

            var lblSubtitulo = new Label
            {
                Text = "Sistema Administrativo",

                Font = new Font(
                    "Segoe UI",
                    11),

                AutoSize = true,

                ForeColor =
                    Color.DimGray,

                Location =
                    new Point(130, 105)
            };

            // ==========================================
            // EMAIL
            // ==========================================

            var lblEmail = new Label
            {
                Text = "E-mail",

                Font = new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Bold),

                AutoSize = true,

                Location =
                    new Point(65, 160)
            };

            txtEmail = new TextBox
            {
                Width = 310,

                Font = new Font(
                    "Segoe UI",
                    11),

                Location =
                    new Point(65, 185)
            };

            // ==========================================
            // SENHA
            // ==========================================

            var lblSenha = new Label
            {
                Text = "Senha",

                Font = new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Bold),

                AutoSize = true,

                Location =
                    new Point(65, 235)
            };

            txtSenha = new TextBox
            {
                Width = 310,

                Font = new Font(
                    "Segoe UI",
                    11),

                Location =
                    new Point(65, 260),

                UseSystemPasswordChar =
                    true
            };

            // ==========================================
            // ENTRAR
            // ==========================================

            btnEntrar = new Button
            {
                Text = "Entrar",

                Width = 310,
                Height = 45,

                Font = new Font(
                    "Segoe UI",
                    11,
                    FontStyle.Bold),

                Location =
                    new Point(65, 320),

                Cursor =
                    Cursors.Hand
            };

            btnEntrar.Click +=
                BtnEntrar_Click;

            lblStatus = new Label
            {
                Width = 310,
                Height = 50,

                TextAlign =
                    ContentAlignment.MiddleCenter,

                ForeColor =
                    Color.Firebrick,

                Location =
                    new Point(65, 375)
            };

            AcceptButton =
                btnEntrar;

            Controls.Add(lblTitulo);
            Controls.Add(lblSubtitulo);

            Controls.Add(lblEmail);
            Controls.Add(txtEmail);

            Controls.Add(lblSenha);
            Controls.Add(txtSenha);

            Controls.Add(btnEntrar);
            Controls.Add(lblStatus);
        }

        // ==============================================
        // LOGIN
        // ==============================================

        private async void BtnEntrar_Click(
            object? sender,
            EventArgs e)
        {
            lblStatus.Text = "";

            var email =
                txtEmail.Text.Trim();

            var senha =
                txtSenha.Text;

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(senha))
            {
                lblStatus.Text =
                    "Informe o e-mail e a senha.";

                return;
            }

            try
            {
                btnEntrar.Enabled = false;
                btnEntrar.Text = "Entrando...";

                var dados = new LoginRequest
                {
                    Email = email,
                    Senha = senha
                };

                var response =
                    await _httpClient.PostAsJsonAsync(
                        "api/Auth/login",
                        dados);

                if (!response.IsSuccessStatusCode)
                {
                    lblStatus.Text =
                        "E-mail ou senha inválidos.";

                    return;
                }

                var login =
                    await response.Content
                        .ReadFromJsonAsync<LoginResponse>();

                if (login == null ||
                    string.IsNullOrWhiteSpace(login.Token))
                {
                    lblStatus.Text =
                        "Não foi possível realizar o login.";

                    return;
                }

                // Desktop somente para funcionários.
                if (login.Perfil != "Administrador" &&
                    login.Perfil != "Gerente" &&
                    login.Perfil != "Atendente")
                {
                    lblStatus.Text =
                        "Acesso permitido somente para funcionários.";

                    return;
                }

                // ======================================
                // SALVAR SESSÃO
                // ======================================

                DesktopSession.UsuarioId =
                    login.Id;

                DesktopSession.Nome =
                    login.Nome;

                DesktopSession.Email =
                    login.Email;

                DesktopSession.Perfil =
                    login.Perfil;

                DesktopSession.Token =
                    login.Token;

                // ======================================
                // ABRIR DASHBOARD
                // ======================================

                var dashboard =
                    new DashboardForm();

                Hide();

                dashboard.ShowDialog();

                // Quando fechar o Dashboard,
                // volta para o Login.

                DesktopSession.Limpar();

                Show();

                txtSenha.Clear();
                txtSenha.Focus();
            }
            catch (HttpRequestException)
            {
                lblStatus.Text =
                    "Não foi possível conectar à API.";
            }
            catch (Exception ex)
            {
                lblStatus.Text =
                    $"Erro: {ex.Message}";
            }
            finally
            {
                btnEntrar.Enabled = true;
                btnEntrar.Text = "Entrar";
            }
        }

        // ==============================================
        // DTOs
        // ==============================================

        private class LoginRequest
        {
            public string Email { get; set; }
                = string.Empty;

            public string Senha { get; set; }
                = string.Empty;
        }

        private class LoginResponse
        {
            public int Id { get; set; }

            public string Nome { get; set; }
                = string.Empty;

            public string Email { get; set; }
                = string.Empty;

            public string Perfil { get; set; }
                = string.Empty;

            public string Token { get; set; }
                = string.Empty;
        }
    }

    // ==============================================
    // SESSÃO DO DESKTOP
    // ==============================================

    public static class DesktopSession
    {
        public static int UsuarioId { get; set; }

        public static string Nome { get; set; }
            = string.Empty;

        public static string Email { get; set; }
            = string.Empty;

        public static string Perfil { get; set; }
            = string.Empty;

        public static string Token { get; set; }
            = string.Empty;

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