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
    public class NovoPagamentoForm : Form
    {
        private ComboBox cmbTipoPagamento = null!;
        private ComboBox cmbVinculo = null!;
        private ComboBox cmbFormaPagamento = null!;
        private ComboBox cmbStatus = null!;

        private Label lblVinculoTitulo = null!;
        private Label lblClienteTitulo = null!;
        private Label lblVeiculoTitulo = null!;
        private Label lblValorTotalTitulo = null!;
        private Label lblValorPagamentoTitulo = null!;

        private Label lblCliente = null!;
        private Label lblVeiculo = null!;
        private Label lblValorTotal = null!;
        private Label lblValorPagamento = null!;

        private TextBox txtObservacoes = null!;

        private Button btnCadastrar = null!;
        private Button btnCancelar = null!;

        private List<ReservaResponse> reservas =
            new List<ReservaResponse>();

        private List<LocacaoResponse> locacoes =
            new List<LocacaoResponse>();

        private List<PagamentoResponse> pagamentos =
            new List<PagamentoResponse>();

        public bool PagamentoCadastrado { get; private set; }

        // =====================================================
        // DTO RESERVA
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

                return
                    $"Reserva #{Id} - {cliente} - {veiculo}";
            }
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

            public override string ToString()
            {
                return
                    $"Locação #{Id} - Reserva #{ReservaId}";
            }
        }

        // =====================================================
        // DTO PAGAMENTO
        // =====================================================

        private class PagamentoResponse
        {
            public int Id { get; set; }

            public int? LocacaoId { get; set; }

            public int? ReservaId { get; set; }

            public string Tipo { get; set; } =
                string.Empty;

            public int FormaPagamento { get; set; }

            public int Status { get; set; }

            public decimal Valor { get; set; }

            public DateTime? DataPagamento { get; set; }

            public string? Observacoes { get; set; }

            public bool IsAtivo { get; set; }

            public DateTime DataCriacao { get; set; }
        }

        // =====================================================
        // COMBO
        // =====================================================

        private class ComboItem
        {
            public int Valor { get; set; }

            public string Texto { get; set; } =
                string.Empty;

            public override string ToString()
            {
                return Texto;
            }
        }

        // =====================================================
        // CONSTRUTOR
        // =====================================================

        public NovoPagamentoForm()
        {
            ConfigurarFormulario();

            CriarInterface();

            Shown += async (s, e) =>
            {
                await CarregarDadosAsync();
            };
        }

        // =====================================================
        // FORMULÁRIO
        // =====================================================

        private void ConfigurarFormulario()
        {
            Text =
                "Novo Pagamento - GoCar";

            StartPosition =
                FormStartPosition.CenterParent;

            ClientSize =
                new Size(900, 780);

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
            Label lblTitulo =
                new Label
                {
                    Text =
                        "Registrar Pagamento",

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

            Label lblDescricao =
                new Label
                {
                    Text =
                        "Registre entradas de reservas ou pagamentos de locações.",

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

            Controls.Add(lblTitulo);
            Controls.Add(lblDescricao);

            // =================================================
            // TIPO
            // =================================================

            CriarLabelCampo(
                "Tipo de pagamento",
                40,
                115);

            cmbTipoPagamento =
                CriarComboBox(
                    40,
                    140,
                    820);

            cmbTipoPagamento.Items.Add(
                new ComboItem
                {
                    Valor = 1,
                    Texto = "Entrada de Reserva"
                });

            cmbTipoPagamento.Items.Add(
                new ComboItem
                {
                    Valor = 2,
                    Texto = "Saldo da Locação"
                });

            cmbTipoPagamento.SelectedIndexChanged +=
                async (s, e) =>
                {
                    await AtualizarTipoPagamentoAsync();
                };

            // =================================================
            // VÍNCULO
            // =================================================

            lblVinculoTitulo =
                CriarLabelCampo(
                    "Reserva pendente",
                    40,
                    195);

            cmbVinculo =
                CriarComboBox(
                    40,
                    220,
                    820);

            cmbVinculo.SelectedIndexChanged +=
                (s, e) =>
                {
                    AtualizarDadosSelecionados();
                };

            // =================================================
            // DADOS
            // =================================================

            lblClienteTitulo =
                CriarLabelCampo(
                    "Cliente",
                    40,
                    285);

            lblCliente =
                CriarLabelValor(
                    40,
                    310,
                    380);

            lblVeiculoTitulo =
                CriarLabelCampo(
                    "Veículo",
                    460,
                    285);

            lblVeiculo =
                CriarLabelValor(
                    460,
                    310,
                    400);

            lblValorTotalTitulo =
                CriarLabelCampo(
                    "Valor total da reserva",
                    40,
                    370);

            lblValorTotal =
                CriarLabelDestaque(
                    40,
                    395);

            lblValorPagamentoTitulo =
                CriarLabelCampo(
                    "Entrada obrigatória (30%)",
                    460,
                    370);

            lblValorPagamento =
                CriarLabelDestaque(
                    460,
                    395);

            // =================================================
            // PAGAMENTO
            // =================================================

            CriarTituloSecao(
                "Pagamento",
                460);

            CriarLabelCampo(
                "Forma de pagamento",
                40,
                500);

            cmbFormaPagamento =
                CriarComboBox(
                    40,
                    525,
                    380);

            cmbFormaPagamento.Items.Add(
                new ComboItem
                {
                    Valor = 1,
                    Texto = "Pix"
                });

            cmbFormaPagamento.Items.Add(
                new ComboItem
                {
                    Valor = 2,
                    Texto = "Cartão de Crédito"
                });

            cmbFormaPagamento.Items.Add(
                new ComboItem
                {
                    Valor = 3,
                    Texto = "Cartão de Débito"
                });

            cmbFormaPagamento.Items.Add(
                new ComboItem
                {
                    Valor = 4,
                    Texto = "Dinheiro"
                });

            cmbFormaPagamento.SelectedIndex = 0;

            CriarLabelCampo(
                "Status",
                460,
                500);

            cmbStatus =
                CriarComboBox(
                    460,
                    525,
                    400);

            cmbStatus.Items.Add(
                new ComboItem
                {
                    Valor = 1,
                    Texto = "Pendente"
                });

            cmbStatus.Items.Add(
                new ComboItem
                {
                    Valor = 2,
                    Texto = "Pago"
                });

            cmbStatus.SelectedIndex = 1;

            // =================================================
            // OBSERVAÇÕES
            // =================================================

            CriarLabelCampo(
                "Observações",
                40,
                590);

            txtObservacoes =
                new TextBox
                {
                    Location =
                        new Point(
                            40,
                            615),

                    Size =
                        new Size(
                            820,
                            65),

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
                            565,
                            710),

                    Size =
                        new Size(
                            140,
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

            btnCadastrar =
                new Button
                {
                    Text =
                        "Registrar Pagamento",

                    Location =
                        new Point(
                            720,
                            710),

                    Size =
                        new Size(
                            140,
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

            btnCadastrar
                .FlatAppearance
                .BorderSize = 0;

            btnCancelar.Click +=
                (s, e) =>
                {
                    DialogResult =
                        DialogResult.Cancel;

                    Close();
                };

            btnCadastrar.Click +=
                async (s, e) =>
                {
                    await CadastrarPagamentoAsync();
                };

            Controls.Add(btnCancelar);
            Controls.Add(btnCadastrar);

            AcceptButton =
                btnCadastrar;

            CancelButton =
                btnCancelar;
        }

        // =====================================================
        // CARREGAR DADOS
        // =====================================================

        private async Task CarregarDadosAsync()
        {
            try
            {
                Cursor =
                    Cursors.WaitCursor;

                btnCadastrar.Enabled =
                    false;

                ConfigurarToken();

                List<ReservaResponse>? reservasResposta =
                    await ApiClient
                        .GetAsync<List<ReservaResponse>>(
                            "api/Reservas");

                List<LocacaoResponse>? locacoesResposta =
                    await ApiClient
                        .GetAsync<List<LocacaoResponse>>(
                            "api/Locacoes");

                List<PagamentoResponse>? pagamentosResposta =
                    await ApiClient
                        .GetAsync<List<PagamentoResponse>>(
                            "api/Pagamentos");

                reservas =
                    reservasResposta ??
                    new List<ReservaResponse>();

                locacoes =
                    locacoesResposta ??
                    new List<LocacaoResponse>();

                pagamentos =
                    pagamentosResposta ??
                    new List<PagamentoResponse>();

                cmbTipoPagamento.SelectedIndex = 0;

                await AtualizarTipoPagamentoAsync();
            }
            catch (Exception ex)
            {
                btnCadastrar.Enabled =
                    false;

                MessageBox.Show(
                    "Não foi possível carregar os dados financeiros.\n\n" +
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
        // ALTERAR TIPO
        // =====================================================

        private Task AtualizarTipoPagamentoAsync()
        {
            if (cmbTipoPagamento.SelectedItem
                is not ComboItem tipo)
            {
                return Task.CompletedTask;
            }

            cmbVinculo.DataSource = null;

            if (tipo.Valor == 1)
            {
                CarregarReservasPendentes();
            }
            else
            {
                CarregarLocacoesFinalizadas();
            }

            return Task.CompletedTask;
        }

        // =====================================================
        // RESERVAS PENDENTES
        // =====================================================

        private void CarregarReservasPendentes()
        {
            lblVinculoTitulo.Text =
                "Reserva pendente";

            lblClienteTitulo.Text =
                "Cliente";

            lblVeiculoTitulo.Text =
                "Veículo";

            lblValorTotalTitulo.Text =
                "Valor total da reserva";

            lblValorPagamentoTitulo.Text =
                "Entrada obrigatória (30%)";

            List<ReservaResponse> disponiveis =
                reservas
                    .Where(r =>
                        r.IsAtiva &&
                        r.Status == 1)
                    .OrderBy(r =>
                        r.DataRetirada)
                    .ToList();

            cmbVinculo.DataSource =
                disponiveis;

            if (disponiveis.Count > 0)
            {
                cmbVinculo.SelectedIndex = 0;

                btnCadastrar.Enabled = true;

                AtualizarDadosSelecionados();
            }
            else
            {
                LimparDados();

                lblCliente.Text =
                    "Nenhuma reserva pendente.";

                btnCadastrar.Enabled =
                    false;
            }
        }

        // =====================================================
        // LOCAÇÕES FINALIZADAS
        // =====================================================

        private void CarregarLocacoesFinalizadas()
        {
            lblVinculoTitulo.Text =
                "Locação finalizada";

            lblClienteTitulo.Text =
                "Reserva";

            lblVeiculoTitulo.Text =
                "Situação";

            lblValorTotalTitulo.Text =
                "Valor total da locação";

            lblValorPagamentoTitulo.Text =
                "Saldo restante";

            List<LocacaoResponse> disponiveis =
                locacoes
                    .Where(l =>
                        l.Status == 2 &&
                        !l.IsAtiva &&
                        !ExistePagamentoFinalPago(l.Id))
                    .OrderByDescending(l =>
                        l.DataDevolucaoReal ??
                        l.DataRetirada)
                    .ToList();

            cmbVinculo.DataSource =
                disponiveis;

            if (disponiveis.Count > 0)
            {
                cmbVinculo.SelectedIndex = 0;

                btnCadastrar.Enabled = true;

                AtualizarDadosSelecionados();
            }
            else
            {
                LimparDados();

                lblCliente.Text =
                    "Nenhuma locação aguardando pagamento.";

                lblVeiculo.Text = "-";

                btnCadastrar.Enabled =
                    false;
            }
        }

        // =====================================================
        // PAGAMENTO FINAL JÁ EXISTE?
        // =====================================================

        private bool ExistePagamentoFinalPago(
            int locacaoId)
        {
            return pagamentos.Any(
                p =>
                    p.LocacaoId == locacaoId &&
                    p.IsAtivo &&
                    p.Status == 2 &&
                    string.Equals(
                        p.Tipo,
                        "Locação",
                        StringComparison.OrdinalIgnoreCase));
        }

        // =====================================================
        // DADOS SELECIONADOS
        // =====================================================

        private void AtualizarDadosSelecionados()
        {
            if (cmbTipoPagamento.SelectedItem
                is not ComboItem tipo)
            {
                LimparDados();
                return;
            }

            if (tipo.Valor == 1)
            {
                AtualizarDadosReserva();
            }
            else
            {
                AtualizarDadosLocacao();
            }
        }

        // =====================================================
        // DADOS DA RESERVA
        // =====================================================

        private void AtualizarDadosReserva()
        {
            if (cmbVinculo.SelectedItem
                is not ReservaResponse reserva)
            {
                LimparDados();
                return;
            }

            CultureInfo cultura =
                new CultureInfo(
                    "pt-BR");

            lblCliente.Text =
                string.IsNullOrWhiteSpace(
                    reserva.ClienteNome)
                    ? $"Cliente #{reserva.ClienteId}"
                    : reserva.ClienteNome;

            string nomeVeiculo =
                string.IsNullOrWhiteSpace(
                    reserva.VeiculoNome)
                    ? $"Veículo #{reserva.VeiculoId}"
                    : reserva.VeiculoNome;

            if (!string.IsNullOrWhiteSpace(
                    reserva.VeiculoPlaca))
            {
                nomeVeiculo +=
                    $" - {reserva.VeiculoPlaca}";
            }

            lblVeiculo.Text =
                nomeVeiculo;

            lblValorTotal.Text =
                reserva
                    .ValorTotalPrevisto
                    .ToString(
                        "C2",
                        cultura);

            decimal entrada =
                CalcularEntrada(
                    reserva.ValorTotalPrevisto);

            lblValorPagamento.Text =
                entrada.ToString(
                    "C2",
                    cultura);
        }

        // =====================================================
        // DADOS DA LOCAÇÃO
        // =====================================================

        private void AtualizarDadosLocacao()
        {
            if (cmbVinculo.SelectedItem
                is not LocacaoResponse locacao)
            {
                LimparDados();
                return;
            }

            CultureInfo cultura =
                new CultureInfo(
                    "pt-BR");

            lblCliente.Text =
                $"Reserva #{locacao.ReservaId}";

            lblVeiculo.Text =
                "Finalizada";

            lblValorTotal.Text =
                locacao
                    .ValorTotal
                    .ToString(
                        "C2",
                        cultura);

            decimal entradaPaga =
                ObterEntradaPaga(
                    locacao.ReservaId);

            decimal saldo =
                locacao.ValorTotal -
                entradaPaga;

            if (saldo < 0)
            {
                saldo = 0;
            }

            lblValorPagamento.Text =
                saldo.ToString(
                    "C2",
                    cultura);
        }

        // =====================================================
        // ENTRADA PAGA
        // =====================================================

        private decimal ObterEntradaPaga(
            int reservaId)
        {
            return pagamentos
                .Where(p =>
                    p.ReservaId == reservaId &&
                    p.IsAtivo &&
                    p.Status == 2 &&
                    string.Equals(
                        p.Tipo,
                        "Entrada",
                        StringComparison.OrdinalIgnoreCase))
                .Sum(p => p.Valor);
        }

        // =====================================================
        // LIMPAR
        // =====================================================

        private void LimparDados()
        {
            lblCliente.Text = "-";

            lblVeiculo.Text = "-";

            lblValorTotal.Text =
                "R$ 0,00";

            lblValorPagamento.Text =
                "R$ 0,00";
        }

        // =====================================================
        // CADASTRAR
        // =====================================================

        private async Task CadastrarPagamentoAsync()
        {
            if (cmbTipoPagamento.SelectedItem
                is not ComboItem tipo)
            {
                MessageBox.Show(
                    "Selecione o tipo de pagamento.",

                    "GoCar",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Warning);

                return;
            }

            if (cmbFormaPagamento.SelectedItem
                is not ComboItem formaPagamento)
            {
                MessageBox.Show(
                    "Selecione a forma de pagamento.",

                    "GoCar",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Warning);

                return;
            }

            if (cmbStatus.SelectedItem
                is not ComboItem status)
            {
                MessageBox.Show(
                    "Selecione o status do pagamento.",

                    "GoCar",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Warning);

                return;
            }

            if (tipo.Valor == 1)
            {
                await CadastrarEntradaReservaAsync(
                    formaPagamento,
                    status);
            }
            else
            {
                await CadastrarSaldoLocacaoAsync(
                    formaPagamento,
                    status);
            }
        }

        // =====================================================
        // ENTRADA DA RESERVA
        // =====================================================

        private async Task CadastrarEntradaReservaAsync(
            ComboItem formaPagamento,
            ComboItem status)
        {
            if (cmbVinculo.SelectedItem
                is not ReservaResponse reserva)
            {
                MessageBox.Show(
                    "Selecione uma reserva pendente.",

                    "GoCar",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Warning);

                return;
            }

            decimal entrada =
                CalcularEntrada(
                    reserva.ValorTotalPrevisto);

            string valorTexto =
                entrada.ToString(
                    "C2",
                    new CultureInfo(
                        "pt-BR"));

            string mensagem =
                status.Valor == 2
                    ? "Registrar a entrada como PAGA?\n\n" +
                      $"Reserva: #{reserva.Id}\n" +
                      $"Entrada: {valorTexto}\n" +
                      $"Forma: {formaPagamento.Texto}\n\n" +
                      "A reserva será confirmada automaticamente."
                    : "Gerar a entrada como PENDENTE?\n\n" +
                      $"Reserva: #{reserva.Id}\n" +
                      $"Entrada: {valorTexto}\n" +
                      $"Forma: {formaPagamento.Texto}";

            if (MessageBox.Show(
                    mensagem,
                    "Confirmar Pagamento",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question)
                != DialogResult.Yes)
            {
                return;
            }

            var request =
                new
                {
                    locacaoId =
                        (int?)null,

                    reservaId =
                        (int?)reserva.Id,

                    tipo =
                        "Entrada",

                    formaPagamento =
                        formaPagamento.Valor,

                    status =
                        status.Valor,

                    valor =
                        entrada,

                    dataPagamento =
                        status.Valor == 2
                            ? DateTime.Now
                            : (DateTime?)null,

                    observacoes =
                        string.IsNullOrWhiteSpace(
                            txtObservacoes.Text)
                            ? $"Entrada de 30% da reserva #{reserva.Id}"
                            : txtObservacoes.Text.Trim()
                };

            await EnviarPagamentoAsync(
                request,
                status.Valor == 2
                    ? "Pagamento registrado com sucesso!\n\n" +
                      "A reserva foi confirmada automaticamente."
                    : "Pagamento pendente criado com sucesso!");
        }

        // =====================================================
        // SALDO DA LOCAÇÃO
        // =====================================================

        private async Task CadastrarSaldoLocacaoAsync(
            ComboItem formaPagamento,
            ComboItem status)
        {
            if (cmbVinculo.SelectedItem
                is not LocacaoResponse locacao)
            {
                MessageBox.Show(
                    "Selecione uma locação finalizada.",

                    "GoCar",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Warning);

                return;
            }

            decimal entradaPaga =
                ObterEntradaPaga(
                    locacao.ReservaId);

            decimal saldo =
                locacao.ValorTotal -
                entradaPaga;

            if (saldo < 0)
            {
                saldo = 0;
            }

            CultureInfo cultura =
                new CultureInfo(
                    "pt-BR");

            string mensagem =
                status.Valor == 2
                    ? "Registrar o saldo da locação como PAGO?\n\n" +
                      $"Locação: #{locacao.Id}\n" +
                      $"Valor total: {locacao.ValorTotal.ToString("C2", cultura)}\n" +
                      $"Entrada paga: {entradaPaga.ToString("C2", cultura)}\n" +
                      $"Saldo: {saldo.ToString("C2", cultura)}\n" +
                      $"Forma: {formaPagamento.Texto}"
                    : "Gerar o saldo da locação como PENDENTE?\n\n" +
                      $"Locação: #{locacao.Id}\n" +
                      $"Saldo: {saldo.ToString("C2", cultura)}\n" +
                      $"Forma: {formaPagamento.Texto}";

            if (MessageBox.Show(
                    mensagem,
                    "Confirmar Pagamento",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question)
                != DialogResult.Yes)
            {
                return;
            }

            // Para Tipo = "Locação", o PagamentoService
            // calcula novamente o saldo correto no backend.

            var request =
                new
                {
                    locacaoId =
                        (int?)locacao.Id,

                    reservaId =
                        (int?)null,

                    tipo =
                        "Locação",

                    formaPagamento =
                        formaPagamento.Valor,

                    status =
                        status.Valor,

                    valor =
                        saldo,

                    dataPagamento =
                        status.Valor == 2
                            ? DateTime.Now
                            : (DateTime?)null,

                    observacoes =
                        string.IsNullOrWhiteSpace(
                            txtObservacoes.Text)
                            ? $"Pagamento da locação #{locacao.Id}"
                            : txtObservacoes.Text.Trim()
                };

            await EnviarPagamentoAsync(
                request,
                status.Valor == 2
                    ? "Pagamento da locação registrado com sucesso!"
                    : "Pagamento pendente da locação criado com sucesso!");
        }

        // =====================================================
        // ENVIAR
        // =====================================================

        private async Task EnviarPagamentoAsync(
            object request,
            string mensagemSucesso)
        {
            try
            {
                btnCadastrar.Enabled =
                    false;

                btnCancelar.Enabled =
                    false;

                cmbTipoPagamento.Enabled =
                    false;

                cmbVinculo.Enabled =
                    false;

                btnCadastrar.Text =
                    "Registrando...";

                Cursor =
                    Cursors.WaitCursor;

                ConfigurarToken();

                HttpResponseMessage response =
                    await ApiClient.PostAsync(
                        "api/Pagamentos",
                        request);

                if (response.IsSuccessStatusCode)
                {
                    PagamentoCadastrado =
                        true;

                    MessageBox.Show(
                        mensagemSucesso,

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

                    "Não foi possível registrar o pagamento",

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
                    "Ocorreu um erro ao registrar o pagamento.\n\n" +
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
                        cmbVinculo.SelectedItem != null;

                    btnCancelar.Enabled =
                        true;

                    cmbTipoPagamento.Enabled =
                        true;

                    cmbVinculo.Enabled =
                        true;

                    btnCadastrar.Text =
                        "Registrar Pagamento";

                    Cursor =
                        Cursors.Default;
                }
            }
        }

        // =====================================================
        // CÁLCULO
        // =====================================================

        private static decimal CalcularEntrada(
            decimal valor)
        {
            return Math.Round(
                valor * 0.30m,
                2,
                MidpointRounding.AwayFromZero);
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

        private Label CriarLabelDestaque(
            int x,
            int y)
        {
            Label label =
                new Label
                {
                    Text =
                        "R$ 0,00",

                    Location =
                        new Point(
                            x,
                            y),

                    AutoSize = true,

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

            Controls.Add(label);

            return label;
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
                        new Point(
                            x,
                            y),

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

        private void CriarTituloSecao(
            string texto,
            int y)
        {
            Label label =
                new Label
                {
                    Text = texto,

                    Location =
                        new Point(
                            40,
                            y),

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

            Controls.Add(label);
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
                    "Não foi possível registrar o pagamento.";
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
                        ?? "Não foi possível registrar o pagamento.";
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
                // Resposta não era JSON.
            }

            return conteudo;
        }
    }
}