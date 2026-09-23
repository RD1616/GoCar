using GoCar.Desktop.Services;
using GoCar.Desktop.Forms.Clientes;

namespace GoCar.Desktop.Forms
{
    public class DashboardForm : Form
    {
        public DashboardForm()
        {
            // Configura o token JWT para as próximas requisições à API
            if (!string.IsNullOrWhiteSpace(DesktopSession.Token))
            {
                ApiClient.ConfigurarToken(DesktopSession.Token);
            }

            // =========================
            // CONFIGURAÇÕES DO FORM
            // =========================

            Text = "GoCar - Administração";

            StartPosition = FormStartPosition.CenterScreen;

            Width = 1000;
            Height = 650;

            MinimumSize = new Size(900, 550);


            // =========================
            // TÍTULO GOCAR
            // =========================

            var lblTitulo = new Label
            {
                Text = "GoCar",

                Font = new Font(
                    "Segoe UI",
                    26,
                    FontStyle.Bold
                ),

                AutoSize = true,

                Location = new Point(40, 35)
            };


            // =========================
            // USUÁRIO LOGADO
            // =========================

            var lblUsuario = new Label
            {
                Text =
                    $"Bem-vindo, {DesktopSession.Nome} | " +
                    $"{DesktopSession.Perfil}",

                Font = new Font(
                    "Segoe UI",
                    11,
                    FontStyle.Regular
                ),

                AutoSize = true,

                Location = new Point(40, 90)
            };


            // =========================
            // TÍTULO DO PAINEL
            // =========================

            var lblPainel = new Label
            {
                Text = "Painel Administrativo",

                Font = new Font(
                    "Segoe UI",
                    18,
                    FontStyle.Bold
                ),

                AutoSize = true,

                Location = new Point(40, 155)
            };


            // =========================
            // BOTÃO CLIENTES
            // =========================

            var btnClientes = CriarBotao(
                "Clientes",
                40,
                215
            );

            btnClientes.Click += (sender, e) =>
            {
                using var clientesForm = new ClientesForm();

                clientesForm.ShowDialog(this);
            };


            // =========================
            // BOTÃO VEÍCULOS
            // =========================

            var btnVeiculos = CriarBotao(
                "Veículos",
                250,
                215
            );

            btnVeiculos.Click += (sender, e) =>
            {
                ModuloEmConstrucao("Veículos");
            };


            // =========================
            // BOTÃO RESERVAS
            // =========================

            var btnReservas = CriarBotao(
                "Reservas",
                460,
                215
            );

            btnReservas.Click += (sender, e) =>
            {
                ModuloEmConstrucao("Reservas");
            };


            // =========================
            // BOTÃO LOCAÇÕES
            // =========================

            var btnLocacoes = CriarBotao(
                "Locações",
                40,
                305
            );

            btnLocacoes.Click += (sender, e) =>
            {
                ModuloEmConstrucao("Locações");
            };


            // =========================
            // BOTÃO PAGAMENTOS
            // =========================

            var btnPagamentos = CriarBotao(
                "Pagamentos",
                250,
                305
            );

            btnPagamentos.Click += (sender, e) =>
            {
                ModuloEmConstrucao("Pagamentos");
            };


            // =========================
            // BOTÃO SAIR
            // =========================

            var btnSair = CriarBotao(
                "Sair",
                460,
                305
            );

            btnSair.Click += (sender, e) =>
            {
                Close();
            };


            // =========================
            // ADICIONA CONTROLES
            // =========================

            Controls.Add(lblTitulo);
            Controls.Add(lblUsuario);
            Controls.Add(lblPainel);

            Controls.Add(btnClientes);
            Controls.Add(btnVeiculos);
            Controls.Add(btnReservas);
            Controls.Add(btnLocacoes);
            Controls.Add(btnPagamentos);
            Controls.Add(btnSair);
        }


        // =========================
        // CRIAÇÃO PADRÃO DOS BOTÕES
        // =========================

        private Button CriarBotao(
            string texto,
            int x,
            int y
        )
        {
            return new Button
            {
                Text = texto,

                Width = 180,
                Height = 60,

                Location = new Point(x, y),

                Font = new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Bold
                ),

                Cursor = Cursors.Hand
            };
        }


        // =========================
        // MÓDULOS AINDA NÃO CRIADOS
        // =========================

        private void ModuloEmConstrucao(string modulo)
        {
            MessageBox.Show(
                $"Módulo {modulo} preparado para implementação.",
                "GoCar",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
    }
}