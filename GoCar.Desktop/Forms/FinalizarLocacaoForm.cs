using GoCar.Desktop.Services;
using GoCar.Desktop.Session;
using System;
using System.Drawing;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoCar.Desktop.Forms
{
    public class FinalizarLocacaoForm : Form
    {
        private readonly LocacaoDados _locacao;

        private Label lblLocacao = null!;
        private Label lblRetirada = null!;
        private Label lblKmSaida = null!;
        private Label lblCombustivelSaida = null!;
        private Label lblValor = null!;

        private NumericUpDown numKmEntrada = null!;
        private NumericUpDown numCombustivelEntrada = null!;
        private TextBox txtObservacoes = null!;

        private Button btnCancelar = null!;
        private Button btnFinalizar = null!;

        public bool LocacaoFinalizada { get; private set; }

        // =====================================================
        // DADOS DA LOCAÇÃO
        // =====================================================

        public class LocacaoDados
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

        public FinalizarLocacaoForm(
            LocacaoDados locacao)
        {
            _locacao =
                locacao ?? throw new ArgumentNullException(
                    nameof(locacao));

            ConfigurarFormulario();

            CriarInterface();

            PreencherDados();
        }

        // =====================================================
        // CONFIGURAÇÃO
        // =====================================================

        private void ConfigurarFormulario()
        {
            Text =
                "Finalizar Locação - GoCar";

            StartPosition =
                FormStartPosition.CenterParent;

            ClientSize =
                new Size(850, 720);

            BackColor =
                Color.FromArgb(
                    18,
                    18,
                    24);

            ForeColor =
                Color.White;

            Font =
                new Font(
                    "Segoe UI",
                    10);

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
                    Text = "Finalizar Locação",

                    Font =
                        new Font(
                            "Segoe UI",
                            22,
                            FontStyle.Bold),

                    ForeColor =
                        Color.White,

                    AutoSize = true,

                    Location =
                        new Point(
                            40,
                            25)
                };

            Label descricao =
                new Label
                {
                    Text =
                        "Registre a devolução do veículo e encerre a locação.",

                    ForeColor =
                        Color.FromArgb(
                            170,
                            170,
                            180),

                    AutoSize = true,

                    Location =
                        new Point(
                            42,
                            70)
                };

            Controls.Add(titulo);
            Controls.Add(descricao);

            // =================================================
            // DADOS DA LOCAÇÃO
            // =================================================

            CriarLabelCampo(
                "Locação",
                40,
                120);

            lblLocacao =
                CriarLabelValor(
                    40,
                    145,
                    350);

            CriarLabelCampo(
                "Data da retirada",
                430,
                120);

            lblRetirada =
                CriarLabelValor(
                    430,
                    145,
                    350);

            CriarLabelCampo(
                "KM de saída",
                40,
                205);

            lblKmSaida =
                CriarLabelValor(
                    40,
                    230,
                    350);

            CriarLabelCampo(
                "Combustível na saída",
                430,
                205);

            lblCombustivelSaida =
                CriarLabelValor(
                    430,
                    230,
                    350);

            CriarLabelCampo(
                "Valor da locação",
                40,
                290);

            lblValor =
                new Label
                {
                    Text = "R$ 0,00",

                    Location =
                        new Point(
                            40,
                            315),

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
            // DEVOLUÇÃO
            // =================================================

            Label secao =
                new Label
                {
                    Text =
                        "Dados da devolução",

                    Location =
                        new Point(
                            40,
                            375),

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

            CriarLabelCampo(
                "KM de entrada",
                40,
                420);

            numKmEntrada =
                new NumericUpDown
                {
                    Location =
                        new Point(
                            40,
                            445),

                    Size =
                        new Size(
                            350,
                            32),

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

            Controls.Add(numKmEntrada);

            CriarLabelCampo(
                "Combustível na entrada (%)",
                430,
                420);

            numCombustivelEntrada =
                new NumericUpDown
                {
                    Location =
                        new Point(
                            430,
                            445),

                    Size =
                        new Size(
                            380,
                            32),

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

            Controls.Add(
                numCombustivelEntrada);

            CriarLabelCampo(
                "Observações",
                40,
                510);

            txtObservacoes =
                new TextBox
                {
                    Location =
                        new Point(
                            40,
                            535),

                    Size =
                        new Size(
                            770,
                            75),

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

            Controls.Add(
                txtObservacoes);

            // =================================================
            // BOTÕES
            // =================================================

            btnCancelar =
                new Button
                {
                    Text =
                        "Cancelar",

                    Location =
                        new Point(
                            515,
                            645),

                    Size =
                        new Size(
                            135,
                            45),

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

            btnFinalizar =
                new Button
                {
                    Text =
                        "Finalizar Locação",

                    Location =
                        new Point(
                            665,
                            645),

                    Size =
                        new Size(
                            145,
                            45),

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

            btnFinalizar
                .FlatAppearance
                .BorderSize = 0;

            btnCancelar.Click +=
                (s, e) =>
                {
                    DialogResult =
                        DialogResult.Cancel;

                    Close();
                };

            btnFinalizar.Click +=
                async (s, e) =>
                {
                    await FinalizarAsync();
                };

            Controls.Add(btnCancelar);
            Controls.Add(btnFinalizar);

            AcceptButton =
                btnFinalizar;

            CancelButton =
                btnCancelar;
        }

        // =====================================================
        // PREENCHER DADOS
        // =====================================================

        private void PreencherDados()
        {
            lblLocacao.Text =
                $"Locação #{_locacao.Id} - Reserva #{_locacao.ReservaId}";

            lblRetirada.Text =
                _locacao.DataRetirada
                    .ToString(
                        "dd/MM/yyyy HH:mm");

            lblKmSaida.Text =
                _locacao.KmSaida
                    .ToString("N0");

            lblCombustivelSaida.Text =
                _locacao
                    .CombustivelSaidaPercentual
                    .ToString("N0")
                + "%";

            lblValor.Text =
                _locacao
                    .ValorTotal
                    .ToString("C2");

            // O KM de entrada não pode ser menor
            // que o KM registrado na retirada.

            numKmEntrada.Minimum =
                _locacao.KmSaida;

            numKmEntrada.Value =
                _locacao.KmSaida;

            decimal combustivel =
                _locacao
                    .CombustivelSaidaPercentual;

            if (combustivel < 0)
                combustivel = 0;

            if (combustivel > 100)
                combustivel = 100;

            numCombustivelEntrada.Value =
                combustivel;

            txtObservacoes.Text =
                _locacao.Observacoes ??
                string.Empty;
        }

        // =====================================================
        // FINALIZAR
        // =====================================================

        private async Task FinalizarAsync()
        {
            int kmEntrada =
                Convert.ToInt32(
                    numKmEntrada.Value);

            decimal combustivelEntrada =
                numCombustivelEntrada.Value;

            if (kmEntrada <
                _locacao.KmSaida)
            {
                MessageBox.Show(
                    "O KM de entrada não pode ser menor " +
                    "que o KM de saída.",

                    "GoCar",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Warning);

                return;
            }

            DialogResult confirmar =
                MessageBox.Show(
                    "Deseja finalizar esta locação?\n\n" +
                    $"Locação: #{_locacao.Id}\n" +
                    $"Reserva: #{_locacao.ReservaId}\n" +
                    $"KM saída: {_locacao.KmSaida:N0}\n" +
                    $"KM entrada: {kmEntrada:N0}\n" +
                    $"Combustível entrada: {combustivelEntrada:N0}%\n\n" +
                    "A devolução do veículo será registrada.",

                    "Confirmar devolução",

                    MessageBoxButtons.YesNo,

                    MessageBoxIcon.Question);

            if (confirmar !=
                DialogResult.Yes)
            {
                return;
            }

            try
            {
                btnFinalizar.Enabled =
                    false;

                btnCancelar.Enabled =
                    false;

                btnFinalizar.Text =
                    "Finalizando...";

                Cursor =
                    Cursors.WaitCursor;

                ConfigurarToken();

                // StatusLocacao.Finalizada = 2

                var request =
                    new
                    {
                        dataRetirada =
                            _locacao.DataRetirada,

                        dataDevolucaoReal =
                            DateTime.Now,

                        kmSaida =
                            _locacao.KmSaida,

                        kmEntrada =
                            kmEntrada,

                        combustivelSaidaPercentual =
                            _locacao
                                .CombustivelSaidaPercentual,

                        combustivelEntradaPercentual =
                            combustivelEntrada,

                        valorTotal =
                            _locacao.ValorTotal,

                        status = 2,

                        observacoes =
                            string.IsNullOrWhiteSpace(
                                txtObservacoes.Text)
                                ? null
                                : txtObservacoes.Text.Trim(),

                        isAtiva = false
                    };

                HttpResponseMessage response =
                    await ApiClient.PutAsync(
                        $"api/Locacoes/{_locacao.Id}",
                        request);

                if (response.IsSuccessStatusCode)
                {
                    LocacaoFinalizada =
                        true;

                    MessageBox.Show(
                        "Locação finalizada com sucesso!\n\n" +
                        "A devolução do veículo foi registrada.",

                        "GoCar",

                        MessageBoxButtons.OK,

                        MessageBoxIcon.Information);

                    DialogResult =
                        DialogResult.OK;

                    Close();

                    return;
                }

                string conteudo =
                    await response
                        .Content
                        .ReadAsStringAsync();

                MessageBox.Show(
                    ObterMensagemErro(
                        conteudo),

                    "Não foi possível finalizar a locação",

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
                    "Ocorreu um erro ao finalizar a locação.\n\n" +
                    ex.Message,

                    "GoCar",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Error);
            }
            finally
            {
                if (!IsDisposed)
                {
                    btnFinalizar.Enabled =
                        true;

                    btnCancelar.Enabled =
                        true;

                    btnFinalizar.Text =
                        "Finalizar Locação";

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
                        new Point(
                            x,
                            y),

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
                        new Point(
                            x,
                            y),

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

                    AutoEllipsis =
                        true
                };

            Controls.Add(label);

            return label;
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
        // ERRO
        // =====================================================

        private static string ObterMensagemErro(
            string conteudo)
        {
            if (string.IsNullOrWhiteSpace(
                    conteudo))
            {
                return
                    "Não foi possível finalizar a locação.";
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
                        ?? "Não foi possível finalizar a locação.";
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
                // Resposta não estava em JSON.
            }

            return conteudo;
        }
    }
}