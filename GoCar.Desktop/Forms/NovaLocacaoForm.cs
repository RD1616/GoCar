using GoCar.Desktop.Services;
using GoCar.Desktop.Session;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoCar.Desktop.Forms
{
    public class NovaLocacaoForm : Form
    {
        private ComboBox cmbReserva = null!;
        private NumericUpDown numKmSaida = null!;
        private NumericUpDown numCombustivel = null!;
        private TextBox txtObservacoes = null!;

        private Label lblCliente = null!;
        private Label lblVeiculo = null!;
        private Label lblPeriodo = null!;
        private Label lblValor = null!;

        private Button btnIniciarLocacao = null!;
        private Button btnCancelar = null!;

        private List<ReservaResponse> reservas =
            new List<ReservaResponse>();

        public bool LocacaoCadastrada { get; private set; }

        // =====================================================
        // RESERVA
        // =====================================================

        private class ReservaResponse
        {
            public int Id { get; set; }

            public int ClienteId { get; set; }

            public string? ClienteNome { get; set; }

            public int VeiculoId { get; set; }

            public string? VeiculoNome { get; set; }

            public string? VeiculoPlaca { get; set; }

            public int FilialRetiradaId { get; set; }

            public string? FilialRetiradaNome { get; set; }

            public int FilialDevolucaoId { get; set; }

            public string? FilialDevolucaoNome { get; set; }

            public DateTime DataReserva { get; set; }

            public DateTime DataRetirada { get; set; }

            public DateTime DataDevolucaoPrevista { get; set; }

            public int Status { get; set; }

            public decimal ValorTotalPrevisto { get; set; }

            public string? Observacoes { get; set; }

            public bool IsAtiva { get; set; }

            public DateTime? DataCancelamento { get; set; }

            public override string ToString()
            {
                string cliente =
                    string.IsNullOrWhiteSpace(ClienteNome)
                        ? $"Cliente #{ClienteId}"
                        : ClienteNome;

                string veiculo =
                    string.IsNullOrWhiteSpace(VeiculoNome)
                        ? $"Veículo #{VeiculoId}"
                        : VeiculoNome;

                return $"Reserva #{Id} - {cliente} - {veiculo}";
            }
        }

        // =====================================================
        // CONSTRUTOR
        // =====================================================

        public NovaLocacaoForm()
        {
            ConfigurarFormulario();
            CriarInterface();

            Shown += async (s, e) =>
            {
                await CarregarReservasAsync();
            };
        }

        // =====================================================
        // CONFIGURAÇÃO
        // =====================================================

        private void ConfigurarFormulario()
        {
            Text = "Nova Locação - GoCar";

            StartPosition =
                FormStartPosition.CenterParent;

            ClientSize =
                new Size(850, 720);

            BackColor =
                Color.FromArgb(18, 18, 24);

            ForeColor =
                Color.White;

            Font =
                new Font("Segoe UI", 10);

            FormBorderStyle =
                FormBorderStyle.FixedDialog;

            MaximizeBox = false;
            MinimizeBox = false;
        }

        // =====================================================
        // INTERFACE
        // =====================================================

        private void CriarInterface()
        {
            Label titulo =
                new Label
                {
                    Text = "Iniciar Locação",

                    Font =
                        new Font(
                            "Segoe UI",
                            22,
                            FontStyle.Bold),

                    ForeColor = Color.White,

                    AutoSize = true,

                    Location =
                        new Point(40, 25)
                };

            Label descricao =
                new Label
                {
                    Text =
                        "Registre a retirada do veículo de uma reserva confirmada.",

                    ForeColor =
                        Color.FromArgb(
                            170,
                            170,
                            180),

                    AutoSize = true,

                    Location =
                        new Point(42, 70)
                };

            Controls.Add(titulo);
            Controls.Add(descricao);

            // =================================================
            // RESERVA
            // =================================================

            CriarLabelCampo(
                "Reserva confirmada",
                40,
                120);

            cmbReserva =
                CriarCombo(
                    40,
                    145,
                    770);

            cmbReserva.SelectedIndexChanged +=
                (s, e) =>
                {
                    AtualizarDadosReserva();
                };

            // =================================================
            // DADOS
            // =================================================

            CriarLabelCampo(
                "Cliente",
                40,
                210);

            lblCliente =
                CriarLabelValor(
                    40,
                    235,
                    350);

            CriarLabelCampo(
                "Veículo",
                430,
                210);

            lblVeiculo =
                CriarLabelValor(
                    430,
                    235,
                    380);

            CriarLabelCampo(
                "Período da reserva",
                40,
                290);

            lblPeriodo =
                CriarLabelValor(
                    40,
                    315,
                    500);

            CriarLabelCampo(
                "Valor previsto",
                600,
                290);

            lblValor =
                new Label
                {
                    Text = "R$ 0,00",

                    Location =
                        new Point(600, 315),

                    AutoSize = true,

                    ForeColor =
                        Color.FromArgb(
                            190,
                            135,
                            235),

                    Font =
                        new Font(
                            "Segoe UI",
                            15,
                            FontStyle.Bold)
                };

            Controls.Add(lblValor);

            // =================================================
            // RETIRADA
            // =================================================

            Label secao =
                new Label
                {
                    Text = "Dados da retirada",

                    Location =
                        new Point(40, 380),

                    AutoSize = true,

                    ForeColor =
                        Color.FromArgb(
                            190,
                            135,
                            235),

                    Font =
                        new Font(
                            "Segoe UI",
                            12,
                            FontStyle.Bold)
                };

            Controls.Add(secao);

            // KM

            CriarLabelCampo(
                "KM de saída",
                40,
                425);

            numKmSaida =
                new NumericUpDown
                {
                    Location =
                        new Point(40, 450),

                    Size =
                        new Size(350, 32),

                    Minimum = 0,

                    Maximum = 9999999,

                    ThousandsSeparator = true,

                    BackColor =
                        Color.FromArgb(
                            32,
                            32,
                            42),

                    ForeColor =
                        Color.White,

                    BorderStyle =
                        BorderStyle.FixedSingle,

                    Font =
                        new Font(
                            "Segoe UI",
                            10)
                };

            Controls.Add(numKmSaida);

            // COMBUSTÍVEL

            CriarLabelCampo(
                "Combustível na saída (%)",
                430,
                425);

            numCombustivel =
                new NumericUpDown
                {
                    Location =
                        new Point(430, 450),

                    Size =
                        new Size(380, 32),

                    Minimum = 0,

                    Maximum = 100,

                    DecimalPlaces = 0,

                    Increment = 5,

                    Value = 100,

                    BackColor =
                        Color.FromArgb(
                            32,
                            32,
                            42),

                    ForeColor =
                        Color.White,

                    BorderStyle =
                        BorderStyle.FixedSingle,

                    Font =
                        new Font(
                            "Segoe UI",
                            10)
                };

            Controls.Add(numCombustivel);

            // =================================================
            // OBSERVAÇÕES
            // =================================================

            CriarLabelCampo(
                "Observações",
                40,
                515);

            txtObservacoes =
                new TextBox
                {
                    Location =
                        new Point(40, 540),

                    Size =
                        new Size(770, 75),

                    Multiline = true,

                    ScrollBars =
                        ScrollBars.Vertical,

                    BackColor =
                        Color.FromArgb(
                            32,
                            32,
                            42),

                    ForeColor =
                        Color.White,

                    BorderStyle =
                        BorderStyle.FixedSingle,

                    Font =
                        new Font(
                            "Segoe UI",
                            10)
                };

            Controls.Add(txtObservacoes);

            // =================================================
            // BOTÕES
            // =================================================

            btnCancelar =
                new Button
                {
                    Text = "Cancelar",

                    Location =
                        new Point(515, 645),

                    Size =
                        new Size(135, 45),

                    FlatStyle =
                        FlatStyle.Flat,

                    BackColor =
                        Color.FromArgb(
                            45,
                            45,
                            55),

                    ForeColor =
                        Color.White,

                    Cursor =
                        Cursors.Hand
                };

            btnCancelar
                .FlatAppearance
                .BorderColor =
                Color.FromArgb(
                    75,
                    75,
                    85);

            btnIniciarLocacao =
                new Button
                {
                    Text = "Iniciar Locação",

                    Location =
                        new Point(665, 645),

                    Size =
                        new Size(145, 45),

                    FlatStyle =
                        FlatStyle.Flat,

                    BackColor =
                        Color.FromArgb(
                            111,
                            38,
                            201),

                    ForeColor =
                        Color.White,

                    Cursor =
                        Cursors.Hand,

                    Font =
                        new Font(
                            "Segoe UI",
                            9,
                            FontStyle.Bold)
                };

            btnIniciarLocacao
                .FlatAppearance
                .BorderSize = 0;

            btnCancelar.Click +=
                (s, e) =>
                {
                    DialogResult =
                        DialogResult.Cancel;

                    Close();
                };

            btnIniciarLocacao.Click +=
                async (s, e) =>
                {
                    await IniciarLocacaoAsync();
                };

            Controls.Add(btnCancelar);
            Controls.Add(btnIniciarLocacao);

            AcceptButton =
                btnIniciarLocacao;

            CancelButton =
                btnCancelar;
        }

        // =====================================================
        // CARREGAR RESERVAS CONFIRMADAS
        // =====================================================

        private async Task CarregarReservasAsync()
        {
            try
            {
                Cursor =
                    Cursors.WaitCursor;

                ConfigurarToken();

                List<ReservaResponse>? resposta =
                    await ApiClient
                        .GetAsync<List<ReservaResponse>>(
                            "api/Reservas");

                // Status 2 = Confirmada
                reservas =
                    resposta?
                        .Where(r =>
                            r.IsAtiva &&
                            r.Status == 2)
                        .OrderBy(r =>
                            r.DataRetirada)
                        .ToList()
                    ?? new List<ReservaResponse>();

                cmbReserva.DataSource = null;

                cmbReserva.DataSource =
                    reservas;

                if (reservas.Count > 0)
                {
                    cmbReserva.SelectedIndex = 0;

                    btnIniciarLocacao.Enabled =
                        true;

                    AtualizarDadosReserva();
                }
                else
                {
                    cmbReserva.DataSource = null;

                    lblCliente.Text =
                        "Nenhuma reserva confirmada.";

                    lblVeiculo.Text = "-";
                    lblPeriodo.Text = "-";
                    lblValor.Text = "R$ 0,00";

                    btnIniciarLocacao.Enabled =
                        false;

                    MessageBox.Show(
                        "Não existem reservas confirmadas e ativas " +
                        "disponíveis para iniciar uma locação.",

                        "GoCar",

                        MessageBoxButtons.OK,

                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                btnIniciarLocacao.Enabled =
                    false;

                MessageBox.Show(
                    "Não foi possível carregar as reservas.\n\n" +
                    ex.Message,

                    "GoCar",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Error);
            }
            finally
            {
                Cursor =
                    Cursors.Default;
            }
        }

        // =====================================================
        // ATUALIZAR RESERVA
        // =====================================================

        private void AtualizarDadosReserva()
        {
            if (cmbReserva.SelectedItem
                is not ReservaResponse reserva)
            {
                lblCliente.Text = "-";
                lblVeiculo.Text = "-";
                lblPeriodo.Text = "-";
                lblValor.Text = "R$ 0,00";

                return;
            }

            lblCliente.Text =
                string.IsNullOrWhiteSpace(
                    reserva.ClienteNome)
                    ? $"Cliente #{reserva.ClienteId}"
                    : reserva.ClienteNome;

            string veiculo =
                string.IsNullOrWhiteSpace(
                    reserva.VeiculoNome)
                    ? $"Veículo #{reserva.VeiculoId}"
                    : reserva.VeiculoNome;

            if (!string.IsNullOrWhiteSpace(
                    reserva.VeiculoPlaca))
            {
                veiculo +=
                    $" - {reserva.VeiculoPlaca}";
            }

            lblVeiculo.Text =
                veiculo;

            lblPeriodo.Text =
                $"{reserva.DataRetirada:dd/MM/yyyy HH:mm} até " +
                $"{reserva.DataDevolucaoPrevista:dd/MM/yyyy HH:mm}";

            lblValor.Text =
                reserva.ValorTotalPrevisto
                    .ToString("C2");

            // Como sugestão inicial, não alteramos automaticamente
            // o KM porque o funcionário deve conferir o painel
            // físico do veículo.
            numKmSaida.Value = 0;
        }

        // =====================================================
        // INICIAR LOCAÇÃO
        // =====================================================

        private async Task IniciarLocacaoAsync()
        {
            if (cmbReserva.SelectedItem
                is not ReservaResponse reserva)
            {
                MessageBox.Show(
                    "Selecione uma reserva confirmada.",

                    "GoCar",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Warning);

                return;
            }

            int kmSaida =
                Convert.ToInt32(
                    numKmSaida.Value);

            decimal combustivel =
                numCombustivel.Value;

            DialogResult confirmar =
                MessageBox.Show(
                    "Deseja iniciar esta locação?\n\n" +
                    $"Reserva: #{reserva.Id}\n" +
                    $"Cliente: {reserva.ClienteNome}\n" +
                    $"Veículo: {reserva.VeiculoNome}\n" +
                    $"KM de saída: {kmSaida:N0}\n" +
                    $"Combustível: {combustivel:N0}%\n\n" +
                    "O veículo será registrado como alugado.",

                    "Confirmar retirada",

                    MessageBoxButtons.YesNo,

                    MessageBoxIcon.Question);

            if (confirmar !=
                DialogResult.Yes)
            {
                return;
            }

            try
            {
                btnIniciarLocacao.Enabled =
                    false;

                btnCancelar.Enabled =
                    false;

                btnIniciarLocacao.Text =
                    "Iniciando...";

                Cursor =
                    Cursors.WaitCursor;

                ConfigurarToken();

                var request =
                    new
                    {
                        reservaId =
                            reserva.Id,

                        dataRetirada =
                            DateTime.Now,

                        kmSaida =
                            kmSaida,

                        combustivelSaidaPercentual =
                            combustivel,

                        // O backend utiliza o valor
                        // correspondente à reserva.
                        valorTotal =
                            reserva.ValorTotalPrevisto,

                        // Status inicial.
                        status = 1,

                        observacoes =
                            string.IsNullOrWhiteSpace(
                                txtObservacoes.Text)
                                ? null
                                : txtObservacoes.Text.Trim()
                    };

                HttpResponseMessage response =
                    await ApiClient.PostAsync(
                        "api/Locacoes",
                        request);

                if (response.IsSuccessStatusCode)
                {
                    LocacaoCadastrada =
                        true;

                    MessageBox.Show(
                        "Locação iniciada com sucesso!\n\n" +
                        "O veículo agora está registrado como alugado.",

                        "GoCar",

                        MessageBoxButtons.OK,

                        MessageBoxIcon.Information);

                    DialogResult =
                        DialogResult.OK;

                    Close();

                    return;
                }

                string conteudo =
                    await response.Content
                        .ReadAsStringAsync();

                MessageBox.Show(
                    ObterMensagemErro(
                        conteudo),

                    "Não foi possível iniciar a locação",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Warning);
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(
                    "Não foi possível conectar à API.\n\n" +
                    ex.Message,

                    "Erro de conexão",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ocorreu um erro ao iniciar a locação.\n\n" +
                    ex.Message,

                    "GoCar",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Error);
            }
            finally
            {
                if (!IsDisposed)
                {
                    btnIniciarLocacao.Enabled =
                        true;

                    btnCancelar.Enabled =
                        true;

                    btnIniciarLocacao.Text =
                        "Iniciar Locação";

                    Cursor =
                        Cursors.Default;
                }
            }
        }

        // =====================================================
        // COMPONENTES
        // =====================================================

        private Label CriarLabelCampo(
            string texto,
            int x,
            int y)
        {
            Label label =
                new Label
                {
                    Text = texto,

                    Location =
                        new Point(x, y),

                    AutoSize = true,

                    ForeColor =
                        Color.FromArgb(
                            200,
                            200,
                            210),

                    Font =
                        new Font(
                            "Segoe UI",
                            9,
                            FontStyle.Bold)
                };

            Controls.Add(label);

            return label;
        }

        private Label CriarLabelValor(
            int x,
            int y,
            int largura)
        {
            Label label =
                new Label
                {
                    Text = "-",

                    Location =
                        new Point(x, y),

                    Size =
                        new Size(
                            largura,
                            35),

                    ForeColor =
                        Color.White,

                    Font =
                        new Font(
                            "Segoe UI",
                            11,
                            FontStyle.Bold),

                    AutoEllipsis = true
                };

            Controls.Add(label);

            return label;
        }

        private ComboBox CriarCombo(
            int x,
            int y,
            int largura)
        {
            ComboBox combo =
                new ComboBox
                {
                    Location =
                        new Point(x, y),

                    Size =
                        new Size(
                            largura,
                            32),

                    DropDownStyle =
                        ComboBoxStyle.DropDownList,

                    BackColor =
                        Color.FromArgb(
                            32,
                            32,
                            42),

                    ForeColor =
                        Color.White,

                    FlatStyle =
                        FlatStyle.Flat,

                    Font =
                        new Font(
                            "Segoe UI",
                            10)
                };

            Controls.Add(combo);

            return combo;
        }

        // =====================================================
        // TOKEN
        // =====================================================

        private void ConfigurarToken()
        {
            if (!string.IsNullOrWhiteSpace(
                    DesktopSession.Token))
            {
                ApiClient.ConfigurarToken(
                    DesktopSession.Token);
            }
        }

        // =====================================================
        // ERRO DA API
        // =====================================================

        private static string ObterMensagemErro(
            string conteudo)
        {
            if (string.IsNullOrWhiteSpace(
                    conteudo))
            {
                return
                    "Não foi possível iniciar a locação.";
            }

            try
            {
                using JsonDocument json =
                    JsonDocument.Parse(
                        conteudo);

                if (json.RootElement
                    .TryGetProperty(
                        "mensagem",
                        out JsonElement mensagem))
                {
                    return mensagem.GetString()
                        ?? "Não foi possível iniciar a locação.";
                }

                if (json.RootElement
                    .TryGetProperty(
                        "errors",
                        out JsonElement errors))
                {
                    foreach (
                        JsonProperty propriedade
                        in errors.EnumerateObject())
                    {
                        if (propriedade.Value.ValueKind !=
                            JsonValueKind.Array)
                        {
                            continue;
                        }

                        foreach (
                            JsonElement item
                            in propriedade.Value
                                .EnumerateArray())
                        {
                            string? texto =
                                item.GetString();

                            if (!string.IsNullOrWhiteSpace(
                                    texto))
                            {
                                return texto;
                            }
                        }
                    }
                }
            }
            catch
            {
                // A resposta não estava em JSON.
            }

            return conteudo;
        }
    }
}