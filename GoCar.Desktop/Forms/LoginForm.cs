using GoCar.Desktop.Services;
using GoCar.Desktop.Session;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows.Forms;

namespace GoCar.Desktop.Forms
{
    public partial class LoginForm : Form
    {
        private Image? imagemFundoLogin;

        public LoginForm()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;

            // ==========================================
            // CONFIGURAÇÕES DO FORMULÁRIO
            // ==========================================

            ClientSize = new Size(533, 840);
            BackColor = Color.FromArgb(8, 0, 20);


            // ==========================================
            // BOTÃO FECHAR APLICAÇÃO
            // ==========================================

            Button btnFechar = new Button
            {
                Text = "✕",
                Size = new Size(40, 40),
                Location = new Point(ClientSize.Width - 50, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                Cursor = Cursors.Hand,
                TabStop = false
            };

            btnFechar.FlatAppearance.BorderSize = 0;
            btnFechar.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(70, 40, 40);
            btnFechar.FlatAppearance.MouseDownBackColor =
                Color.FromArgb(110, 40, 40);

            // Encerra completamente o processo do GoCar,
            // inclusive quando executado pelo Visual Studio.
            btnFechar.Click += (s, e) => Environment.Exit(0);

            Controls.Add(btnFechar);
            btnFechar.BringToFront();

            // ==========================================
            // FUNDO EM MODO "COVER"
            // ==========================================
            // A imagem usada no painel de login é preservada e desenhada
            // proporcionalmente, preenchendo toda a área sem distorção.

            imagemFundoLogin = pnlLogin.BackgroundImage;
            pnlLogin.BackgroundImage = null;
            pnlLogin.Paint += DesenharFundoLoginCover;
            pnlLogin.Resize += (s, e) => pnlLogin.Invalidate();

            // ==========================================
            // PAINEL PRINCIPAL
            // ==========================================

            pnlLogin.BackColor = Color.FromArgb(29, 5, 39);

            // ==========================================
            // CAMPO DE E-MAIL
            // ==========================================

            pnlEmail.BackColor = Color.FromArgb(10, 0, 15);
            txtEmail.BackColor = Color.FromArgb(10, 0, 15);
            txtEmail.ForeColor = Color.White;

            // ==========================================
            // CAMPO DE SENHA
            // ==========================================

            pnlSenha.BackColor = Color.FromArgb(10, 0, 15);
            txtSenha.BackColor = Color.FromArgb(10, 0, 15);
            txtSenha.ForeColor = Color.White;
            txtSenha.UseSystemPasswordChar = true;

            // ==========================================
            // BOTÃO ENTRAR
            // ==========================================

            btnEntrar.BackColor = Color.FromArgb(111, 38, 201);
            btnEntrar.ForeColor = Color.White;
            btnEntrar.FlatStyle = FlatStyle.Flat;
            btnEntrar.FlatAppearance.BorderSize = 0;
            btnEntrar.Cursor = Cursors.Hand;

            // ==========================================
            // TEXTOS
            // ==========================================

            lblTitulo.ForeColor = Color.White;
            lblDescricao.ForeColor = Color.Gainsboro;
            lblRestrito.ForeColor = Color.White;

            // ==========================================
            // LINHAS DO RODAPÉ
            // ==========================================

            pnlLinhaEsquerda.BackColor = Color.FromArgb(150, 150, 150);
            pnlLinhaDireita.BackColor = Color.FromArgb(150, 150, 150);

            // ==========================================
            // CANTOS ARREDONDADOS
            // ==========================================

            ArredondarControle(pnlLogin, 15);
            ArredondarControle(pnlEmail, 6);
            ArredondarControle(pnlSenha, 6);
            ArredondarControle(btnEntrar, 6);
        }

        // ==============================================
        // DESENHA A IMAGEM COMO "BACKGROUND-SIZE: COVER"
        // ==============================================

        private void DesenharFundoLoginCover(object? sender, PaintEventArgs e)
        {
            if (imagemFundoLogin == null ||
                pnlLogin.ClientSize.Width <= 0 ||
                pnlLogin.ClientSize.Height <= 0)
            {
                return;
            }

            float escalaX =
                (float)pnlLogin.ClientSize.Width / imagemFundoLogin.Width;

            float escalaY =
                (float)pnlLogin.ClientSize.Height / imagemFundoLogin.Height;

            // Maior escala = preenche toda a área.
            float escala = Math.Max(escalaX, escalaY);

            int largura =
                (int)Math.Ceiling(imagemFundoLogin.Width * escala);

            int altura =
                (int)Math.Ceiling(imagemFundoLogin.Height * escala);

            int x =
                (pnlLogin.ClientSize.Width - largura) / 2;

            int y =
                (pnlLogin.ClientSize.Height - altura) / 2;

            e.Graphics.InterpolationMode =
                InterpolationMode.HighQualityBicubic;

            e.Graphics.PixelOffsetMode =
                PixelOffsetMode.HighQuality;

            e.Graphics.CompositingQuality =
                CompositingQuality.HighQuality;

            e.Graphics.DrawImage(
                imagemFundoLogin,
                new Rectangle(x, y, largura, altura));
        }

        // ==============================================
        // MÉTODO PARA ARREDONDAR CONTROLES
        // ==============================================

        private void ArredondarControle(
            Control controle,
            int raio)
        {
            GraphicsPath caminho = new GraphicsPath();

            int diametro = raio * 2;

            caminho.StartFigure();

            caminho.AddArc(
                0,
                0,
                diametro,
                diametro,
                180,
                90);

            caminho.AddArc(
                controle.Width - diametro,
                0,
                diametro,
                diametro,
                270,
                90);

            caminho.AddArc(
                controle.Width - diametro,
                controle.Height - diametro,
                diametro,
                diametro,
                0,
                90);

            caminho.AddArc(
                0,
                controle.Height - diametro,
                diametro,
                diametro,
                90,
                90);

            caminho.CloseFigure();

            controle.Region = new Region(caminho);

            caminho.Dispose();
        }

        private void pnlEmail_Paint(object sender, PaintEventArgs e)
        {
        }

        private void picOlho_Click(object sender, EventArgs e)
        {
            txtSenha.UseSystemPasswordChar =
                !txtSenha.UseSystemPasswordChar;
        }

        private void pnlLogin_Paint(object sender, PaintEventArgs e)
        {
        }

        private class LoginResponse
        {
            public int Id { get; set; }

            public string Nome { get; set; } = string.Empty;

            public string Email { get; set; } = string.Empty;

            public string Perfil { get; set; } = string.Empty;

            public string Token { get; set; } = string.Empty;
        }

        private async void btnEntrar_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "";

            string email = txtEmail.Text.Trim();
            string senha = txtSenha.Text;

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(senha))
            {
                lblStatus.Text = "Preencha o e-mail e a senha.";
                lblStatus.ForeColor = Color.IndianRed;
                return;
            }

            try
            {
                btnEntrar.Enabled = false;
                btnEntrar.Text = "Entrando...";

                var dadosLogin = new
                {
                    email = email,
                    senha = senha
                };

                var resposta = await ApiClient.PostAsync(
                    "api/Auth/login",
                    dadosLogin);

                if (!resposta.IsSuccessStatusCode)
                {
                    lblStatus.Text = "E-mail ou senha inválidos.";
                    lblStatus.ForeColor = Color.IndianRed;
                    return;
                }

                var resultado =
                    await resposta.Content.ReadFromJsonAsync<LoginResponse>();

                if (resultado == null ||
                    string.IsNullOrWhiteSpace(resultado.Token))
                {
                    lblStatus.Text = "Resposta inválida da API.";
                    lblStatus.ForeColor = Color.IndianRed;
                    return;
                }

                DesktopSession.Token = resultado.Token;
                DesktopSession.Nome = resultado.Nome;
                DesktopSession.Perfil = resultado.Perfil;

                ApiClient.ConfigurarToken(resultado.Token);

                if (!string.Equals(
                        resultado.Perfil,
                        "Administrador",
                        StringComparison.OrdinalIgnoreCase))
                {
                    lblStatus.Text =
                        "Acesso permitido somente para administradores.";

                    lblStatus.ForeColor = Color.IndianRed;

                    return;
                }

                Hide();

                using (var dashboard = new DashboardForm())
                {
                    dashboard.ShowDialog(this);
                }

                txtSenha.Clear();
                lblStatus.Text = "";

                Show();
                Activate();
                txtEmail.Focus();
            }
            catch (HttpRequestException)
            {
                lblStatus.Text = "Não foi possível conectar à API.";
                lblStatus.ForeColor = Color.IndianRed;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Erro ao realizar login.";

                MessageBox.Show(
                    ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnEntrar.Enabled = true;
                btnEntrar.Text = "Entrar →";
            }
        }
    }
}
