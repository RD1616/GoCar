using GoCar.Desktop.Services;
using GoCar.Desktop.Session;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoCar.Desktop.Forms
{
    public class NovaReservaForm : Form
    {
        private ComboBox cmbCliente = null!;
        private ComboBox cmbVeiculo = null!;
        private ComboBox cmbFilialRetirada = null!;
        private ComboBox cmbFilialDevolucao = null!;

        private DateTimePicker dtpRetirada = null!;
        private DateTimePicker dtpDevolucao = null!;

        private TextBox txtObservacoes = null!;

        private Label lblDiaria = null!;
        private Label lblDisponibilidade = null!;
        private Label lblValorPrevisto = null!;

        private Button btnVerificar = null!;
        private Button btnCadastrar = null!;
        private Button btnCancelar = null!;

        private List<ClienteResponse> clientes =
            new List<ClienteResponse>();

        private List<VeiculoResponse> veiculos =
            new List<VeiculoResponse>();

        private List<FilialResponse> filiais =
            new List<FilialResponse>();

        private bool disponibilidadeVerificada;
        private int quantidadeDisponivel;

        private readonly int? _reservaId;
        private bool ModoEdicao => _reservaId.HasValue;

        public bool ReservaCadastrada { get; private set; }

        // =====================================================
        // DTOs
        // =====================================================

        private class ClienteResponse
        {
            public int Id { get; set; }

            public string Nome { get; set; } =
                string.Empty;

            public string CPF { get; set; } =
                string.Empty;

            public bool IsAtivo { get; set; }

            public override string ToString()
            {
                return $"{Nome} - {FormatarCpf(CPF)}";
            }
        }

        private class VeiculoResponse
        {
            public int Id { get; set; }

            public string Placa { get; set; } =
                string.Empty;

            public string Modelo { get; set; } =
                string.Empty;

            public string Marca { get; set; } =
                string.Empty;

            public decimal ValorDiaria { get; set; }

            public bool IsAtivo { get; set; }

            public int Status { get; set; }

            public int FilialId { get; set; }

            public override string ToString()
            {
                return $"{Marca} {Modelo} - {Placa}";
            }
        }

        private class FilialResponse
        {
            public int Id { get; set; }

            public string Nome { get; set; } =
                string.Empty;

            public bool IsAtivo { get; set; }

            public override string ToString()
            {
                return Nome;
            }
        }

        private class DisponibilidadeResponse
        {
            public bool Disponivel { get; set; }

            public int QuantidadeDisponivel { get; set; }
        }

        private class ReservaEdicaoResponse
        {
            public int Id { get; set; }
            public int ClienteId { get; set; }
            public int VeiculoId { get; set; }
            public int FilialRetiradaId { get; set; }
            public int FilialDevolucaoId { get; set; }
            public DateTime DataRetirada { get; set; }
            public DateTime DataDevolucaoPrevista { get; set; }
            public int Status { get; set; }
            public decimal ValorTotalPrevisto { get; set; }
            public string? Observacoes { get; set; }
        }

        // =====================================================
        // CONSTRUTOR
        // =====================================================

        public NovaReservaForm()
            : this(null)
        {
        }

        public NovaReservaForm(int? reservaId)
        {
            _reservaId = reservaId;

            ConfigurarFormulario();
            CriarInterface();

            Shown += async (s, e) =>
            {
                await CarregarDadosAsync();

                if (ModoEdicao)
                {
                    await CarregarReservaParaEdicaoAsync();
                }
            };
        }

        // =====================================================
        // CONFIGURAÇÃO
        // =====================================================

        private void ConfigurarFormulario()
        {
            Text = ModoEdicao ? "Editar Reserva - GoCar" : "Nova Reserva - GoCar";

            StartPosition =
                FormStartPosition.CenterParent;

            ClientSize =
                new Size(900, 760);

            MinimumSize =
                new Size(900, 760);

            FormBorderStyle =
                FormBorderStyle.FixedDialog;

            MaximizeBox = false;
            MinimizeBox = false;

            BackColor =
                Color.FromArgb(18, 18, 24);

            ForeColor =
                Color.White;

            Font =
                new Font("Segoe UI", 10);
        }

        // =====================================================
        // INTERFACE
        // =====================================================

        private void CriarInterface()
        {
            Label lblTitulo =
                new Label
                {
                    Text = ModoEdicao ? "Editar Reserva" : "Nova Reserva",
                    Font = new Font(
                        "Segoe UI",
                        22,
                        FontStyle.Bold),
                    ForeColor = Color.White,
                    AutoSize = true,
                    Location = new Point(40, 25)
                };

            Label lblDescricao =
                new Label
                {
                    Text =
                        ModoEdicao
                            ? "Altere os dados permitidos da reserva."
                            : "Selecione o cliente, veículo, filiais e período da reserva.",
                    ForeColor =
                        Color.FromArgb(170, 170, 180),
                    AutoSize = true,
                    Location = new Point(42, 70)
                };

            Controls.Add(lblTitulo);
            Controls.Add(lblDescricao);

            // =================================================
            // CLIENTE
            // =================================================

            CriarLabel(
                "Cliente",
                40,
                120);

            cmbCliente =
                CriarComboBox(
                    40,
                    145,
                    820);

            // =================================================
            // VEÍCULO
            // =================================================

            CriarLabel(
                "Veículo",
                40,
                205);

            cmbVeiculo =
                CriarComboBox(
                    40,
                    230,
                    520);

            CriarLabel(
                "Valor da diária",
                590,
                205);

            lblDiaria =
                new Label
                {
                    Text = "R$ 0,00",
                    Location =
                        new Point(590, 232),
                    AutoSize = true,
                    ForeColor =
                        Color.FromArgb(
                            190,
                            135,
                            235),
                    Font =
                        new Font(
                            "Segoe UI",
                            13,
                            FontStyle.Bold)
                };

            Controls.Add(lblDiaria);

            // =================================================
            // FILIAIS
            // =================================================

            CriarLabel(
                "Filial de retirada",
                40,
                290);

            cmbFilialRetirada =
                CriarComboBox(
                    40,
                    315,
                    390);

            CriarLabel(
                "Filial de devolução",
                470,
                290);

            cmbFilialDevolucao =
                CriarComboBox(
                    470,
                    315,
                    390);

            // =================================================
            // DATAS
            // =================================================

            CriarLabel(
                "Data e hora da retirada",
                40,
                375);

            dtpRetirada =
                CriarDateTimePicker(
                    40,
                    400,
                    390);

            CriarLabel(
                "Data e hora da devolução",
                470,
                375);

            dtpDevolucao =
                CriarDateTimePicker(
                    470,
                    400,
                    390);

            dtpRetirada.Value =
                DateTime.Now
                    .AddDays(1)
                    .Date
                    .AddHours(10);

            dtpDevolucao.Value =
                DateTime.Now
                    .AddDays(2)
                    .Date
                    .AddHours(10);

            // =================================================
            // DISPONIBILIDADE
            // =================================================

            btnVerificar =
                new Button
                {
                    Text =
                        "Verificar disponibilidade",

                    Location =
                        new Point(40, 465),

                    Size =
                        new Size(220, 42),

                    FlatStyle =
                        FlatStyle.Flat,

                    BackColor =
                        Color.FromArgb(
                            65,
                            25,
                            90),

                    ForeColor =
                        Color.White,

                    Cursor =
                        Cursors.Hand,

                    Font =
                        new Font(
                            "Segoe UI",
                            10,
                            FontStyle.Bold)
                };

            btnVerificar
                .FlatAppearance
                .BorderSize = 0;

            Controls.Add(btnVerificar);

            lblDisponibilidade =
                new Label
                {
                    Text =
                        "Disponibilidade ainda não verificada.",

                    Location =
                        new Point(285, 476),

                    AutoSize =
                        true,

                    ForeColor =
                        Color.FromArgb(
                            170,
                            170,
                            180),

                    Font =
                        new Font(
                            "Segoe UI",
                            10)
                };

            Controls.Add(
                lblDisponibilidade);

            // =================================================
            // VALOR
            // =================================================

            CriarLabel(
                "Valor previsto",
                40,
                535);

            lblValorPrevisto =
                new Label
                {
                    Text = "R$ 0,00",

                    Location =
                        new Point(40, 560),

                    AutoSize =
                        true,

                    ForeColor =
                        Color.FromArgb(
                            190,
                            135,
                            235),

                    Font =
                        new Font(
                            "Segoe UI",
                            17,
                            FontStyle.Bold)
                };

            Controls.Add(
                lblValorPrevisto);

            // =================================================
            // OBSERVAÇÕES
            // =================================================

            CriarLabel(
                "Observações",
                300,
                535);

            txtObservacoes =
                new TextBox
                {
                    Location =
                        new Point(300, 560),

                    Size =
                        new Size(560, 70),

                    Multiline =
                        true,

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
                    Text = "Cancelar",

                    Location =
                        new Point(555, 665),

                    Size =
                        new Size(140, 45),

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

            btnCadastrar =
                new Button
                {
                    Text =
                        ModoEdicao
                            ? "Salvar Alterações"
                            : "Criar Reserva",

                    Location =
                        new Point(710, 665),

                    Size =
                        new Size(150, 45),

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
                            10,
                            FontStyle.Bold)
                };

            btnCadastrar
                .FlatAppearance
                .BorderSize = 0;

            Controls.Add(btnCancelar);
            Controls.Add(btnCadastrar);

            // =================================================
            // EVENTOS
            // =================================================

            btnCancelar.Click +=
                (s, e) =>
                {
                    DialogResult =
                        DialogResult.Cancel;

                    Close();
                };

            btnVerificar.Click +=
                async (s, e) =>
                {
                    await VerificarDisponibilidadeAsync();
                };

            btnCadastrar.Click +=
                async (s, e) =>
                {
                    await CadastrarReservaAsync();
                };

            cmbVeiculo.SelectedIndexChanged +=
                (s, e) =>
                {
                    AtualizarValor();
                    InvalidarDisponibilidade();
                };

            cmbFilialRetirada.SelectedIndexChanged +=
                (s, e) =>
                {
                    InvalidarDisponibilidade();
                };

            dtpRetirada.ValueChanged +=
                (s, e) =>
                {
                    AtualizarValor();
                    InvalidarDisponibilidade();
                };

            dtpDevolucao.ValueChanged +=
                (s, e) =>
                {
                    AtualizarValor();
                    InvalidarDisponibilidade();
                };

            AcceptButton = btnCadastrar;
            CancelButton = btnCancelar;
        }

        // =====================================================
        // CARREGAR DADOS
        // =====================================================

        private async Task CarregarDadosAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                ConfigurarToken();

                List<ClienteResponse>? clientesApi =
                    await ApiClient
                        .GetAsync<List<ClienteResponse>>(
                            "api/Clientes");

                List<VeiculoResponse>? veiculosApi =
                    await ApiClient
                        .GetAsync<List<VeiculoResponse>>(
                            "api/Veiculos");

                List<FilialResponse>? filiaisApi =
                    await ApiClient
                        .GetAsync<List<FilialResponse>>(
                            "api/Filiais");

                clientes =
                    clientesApi?
                        .Where(c => c.IsAtivo)
                        .OrderBy(c => c.Nome)
                        .ToList()
                    ?? new List<ClienteResponse>();

                veiculos =
                    veiculosApi?
                        .Where(v => v.IsAtivo)
                        .OrderBy(v => v.Marca)
                        .ThenBy(v => v.Modelo)
                        .ToList()
                    ?? new List<VeiculoResponse>();

                filiais =
                    filiaisApi?
                        .Where(f => f.IsAtivo)
                        .OrderBy(f => f.Nome)
                        .ToList()
                    ?? new List<FilialResponse>();

                cmbCliente.DataSource =
                    clientes.ToList();

                cmbVeiculo.DataSource =
                    veiculos.ToList();

                cmbFilialRetirada.DataSource =
                    filiais.ToList();

                cmbFilialDevolucao.DataSource =
                    filiais.ToList();

                if (cmbCliente.Items.Count > 0)
                    cmbCliente.SelectedIndex = 0;

                if (cmbVeiculo.Items.Count > 0)
                    cmbVeiculo.SelectedIndex = 0;

                if (cmbFilialRetirada.Items.Count > 0)
                    cmbFilialRetirada.SelectedIndex = 0;

                if (cmbFilialDevolucao.Items.Count > 0)
                    cmbFilialDevolucao.SelectedIndex = 0;

                AtualizarValor();
                InvalidarDisponibilidade();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Não foi possível carregar os dados " +
                    "necessários para a reserva.\n\n" +
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
        // CARREGAR RESERVA PARA EDIÇÃO
        // =====================================================

        private async Task CarregarReservaParaEdicaoAsync()
        {
            if (!ModoEdicao)
                return;

            try
            {
                Cursor = Cursors.WaitCursor;

                ConfigurarToken();

                ReservaEdicaoResponse? reserva =
                    await ApiClient
                        .GetAsync<ReservaEdicaoResponse>(
                            $"api/Reservas/{_reservaId!.Value}");

                if (reserva == null)
                {
                    MessageBox.Show(
                        "Reserva não encontrada.",
                        "GoCar",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    Close();
                    return;
                }

                if (reserva.Status == 3 ||
                    reserva.Status == 4 ||
                    reserva.Status == 5)
                {
                    MessageBox.Show(
                        "Esta reserva não pode mais ser alterada.",
                        "GoCar",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    Close();
                    return;
                }

                SelecionarComboPorId(
                    cmbCliente,
                    clientes,
                    reserva.ClienteId,
                    c => c.Id);

                SelecionarComboPorId(
                    cmbVeiculo,
                    veiculos,
                    reserva.VeiculoId,
                    v => v.Id);

                SelecionarComboPorId(
                    cmbFilialRetirada,
                    filiais,
                    reserva.FilialRetiradaId,
                    f => f.Id);

                SelecionarComboPorId(
                    cmbFilialDevolucao,
                    filiais,
                    reserva.FilialDevolucaoId,
                    f => f.Id);

                dtpRetirada.Value =
                    reserva.DataRetirada;

                dtpDevolucao.Value =
                    reserva.DataDevolucaoPrevista;

                txtObservacoes.Text =
                    reserva.Observacoes ?? string.Empty;

                AtualizarValor();
                InvalidarDisponibilidade();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Não foi possível carregar a reserva para edição.\n\n" +
                    ex.Message,
                    "GoCar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                Close();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private static void SelecionarComboPorId<T>(
            ComboBox combo,
            List<T> itens,
            int id,
            Func<T, int> obterId)
        {
            int indice =
                itens.FindIndex(
                    item => obterId(item) == id);

            if (indice >= 0)
            {
                combo.SelectedIndex =
                    indice;
            }
        }

        // =====================================================
        // DISPONIBILIDADE
        // =====================================================

        private async Task VerificarDisponibilidadeAsync()
        {
            if (!ValidarSelecoes())
                return;

            VeiculoResponse veiculo =
                (VeiculoResponse)
                    cmbVeiculo.SelectedItem!;

            FilialResponse filial =
                (FilialResponse)
                    cmbFilialRetirada.SelectedItem!;

            try
            {
                btnVerificar.Enabled =
                    false;

                btnVerificar.Text =
                    "Verificando...";

                ConfigurarToken();

                string retirada =
                    Uri.EscapeDataString(
                        dtpRetirada.Value
                            .ToString(
                                "yyyy-MM-ddTHH:mm:ss",
                                CultureInfo.InvariantCulture));

                string devolucao =
                    Uri.EscapeDataString(
                        dtpDevolucao.Value
                            .ToString(
                                "yyyy-MM-ddTHH:mm:ss",
                                CultureInfo.InvariantCulture));

                string endpoint =
                    "api/Reservas/disponibilidade" +
                    $"?veiculoId={veiculo.Id}" +
                    $"&filialRetiradaId={filial.Id}" +
                    $"&dataRetirada={retirada}" +
                    $"&dataDevolucaoPrevista={devolucao}";

                DisponibilidadeResponse? resposta =
                    await ApiClient
                        .GetAsync<DisponibilidadeResponse>(
                            endpoint);

                if (resposta == null)
                {
                    disponibilidadeVerificada =
                        false;

                    quantidadeDisponivel =
                        0;

                    lblDisponibilidade.Text =
                        "Não foi possível verificar.";

                    lblDisponibilidade.ForeColor =
                        Color.OrangeRed;

                    return;
                }

                disponibilidadeVerificada =
                    resposta.Disponivel;

                quantidadeDisponivel =
                    resposta.QuantidadeDisponivel;

                if (resposta.Disponivel)
                {
                    lblDisponibilidade.Text =
                        quantidadeDisponivel == 1
                            ? "Disponível - 1 unidade encontrada."
                            : $"Disponível - {quantidadeDisponivel} unidades encontradas.";

                    lblDisponibilidade.ForeColor =
                        Color.LightGreen;
                }
                else
                {
                    lblDisponibilidade.Text =
                        "Nenhuma unidade disponível para este período.";

                    lblDisponibilidade.ForeColor =
                        Color.OrangeRed;
                }
            }
            catch (Exception ex)
            {
                disponibilidadeVerificada =
                    false;

                quantidadeDisponivel =
                    0;

                lblDisponibilidade.Text =
                    "Erro ao verificar disponibilidade.";

                lblDisponibilidade.ForeColor =
                    Color.OrangeRed;

                MessageBox.Show(
                    "Não foi possível verificar a disponibilidade.\n\n" +
                    ex.Message,

                    "GoCar",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Warning);
            }
            finally
            {
                btnVerificar.Enabled =
                    true;

                btnVerificar.Text =
                    "Verificar disponibilidade";
            }
        }

        // =====================================================
        // CADASTRAR
        // =====================================================

        private async Task CadastrarReservaAsync()
        {
            if (!ValidarSelecoes())
                return;

            if (!disponibilidadeVerificada ||
                quantidadeDisponivel <= 0)
            {
                MessageBox.Show(
                    "Verifique a disponibilidade do veículo " +
                    "antes de criar a reserva.",

                    "GoCar",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Warning);

                return;
            }

            ClienteResponse cliente =
                (ClienteResponse)
                    cmbCliente.SelectedItem!;

            VeiculoResponse veiculo =
                (VeiculoResponse)
                    cmbVeiculo.SelectedItem!;

            FilialResponse filialRetirada =
                (FilialResponse)
                    cmbFilialRetirada.SelectedItem!;

            FilialResponse filialDevolucao =
                (FilialResponse)
                    cmbFilialDevolucao.SelectedItem!;

            var request =
                new
                {
                    clienteId =
                        cliente.Id,

                    veiculoId =
                        veiculo.Id,

                    filialRetiradaId =
                        filialRetirada.Id,

                    filialDevolucaoId =
                        filialDevolucao.Id,

                    dataRetirada =
                        dtpRetirada.Value,

                    dataDevolucaoPrevista =
                        dtpDevolucao.Value,

                    status = 1,

                    valorTotalPrevisto =
                        0m,

                    observacoes =
                        string.IsNullOrWhiteSpace(
                            txtObservacoes.Text)
                            ? null
                            : txtObservacoes.Text.Trim()
                };

            try
            {
                btnCadastrar.Enabled =
                    false;

                btnCancelar.Enabled =
                    false;

                btnVerificar.Enabled =
                    false;

                btnCadastrar.Text =
                    "Criando...";

                ConfigurarToken();

                HttpResponseMessage response;

                if (ModoEdicao)
                {
                    response =
                        await ApiClient.PutAsync(
                            $"api/Reservas/{_reservaId!.Value}",
                            request);
                }
                else
                {
                    response =
                        await ApiClient.PostAsync(
                            "api/Reservas",
                            request);
                }

                if (response.IsSuccessStatusCode)
                {
                    ReservaCadastrada =
                        true;

                    MessageBox.Show(
                        ModoEdicao
                            ? "Reserva atualizada com sucesso!"
                            : "Reserva criada com sucesso!",

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
                    ObterMensagemErro(conteudo),

                    ModoEdicao
                        ? "Não foi possível atualizar a reserva"
                        : "Não foi possível criar a reserva",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    (ModoEdicao
                        ? "Ocorreu um erro ao atualizar a reserva.\n\n"
                        : "Ocorreu um erro ao criar a reserva.\n\n") +
                    ex.Message,

                    "GoCar",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Error);
            }
            finally
            {
                if (!IsDisposed)
                {
                    btnCadastrar.Enabled =
                        true;

                    btnCancelar.Enabled =
                        true;

                    btnVerificar.Enabled =
                        true;

                    btnCadastrar.Text =
                        ModoEdicao
                            ? "Salvar Alterações"
                            : "Criar Reserva";
                }
            }
        }

        // =====================================================
        // VALIDAÇÃO
        // =====================================================

        private bool ValidarSelecoes()
        {
            if (cmbCliente.SelectedItem == null)
            {
                Avisar(
                    "Selecione um cliente.",
                    cmbCliente);

                return false;
            }

            if (cmbVeiculo.SelectedItem == null)
            {
                Avisar(
                    "Selecione um veículo.",
                    cmbVeiculo);

                return false;
            }

            if (cmbFilialRetirada.SelectedItem == null)
            {
                Avisar(
                    "Selecione a filial de retirada.",
                    cmbFilialRetirada);

                return false;
            }

            if (cmbFilialDevolucao.SelectedItem == null)
            {
                Avisar(
                    "Selecione a filial de devolução.",
                    cmbFilialDevolucao);

                return false;
            }

            if (dtpRetirada.Value <=
                DateTime.Now)
            {
                Avisar(
                    "A data de retirada deve ser futura.",
                    dtpRetirada);

                return false;
            }

            if (dtpDevolucao.Value <=
                dtpRetirada.Value)
            {
                Avisar(
                    "A data de devolução deve ser posterior " +
                    "à data de retirada.",
                    dtpDevolucao);

                return false;
            }

            return true;
        }

        // =====================================================
        // VALOR
        // =====================================================

        private void AtualizarValor()
        {
            if (cmbVeiculo.SelectedItem
                is not VeiculoResponse veiculo)
            {
                lblDiaria.Text =
                    "R$ 0,00";

                lblValorPrevisto.Text =
                    "R$ 0,00";

                return;
            }

            CultureInfo cultura =
                new CultureInfo(
                    "pt-BR");

            lblDiaria.Text =
                veiculo.ValorDiaria
                    .ToString(
                        "C2",
                        cultura);

            TimeSpan diferenca =
                dtpDevolucao.Value -
                dtpRetirada.Value;

            int quantidadeDiarias =
                Math.Max(
                    1,
                    (int)Math.Ceiling(
                        diferenca.TotalDays));

            decimal valor =
                veiculo.ValorDiaria *
                quantidadeDiarias;

            lblValorPrevisto.Text =
                valor.ToString(
                    "C2",
                    cultura);
        }

        // =====================================================
        // INVALIDAR DISPONIBILIDADE
        // =====================================================

        private void InvalidarDisponibilidade()
        {
            disponibilidadeVerificada =
                false;

            quantidadeDisponivel =
                0;

            if (lblDisponibilidade == null)
                return;

            lblDisponibilidade.Text =
                "Disponibilidade ainda não verificada.";

            lblDisponibilidade.ForeColor =
                Color.FromArgb(
                    170,
                    170,
                    180);
        }

        // =====================================================
        // COMPONENTES
        // =====================================================

        private void CriarLabel(
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

                    AutoSize =
                        true,

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
        }

        private ComboBox CriarComboBox(
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

        private DateTimePicker CriarDateTimePicker(
            int x,
            int y,
            int largura)
        {
            DateTimePicker picker =
                new DateTimePicker
                {
                    Location =
                        new Point(x, y),

                    Size =
                        new Size(
                            largura,
                            32),

                    Format =
                        DateTimePickerFormat.Custom,

                    CustomFormat =
                        "dd/MM/yyyy HH:mm",

                    ShowUpDown =
                        false,

                    Font =
                        new Font(
                            "Segoe UI",
                            10)
                };

            Controls.Add(picker);

            return picker;
        }

        private void Avisar(
            string mensagem,
            Control controle)
        {
            MessageBox.Show(
                mensagem,

                "Atenção",

                MessageBoxButtons.OK,

                MessageBoxIcon.Warning);

            controle.Focus();
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
                    "Não foi possível criar a reserva.";
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
                        ?? "Não foi possível criar a reserva.";
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
                // A resposta não era JSON.
            }

            return conteudo;
        }

        // =====================================================
        // CPF
        // =====================================================

        private static string FormatarCpf(
            string cpf)
        {
            string numeros =
                new string(
                    (cpf ?? string.Empty)
                        .Where(char.IsDigit)
                        .ToArray());

            if (numeros.Length != 11)
                return cpf ?? string.Empty;

            return
                $"{numeros.Substring(0, 3)}." +
                $"{numeros.Substring(3, 3)}." +
                $"{numeros.Substring(6, 3)}-" +
                $"{numeros.Substring(9, 2)}";
        }
    }
}