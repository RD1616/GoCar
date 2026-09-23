using GoCar.Desktop.Services;
using GoCar.Desktop.Session;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoCar.Desktop.Forms
{
    public class DashboardForm : Form
    {
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;
        private const int SB_BOTH = 3;

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(
            IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool ShowScrollBar(
            IntPtr hWnd, int wBar, bool bShow);

        // =====================================================
        // PAINÉIS PRINCIPAIS
        // =====================================================

        private Panel pnlMenu = null!;
        private Panel pnlTopo = null!;
        private Panel pnlConteudo = null!;

        // =====================================================
        // MENU
        // =====================================================

        private PictureBox picLogo = null!;

        private Button btnDashboard = null!;
        private Button btnVeiculos = null!;
        private Button btnCategorias = null!;
        private Button btnFiliais = null!;
        private Button btnClientes = null!;
        private Button btnReservas = null!;
        private Button btnLocacoes = null!;
        private Button btnFinanceiro = null!;
        private Button btnRelatorios = null!;
        private Button btnConfiguracoes = null!;

        // =====================================================
        // DADOS
        // =====================================================

        private List<VeiculoResponse> veiculosCarregados =
            new List<VeiculoResponse>();

        private List<CategoriaResponse> categoriasCarregadas =
            new List<CategoriaResponse>();

        private List<FilialResponse> filiaisCarregadas =
            new List<FilialResponse>();

        private List<ClienteResponse> clientesCarregados =
            new List<ClienteResponse>();

        private List<ReservaResponse> reservasCarregadas =
            new List<ReservaResponse>();

        private List<PagamentoResponse> pagamentosCarregados =
            new List<PagamentoResponse>();

        private List<LocacaoResponse> locacoesCarregadas =
            new List<LocacaoResponse>();

        // =====================================================
        // DTO DASHBOARD
        // =====================================================

        private class DashboardResumoResponse
        {
            public int TotalVeiculos { get; set; }

            public int LocacoesAtivas { get; set; }

            public int TotalClientes { get; set; }

            public decimal ReceitaMes { get; set; }
        }

        // =====================================================
        // DTO VEÍCULO
        // =====================================================

        private class VeiculoResponse
        {
            public int Id { get; set; }

            public string Placa { get; set; } =
                string.Empty;

            public string Chassi { get; set; } =
                string.Empty;

            public string Renavam { get; set; } =
                string.Empty;

            public string Modelo { get; set; } =
                string.Empty;

            public string Marca { get; set; } =
                string.Empty;

            public int AnoFabricacao { get; set; }

            public int AnoModelo { get; set; }

            public string Cor { get; set; } =
                string.Empty;

            public int Combustivel { get; set; }

            public int Cambio { get; set; }

            public int Status { get; set; }

            public int KmAtual { get; set; }

            public decimal ValorDiaria { get; set; }

            public bool IsAtivo { get; set; }

            public int CategoriaId { get; set; }

            public int FilialId { get; set; }
        }

        // =====================================================
        // DTO CATEGORIA
        // =====================================================

        private class CategoriaResponse
        {
            public int Id { get; set; }

            public string Nome { get; set; } =
                string.Empty;

            public string Descricao { get; set; } =
                string.Empty;

            public decimal DiariaBase { get; set; }

            public decimal KmLivre { get; set; }

            public bool IsAtivo { get; set; }
        }

        // =====================================================
        // DTO FILIAL
        // =====================================================

        private class FilialResponse
        {
            public int Id { get; set; }
            public string Nome { get; set; } = string.Empty;
            public string CNPJ { get; set; } = string.Empty;
            public string Telefone { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Endereco { get; set; } = string.Empty;
            public string Numero { get; set; } = string.Empty;
            public string Bairro { get; set; } = string.Empty;
            public string Cidade { get; set; } = string.Empty;
            public string Estado { get; set; } = string.Empty;
            public string CEP { get; set; } = string.Empty;
            public bool IsAtivo { get; set; }
        }

        // =====================================================
        // DTO CLIENTE
        // =====================================================

        private class ClienteResponse
        {
            public int Id { get; set; }
            public string Nome { get; set; } = string.Empty;
            public string CPF { get; set; } = string.Empty;
            public DateTime DataNascimento { get; set; }
            public string Telefone { get; set; } = string.Empty;
            public string CNH { get; set; } = string.Empty;
            public string CategoriaCNH { get; set; } = string.Empty;
            public DateTime DataValidadeCNH { get; set; }
            public string Endereco { get; set; } = string.Empty;
            public string Numero { get; set; } = string.Empty;
            public string Bairro { get; set; } = string.Empty;
            public string Cidade { get; set; } = string.Empty;
            public string Estado { get; set; } = string.Empty;
            public string CEP { get; set; } = string.Empty;
            public bool IsAtivo { get; set; }
            public DateTime DataCadastro { get; set; }
        }

        // =====================================================
        // DTO RESERVA
        // =====================================================

        private class ReservaResponse
        {
            public int Id { get; set; }
            public int ClienteId { get; set; }
            public string ClienteNome { get; set; } = string.Empty;
            public int VeiculoId { get; set; }
            public string VeiculoNome { get; set; } = string.Empty;
            public string VeiculoPlaca { get; set; } = string.Empty;
            public int FilialRetiradaId { get; set; }
            public string FilialRetiradaNome { get; set; } = string.Empty;
            public int FilialDevolucaoId { get; set; }
            public string FilialDevolucaoNome { get; set; } = string.Empty;
            public DateTime DataReserva { get; set; }
            public DateTime DataRetirada { get; set; }
            public DateTime DataDevolucaoPrevista { get; set; }
            public int Status { get; set; }
            public decimal ValorTotalPrevisto { get; set; }
            public string? Observacoes { get; set; }
            public bool IsAtiva { get; set; }
            public DateTime? DataCancelamento { get; set; }
        }

        // =====================================================
        // DTO PAGAMENTO
        // =====================================================

        private class PagamentoResponse
        {
            public int Id { get; set; }
            public int? LocacaoId { get; set; }
            public int? ReservaId { get; set; }
            public string Tipo { get; set; } = string.Empty;
            public int FormaPagamento { get; set; }
            public int Status { get; set; }
            public decimal Valor { get; set; }
            public DateTime? DataPagamento { get; set; }
            public string? Observacoes { get; set; }
            public bool IsAtivo { get; set; }
            public DateTime DataCriacao { get; set; }
        }

        // =====================================================
        // DTO LOCAÇÃO
        // =====================================================

        private class LocacaoResponse
        {
            public int Id { get; set; }
            public int ReservaId { get; set; }
            public DateTime DataRetirada { get; set; }
            public DateTime? DataDevolucaoReal { get; set; }
            public int KmSaida { get; set; }
            public int? KmEntrada { get; set; }
            public decimal CombustivelSaidaPercentual { get; set; }
            public decimal? CombustivelEntradaPercentual { get; set; }
            public decimal ValorTotal { get; set; }
            public int Status { get; set; }
            public string? Observacoes { get; set; }
            public bool IsAtiva { get; set; }
            public DateTime DataCriacao { get; set; }
        }

        // =====================================================
        // CONSTRUTOR
        // =====================================================

        public DashboardForm()
        {
            if (!string.IsNullOrWhiteSpace(
                    DesktopSession.Token))
            {
                ApiClient.ConfigurarToken(
                    DesktopSession.Token
                );
            }

            ConfigurarFormulario();

            CriarMenu();

            CriarTopo();

            CriarConteudoDashboard();
        }

        // =====================================================
        // CONFIGURAÇÃO
        // =====================================================

        private void ConfigurarFormulario()
        {
            Text =
                "GoCar - Administração";

            StartPosition =
                FormStartPosition.CenterScreen;

            MinimumSize =
                new Size(
                    1000,
                    650
                );

            FormBorderStyle =
                FormBorderStyle.None;

            WindowState =
                FormWindowState.Maximized;

            BackColor =
                Color.FromArgb(
                    8,
                    0,
                    20
                );

            pnlMenu =
                new Panel
                {
                    Dock =
                        DockStyle.Left,

                    Width =
                        210,

                    BackColor =
                        Color.FromArgb(
                            10,
                            5,
                            20
                        )
                };

            pnlTopo =
                new Panel
                {
                    Dock =
                        DockStyle.Top,

                    Height =
                        60,

                    BackColor =
                        Color.FromArgb(
                            12,
                            6,
                            24
                        )
                };

            pnlConteudo =
                new Panel
                {
                    Dock =
                        DockStyle.Fill,

                    BackColor =
                        Color.FromArgb(
                            8,
                            0,
                            20
                        ),

                    AutoScroll =
                        true
                };

            Controls.Add(
                pnlConteudo
            );

            Controls.Add(
                pnlTopo
            );

            Controls.Add(
                pnlMenu
            );

            pnlConteudo.HandleCreated +=
                (s, e) => OcultarScrollbarsConteudo();

            pnlConteudo.Resize +=
                (s, e) => OcultarScrollbarsConteudo();

            pnlConteudo.Scroll +=
                (s, e) => OcultarScrollbarsConteudo();
        }

        private void OcultarScrollbarsConteudo()
        {
            if (pnlConteudo == null ||
                !pnlConteudo.IsHandleCreated)
                return;

            ShowScrollBar(
                pnlConteudo.Handle,
                SB_BOTH,
                false
            );
        }

        // =====================================================
        // MENU
        // =====================================================

        private void CriarMenu()
        {
            picLogo =
                new PictureBox
                {
                    Size =
                        new Size(
                            150,
                            60
                        ),

                    Location =
                        new Point(
                            25,
                            10
                        ),

                    SizeMode =
                        PictureBoxSizeMode.Zoom,

                    BackColor =
                        Color.Transparent
                };

            try
            {
                picLogo.Image =
                    Properties.Resources
                        .ResourceManager
                        .GetObject(
                            "logo"
                        ) as Image;
            }
            catch
            {
                // Continua sem a logo.
            }

            pnlMenu.Controls.Add(
                picLogo
            );

            btnDashboard =
                CriarBotaoMenu(
                    "▣   Dashboard",
                    80,
                    true
                );

            btnVeiculos =
                CriarBotaoMenu(
                    "▣   Veículos",
                    128
                );

            btnCategorias =
                CriarBotaoMenu(
                    "◆   Categorias",
                    176
                );

            btnFiliais =
                CriarBotaoMenu(
                    "◆   Filiais",
                    224
                );

            btnClientes =
                CriarBotaoMenu(
                    "●   Clientes",
                    272
                );

            btnReservas =
                CriarBotaoMenu(
                    "▦   Reservas",
                    320
                );

            btnLocacoes =
                CriarBotaoMenu(
                    "▣   Locações",
                    368
                );

            btnFinanceiro =
                CriarBotaoMenu(
                    "◉   Financeiro",
                    416
                );

            btnRelatorios =
                CriarBotaoMenu(
                    "▥   Relatórios",
                    464
                );

            btnConfiguracoes =
                CriarBotaoMenu(
                    "⚙   Configurações",
                    512
                );

            // =================================================
            // EVENTOS
            // =================================================

            btnDashboard.Click +=
                (s, e) =>
                {
                    SelecionarBotaoMenu(
                        btnDashboard
                    );

                    CriarConteudoDashboard();
                };

            btnVeiculos.Click +=
                async (s, e) =>
                {
                    SelecionarBotaoMenu(
                        btnVeiculos
                    );

                    await CriarConteudoVeiculos();
                };

            btnCategorias.Click +=
                async (s, e) =>
                {
                    SelecionarBotaoMenu(
                        btnCategorias
                    );

                    await CriarConteudoCategorias();
                };

            btnFiliais.Click +=
                async (s, e) =>
                {
                    SelecionarBotaoMenu(
                        btnFiliais
                    );

                    await CriarConteudoFiliais();
                };

            btnClientes.Click +=
                async (s, e) =>
                {
                    SelecionarBotaoMenu(
                        btnClientes
                    );

                    await CriarConteudoClientes();
                };

            btnReservas.Click +=
                async (s, e) =>
                {
                    SelecionarBotaoMenu(
                        btnReservas
                    );

                    await CriarConteudoReservas();
                };

            btnLocacoes.Click +=
                async (s, e) =>
                {
                    SelecionarBotaoMenu(
                        btnLocacoes
                    );

                    await CriarConteudoLocacoes();
                };

            btnFinanceiro.Click +=
                async (s, e) =>
                {
                    SelecionarBotaoMenu(
                        btnFinanceiro
                    );

                    await CriarConteudoFinanceiro();
                };

            btnRelatorios.Click +=
                async (s, e) =>
                {
                    SelecionarBotaoMenu(
                        btnRelatorios
                    );

                    await CriarConteudoRelatorios();
                };

            btnConfiguracoes.Click +=
                (s, e) =>
                {
                    SelecionarBotaoMenu(
                        btnConfiguracoes
                    );

                    CriarConteudoConfiguracoes();
                };
        }

        // =====================================================
        // BOTÃO DO MENU
        // =====================================================

        private Button CriarBotaoMenu(
            string texto,
            int y,
            bool selecionado = false)
        {
            Button botao =
                new Button
                {
                    Text =
                        texto,

                    Location =
                        new Point(
                            10,
                            y
                        ),

                    Size =
                        new Size(
                            190,
                            48
                        ),

                    FlatStyle =
                        FlatStyle.Flat,

                    BackColor =
                        selecionado
                            ? Color.FromArgb(
                                29,
                                5,
                                39
                            )
                            : Color.FromArgb(
                                10,
                                5,
                                20
                            ),

                    ForeColor =
                        Color.White,

                    Font =
                        new Font(
                            "Segoe UI",
                            10
                        ),

                    TextAlign =
                        ContentAlignment
                            .MiddleLeft,

                    Cursor =
                        Cursors.Hand
                };

            botao
                .FlatAppearance
                .BorderSize = 0;

            botao
                .FlatAppearance
                .MouseOverBackColor =
                Color.FromArgb(
                    35,
                    10,
                    48
                );

            pnlMenu.Controls.Add(
                botao
            );

            return botao;
        }

        // =====================================================
        // SELEÇÃO DO MENU
        // =====================================================

        private void SelecionarBotaoMenu(
            Button selecionado)
        {
            Button[] botoes =
            {
                btnDashboard,
                btnVeiculos,
                btnCategorias,
                btnFiliais,
                btnClientes,
                btnReservas,
                btnLocacoes,
                btnFinanceiro,
                btnRelatorios,
                btnConfiguracoes
            };

            foreach (
                Button botao
                in botoes
            )
            {
                botao.BackColor =
                    Color.FromArgb(
                        10,
                        5,
                        20
                    );
            }

            selecionado.BackColor =
                Color.FromArgb(
                    29,
                    5,
                    39
                );
        }

        // =====================================================
        // TOPO
        // =====================================================

        private void CriarTopo()
        {
            Label lblUsuario = new Label
            {
                Text = $"{DesktopSession.Nome}  |  {DesktopSession.Perfil}",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                AutoSize = true
            };

            Button btnMinimizar = CriarBotaoJanela("—");
            Button btnMaximizar = CriarBotaoJanela("□");
            Button btnFechar = CriarBotaoJanela("×");

            btnFechar.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(180, 45, 55);

            pnlTopo.Controls.Add(lblUsuario);
            pnlTopo.Controls.Add(btnMinimizar);
            pnlTopo.Controls.Add(btnMaximizar);
            pnlTopo.Controls.Add(btnFechar);

            void PosicionarTopo()
            {
                btnFechar.Location =
                    new Point(pnlTopo.ClientSize.Width - 48, 0);

                btnMaximizar.Location =
                    new Point(pnlTopo.ClientSize.Width - 96, 0);

                btnMinimizar.Location =
                    new Point(pnlTopo.ClientSize.Width - 144, 0);

                lblUsuario.Location =
                    new Point(
                        Math.Max(
                            10,
                            pnlTopo.ClientSize.Width -
                            lblUsuario.Width - 170
                        ),
                        20
                    );
            }

            btnMinimizar.Click +=
                (s, e) => WindowState = FormWindowState.Minimized;

            btnMaximizar.Click +=
                (s, e) =>
                {
                    WindowState =
                        WindowState == FormWindowState.Maximized
                            ? FormWindowState.Normal
                            : FormWindowState.Maximized;

                    PosicionarTopo();
                };

            btnFechar.Click +=
                (s, e) => Close();

            pnlTopo.MouseDown +=
                (s, e) =>
                {
                    if (e.Button == MouseButtons.Left &&
                        WindowState != FormWindowState.Maximized)
                    {
                        ReleaseCapture();

                        SendMessage(
                            Handle,
                            WM_NCLBUTTONDOWN,
                            (IntPtr)HTCAPTION,
                            IntPtr.Zero
                        );
                    }
                };

            pnlTopo.DoubleClick +=
                (s, e) =>
                {
                    WindowState =
                        WindowState == FormWindowState.Maximized
                            ? FormWindowState.Normal
                            : FormWindowState.Maximized;

                    PosicionarTopo();
                };

            pnlTopo.Resize +=
                (s, e) => PosicionarTopo();

            PosicionarTopo();
        }

        private Button CriarBotaoJanela(string texto)
        {
            Button botao = new Button
            {
                Text = texto,
                Size = new Size(48, 60),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(12, 6, 24),
                ForeColor = Color.White,
                Font = new Font("Segoe UI Symbol", 12),
                Cursor = Cursors.Hand,
                TabStop = false
            };

            botao.FlatAppearance.BorderSize = 0;
            botao.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(35, 20, 50);

            return botao;
        }

        // =====================================================
        // DASHBOARD
        // =====================================================

        private async void CriarConteudoDashboard()
        {
            pnlConteudo.Controls.Clear();

            int totalVeiculos = 0;
            int locacoesAtivas = 0;
            int totalClientes = 0;

            decimal receitaMes =
                0m;

            List<LocacaoResponse> locacoesDashboard =
                new List<LocacaoResponse>();

            List<ReservaResponse> reservasDashboard =
                new List<ReservaResponse>();

            List<VeiculoResponse> veiculosDashboard =
                new List<VeiculoResponse>();

            List<CategoriaResponse> categoriasDashboard =
                new List<CategoriaResponse>();

            try
            {
                ConfigurarToken();

                DashboardResumoResponse? resumo =
                    await ApiClient
                        .GetAsync<DashboardResumoResponse>(
                            "api/Dashboard/resumo"
                        );

                if (resumo != null)
                {
                    totalVeiculos =
                        resumo.TotalVeiculos;

                    locacoesAtivas =
                        resumo.LocacoesAtivas;

                    totalClientes =
                        resumo.TotalClientes;

                    receitaMes =
                        resumo.ReceitaMes;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Não foi possível carregar os dados " +
                    "do Dashboard.\n\n" +
                    ex.Message,

                    "GoCar",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Warning
                );
            }

            try
            {
                ConfigurarToken();

                List<LocacaoResponse>? locacoes =
                    await ApiClient.GetAsync<List<LocacaoResponse>>(
                        "api/Locacoes"
                    );

                locacoesDashboard =
                    locacoes ?? new List<LocacaoResponse>();
            }
            catch
            {
                // O resumo principal continua funcionando mesmo se
                // o gráfico não conseguir carregar as locações.
                locacoesDashboard = new List<LocacaoResponse>();
            }

            try
            {
                ConfigurarToken();

                reservasDashboard =
                    await ApiClient.GetAsync<List<ReservaResponse>>(
                        "api/Reservas")
                    ?? new List<ReservaResponse>();

                veiculosDashboard =
                    await ApiClient.GetAsync<List<VeiculoResponse>>(
                        "api/Veiculos")
                    ?? new List<VeiculoResponse>();

                categoriasDashboard =
                    await ApiClient.GetAsync<List<CategoriaResponse>>(
                        "api/Categorias")
                    ?? new List<CategoriaResponse>();
            }
            catch
            {
                // Se algum dado complementar falhar, o restante do
                // Dashboard continua disponível normalmente.
                reservasDashboard = new List<ReservaResponse>();
                veiculosDashboard = new List<VeiculoResponse>();
                categoriasDashboard = new List<CategoriaResponse>();
            }

            Label lblBemVindo =
                new Label
                {
                    Text =
                        $"Bem-vindo(a), " +
                        $"{DesktopSession.Nome}",

                    ForeColor =
                        Color.FromArgb(
                            130,
                            80,
                            180
                        ),

                    Font =
                        new Font(
                            "Segoe UI",
                            9
                        ),

                    AutoSize =
                        true,

                    Location =
                        new Point(
                            40,
                            30
                        )
                };

            Label lblDashboard =
                new Label
                {
                    Text =
                        "Dashboard",

                    ForeColor =
                        Color.White,

                    Font =
                        new Font(
                            "Segoe UI",
                            22,
                            FontStyle.Bold
                        ),

                    AutoSize =
                        true,

                    Location =
                        new Point(
                            40,
                            55
                        )
                };

            Label lblDescricao =
                new Label
                {
                    Text =
                        "Acompanhe o resumo das atividades " +
                        "e o desempenho da sua locadora.",

                    ForeColor =
                        Color.Gray,

                    Font =
                        new Font(
                            "Segoe UI",
                            9
                        ),

                    AutoSize =
                        true,

                    Location =
                        new Point(
                            42,
                            100
                        )
                };

            pnlConteudo.Controls.Add(
                lblBemVindo
            );

            pnlConteudo.Controls.Add(
                lblDashboard
            );

            pnlConteudo.Controls.Add(
                lblDescricao
            );

            Panel cardVeiculos =
                CriarCard(
                    "Total de Veículos",
                    totalVeiculos.ToString(),
                    "▣",
                    40
                );

            Panel cardLocacoes =
                CriarCard(
                    "Locações Ativas",
                    locacoesAtivas.ToString(),
                    "▦",
                    275
                );

            Panel cardClientes =
                CriarCard(
                    "Clientes Cadastrados",
                    totalClientes.ToString(),
                    "●",
                    510
                );

            string receitaFormatada =
                receitaMes.ToString(
                    "C2",
                    new CultureInfo(
                        "pt-BR"
                    )
                );

            Panel cardReceita =
                CriarCard(
                    "Receita do Mês",
                    receitaFormatada,
                    "$",
                    745
                );

            pnlConteudo.Controls.Add(
                cardVeiculos
            );

            pnlConteudo.Controls.Add(
                cardLocacoes
            );

            pnlConteudo.Controls.Add(
                cardClientes
            );

            pnlConteudo.Controls.Add(
                cardReceita
            );

            Panel graficoLocacoes =
                CriarGraficoLocacoesUltimos7Dias(
                    locacoesDashboard
                );

            pnlConteudo.Controls.Add(
                graficoLocacoes
            );

            Panel categoriasMaisAlugadas =
                CriarCategoriasMaisAlugadas(
                    locacoesDashboard,
                    reservasDashboard,
                    veiculosDashboard,
                    categoriasDashboard
                );

            pnlConteudo.Controls.Add(
                categoriasMaisAlugadas
            );

            Panel ultimosVeiculos =
                CriarUltimosVeiculosCadastrados(
                    veiculosDashboard,
                    categoriasDashboard
                );

            pnlConteudo.Controls.Add(
                ultimosVeiculos
            );

            Panel atalhosRapidos = CriarAtalhosRapidos();
            pnlConteudo.Controls.Add(atalhosRapidos);
        }

        // =====================================================
        // ATALHOS RÁPIDOS
        // =====================================================

        private Panel CriarAtalhosRapidos()
        {
            Panel painel = new Panel
            {
                Location = new Point(40, 1255),
                Size = new Size(925, 185),
                BackColor = Color.FromArgb(24, 11, 42)
            };

            Label lblTitulo = new Label
            {
                Text = "Atalhos rápidos",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(22, 18)
            };

            Label lblDescricao = new Label
            {
                Text = "Acesse rapidamente as principais operações do sistema.",
                ForeColor = Color.FromArgb(170, 160, 180),
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(24, 50)
            };

            painel.Controls.Add(lblTitulo);
            painel.Controls.Add(lblDescricao);

            Button CriarAtalho(string texto, int x)
            {
                Button botao = new Button
                {
                    Text = texto,
                    Location = new Point(x, 92),
                    Size = new Size(200, 55),
                    BackColor = Color.FromArgb(111, 38, 201),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                botao.FlatAppearance.BorderSize = 0;
                botao.FlatAppearance.MouseOverBackColor = Color.FromArgb(130, 55, 220);
                return botao;
            }

            Button btnNovaReserva = CriarAtalho("+ Nova Reserva", 24);
            Button btnNovoCliente = CriarAtalho("+ Novo Cliente", 244);
            Button btnNovoVeiculo = CriarAtalho("+ Novo Veículo", 464);
            Button btnNovaLocacao = CriarAtalho("+ Nova Locação", 684);

            btnNovaReserva.Click += (s, e) =>
            {
                using (NovaReservaForm form = new NovaReservaForm()) form.ShowDialog(this);
                CriarConteudoDashboard();
            };
            btnNovoCliente.Click += (s, e) =>
            {
                using (NovoClienteForm form = new NovoClienteForm()) form.ShowDialog(this);
                CriarConteudoDashboard();
            };
            btnNovoVeiculo.Click += (s, e) =>
            {
                using (NovoVeiculoForm form = new NovoVeiculoForm()) form.ShowDialog(this);
                CriarConteudoDashboard();
            };
            btnNovaLocacao.Click += (s, e) =>
            {
                using (NovaLocacaoForm form = new NovaLocacaoForm()) form.ShowDialog(this);
                CriarConteudoDashboard();
            };

            painel.Controls.Add(btnNovaReserva);
            painel.Controls.Add(btnNovoCliente);
            painel.Controls.Add(btnNovoVeiculo);
            painel.Controls.Add(btnNovaLocacao);
            return painel;
        }

        // =====================================================
        // ÚLTIMOS VEÍCULOS CADASTRADOS
        // =====================================================

        private Panel CriarUltimosVeiculosCadastrados(
            IEnumerable<VeiculoResponse> veiculos,
            IEnumerable<CategoriaResponse> categorias)
        {
            Panel painel = new Panel
            {
                Location = new Point(40, 960),
                Size = new Size(925, 270),
                BackColor = Color.FromArgb(24, 11, 42)
            };

            Label lblTitulo = new Label
            {
                Text = "Últimos veículos cadastrados",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(22, 18)
            };

            Label lblDescricao = new Label
            {
                Text = "Veículos adicionados mais recentemente ao sistema.",
                ForeColor = Color.FromArgb(170, 160, 180),
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(24, 50)
            };

            painel.Controls.Add(lblTitulo);
            painel.Controls.Add(lblDescricao);

            var categoriasPorId = categorias
                .GroupBy(c => c.Id)
                .ToDictionary(g => g.Key, g => g.First());

            // Como o DTO atual de veículo não possui DataCriacao,
            // o Id (identity) é usado como ordem de cadastro.
            var ultimos = veiculos
                .OrderByDescending(v => v.Id)
                .Take(5)
                .ToList();

            if (ultimos.Count == 0)
            {
                Label lblSemDados = new Label
                {
                    Text = "Nenhum veículo cadastrado para exibir.",
                    ForeColor = Color.FromArgb(190, 185, 200),
                    Font = new Font("Segoe UI", 10),
                    AutoSize = true,
                    Location = new Point(24, 105)
                };

                painel.Controls.Add(lblSemDados);
                return painel;
            }

            int yCabecalho = 86;

            void AdicionarCabecalho(string texto, int x, int largura)
            {
                painel.Controls.Add(new Label
                {
                    Text = texto,
                    ForeColor = Color.FromArgb(150, 135, 165),
                    Font = new Font("Segoe UI", 8, FontStyle.Bold),
                    Size = new Size(largura, 20),
                    Location = new Point(x, yCabecalho)
                });
            }

            AdicionarCabecalho("VEÍCULO", 24, 240);
            AdicionarCabecalho("PLACA", 280, 115);
            AdicionarCabecalho("CATEGORIA", 410, 150);
            AdicionarCabecalho("DIÁRIA", 580, 135);
            AdicionarCabecalho("STATUS", 750, 120);

            Panel linhaCabecalho = new Panel
            {
                BackColor = Color.FromArgb(48, 31, 65),
                Location = new Point(24, 108),
                Size = new Size(865, 1)
            };
            painel.Controls.Add(linhaCabecalho);

            int y = 120;
            CultureInfo cultura = new CultureInfo("pt-BR");

            foreach (VeiculoResponse veiculo in ultimos)
            {
                string nomeVeiculo = $"{veiculo.Marca} {veiculo.Modelo}".Trim();
                if (string.IsNullOrWhiteSpace(nomeVeiculo))
                    nomeVeiculo = $"Veículo #{veiculo.Id}";

                string categoriaNome = categoriasPorId.TryGetValue(
                    veiculo.CategoriaId, out CategoriaResponse? categoria)
                    ? categoria.Nome
                    : "Não informada";

                string status = veiculo.IsAtivo ? "Ativo" : "Inativo";

                Label CriarCelula(string texto, int x, int largura, Color cor)
                {
                    return new Label
                    {
                        Text = texto,
                        ForeColor = cor,
                        Font = new Font("Segoe UI", 9),
                        AutoEllipsis = true,
                        Size = new Size(largura, 24),
                        Location = new Point(x, y)
                    };
                }

                painel.Controls.Add(CriarCelula(
                    nomeVeiculo, 24, 240, Color.White));
                painel.Controls.Add(CriarCelula(
                    veiculo.Placa, 280, 115, Color.Gainsboro));
                painel.Controls.Add(CriarCelula(
                    categoriaNome, 410, 150, Color.Gainsboro));
                painel.Controls.Add(CriarCelula(
                    veiculo.ValorDiaria.ToString("C2", cultura),
                    580, 135, Color.Gainsboro));
                painel.Controls.Add(CriarCelula(
                    status, 750, 120,
                    veiculo.IsAtivo
                        ? Color.FromArgb(125, 220, 160)
                        : Color.FromArgb(210, 150, 150)));

                y += 30;
            }

            return painel;
        }

        // =====================================================
        // CATEGORIAS MAIS ALUGADAS
        // =====================================================

        private Panel CriarCategoriasMaisAlugadas(
            IEnumerable<LocacaoResponse> locacoes,
            IEnumerable<ReservaResponse> reservas,
            IEnumerable<VeiculoResponse> veiculos,
            IEnumerable<CategoriaResponse> categorias)
        {
            Panel painel = new Panel
            {
                Location = new Point(40, 650),
                Size = new Size(925, 285),
                BackColor = Color.FromArgb(24, 11, 42)
            };

            Label lblTitulo = new Label
            {
                Text = "Categorias mais alugadas",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(22, 18)
            };

            Label lblDescricao = new Label
            {
                Text = "Quantidade de locações realizadas por categoria de veículo.",
                ForeColor = Color.FromArgb(170, 160, 180),
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(24, 50)
            };

            painel.Controls.Add(lblTitulo);
            painel.Controls.Add(lblDescricao);

            var reservasPorId = reservas.ToDictionary(r => r.Id);
            var veiculosPorId = veiculos.ToDictionary(v => v.Id);

            var dados = categorias
                .Select(c => new
                {
                    Categoria = c,
                    Quantidade = locacoes.Count(l =>
                        reservasPorId.TryGetValue(l.ReservaId, out ReservaResponse? reserva) &&
                        veiculosPorId.TryGetValue(reserva.VeiculoId, out VeiculoResponse? veiculo) &&
                        veiculo.CategoriaId == c.Id)
                })
                .OrderByDescending(x => x.Quantidade)
                .ThenBy(x => x.Categoria.Nome)
                .Take(5)
                .ToList();

            if (dados.Count == 0)
            {
                Label lblSemDados = new Label
                {
                    Text = "Nenhuma categoria cadastrada para exibir.",
                    ForeColor = Color.FromArgb(190, 185, 200),
                    Font = new Font("Segoe UI", 10),
                    AutoSize = true,
                    Location = new Point(24, 100)
                };

                painel.Controls.Add(lblSemDados);
                return painel;
            }

            int maiorValor = Math.Max(1, dados.Max(d => d.Quantidade));
            int y = 88;

            foreach (var item in dados)
            {
                Label lblNome = new Label
                {
                    Text = item.Categoria.Nome,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    AutoEllipsis = true,
                    Size = new Size(170, 24),
                    Location = new Point(24, y)
                };

                Panel trilha = new Panel
                {
                    BackColor = Color.FromArgb(48, 31, 65),
                    Size = new Size(610, 14),
                    Location = new Point(205, y + 4)
                };

                int largura = item.Quantidade == 0
                    ? 4
                    : Math.Max(20,
                        (int)Math.Round(
                            (item.Quantidade / (double)maiorValor) *
                            trilha.Width));

                Panel barra = new Panel
                {
                    BackColor = Color.FromArgb(111, 38, 201),
                    Size = new Size(largura, trilha.Height),
                    Location = new Point(0, 0)
                };

                trilha.Controls.Add(barra);

                Label lblQuantidade = new Label
                {
                    Text = item.Quantidade.ToString(),
                    ForeColor = Color.FromArgb(205, 160, 245),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleRight,
                    Size = new Size(55, 24),
                    Location = new Point(835, y - 1)
                };

                painel.Controls.Add(lblNome);
                painel.Controls.Add(trilha);
                painel.Controls.Add(lblQuantidade);

                y += 36;
            }

            int totalLocacoesMapeadas = dados.Sum(d => d.Quantidade);

            Label lblRodape = new Label
            {
                Text = $"Locações consideradas: {totalLocacoesMapeadas}",
                ForeColor = Color.FromArgb(190, 135, 235),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(24, 252)
            };

            painel.Controls.Add(lblRodape);

            return painel;
        }

        // =====================================================
        // GRÁFICO - LOCAÇÕES NOS ÚLTIMOS 7 DIAS
        // =====================================================

        private Panel CriarGraficoLocacoesUltimos7Dias(
            IEnumerable<LocacaoResponse> locacoes)
        {
            Panel painel = new Panel
            {
                Location = new Point(40, 310),
                Size = new Size(925, 315),
                BackColor = Color.FromArgb(24, 11, 42)
            };

            Label lblTitulo = new Label
            {
                Text = "Locações nos últimos 7 dias",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(22, 18)
            };

            Label lblDescricao = new Label
            {
                Text = "Quantidade de locações iniciadas por dia.",
                ForeColor = Color.FromArgb(170, 160, 180),
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(24, 50)
            };

            painel.Controls.Add(lblTitulo);
            painel.Controls.Add(lblDescricao);

            DateTime hoje = DateTime.Today;
            DateTime inicio = hoje.AddDays(-6);

            var dados = Enumerable.Range(0, 7)
                .Select(i =>
                {
                    DateTime dia = inicio.AddDays(i);
                    int quantidade = locacoes.Count(l =>
                        l.DataRetirada.Date == dia.Date);

                    return new
                    {
                        Dia = dia,
                        Quantidade = quantidade
                    };
                })
                .ToList();

            int maiorValor = Math.Max(1, dados.Max(d => d.Quantidade));

            int areaTopo = 90;
            int alturaGrafico = 150;
            int larguraBarra = 52;
            int espacamento = 65;
            int inicioX = 55;

            Panel linhaBase = new Panel
            {
                Location = new Point(30, areaTopo + alturaGrafico),
                Size = new Size(865, 1),
                BackColor = Color.FromArgb(70, 55, 85)
            };

            painel.Controls.Add(linhaBase);

            for (int i = 0; i < dados.Count; i++)
            {
                int x = inicioX + (i * (larguraBarra + espacamento));
                int alturaBarra = dados[i].Quantidade == 0
                    ? 4
                    : Math.Max(18,
                        (int)Math.Round(
                            (dados[i].Quantidade / (double)maiorValor) *
                            alturaGrafico));

                Label lblQuantidade = new Label
                {
                    Text = dados[i].Quantidade.ToString(),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(larguraBarra, 22),
                    Location = new Point(
                        x,
                        areaTopo + alturaGrafico - alturaBarra - 24)
                };

                Panel barra = new Panel
                {
                    Location = new Point(
                        x,
                        areaTopo + alturaGrafico - alturaBarra),
                    Size = new Size(larguraBarra, alturaBarra),
                    BackColor = Color.FromArgb(111, 38, 201)
                };

                Label lblDia = new Label
                {
                    Text = dados[i].Dia.ToString("dd/MM"),
                    ForeColor = Color.FromArgb(200, 195, 210),
                    Font = new Font("Segoe UI", 9),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(70, 25),
                    Location = new Point(x - 9, areaTopo + alturaGrafico + 8)
                };

                painel.Controls.Add(lblQuantidade);
                painel.Controls.Add(barra);
                painel.Controls.Add(lblDia);
            }

            int totalPeriodo = dados.Sum(d => d.Quantidade);

            Label lblTotal = new Label
            {
                Text = $"Total no período: {totalPeriodo}",
                ForeColor = Color.FromArgb(190, 135, 235),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(24, 282)
            };

            painel.Controls.Add(lblTotal);

            return painel;
        }

        // =====================================================
        // CARD
        // =====================================================

        private Panel CriarCard(
            string titulo,
            string valor,
            string icone,
            int x)
        {
            Panel card =
                new Panel
                {
                    Location =
                        new Point(
                            x,
                            150
                        ),

                    Size =
                        new Size(
                            220,
                            125
                        ),

                    BackColor =
                        Color.FromArgb(
                            24,
                            11,
                            42
                        )
                };

            Label lblIcone =
                new Label
                {
                    Text =
                        icone,

                    ForeColor =
                        Color.White,

                    Font =
                        new Font(
                            "Segoe UI Symbol",
                            18
                        ),

                    AutoSize =
                        true,

                    Location =
                        new Point(
                            15,
                            10
                        )
                };

            Label lblTitulo =
                new Label
                {
                    Text =
                        titulo,

                    ForeColor =
                        Color.FromArgb(
                            190,
                            180,
                            200
                        ),

                    Font =
                        new Font(
                            "Segoe UI",
                            9
                        ),

                    AutoSize =
                        true,

                    Location =
                        new Point(
                            15,
                            45
                        )
                };

            Label lblValor =
                new Label
                {
                    Text =
                        valor,

                    ForeColor =
                        Color.White,

                    Font =
                        new Font(
                            "Segoe UI",
                            18,
                            FontStyle.Bold
                        ),

                    AutoSize =
                        true,

                    Location =
                        new Point(
                            15,
                            64
                        )
                };

            Label lblVariacao =
                new Label
                {
                    Text =
                        string.Empty,

                    ForeColor =
                        Color.LimeGreen,

                    Font =
                        new Font(
                            "Segoe UI",
                            8
                        ),

                    AutoSize =
                        true,

                    Location =
                        new Point(
                            15,
                            102
                        )
                };

            card.Controls.Add(
                lblIcone
            );

            card.Controls.Add(
                lblTitulo
            );

            card.Controls.Add(
                lblValor
            );

            card.Controls.Add(
                lblVariacao
            );

            return card;
        }

        // =====================================================
        // VEÍCULOS
        // =====================================================

        private async Task CriarConteudoVeiculos()
        {
            pnlConteudo.Controls.Clear();

            Label lblTitulo =
                CriarTituloPagina(
                    "Veículos"
                );

            Label lblDescricao =
                CriarDescricaoPagina(
                    "Gerencie os veículos disponíveis na locadora."
                );

            TextBox txtBusca =
                CriarCampoBusca();

            txtBusca.PlaceholderText =
                "Pesquisar por modelo, marca ou placa...";

            txtBusca.Size =
                new Size(
                    330,
                    35
                );

            ComboBox cmbStatus =
                new ComboBox
                {
                    Location =
                        new Point(
                            385,
                            120
                        ),

                    Size =
                        new Size(
                            155,
                            35
                        ),

                    BackColor =
                        Color.FromArgb(
                            18,
                            8,
                            30
                        ),

                    ForeColor =
                        Color.White,

                    FlatStyle =
                        FlatStyle.Flat,

                    DropDownStyle =
                        ComboBoxStyle.DropDownList,

                    Font =
                        new Font(
                            "Segoe UI",
                            10
                        )
                };

            cmbStatus.Items.AddRange(
                new object[]
                {
                    "Todos os status",
                    "Ativos",
                    "Inativos"
                }
            );

            cmbStatus.SelectedIndex = 0;

            Button btnNovoVeiculo =
                CriarBotaoNovo(
                    "+ Novo Veículo",
                    560,
                    160
                );

            DataGridView dgvVeiculos =
                CriarTabela();

            dgvVeiculos.Columns.Add(
                "Modelo",
                "Modelo"
            );

            dgvVeiculos.Columns.Add(
                "Marca",
                "Marca"
            );

            dgvVeiculos.Columns.Add(
                "Placa",
                "Placa"
            );

            dgvVeiculos.Columns.Add(
                "Ano",
                "Ano"
            );

            dgvVeiculos.Columns.Add(
                "Km",
                "Km atual"
            );

            dgvVeiculos.Columns.Add(
                "Diaria",
                "Diária"
            );

            dgvVeiculos.Columns.Add(
                "Status",
                "Status"
            );

            pnlConteudo.Controls.Add(
                lblTitulo
            );

            pnlConteudo.Controls.Add(
                lblDescricao
            );

            pnlConteudo.Controls.Add(
                txtBusca
            );

            pnlConteudo.Controls.Add(
                cmbStatus
            );

            pnlConteudo.Controls.Add(
                btnNovoVeiculo
            );

            pnlConteudo.Controls.Add(
                dgvVeiculos
            );

            async Task CarregarVeiculosAsync()
            {
                try
                {
                    ConfigurarToken();

                    List<VeiculoResponse>? veiculos =
                        await ApiClient
                            .GetAsync<
                                List<VeiculoResponse>
                            >(
                                "api/Veiculos"
                            );

                    veiculosCarregados =
                        veiculos ??
                        new List<VeiculoResponse>();

                    PreencherTabelaVeiculos(
                        dgvVeiculos,
                        veiculosCarregados
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Não foi possível carregar " +
                        "os veículos.\n\n" +
                        ex.Message,

                        "GoCar",

                        MessageBoxButtons.OK,

                        MessageBoxIcon.Warning
                    );
                }
            }

            btnNovoVeiculo.Click +=
                async (s, e) =>
                {
                    using (
                        NovoVeiculoForm form =
                            new NovoVeiculoForm()
                    )
                    {
                        DialogResult resultado =
                            form.ShowDialog(
                                this
                            );

                        if (
                            resultado ==
                                DialogResult.OK
                            &&
                            form.VeiculoCadastrado
                        )
                        {
                            await CarregarVeiculosAsync();
                        }
                    }
                };

            void AplicarFiltros()
            {
                string busca =
                    txtBusca.Text
                        .Trim()
                        .ToLowerInvariant();

                IEnumerable<VeiculoResponse> filtrados =
                    veiculosCarregados;

                if (
                    !string.IsNullOrWhiteSpace(
                        busca
                    )
                )
                {
                    filtrados =
                        filtrados.Where(
                            v =>
                                v.Modelo
                                    .ToLowerInvariant()
                                    .Contains(busca)
                                ||
                                v.Marca
                                    .ToLowerInvariant()
                                    .Contains(busca)
                                ||
                                v.Placa
                                    .ToLowerInvariant()
                                    .Contains(busca)
                        );
                }

                if (
                    cmbStatus.SelectedIndex == 1
                )
                {
                    filtrados =
                        filtrados.Where(
                            v => v.IsAtivo
                        );
                }
                else if (
                    cmbStatus.SelectedIndex == 2
                )
                {
                    filtrados =
                        filtrados.Where(
                            v => !v.IsAtivo
                        );
                }

                PreencherTabelaVeiculos(
                    dgvVeiculos,
                    filtrados.ToList()
                );
            }

            txtBusca.TextChanged +=
                (s, e) =>
                {
                    AplicarFiltros();
                };

            cmbStatus.SelectedIndexChanged +=
                (s, e) =>
                {
                    AplicarFiltros();
                };

            await CarregarVeiculosAsync();
        }

        // =====================================================
        // PREENCHER VEÍCULOS
        // =====================================================

        private void PreencherTabelaVeiculos(
            DataGridView tabela,
            IEnumerable<VeiculoResponse> veiculos)
        {
            tabela.Rows.Clear();

            CultureInfo cultura =
                new CultureInfo(
                    "pt-BR"
                );

            foreach (
                VeiculoResponse veiculo
                in veiculos
            )
            {
                string diaria =
                    veiculo
                        .ValorDiaria
                        .ToString(
                            "C2",
                            cultura
                        );

                string km =
                    veiculo
                        .KmAtual
                        .ToString(
                            "N0",
                            cultura
                        );

                string status =
                    veiculo.IsAtivo
                        ? "Ativo"
                        : "Inativo";

                tabela.Rows.Add(
                    veiculo.Modelo,
                    veiculo.Marca,
                    veiculo.Placa,
                    veiculo.AnoModelo,
                    km,
                    diaria,
                    status
                );
            }
        }

        // =====================================================
        // CATEGORIAS
        // =====================================================

        private async Task CriarConteudoCategorias()
        {
            pnlConteudo.Controls.Clear();

            Label lblTitulo =
                CriarTituloPagina(
                    "Categorias"
                );

            Label lblDescricao =
                CriarDescricaoPagina(
                    "Gerencie as categorias de veículos da locadora."
                );

            TextBox txtBusca =
                CriarCampoBusca();

            Button btnNovaCategoria =
                CriarBotaoNovo(
                    "+ Nova Categoria",
                    560,
                    170
                );

            DataGridView dgvCategorias =
                CriarTabela();

            dgvCategorias.Columns.Add(
                "Nome",
                "Nome"
            );

            dgvCategorias.Columns.Add(
                "Descricao",
                "Descrição"
            );

            dgvCategorias.Columns.Add(
                "Diaria",
                "Diária Base"
            );

            dgvCategorias.Columns.Add(
                "KmLivre",
                "Km Livre"
            );

            dgvCategorias.Columns.Add(
                "Status",
                "Status"
            );

            pnlConteudo.Controls.Add(
                lblTitulo
            );

            pnlConteudo.Controls.Add(
                lblDescricao
            );

            pnlConteudo.Controls.Add(
                txtBusca
            );

            pnlConteudo.Controls.Add(
                btnNovaCategoria
            );

            pnlConteudo.Controls.Add(
                dgvCategorias
            );

            async Task CarregarCategoriasAsync()
            {
                try
                {
                    ConfigurarToken();

                    List<CategoriaResponse>? categorias =
                        await ApiClient
                            .GetAsync<
                                List<CategoriaResponse>
                            >(
                                "api/Categorias"
                            );

                    categoriasCarregadas =
                        categorias ??
                        new List<CategoriaResponse>();

                    PreencherTabelaCategorias(
                        dgvCategorias,
                        categoriasCarregadas
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Não foi possível carregar " +
                        "as categorias.\n\n" +
                        ex.Message,

                        "GoCar",

                        MessageBoxButtons.OK,

                        MessageBoxIcon.Warning
                    );
                }
            }

            btnNovaCategoria.Click +=
                async (s, e) =>
                {
                    bool cadastrou =
                        await AbrirCadastroCategoria();

                    if (cadastrou)
                    {
                        await CarregarCategoriasAsync();
                    }
                };

            txtBusca.TextChanged +=
                (s, e) =>
                {
                    string busca =
                        txtBusca.Text
                            .Trim()
                            .ToLowerInvariant();

                    if (
                        string.IsNullOrWhiteSpace(
                            busca
                        )
                    )
                    {
                        PreencherTabelaCategorias(
                            dgvCategorias,
                            categoriasCarregadas
                        );

                        return;
                    }

                    List<CategoriaResponse> filtradas =
                        categoriasCarregadas
                            .Where(
                                c =>
                                    c.Nome
                                        .ToLowerInvariant()
                                        .Contains(busca)
                                    ||
                                    c.Descricao
                                        .ToLowerInvariant()
                                        .Contains(busca)
                            )
                            .ToList();

                    PreencherTabelaCategorias(
                        dgvCategorias,
                        filtradas
                    );
                };

            await CarregarCategoriasAsync();
        }

        // =====================================================
        // PREENCHER CATEGORIAS
        // =====================================================

        private void PreencherTabelaCategorias(
            DataGridView tabela,
            IEnumerable<CategoriaResponse> categorias)
        {
            tabela.Rows.Clear();

            CultureInfo cultura =
                new CultureInfo(
                    "pt-BR"
                );

            foreach (
                CategoriaResponse categoria
                in categorias
            )
            {
                tabela.Rows.Add(
                    categoria.Nome,

                    categoria.Descricao,

                    categoria
                        .DiariaBase
                        .ToString(
                            "C2",
                            cultura
                        ),

                    categoria
                        .KmLivre
                        .ToString(
                            "N0",
                            cultura
                        ),

                    categoria.IsAtivo
                        ? "Ativo"
                        : "Inativo"
                );
            }
        }

        // =====================================================
        // FILIAIS
        // =====================================================

        private async Task CriarConteudoFiliais()
        {
            pnlConteudo.Controls.Clear();

            Label lblTitulo = CriarTituloPagina("Filiais");
            Label lblDescricao = CriarDescricaoPagina(
                "Gerencie as unidades da locadora.");
            TextBox txtBusca = CriarCampoBusca();
            Button btnNovaFilial = CriarBotaoNovo(
                "+ Nova Filial", 560, 160);
            DataGridView dgvFiliais = CriarTabela();

            dgvFiliais.Columns.Add("Nome", "Nome");
            dgvFiliais.Columns.Add("CNPJ", "CNPJ");
            dgvFiliais.Columns.Add("Telefone", "Telefone");
            dgvFiliais.Columns.Add("Email", "E-mail");
            dgvFiliais.Columns.Add("Cidade", "Cidade");
            dgvFiliais.Columns.Add("CEP", "CEP");
            dgvFiliais.Columns.Add("Status", "Status");

            pnlConteudo.Controls.Add(lblTitulo);
            pnlConteudo.Controls.Add(lblDescricao);
            pnlConteudo.Controls.Add(txtBusca);
            pnlConteudo.Controls.Add(btnNovaFilial);
            pnlConteudo.Controls.Add(dgvFiliais);

            async Task CarregarFiliaisAsync()
            {
                try
                {
                    ConfigurarToken();

                    List<FilialResponse>? filiais =
                        await ApiClient.GetAsync<List<FilialResponse>>(
                            "api/Filiais");

                    filiaisCarregadas =
                        filiais ?? new List<FilialResponse>();

                    PreencherTabelaFiliais(
                        dgvFiliais, filiaisCarregadas);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Não foi possível carregar as filiais.\n\n" +
                        ex.Message,
                        "GoCar",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }

            btnNovaFilial.Click += async (s, e) =>
            {
                using NovaFilialForm form = new NovaFilialForm();
                DialogResult resultado = form.ShowDialog(this);

                if (resultado == DialogResult.OK &&
                    form.FilialCadastrada)
                {
                    await CarregarFiliaisAsync();
                }
            };

            txtBusca.TextChanged += (s, e) =>
            {
                string busca = txtBusca.Text
                    .Trim()
                    .ToLowerInvariant();

                IEnumerable<FilialResponse> resultado =
                    filiaisCarregadas;

                if (!string.IsNullOrWhiteSpace(busca))
                {
                    resultado = filiaisCarregadas.Where(f =>
                        (f.Nome ?? string.Empty).ToLowerInvariant().Contains(busca) ||
                        (f.CNPJ ?? string.Empty).ToLowerInvariant().Contains(busca) ||
                        (f.Telefone ?? string.Empty).ToLowerInvariant().Contains(busca) ||
                        (f.Email ?? string.Empty).ToLowerInvariant().Contains(busca) ||
                        (f.Cidade ?? string.Empty).ToLowerInvariant().Contains(busca) ||
                        (f.Estado ?? string.Empty).ToLowerInvariant().Contains(busca) ||
                        (f.CEP ?? string.Empty).ToLowerInvariant().Contains(busca));
                }

                PreencherTabelaFiliais(dgvFiliais, resultado);
            };

            await CarregarFiliaisAsync();
        }

        private void PreencherTabelaFiliais(
            DataGridView tabela,
            IEnumerable<FilialResponse> filiais)
        {
            tabela.Rows.Clear();

            foreach (FilialResponse filial in filiais)
            {
                string cidade = string.IsNullOrWhiteSpace(filial.Estado)
                    ? filial.Cidade
                    : $"{filial.Cidade} - {filial.Estado}";

                tabela.Rows.Add(
                    filial.Nome,
                    FormatarCnpj(filial.CNPJ),
                    FormatarTelefone(filial.Telefone),
                    filial.Email,
                    cidade,
                    FormatarCep(filial.CEP),
                    filial.IsAtivo ? "Ativo" : "Inativo");
            }
        }

        private static string FormatarCnpj(string cnpj)
        {
            string numeros = new string(
                (cnpj ?? string.Empty).Where(char.IsDigit).ToArray());

            if (numeros.Length != 14)
                return cnpj ?? string.Empty;

            return $"{numeros.Substring(0, 2)}." +
                   $"{numeros.Substring(2, 3)}." +
                   $"{numeros.Substring(5, 3)}/" +
                   $"{numeros.Substring(8, 4)}-" +
                   $"{numeros.Substring(12, 2)}";
        }

        private static string FormatarCep(string cep)
        {
            string numeros = new string(
                (cep ?? string.Empty).Where(char.IsDigit).ToArray());

            if (numeros.Length != 8)
                return cep ?? string.Empty;

            return $"{numeros.Substring(0, 5)}-{numeros.Substring(5, 3)}";
        }

        // =====================================================
        // CLIENTES
        // =====================================================

        private async Task CriarConteudoClientes()
        {
            pnlConteudo.Controls.Clear();

            Label lblTitulo =
                CriarTituloPagina("Clientes");

            Label lblDescricao =
                CriarDescricaoPagina(
                    "Gerencie os clientes cadastrados na locadora.");

            TextBox txtBusca =
                CriarCampoBusca();

            txtBusca.PlaceholderText =
                "Pesquisar por nome, CPF, telefone, CNH ou cidade...";

            txtBusca.Size =
                new Size(
                    430,
                    35
                );

            Button btnNovoCliente =
                CriarBotaoNovo(
                    "+ Novo Cliente",
                    560,
                    160
                );

            DataGridView dgvClientes =
                CriarTabela();

            dgvClientes.Columns.Add(
                "Nome",
                "Nome"
            );

            dgvClientes.Columns.Add(
                "CPF",
                "CPF"
            );

            dgvClientes.Columns.Add(
                "Telefone",
                "Telefone"
            );

            dgvClientes.Columns.Add(
                "CNH",
                "CNH"
            );

            dgvClientes.Columns.Add(
                "Categoria",
                "Categoria CNH"
            );

            dgvClientes.Columns.Add(
                "Cidade",
                "Cidade"
            );

            dgvClientes.Columns.Add(
                "Status",
                "Status"
            );

            pnlConteudo.Controls.Add(
                lblTitulo
            );

            pnlConteudo.Controls.Add(
                lblDescricao
            );

            pnlConteudo.Controls.Add(
                txtBusca
            );

            pnlConteudo.Controls.Add(
                btnNovoCliente
            );

            pnlConteudo.Controls.Add(
                dgvClientes
            );

            void AplicarFiltro()
            {
                string busca =
                    txtBusca.Text
                        .Trim()
                        .ToLowerInvariant();

                if (
                    string.IsNullOrWhiteSpace(
                        busca
                    )
                )
                {
                    PreencherTabelaClientes(
                        dgvClientes,
                        clientesCarregados
                    );

                    return;
                }

                List<ClienteResponse> filtrados =
                    clientesCarregados
                        .Where(
                            c =>
                                (c.Nome ?? string.Empty)
                                    .ToLowerInvariant()
                                    .Contains(busca)
                                ||
                                (c.CPF ?? string.Empty)
                                    .ToLowerInvariant()
                                    .Contains(busca)
                                ||
                                (c.Telefone ?? string.Empty)
                                    .ToLowerInvariant()
                                    .Contains(busca)
                                ||
                                (c.CNH ?? string.Empty)
                                    .ToLowerInvariant()
                                    .Contains(busca)
                                ||
                                (c.Cidade ?? string.Empty)
                                    .ToLowerInvariant()
                                    .Contains(busca)
                                ||
                                (c.Estado ?? string.Empty)
                                    .ToLowerInvariant()
                                    .Contains(busca)
                        )
                        .ToList();

                PreencherTabelaClientes(
                    dgvClientes,
                    filtrados
                );
            }

            async Task CarregarClientesAsync()
            {
                try
                {
                    ConfigurarToken();

                    List<ClienteResponse>? clientes =
                        await ApiClient.GetAsync<List<ClienteResponse>>(
                            "api/Clientes");

                    clientesCarregados =
                        clientes ??
                        new List<ClienteResponse>();

                    AplicarFiltro();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Não foi possível carregar os clientes.\n\n" +
                        ex.Message,
                        "GoCar",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }

            btnNovoCliente.Click +=
                async (s, e) =>
                {
                    using (
                        NovoClienteForm form =
                            new NovoClienteForm()
                    )
                    {
                        DialogResult resultado =
                            form.ShowDialog(this);

                        if (
                            resultado == DialogResult.OK &&
                            form.ClienteCadastrado
                        )
                        {
                            await CarregarClientesAsync();
                        }
                    }
                };

            txtBusca.TextChanged +=
                (s, e) =>
                {
                    AplicarFiltro();
                };

            await CarregarClientesAsync();
        }

        // =====================================================
        // PREENCHER CLIENTES
        // =====================================================

        private void PreencherTabelaClientes(
            DataGridView tabela,
            IEnumerable<ClienteResponse> clientes)
        {
            tabela.Rows.Clear();

            foreach (ClienteResponse cliente in clientes)
            {
                string status = cliente.IsAtivo ? "Ativo" : "Inativo";

                string cidade = string.IsNullOrWhiteSpace(cliente.Estado)
                    ? cliente.Cidade
                    : $"{cliente.Cidade} - {cliente.Estado}";

                tabela.Rows.Add(
                    cliente.Nome,
                    FormatarCpf(cliente.CPF),
                    FormatarTelefone(cliente.Telefone),
                    cliente.CNH,
                    cliente.CategoriaCNH,
                    cidade,
                    status);
            }
        }

        private static string FormatarCpf(string cpf)
        {
            string numeros = new string(
                (cpf ?? string.Empty).Where(char.IsDigit).ToArray());

            if (numeros.Length != 11)
                return cpf ?? string.Empty;

            return $"{numeros.Substring(0, 3)}." +
                   $"{numeros.Substring(3, 3)}." +
                   $"{numeros.Substring(6, 3)}-" +
                   $"{numeros.Substring(9, 2)}";
        }

        private static string FormatarTelefone(string telefone)
        {
            string numeros = new string(
                (telefone ?? string.Empty).Where(char.IsDigit).ToArray());

            if (numeros.Length == 11)
            {
                return $"({numeros.Substring(0, 2)}) " +
                       $"{numeros.Substring(2, 5)}-" +
                       $"{numeros.Substring(7, 4)}";
            }

            if (numeros.Length == 10)
            {
                return $"({numeros.Substring(0, 2)}) " +
                       $"{numeros.Substring(2, 4)}-" +
                       $"{numeros.Substring(6, 4)}";
            }

            return telefone ?? string.Empty;
        }

        // =====================================================
        // CADASTRO DE CATEGORIA
        // =====================================================

        private async Task<bool> AbrirCadastroCategoria()
        {
            using Form form =
                new Form
                {
                    Text =
                        "GoCar - Nova Categoria",

                    StartPosition =
                        FormStartPosition.CenterParent,

                    ClientSize =
                        new Size(
                            600,
                            500
                        ),

                    FormBorderStyle =
                        FormBorderStyle.FixedDialog,

                    MaximizeBox =
                        false,

                    MinimizeBox =
                        false,

                    BackColor =
                        Color.FromArgb(
                            8,
                            0,
                            20
                        ),

                    ForeColor =
                        Color.White
                };

            Label lblTitulo =
                new Label
                {
                    Text =
                        "Nova Categoria",

                    ForeColor =
                        Color.White,

                    Font =
                        new Font(
                            "Segoe UI",
                            22,
                            FontStyle.Bold
                        ),

                    AutoSize =
                        true,

                    Location =
                        new Point(
                            40,
                            30
                        )
                };

            Label lblSubtitulo =
                new Label
                {
                    Text =
                        "Cadastre uma nova categoria de veículo.",

                    ForeColor =
                        Color.Gray,

                    Font =
                        new Font(
                            "Segoe UI",
                            9
                        ),

                    AutoSize =
                        true,

                    Location =
                        new Point(
                            42,
                            75
                        )
                };

            Label lblNome =
                CriarLabelCampo(
                    "Nome",
                    40,
                    120
                );

            TextBox txtNome =
                CriarCampoCategoria(
                    40,
                    145,
                    500
                );

            Label lblDescricao =
                CriarLabelCampo(
                    "Descrição",
                    40,
                    200
                );

            TextBox txtDescricao =
                CriarCampoCategoria(
                    40,
                    225,
                    500
                );

            Label lblDiaria =
                CriarLabelCampo(
                    "Diária Base",
                    40,
                    280
                );

            TextBox txtDiaria =
                CriarCampoCategoria(
                    40,
                    305,
                    235
                );

            Label lblKm =
                CriarLabelCampo(
                    "Km Livre",
                    305,
                    280
                );

            TextBox txtKm =
                CriarCampoCategoria(
                    305,
                    305,
                    235
                );

            Button btnCancelar =
                new Button
                {
                    Text =
                        "Cancelar",

                    Location =
                        new Point(
                            240,
                            390
                        ),

                    Size =
                        new Size(
                            140,
                            45
                        ),

                    BackColor =
                        Color.FromArgb(
                            18,
                            8,
                            30
                        ),

                    ForeColor =
                        Color.White,

                    FlatStyle =
                        FlatStyle.Flat,

                    Cursor =
                        Cursors.Hand
                };

            Button btnSalvar =
                new Button
                {
                    Text =
                        "Salvar Categoria",

                    Location =
                        new Point(
                            400,
                            390
                        ),

                    Size =
                        new Size(
                            140,
                            45
                        ),

                    BackColor =
                        Color.FromArgb(
                            111,
                            38,
                            201
                        ),

                    ForeColor =
                        Color.White,

                    FlatStyle =
                        FlatStyle.Flat,

                    Cursor =
                        Cursors.Hand,

                    Font =
                        new Font(
                            "Segoe UI",
                            9,
                            FontStyle.Bold
                        )
                };

            btnSalvar
                .FlatAppearance
                .BorderSize = 0;

            form.Controls.Add(
                lblTitulo
            );

            form.Controls.Add(
                lblSubtitulo
            );

            form.Controls.Add(
                lblNome
            );

            form.Controls.Add(
                txtNome
            );

            form.Controls.Add(
                lblDescricao
            );

            form.Controls.Add(
                txtDescricao
            );

            form.Controls.Add(
                lblDiaria
            );

            form.Controls.Add(
                txtDiaria
            );

            form.Controls.Add(
                lblKm
            );

            form.Controls.Add(
                txtKm
            );

            form.Controls.Add(
                btnCancelar
            );

            form.Controls.Add(
                btnSalvar
            );

            bool cadastrado =
                false;

            btnCancelar.Click +=
                (s, e) =>
                {
                    form.DialogResult =
                        DialogResult.Cancel;

                    form.Close();
                };

            btnSalvar.Click +=
                async (s, e) =>
                {
                    if (
                        string.IsNullOrWhiteSpace(
                            txtNome.Text
                        )
                    )
                    {
                        MessageBox.Show(
                            "Informe o nome da categoria.",

                            "GoCar",

                            MessageBoxButtons.OK,

                            MessageBoxIcon.Warning
                        );

                        txtNome.Focus();

                        return;
                    }

                    if (
                        !TentarConverterDecimal(
                            txtDiaria.Text,
                            out decimal diariaBase
                        )
                    )
                    {
                        MessageBox.Show(
                            "Informe uma diária base válida.",

                            "GoCar",

                            MessageBoxButtons.OK,

                            MessageBoxIcon.Warning
                        );

                        txtDiaria.Focus();

                        return;
                    }

                    if (
                        !TentarConverterDecimal(
                            txtKm.Text,
                            out decimal kmLivre
                        )
                    )
                    {
                        MessageBox.Show(
                            "Informe um Km Livre válido.",

                            "GoCar",

                            MessageBoxButtons.OK,

                            MessageBoxIcon.Warning
                        );

                        txtKm.Focus();

                        return;
                    }

                    try
                    {
                        ConfigurarToken();

                        btnSalvar.Enabled =
                            false;

                        btnSalvar.Text =
                            "Salvando...";

                        var novaCategoria =
                            new
                            {
                                nome =
                                    txtNome.Text.Trim(),

                                descricao =
                                    txtDescricao.Text.Trim(),

                                diariaBase =
                                    diariaBase,

                                kmLivre =
                                    kmLivre
                            };

                        HttpResponseMessage response =
                            await ApiClient
                                .PostAsync(
                                    "api/Categorias",
                                    novaCategoria
                                );

                        if (
                            response.IsSuccessStatusCode
                        )
                        {
                            cadastrado =
                                true;

                            MessageBox.Show(
                                "Categoria cadastrada com sucesso!",

                                "GoCar",

                                MessageBoxButtons.OK,

                                MessageBoxIcon.Information
                            );

                            form.DialogResult =
                                DialogResult.OK;

                            form.Close();

                            return;
                        }

                        string erro =
                            await response
                                .Content
                                .ReadAsStringAsync();

                        MessageBox.Show(
                            "Não foi possível cadastrar " +
                            "a categoria.\n\n" +
                            $"Código: " +
                            $"{(int)response.StatusCode}\n\n" +
                            erro,

                            "GoCar",

                            MessageBoxButtons.OK,

                            MessageBoxIcon.Error
                        );
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "Erro ao cadastrar categoria.\n\n" +
                            ex.Message,

                            "GoCar",

                            MessageBoxButtons.OK,

                            MessageBoxIcon.Error
                        );
                    }
                    finally
                    {
                        if (!form.IsDisposed)
                        {
                            btnSalvar.Enabled =
                                true;

                            btnSalvar.Text =
                                "Salvar Categoria";
                        }
                    }
                };

            form.ShowDialog(
                this
            );

            return cadastrado;
        }

        // =====================================================
        // RESERVAS
        // =====================================================

        private async Task CriarConteudoReservas()
        {
            pnlConteudo.Controls.Clear();

            Label lblTitulo =
                CriarTituloPagina("Reservas");

            Label lblDescricao =
                CriarDescricaoPagina(
                    "Gerencie as reservas de veículos da locadora.");

            TextBox txtBusca =
                CriarCampoBusca();

            txtBusca.PlaceholderText =
                "Pesquisar por cliente, veículo ou placa...";

            txtBusca.Size =
                new Size(
                    330,
                    35
                );

            ComboBox cmbStatus =
                new ComboBox
                {
                    Location =
                        new Point(
                            385,
                            120
                        ),

                    Size =
                        new Size(
                            155,
                            35
                        ),

                    BackColor =
                        Color.FromArgb(
                            18,
                            8,
                            30
                        ),

                    ForeColor =
                        Color.White,

                    FlatStyle =
                        FlatStyle.Flat,

                    DropDownStyle =
                        ComboBoxStyle.DropDownList,

                    Font =
                        new Font(
                            "Segoe UI",
                            10
                        )
                };

            cmbStatus.Items.AddRange(
                new object[]
                {
                    "Todos os status",
                    "Pendente",
                    "Confirmada",
                    "Cancelada",
                    "Expirada",
                    "Concluída"
                }
            );

            cmbStatus.SelectedIndex = 0;

            Button btnNovaReserva =
                CriarBotaoNovo(
                    "+ Nova Reserva",
                    560,
                    160
                );

            DataGridView dgvReservas =
                CriarTabela();

            dgvReservas.Columns.Add(
                "Cliente",
                "Cliente"
            );

            dgvReservas.Columns.Add(
                "Veiculo",
                "Veículo"
            );

            dgvReservas.Columns.Add(
                "Placa",
                "Placa"
            );

            dgvReservas.Columns.Add(
                "Retirada",
                "Retirada"
            );

            dgvReservas.Columns.Add(
                "Devolucao",
                "Devolução"
            );

            dgvReservas.Columns.Add(
                "Valor",
                "Valor"
            );

            dgvReservas.Columns.Add(
                "Status",
                "Status"
            );

            pnlConteudo.Controls.Add(
                lblTitulo
            );

            pnlConteudo.Controls.Add(
                lblDescricao
            );

            pnlConteudo.Controls.Add(
                txtBusca
            );

            pnlConteudo.Controls.Add(
                cmbStatus
            );

            pnlConteudo.Controls.Add(
                btnNovaReserva
            );

            pnlConteudo.Controls.Add(
                dgvReservas
            );

            void AplicarFiltros()
            {
                string busca =
                    txtBusca.Text
                        .Trim()
                        .ToLowerInvariant();

                IEnumerable<ReservaResponse> resultado =
                    reservasCarregadas;

                if (
                    !string.IsNullOrWhiteSpace(
                        busca
                    )
                )
                {
                    resultado =
                        resultado.Where(
                            r =>
                                (r.ClienteNome ?? string.Empty)
                                    .ToLowerInvariant()
                                    .Contains(busca)
                                ||
                                (r.VeiculoNome ?? string.Empty)
                                    .ToLowerInvariant()
                                    .Contains(busca)
                                ||
                                (r.VeiculoPlaca ?? string.Empty)
                                    .ToLowerInvariant()
                                    .Contains(busca)
                                ||
                                r.Id
                                    .ToString()
                                    .Contains(busca)
                        );
                }

                if (
                    cmbStatus.SelectedIndex > 0
                )
                {
                    string statusSelecionado =
                        cmbStatus.SelectedItem?
                            .ToString() ??
                        string.Empty;

                    resultado =
                        resultado.Where(
                            r =>
                                string.Equals(
                                    ObterStatusReserva(
                                        r.Status
                                    ),
                                    statusSelecionado,
                                    StringComparison.OrdinalIgnoreCase
                                )
                        );
                }

                PreencherTabelaReservas(
                    dgvReservas,
                    resultado
                );
            }

            async Task CarregarReservasAsync()
            {
                try
                {
                    ConfigurarToken();

                    List<ReservaResponse>? reservas =
                        await ApiClient.GetAsync<List<ReservaResponse>>(
                            "api/Reservas");

                    reservasCarregadas =
                        reservas ??
                        new List<ReservaResponse>();

                    AplicarFiltros();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Não foi possível carregar as reservas.\n\n" +
                        ex.Message,
                        "GoCar",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }

            btnNovaReserva.Click +=
                async (s, e) =>
                {
                    using NovaReservaForm form =
                        new NovaReservaForm();

                    DialogResult resultado =
                        form.ShowDialog(this);

                    if (
                        resultado == DialogResult.OK &&
                        form.ReservaCadastrada
                    )
                    {
                        await CarregarReservasAsync();
                    }
                };

            txtBusca.TextChanged +=
                (s, e) =>
                {
                    AplicarFiltros();
                };

            cmbStatus.SelectedIndexChanged +=
                (s, e) =>
                {
                    AplicarFiltros();
                };

            await CarregarReservasAsync();
        }

        private void PreencherTabelaReservas(
            DataGridView tabela,
            IEnumerable<ReservaResponse> reservas)
        {
            tabela.Rows.Clear();
            CultureInfo cultura = new CultureInfo("pt-BR");

            foreach (ReservaResponse reserva in reservas)
            {
                string cliente = string.IsNullOrWhiteSpace(reserva.ClienteNome)
                    ? $"Cliente #{reserva.ClienteId}"
                    : reserva.ClienteNome;

                string veiculo = string.IsNullOrWhiteSpace(reserva.VeiculoNome)
                    ? $"Veículo #{reserva.VeiculoId}"
                    : reserva.VeiculoNome;

                tabela.Rows.Add(
                    cliente,
                    veiculo,
                    reserva.VeiculoPlaca ?? string.Empty,
                    reserva.DataRetirada.ToString("dd/MM/yyyy HH:mm"),
                    reserva.DataDevolucaoPrevista.ToString("dd/MM/yyyy HH:mm"),
                    reserva.ValorTotalPrevisto.ToString("C2", cultura),
                    ObterStatusReserva(reserva.Status));
            }
        }

        private static string ObterStatusReserva(int status)
        {
            return status switch
            {
                1 => "Pendente",
                2 => "Confirmada",
                3 => "Cancelada",
                4 => "Expirada",
                5 => "Concluída",
                _ => $"Status {status}"
            };
        }

        // =====================================================
        // LOCAÇÕES
        // =====================================================

        private async Task CriarConteudoLocacoes()
        {
            pnlConteudo.Controls.Clear();

            Label lblTitulo = CriarTituloPagina("Locações");
            Label lblDescricao = CriarDescricaoPagina(
                "Gerencie as retiradas e devoluções dos veículos.");
            TextBox txtBusca = CriarCampoBusca();

            txtBusca.PlaceholderText =
                "Pesquisar por ID ou número da reserva...";

            txtBusca.Size = new Size(330, 35);

            ComboBox cmbStatus = new ComboBox
            {
                Location = new Point(385, 120),
                Size = new Size(155, 35),
                BackColor = Color.FromArgb(18, 8, 30),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };

            cmbStatus.Items.AddRange(new object[]
            {
                "Todos os status",
                "Ativa",
                "Finalizada",
                "Cancelada"
            });

            cmbStatus.SelectedIndex = 0;

            Button btnNovaLocacao =
                CriarBotaoNovo("+ Nova Locação", 560, 160);

            Button btnFinalizarLocacao =
                CriarBotaoNovo("Finalizar Locação", 740, 180);

            btnFinalizarLocacao.Enabled = false;

            DataGridView dgvLocacoes = CriarTabela();

            dgvLocacoes.Columns.Add("Id", "ID");
            dgvLocacoes.Columns.Add("Reserva", "Reserva");
            dgvLocacoes.Columns.Add("Retirada", "Data da Retirada");
            dgvLocacoes.Columns.Add("KmSaida", "KM Saída");
            dgvLocacoes.Columns.Add("Combustivel", "Combustível");
            dgvLocacoes.Columns.Add("Valor", "Valor");
            dgvLocacoes.Columns.Add("Status", "Status");

            dgvLocacoes.Columns["Id"].FillWeight = 40;
            dgvLocacoes.Columns["Reserva"].FillWeight = 70;
            dgvLocacoes.Columns["Retirada"].FillWeight = 120;
            dgvLocacoes.Columns["KmSaida"].FillWeight = 80;
            dgvLocacoes.Columns["Combustivel"].FillWeight = 80;
            dgvLocacoes.Columns["Valor"].FillWeight = 90;
            dgvLocacoes.Columns["Status"].FillWeight = 80;

            pnlConteudo.Controls.Add(lblTitulo);
            pnlConteudo.Controls.Add(lblDescricao);
            pnlConteudo.Controls.Add(txtBusca);
            pnlConteudo.Controls.Add(cmbStatus);
            pnlConteudo.Controls.Add(btnNovaLocacao);
            pnlConteudo.Controls.Add(btnFinalizarLocacao);
            pnlConteudo.Controls.Add(dgvLocacoes);

            void AplicarFiltros()
            {
                string busca = txtBusca.Text.Trim().ToLowerInvariant();
                IEnumerable<LocacaoResponse> resultado = locacoesCarregadas;

                if (!string.IsNullOrWhiteSpace(busca))
                {
                    resultado = resultado.Where(l =>
                        l.Id.ToString().Contains(busca) ||
                        l.ReservaId.ToString().Contains(busca) ||
                        $"reserva #{l.ReservaId}".ToLowerInvariant().Contains(busca));
                }

                if (cmbStatus.SelectedIndex > 0)
                {
                    string statusSelecionado =
                        cmbStatus.SelectedItem?.ToString() ?? string.Empty;

                    resultado = resultado.Where(l =>
                        string.Equals(
                            ObterStatusLocacao(l.Status, l.IsAtiva),
                            statusSelecionado,
                            StringComparison.OrdinalIgnoreCase));
                }

                PreencherTabelaLocacoes(dgvLocacoes, resultado);
                btnFinalizarLocacao.Enabled = false;
            }

            async Task CarregarLocacoesAsync()
            {
                try
                {
                    ConfigurarToken();

                    List<LocacaoResponse>? locacoes =
                        await ApiClient.GetAsync<List<LocacaoResponse>>(
                            "api/Locacoes");

                    locacoesCarregadas =
                        locacoes ?? new List<LocacaoResponse>();

                    AplicarFiltros();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Não foi possível carregar as locações.\n\n" + ex.Message,
                        "GoCar",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }

            async Task AbrirFinalizacaoAsync()
            {
                if (dgvLocacoes.CurrentRow?.Tag is not LocacaoResponse locacao)
                {
                    MessageBox.Show(
                        "Selecione uma locação ativa.",
                        "GoCar",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (!locacao.IsAtiva || locacao.Status != 1)
                {
                    MessageBox.Show(
                        "Somente uma locação ativa pode ser finalizada.",
                        "GoCar",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                FinalizarLocacaoForm.LocacaoDados dados =
                    new FinalizarLocacaoForm.LocacaoDados
                    {
                        Id = locacao.Id,
                        ReservaId = locacao.ReservaId,
                        DataRetirada = locacao.DataRetirada,
                        DataDevolucaoReal = locacao.DataDevolucaoReal,
                        KmSaida = locacao.KmSaida,
                        KmEntrada = locacao.KmEntrada,
                        CombustivelSaidaPercentual =
                            locacao.CombustivelSaidaPercentual,
                        CombustivelEntradaPercentual =
                            locacao.CombustivelEntradaPercentual,
                        ValorTotal = locacao.ValorTotal,
                        Status = locacao.Status,
                        Observacoes = locacao.Observacoes,
                        IsAtiva = locacao.IsAtiva,
                        DataCriacao = locacao.DataCriacao
                    };

                using FinalizarLocacaoForm form =
                    new FinalizarLocacaoForm(dados);

                DialogResult resultado = form.ShowDialog(this);

                if (resultado == DialogResult.OK &&
                    form.LocacaoFinalizada)
                {
                    await CarregarLocacoesAsync();
                }
            }

            btnNovaLocacao.Click += async (s, e) =>
            {
                using NovaLocacaoForm form = new NovaLocacaoForm();
                DialogResult resultado = form.ShowDialog(this);

                if (resultado == DialogResult.OK && form.LocacaoCadastrada)
                    await CarregarLocacoesAsync();
            };

            btnFinalizarLocacao.Click += async (s, e) =>
            {
                await AbrirFinalizacaoAsync();
            };

            dgvLocacoes.SelectionChanged += (s, e) =>
            {
                if (dgvLocacoes.CurrentRow?.Tag is LocacaoResponse locacao)
                {
                    btnFinalizarLocacao.Enabled =
                        locacao.IsAtiva && locacao.Status == 1;
                }
                else
                {
                    btnFinalizarLocacao.Enabled = false;
                }
            };

            dgvLocacoes.CellDoubleClick += async (s, e) =>
            {
                if (e.RowIndex < 0)
                    return;

                dgvLocacoes.Rows[e.RowIndex].Selected = true;
                dgvLocacoes.CurrentCell =
                    dgvLocacoes.Rows[e.RowIndex].Cells[0];

                await AbrirFinalizacaoAsync();
            };

            txtBusca.TextChanged += (s, e) =>
            {
                AplicarFiltros();
            };

            cmbStatus.SelectedIndexChanged += (s, e) =>
            {
                AplicarFiltros();
            };

            await CarregarLocacoesAsync();
        }

        private void PreencherTabelaLocacoes(
            DataGridView tabela,
            IEnumerable<LocacaoResponse> locacoes)
        {
            tabela.Rows.Clear();
            CultureInfo cultura = new CultureInfo("pt-BR");

            foreach (LocacaoResponse locacao in
                locacoes.OrderByDescending(l => l.DataCriacao))
            {
                string combustivel =
                    locacao.CombustivelSaidaPercentual.ToString("N0", cultura) + "%";

                string km = locacao.KmSaida.ToString("N0", cultura);
                string valor = locacao.ValorTotal.ToString("C2", cultura);

                int indice = tabela.Rows.Add(
                    locacao.Id,
                    $"Reserva #{locacao.ReservaId}",
                    locacao.DataRetirada.ToString("dd/MM/yyyy HH:mm"),
                    km,
                    combustivel,
                    valor,
                    ObterStatusLocacao(locacao.Status, locacao.IsAtiva));

                tabela.Rows[indice].Tag = locacao;
            }
        }

        private static string ObterStatusLocacao(int status, bool isAtiva)
        {
            return status switch
            {
                1 => "Ativa",
                2 => "Finalizada",
                3 => "Cancelada",
                _ => isAtiva ? "Ativa" : $"Status {status}"
            };
        }

        // =====================================================
        // FINANCEIRO
        // =====================================================

        private async Task CriarConteudoFinanceiro()
        {
            pnlConteudo.Controls.Clear();

            Label lblTitulo = CriarTituloPagina("Financeiro");
            Label lblDescricao = CriarDescricaoPagina(
                "Acompanhe e registre os pagamentos da locadora.");
            TextBox txtBusca = CriarCampoBusca();

            txtBusca.PlaceholderText =
                "Pesquisar por ID, reserva, locação ou valor...";

            txtBusca.Size = new Size(260, 35);

            ComboBox cmbStatus = new ComboBox
            {
                Location = new Point(315, 120),
                Size = new Size(125, 35),
                BackColor = Color.FromArgb(18, 8, 30),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };

            cmbStatus.Items.AddRange(new object[]
            {
                "Todos status",
                "Pendente",
                "Pago",
                "Cancelado"
            });

            cmbStatus.SelectedIndex = 0;

            ComboBox cmbFormaPagamento = new ComboBox
            {
                Location = new Point(450, 120),
                Size = new Size(145, 35),
                BackColor = Color.FromArgb(18, 8, 30),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };

            cmbFormaPagamento.Items.Add("Todas as formas");

            Button btnNovoPagamento = CriarBotaoNovo("+ Novo Pagamento", 610, 180);
            DataGridView dgvPagamentos = CriarTabela();

            dgvPagamentos.Columns.Add("Id", "ID");
            dgvPagamentos.Columns.Add("Vinculo", "Vínculo");
            dgvPagamentos.Columns.Add("Tipo", "Tipo");
            dgvPagamentos.Columns.Add("Forma", "Forma");
            dgvPagamentos.Columns.Add("Status", "Status");
            dgvPagamentos.Columns.Add("Valor", "Valor");
            dgvPagamentos.Columns.Add("Data", "Data do Pagamento");

            dgvPagamentos.Columns["Id"].FillWeight = 45;
            dgvPagamentos.Columns["Vinculo"].FillWeight = 85;
            dgvPagamentos.Columns["Tipo"].FillWeight = 85;
            dgvPagamentos.Columns["Forma"].FillWeight = 105;
            dgvPagamentos.Columns["Status"].FillWeight = 80;
            dgvPagamentos.Columns["Valor"].FillWeight = 85;
            dgvPagamentos.Columns["Data"].FillWeight = 115;

            pnlConteudo.Controls.Add(lblTitulo);
            pnlConteudo.Controls.Add(lblDescricao);
            pnlConteudo.Controls.Add(txtBusca);
            pnlConteudo.Controls.Add(cmbStatus);
            pnlConteudo.Controls.Add(cmbFormaPagamento);
            pnlConteudo.Controls.Add(btnNovoPagamento);
            pnlConteudo.Controls.Add(dgvPagamentos);

            void AtualizarFormasPagamento()
            {
                string? selecionada = cmbFormaPagamento.SelectedItem?.ToString();

                List<string> formas = pagamentosCarregados
                    .Select(p => ObterFormaPagamento(p.FormaPagamento))
                    .Where(f => !string.IsNullOrWhiteSpace(f))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(f => f)
                    .ToList();

                cmbFormaPagamento.BeginUpdate();
                cmbFormaPagamento.Items.Clear();
                cmbFormaPagamento.Items.Add("Todas as formas");

                foreach (string forma in formas)
                    cmbFormaPagamento.Items.Add(forma);

                if (!string.IsNullOrWhiteSpace(selecionada) &&
                    cmbFormaPagamento.Items.Contains(selecionada))
                {
                    cmbFormaPagamento.SelectedItem = selecionada;
                }
                else
                {
                    cmbFormaPagamento.SelectedIndex = 0;
                }

                cmbFormaPagamento.EndUpdate();
            }

            void AplicarFiltros()
            {
                string busca = txtBusca.Text.Trim().ToLowerInvariant();
                IEnumerable<PagamentoResponse> resultado = pagamentosCarregados;

                if (!string.IsNullOrWhiteSpace(busca))
                {
                    resultado = resultado.Where(p =>
                        p.Id.ToString().Contains(busca) ||
                        (p.ReservaId.HasValue &&
                            $"reserva #{p.ReservaId.Value}".ToLowerInvariant().Contains(busca)) ||
                        (p.LocacaoId.HasValue &&
                            $"locação #{p.LocacaoId.Value}".ToLowerInvariant().Contains(busca)) ||
                        (p.Tipo ?? string.Empty).ToLowerInvariant().Contains(busca) ||
                        ObterFormaPagamento(p.FormaPagamento).ToLowerInvariant().Contains(busca) ||
                        ObterStatusPagamento(p.Status).ToLowerInvariant().Contains(busca) ||
                        p.Valor.ToString(CultureInfo.InvariantCulture).Contains(busca) ||
                        p.Valor.ToString(new CultureInfo("pt-BR")).Contains(busca));
                }

                if (cmbStatus.SelectedIndex > 0)
                {
                    string statusSelecionado =
                        cmbStatus.SelectedItem?.ToString() ?? string.Empty;

                    resultado = resultado.Where(p =>
                        string.Equals(
                            ObterStatusPagamento(p.Status),
                            statusSelecionado,
                            StringComparison.OrdinalIgnoreCase));
                }

                if (cmbFormaPagamento.SelectedIndex > 0)
                {
                    string formaSelecionada =
                        cmbFormaPagamento.SelectedItem?.ToString() ?? string.Empty;

                    resultado = resultado.Where(p =>
                        string.Equals(
                            ObterFormaPagamento(p.FormaPagamento),
                            formaSelecionada,
                            StringComparison.OrdinalIgnoreCase));
                }

                PreencherTabelaPagamentos(dgvPagamentos, resultado);
            }

            async Task CarregarPagamentosAsync()
            {
                try
                {
                    ConfigurarToken();

                    List<PagamentoResponse>? pagamentos =
                        await ApiClient.GetAsync<List<PagamentoResponse>>(
                            "api/Pagamentos");

                    pagamentosCarregados = pagamentos ?? new List<PagamentoResponse>();
                    AtualizarFormasPagamento();
                    AplicarFiltros();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Não foi possível carregar os pagamentos.\n\n" + ex.Message,
                        "GoCar",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }

            btnNovoPagamento.Click += async (s, e) =>
            {
                using NovoPagamentoForm form = new NovoPagamentoForm();
                DialogResult resultado = form.ShowDialog(this);

                if (resultado == DialogResult.OK && form.PagamentoCadastrado)
                {
                    await CarregarPagamentosAsync();
                }
            };

            txtBusca.TextChanged += (s, e) =>
            {
                AplicarFiltros();
            };

            cmbStatus.SelectedIndexChanged += (s, e) =>
            {
                AplicarFiltros();
            };

            cmbFormaPagamento.SelectedIndexChanged += (s, e) =>
            {
                AplicarFiltros();
            };

            await CarregarPagamentosAsync();
        }

        private void PreencherTabelaPagamentos(
            DataGridView tabela,
            IEnumerable<PagamentoResponse> pagamentos)
        {
            tabela.Rows.Clear();
            CultureInfo cultura = new CultureInfo("pt-BR");

            foreach (PagamentoResponse pagamento in
                pagamentos.OrderByDescending(p => p.DataCriacao))
            {
                string vinculo;

                if (pagamento.ReservaId.HasValue)
                    vinculo = $"Reserva #{pagamento.ReservaId.Value}";
                else if (pagamento.LocacaoId.HasValue)
                    vinculo = $"Locação #{pagamento.LocacaoId.Value}";
                else
                    vinculo = "-";

                string dataPagamento = pagamento.DataPagamento.HasValue
                    ? pagamento.DataPagamento.Value.ToString("dd/MM/yyyy HH:mm")
                    : "Pendente";

                tabela.Rows.Add(
                    pagamento.Id,
                    vinculo,
                    pagamento.Tipo,
                    ObterFormaPagamento(pagamento.FormaPagamento),
                    ObterStatusPagamento(pagamento.Status),
                    pagamento.Valor.ToString("C2", cultura),
                    dataPagamento);
            }
        }

        private static string ObterFormaPagamento(int formaPagamento)
        {
            return formaPagamento switch
            {
                1 => "Pix",
                2 => "Cartão de Crédito",
                3 => "Cartão de Débito",
                4 => "Dinheiro",
                _ => $"Forma {formaPagamento}"
            };
        }

        private static string ObterStatusPagamento(int status)
        {
            return status switch
            {
                1 => "Pendente",
                2 => "Pago",
                3 => "Cancelado",
                4 => "Estornado",
                _ => $"Status {status}"
            };
        }

        // =====================================================
        // RELATÓRIOS
        // =====================================================

        private async Task CriarConteudoRelatorios()
        {
            pnlConteudo.Controls.Clear();

            Label lblTitulo = CriarTituloPagina("Relatórios");
            Label lblDescricao = CriarDescricaoPagina(
                "Consulte o desempenho financeiro e operacional da locadora.");

            Label lblDataInicial = CriarLabelCampo("Data inicial", 40, 118);
            DateTimePicker dtpDataInicial = new DateTimePicker
            {
                Location = new Point(40, 140),
                Size = new Size(180, 30),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                Font = new Font("Segoe UI", 10)
            };

            Label lblDataFinal = CriarLabelCampo("Data final", 240, 118);
            DateTimePicker dtpDataFinal = new DateTimePicker
            {
                Location = new Point(240, 140),
                Size = new Size(180, 30),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Value = DateTime.Now.Date,
                Font = new Font("Segoe UI", 10)
            };

            Button btnFiltrar = CriarBotaoNovo("Filtrar", 440, 120);
            btnFiltrar.Location = new Point(440, 137);

            Panel cardReceita = CriarCardRelatorio("Receita recebida", "R$ 0,00", 40, 195);
            Panel cardReservas = CriarCardRelatorio("Reservas no período", "0", 300, 195);
            Panel cardLocacoes = CriarCardRelatorio("Locações finalizadas", "0", 560, 195);
            Panel cardTicket = CriarCardRelatorio("Ticket médio", "R$ 0,00", 820, 195);

            DataGridView dgvRelatorio = CriarTabela();
            dgvRelatorio.Location = new Point(40, 340);
            dgvRelatorio.Size = new Size(1050, 345);
            dgvRelatorio.Columns.Add("Id", "ID");
            dgvRelatorio.Columns.Add("Vinculo", "Vínculo");
            dgvRelatorio.Columns.Add("Tipo", "Tipo");
            dgvRelatorio.Columns.Add("Forma", "Forma");
            dgvRelatorio.Columns.Add("Valor", "Valor");
            dgvRelatorio.Columns.Add("Data", "Data do Pagamento");

            dgvRelatorio.Columns["Id"].FillWeight = 40;
            dgvRelatorio.Columns["Vinculo"].FillWeight = 80;
            dgvRelatorio.Columns["Tipo"].FillWeight = 90;
            dgvRelatorio.Columns["Forma"].FillWeight = 110;
            dgvRelatorio.Columns["Valor"].FillWeight = 85;
            dgvRelatorio.Columns["Data"].FillWeight = 120;

            pnlConteudo.Controls.Add(lblTitulo);
            pnlConteudo.Controls.Add(lblDescricao);
            pnlConteudo.Controls.Add(lblDataInicial);
            pnlConteudo.Controls.Add(dtpDataInicial);
            pnlConteudo.Controls.Add(lblDataFinal);
            pnlConteudo.Controls.Add(dtpDataFinal);
            pnlConteudo.Controls.Add(btnFiltrar);
            pnlConteudo.Controls.Add(cardReceita);
            pnlConteudo.Controls.Add(cardReservas);
            pnlConteudo.Controls.Add(cardLocacoes);
            pnlConteudo.Controls.Add(cardTicket);
            pnlConteudo.Controls.Add(dgvRelatorio);

            async Task CarregarRelatorioAsync()
            {
                if (dtpDataFinal.Value.Date < dtpDataInicial.Value.Date)
                {
                    MessageBox.Show(
                        "A data final não pode ser anterior à data inicial.",
                        "GoCar",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    ConfigurarToken();
                    btnFiltrar.Enabled = false;
                    btnFiltrar.Text = "Carregando...";

                    List<PagamentoResponse>? pagamentos =
                        await ApiClient.GetAsync<List<PagamentoResponse>>("api/Pagamentos");
                    List<ReservaResponse>? reservas =
                        await ApiClient.GetAsync<List<ReservaResponse>>("api/Reservas");
                    List<LocacaoResponse>? locacoes =
                        await ApiClient.GetAsync<List<LocacaoResponse>>("api/Locacoes");

                    pagamentosCarregados = pagamentos ?? new List<PagamentoResponse>();
                    reservasCarregadas = reservas ?? new List<ReservaResponse>();
                    locacoesCarregadas = locacoes ?? new List<LocacaoResponse>();

                    DateTime inicio = dtpDataInicial.Value.Date;
                    DateTime fim = dtpDataFinal.Value.Date.AddDays(1).AddTicks(-1);

                    List<PagamentoResponse> pagamentosPeriodo = pagamentosCarregados
                        .Where(p =>
                            p.Status == 2 &&
                            p.IsAtivo &&
                            p.DataPagamento.HasValue &&
                            p.DataPagamento.Value >= inicio &&
                            p.DataPagamento.Value <= fim)
                        .OrderByDescending(p => p.DataPagamento)
                        .ToList();

                    List<ReservaResponse> reservasPeriodo = reservasCarregadas
                        .Where(r => r.DataReserva >= inicio && r.DataReserva <= fim)
                        .ToList();

                    List<LocacaoResponse> locacoesFinalizadas = locacoesCarregadas
                        .Where(l =>
                            l.Status == 2 &&
                            l.DataDevolucaoReal.HasValue &&
                            l.DataDevolucaoReal.Value >= inicio &&
                            l.DataDevolucaoReal.Value <= fim)
                        .ToList();

                    decimal receita = pagamentosPeriodo.Sum(p => p.Valor);
                    decimal ticketMedio = locacoesFinalizadas.Count > 0
                        ? receita / locacoesFinalizadas.Count
                        : 0m;

                    AtualizarValorCardRelatorio(cardReceita,
                        receita.ToString("C2", new CultureInfo("pt-BR")));
                    AtualizarValorCardRelatorio(cardReservas,
                        reservasPeriodo.Count.ToString());
                    AtualizarValorCardRelatorio(cardLocacoes,
                        locacoesFinalizadas.Count.ToString());
                    AtualizarValorCardRelatorio(cardTicket,
                        ticketMedio.ToString("C2", new CultureInfo("pt-BR")));

                    PreencherTabelaRelatorio(dgvRelatorio, pagamentosPeriodo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Não foi possível carregar o relatório.\n\n" + ex.Message,
                        "GoCar",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                finally
                {
                    btnFiltrar.Enabled = true;
                    btnFiltrar.Text = "Filtrar";
                }
            }

            btnFiltrar.Click += async (s, e) => await CarregarRelatorioAsync();
            await CarregarRelatorioAsync();
        }

        private Panel CriarCardRelatorio(string titulo, string valor, int x, int y)
        {
            Panel card = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(235, 110),
                BackColor = Color.FromArgb(24, 11, 42)
            };

            Label lblTitulo = new Label
            {
                Text = titulo,
                ForeColor = Color.FromArgb(190, 180, 200),
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(15, 18)
            };

            Label lblValor = new Label
            {
                Name = "lblValorRelatorio",
                Text = valor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 50)
            };

            card.Controls.Add(lblTitulo);
            card.Controls.Add(lblValor);
            return card;
        }

        private void AtualizarValorCardRelatorio(Panel card, string valor)
        {
            Control[] controles = card.Controls.Find("lblValorRelatorio", false);
            if (controles.Length > 0)
                controles[0].Text = valor;
        }

        private void PreencherTabelaRelatorio(
            DataGridView tabela,
            IEnumerable<PagamentoResponse> pagamentos)
        {
            tabela.Rows.Clear();
            CultureInfo cultura = new CultureInfo("pt-BR");

            foreach (PagamentoResponse pagamento in pagamentos)
            {
                string vinculo;
                if (pagamento.ReservaId.HasValue)
                    vinculo = $"Reserva #{pagamento.ReservaId.Value}";
                else if (pagamento.LocacaoId.HasValue)
                    vinculo = $"Locação #{pagamento.LocacaoId.Value}";
                else
                    vinculo = "-";

                tabela.Rows.Add(
                    pagamento.Id,
                    vinculo,
                    pagamento.Tipo,
                    ObterFormaPagamento(pagamento.FormaPagamento),
                    pagamento.Valor.ToString("C2", cultura),
                    pagamento.DataPagamento?.ToString("dd/MM/yyyy HH:mm") ?? "-");
            }
        }

        // =====================================================
        // COMPONENTES REUTILIZÁVEIS
        // =====================================================

        private Label CriarTituloPagina(
            string texto)
        {
            return new Label
            {
                Text =
                    texto,

                ForeColor =
                    Color.White,

                Font =
                    new Font(
                        "Segoe UI",
                        22,
                        FontStyle.Bold
                    ),

                AutoSize =
                    true,

                Location =
                    new Point(
                        40,
                        30
                    )
            };
        }

        private Label CriarDescricaoPagina(
            string texto)
        {
            return new Label
            {
                Text =
                    texto,

                ForeColor =
                    Color.Gray,

                Font =
                    new Font(
                        "Segoe UI",
                        9
                    ),

                AutoSize =
                    true,

                Location =
                    new Point(
                        42,
                        75
                    )
            };
        }

        private TextBox CriarCampoBusca()
        {
            return new TextBox
            {
                Location =
                    new Point(
                        40,
                        120
                    ),

                Size =
                    new Size(
                        500,
                        35
                    ),

                BackColor =
                    Color.FromArgb(
                        18,
                        8,
                        30
                    ),

                ForeColor =
                    Color.White,

                BorderStyle =
                    BorderStyle.FixedSingle,

                Font =
                    new Font(
                        "Segoe UI",
                        11
                    )
            };
        }

        private Button CriarBotaoNovo(
            string texto,
            int x,
            int largura)
        {
            Button botao =
                new Button
                {
                    Text =
                        texto,

                    Location =
                        new Point(
                            x,
                            117
                        ),

                    Size =
                        new Size(
                            largura,
                            40
                        ),

                    BackColor =
                        Color.FromArgb(
                            111,
                            38,
                            201
                        ),

                    ForeColor =
                        Color.White,

                    FlatStyle =
                        FlatStyle.Flat,

                    Cursor =
                        Cursors.Hand,

                    Font =
                        new Font(
                            "Segoe UI",
                            10,
                            FontStyle.Bold
                        )
                };

            botao
                .FlatAppearance
                .BorderSize = 0;

            return botao;
        }

        private DataGridView CriarTabela()
        {
            DataGridView tabela =
                new DataGridView
                {
                    Location =
                        new Point(
                            40,
                            185
                        ),

                    Size =
                        new Size(
                            1050,
                            500
                        ),

                    BackgroundColor =
                        Color.FromArgb(
                            12,
                            5,
                            24
                        ),

                    BorderStyle =
                        BorderStyle.None,

                    RowHeadersVisible =
                        false,

                    AllowUserToAddRows =
                        false,

                    AllowUserToDeleteRows =
                        false,

                    AllowUserToResizeRows =
                        false,

                    ReadOnly =
                        true,

                    MultiSelect =
                        false,

                    SelectionMode =
                        DataGridViewSelectionMode
                            .FullRowSelect,

                    AutoSizeColumnsMode =
                        DataGridViewAutoSizeColumnsMode
                            .Fill
                };

            tabela.EnableHeadersVisualStyles =
                false;

            tabela
                .ColumnHeadersDefaultCellStyle
                .BackColor =
                Color.FromArgb(
                    29,
                    5,
                    39
                );

            tabela
                .ColumnHeadersDefaultCellStyle
                .ForeColor =
                Color.White;

            tabela
                .ColumnHeadersDefaultCellStyle
                .Font =
                new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Bold
                );

            tabela
                .ColumnHeadersDefaultCellStyle
                .SelectionBackColor =
                Color.FromArgb(
                    29,
                    5,
                    39
                );

            tabela.ColumnHeadersHeight =
                42;

            tabela
                .DefaultCellStyle
                .BackColor =
                Color.FromArgb(
                    18,
                    8,
                    30
                );

            tabela
                .DefaultCellStyle
                .ForeColor =
                Color.White;

            tabela
                .DefaultCellStyle
                .SelectionBackColor =
                Color.FromArgb(
                    65,
                    25,
                    90
                );

            tabela
                .DefaultCellStyle
                .SelectionForeColor =
                Color.White;

            tabela
                .DefaultCellStyle
                .Font =
                new Font(
                    "Segoe UI",
                    10
                );

            tabela.RowTemplate.Height =
                42;

            tabela.GridColor =
                Color.FromArgb(
                    45,
                    25,
                    60
                );

            return tabela;
        }

        private Label CriarLabelCampo(
            string texto,
            int x,
            int y)
        {
            return new Label
            {
                Text =
                    texto,

                ForeColor =
                    Color.White,

                Font =
                    new Font(
                        "Segoe UI",
                        9
                    ),

                AutoSize =
                    true,

                Location =
                    new Point(
                        x,
                        y
                    )
            };
        }

        private TextBox CriarCampoCategoria(
            int x,
            int y,
            int largura)
        {
            return new TextBox
            {
                Location =
                    new Point(
                        x,
                        y
                    ),

                Size =
                    new Size(
                        largura,
                        30
                    ),

                BackColor =
                    Color.FromArgb(
                        18,
                        8,
                        30
                    ),

                ForeColor =
                    Color.White,

                BorderStyle =
                    BorderStyle.FixedSingle,

                Font =
                    new Font(
                        "Segoe UI",
                        10
                    )
            };
        }

        // =====================================================
        // CONFIGURAÇÕES
        // =====================================================

        private void CriarConteudoConfiguracoes()
        {
            pnlConteudo.Controls.Clear();

            Label lblTitulo = CriarTituloPagina("Configurações");
            Label lblDescricao = CriarDescricaoPagina(
                "Gerencie sua sessão e consulte as informações do sistema.");

            pnlConteudo.Controls.Add(lblTitulo);
            pnlConteudo.Controls.Add(lblDescricao);

            Panel cardConta = new Panel
            {
                Location = new Point(40, 150),
                Size = new Size(500, 245),
                BackColor = Color.FromArgb(24, 11, 42)
            };

            Label lblContaTitulo = new Label
            {
                Text = "Minha conta",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 22)
            };

            Label lblNomeTitulo = new Label
            {
                Text = "Nome",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(25, 75)
            };

            Label lblNomeValor = new Label
            {
                Text = string.IsNullOrWhiteSpace(DesktopSession.Nome)
                    ? "Não informado"
                    : DesktopSession.Nome,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 96)
            };

            Label lblEmailTitulo = new Label
            {
                Text = "E-mail",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(25, 135)
            };

            Label lblEmailValor = new Label
            {
                Text = string.IsNullOrWhiteSpace(DesktopSession.Email)
                    ? "Não informado"
                    : DesktopSession.Email,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 156)
            };

            Label lblPerfilTitulo = new Label
            {
                Text = "Perfil",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(300, 75)
            };

            Label lblPerfilValor = new Label
            {
                Text = string.IsNullOrWhiteSpace(DesktopSession.Perfil)
                    ? "Não informado"
                    : DesktopSession.Perfil,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(300, 96)
            };

            Label lblUsuarioIdTitulo = new Label
            {
                Text = "ID do usuário",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(300, 135)
            };

            Label lblUsuarioIdValor = new Label
            {
                Text = DesktopSession.UsuarioId > 0
                    ? DesktopSession.UsuarioId.ToString()
                    : "Não informado",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(300, 156)
            };

            Label lblSessao = new Label
            {
                Text = string.IsNullOrWhiteSpace(DesktopSession.Token)
                    ? "● Sessão não autenticada"
                    : "● Sessão autenticada",
                ForeColor = string.IsNullOrWhiteSpace(DesktopSession.Token)
                    ? Color.OrangeRed
                    : Color.LimeGreen,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 205)
            };

            cardConta.Controls.Add(lblContaTitulo);
            cardConta.Controls.Add(lblNomeTitulo);
            cardConta.Controls.Add(lblNomeValor);
            cardConta.Controls.Add(lblEmailTitulo);
            cardConta.Controls.Add(lblEmailValor);
            cardConta.Controls.Add(lblPerfilTitulo);
            cardConta.Controls.Add(lblPerfilValor);
            cardConta.Controls.Add(lblUsuarioIdTitulo);
            cardConta.Controls.Add(lblUsuarioIdValor);
            cardConta.Controls.Add(lblSessao);

            Panel cardSistema = new Panel
            {
                Location = new Point(565, 150),
                Size = new Size(430, 245),
                BackColor = Color.FromArgb(24, 11, 42)
            };

            Label lblSistemaTitulo = new Label
            {
                Text = "Sistema",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 22)
            };

            Label lblAplicacaoTitulo = new Label
            {
                Text = "Aplicação",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(25, 75)
            };

            Label lblAplicacaoValor = new Label
            {
                Text = "GoCar - Administração",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 96)
            };

            Label lblApiTitulo = new Label
            {
                Text = "API",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(25, 135)
            };

            Label lblApiValor = new Label
            {
                Text = "GoCar.API",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 156)
            };

            Label lblAmbiente = new Label
            {
                Text = "Sistema administrativo conectado à API GoCar.",
                ForeColor = Color.FromArgb(190, 180, 200),
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(25, 205)
            };

            cardSistema.Controls.Add(lblSistemaTitulo);
            cardSistema.Controls.Add(lblAplicacaoTitulo);
            cardSistema.Controls.Add(lblAplicacaoValor);
            cardSistema.Controls.Add(lblApiTitulo);
            cardSistema.Controls.Add(lblApiValor);
            cardSistema.Controls.Add(lblAmbiente);

            Button btnAtualizar = CriarBotaoNovo(
                "Atualizar", 40, 130);
            btnAtualizar.Location = new Point(40, 430);

            Button btnSair = CriarBotaoNovo(
                "Sair da conta", 220, 150);
            btnSair.Location = new Point(220, 430);
            btnSair.BackColor = Color.FromArgb(125, 35, 55);

            btnAtualizar.Click += (s, e) =>
            {
                CriarConteudoConfiguracoes();
            };

            btnSair.Click += (s, e) =>
            {
                DialogResult confirmacao = MessageBox.Show(
                    "Deseja realmente sair da sua conta?",
                    "GoCar",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacao != DialogResult.Yes)
                    return;

                DesktopSession.Limpar();
                DialogResult = DialogResult.Cancel;
                Close();
            };

            pnlConteudo.Controls.Add(cardConta);
            pnlConteudo.Controls.Add(cardSistema);
            pnlConteudo.Controls.Add(btnAtualizar);
            pnlConteudo.Controls.Add(btnSair);
        }

        // =====================================================
        // CONVERSÃO DECIMAL
        // =====================================================

        private bool TentarConverterDecimal(
            string texto,
            out decimal valor)
        {
            texto =
                texto
                    .Trim()
                    .Replace(
                        "R$",
                        ""
                    )
                    .Trim();

            if (
                decimal.TryParse(
                    texto,
                    NumberStyles.Number,
                    new CultureInfo(
                        "pt-BR"
                    ),
                    out valor
                )
            )
            {
                return true;
            }

            return decimal.TryParse(
                texto.Replace(
                    ",",
                    "."
                ),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out valor
            );
        }

        // =====================================================
        // TOKEN
        // =====================================================

        private void ConfigurarToken()
        {
            if (
                !string.IsNullOrWhiteSpace(
                    DesktopSession.Token
                )
            )
            {
                ApiClient.ConfigurarToken(
                    DesktopSession.Token
                );
            }
        }

        // =====================================================
        // EM CONSTRUÇÃO
        // =====================================================

        private void ModuloEmConstrucao(
            string modulo)
        {
            MessageBox.Show(
                $"Módulo {modulo} preparado " +
                "para implementação.",

                "GoCar",

                MessageBoxButtons.OK,

                MessageBoxIcon.Information
            );
        }
    }
}