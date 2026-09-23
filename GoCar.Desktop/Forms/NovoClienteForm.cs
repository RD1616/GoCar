using GoCar.Desktop.Services;
using System;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoCar.Desktop.Forms
{
    public partial class NovoClienteForm : Form
    {
        private TextBox txtNome = null!;
        private TextBox txtCpf = null!;
        private TextBox txtEmail = null!;
        private TextBox txtTelefone = null!;
        private TextBox txtSenha = null!;

        private DateTimePicker dtpNascimento = null!;

        private TextBox txtCnh = null!;
        private ComboBox cmbCategoriaCnh = null!;
        private DateTimePicker dtpValidadeCnh = null!;

        private TextBox txtCep = null!;
        private TextBox txtEndereco = null!;
        private TextBox txtNumero = null!;
        private TextBox txtBairro = null!;
        private TextBox txtCidade = null!;
        private ComboBox cmbEstado = null!;

        private Button btnCadastrar = null!;
        private Button btnCancelar = null!;

        public bool ClienteCadastrado { get; private set; }

        // =========================================================
        // RESPOSTA DO VIACEP
        // =========================================================

        private class ViaCepResponse
        {
            [JsonPropertyName("cep")]
            public string Cep { get; set; } =
                string.Empty;

            [JsonPropertyName("logradouro")]
            public string Logradouro { get; set; } =
                string.Empty;

            [JsonPropertyName("bairro")]
            public string Bairro { get; set; } =
                string.Empty;

            [JsonPropertyName("localidade")]
            public string Cidade { get; set; } =
                string.Empty;

            [JsonPropertyName("uf")]
            public string Estado { get; set; } =
                string.Empty;

            [JsonPropertyName("erro")]
            public bool Erro { get; set; }
        }

        // =========================================================
        // CONSTRUTOR
        // =========================================================

        public NovoClienteForm()
        {
            ConfigurarFormulario();
            CriarInterface();
        }

        // =========================================================
        // CONFIGURAÇÃO
        // =========================================================

        private void ConfigurarFormulario()
        {
            Text =
                "Novo Cliente - GoCar";

            StartPosition =
                FormStartPosition.CenterParent;

            ClientSize =
                new Size(850, 800);

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

            AutoScroll = true;
        }

        // =========================================================
        // INTERFACE
        // =========================================================

        private void CriarInterface()
        {
            Label lblTitulo =
                new Label
                {
                    Text =
                        "Novo Cliente",

                    Font =
                        new Font(
                            "Segoe UI",
                            22,
                            FontStyle.Bold),

                    ForeColor =
                        Color.White,

                    AutoSize =
                        true,

                    Location =
                        new Point(
                            40,
                            25)
                };

            Controls.Add(
                lblTitulo);

            Label lblDescricao =
                new Label
                {
                    Text =
                        "Preencha os dados para cadastrar um novo cliente.",

                    Font =
                        new Font(
                            "Segoe UI",
                            10),

                    ForeColor =
                        Color.FromArgb(
                            170,
                            170,
                            180),

                    AutoSize =
                        true,

                    Location =
                        new Point(
                            42,
                            70)
                };

            Controls.Add(
                lblDescricao);

            // =====================================================
            // DADOS PESSOAIS
            // =====================================================

            CriarTituloSecao(
                "Dados pessoais",
                110);

            txtNome =
                CriarCampo(
                    "Nome completo",
                    40,
                    150,
                    360);

            txtCpf =
                CriarCampo(
                    "CPF",
                    430,
                    150,
                    350);

            txtEmail =
                CriarCampo(
                    "E-mail",
                    40,
                    225,
                    360);

            txtTelefone =
                CriarCampo(
                    "Telefone",
                    430,
                    225,
                    350);

            txtSenha =
                CriarCampo(
                    "Senha",
                    40,
                    300,
                    360);

            txtSenha.UseSystemPasswordChar =
                true;

            CriarLabelCampo(
                "Data de nascimento",
                430,
                300);

            dtpNascimento =
                CriarDatePicker(
                    430,
                    325,
                    350);

            dtpNascimento.Value =
                DateTime.Today
                    .AddYears(-18);

            dtpNascimento.MaxDate =
                DateTime.Today;

            // =====================================================
            // CNH
            // =====================================================

            CriarTituloSecao(
                "Carteira de habilitação",
                385);

            txtCnh =
                CriarCampo(
                    "Número da CNH",
                    40,
                    425,
                    230);

            CriarLabelCampo(
                "Categoria",
                295,
                425);

            cmbCategoriaCnh =
                new ComboBox
                {
                    Location =
                        new Point(
                            295,
                            450),

                    Size =
                        new Size(
                            180,
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

            cmbCategoriaCnh.Items.AddRange(
                new object[]
                {
                    "A",
                    "B",
                    "AB",
                    "C",
                    "D",
                    "E",
                    "AC",
                    "AD",
                    "AE"
                });

            cmbCategoriaCnh.SelectedItem =
                "B";

            Controls.Add(
                cmbCategoriaCnh);

            CriarLabelCampo(
                "Validade da CNH",
                500,
                425);

            dtpValidadeCnh =
                CriarDatePicker(
                    500,
                    450,
                    280);

            dtpValidadeCnh.Value =
                DateTime.Today
                    .AddYears(5);

            dtpValidadeCnh.MinDate =
                DateTime.Today;

            // =====================================================
            // ENDEREÇO
            // =====================================================

            CriarTituloSecao(
                "Endereço",
                510);

            txtCep =
                CriarCampo(
                    "CEP",
                    40,
                    550,
                    180);

            // Ao sair do campo CEP,
            // consulta automaticamente.
            txtCep.Leave +=
                async (s, e) =>
                {
                    await BuscarCepAsync();
                };

            txtEndereco =
                CriarCampo(
                    "Endereço",
                    245,
                    550,
                    360);

            txtNumero =
                CriarCampo(
                    "Número",
                    630,
                    550,
                    150);

            txtBairro =
                CriarCampo(
                    "Bairro",
                    40,
                    625,
                    230);

            txtCidade =
                CriarCampo(
                    "Cidade",
                    295,
                    625,
                    280);

            CriarLabelCampo(
                "Estado",
                600,
                625);

            cmbEstado =
                new ComboBox
                {
                    Location =
                        new Point(
                            600,
                            650),

                    Size =
                        new Size(
                            180,
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

            cmbEstado.Items.AddRange(
                new object[]
                {
                    "AC",
                    "AL",
                    "AP",
                    "AM",
                    "BA",
                    "CE",
                    "DF",
                    "ES",
                    "GO",
                    "MA",
                    "MT",
                    "MS",
                    "MG",
                    "PA",
                    "PB",
                    "PR",
                    "PE",
                    "PI",
                    "RJ",
                    "RN",
                    "RS",
                    "RO",
                    "RR",
                    "SC",
                    "SP",
                    "SE",
                    "TO"
                });

            cmbEstado.SelectedItem =
                "SP";

            Controls.Add(
                cmbEstado);

            // =====================================================
            // BOTÕES
            // =====================================================

            btnCancelar =
                new Button
                {
                    Text =
                        "Cancelar",

                    Size =
                        new Size(
                            130,
                            45),

                    Location =
                        new Point(
                            500,
                            720),

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
                        Cursors.Hand,

                    Font =
                        new Font(
                            "Segoe UI",
                            10)
                };

            btnCancelar
                .FlatAppearance
                .BorderColor =
                Color.FromArgb(
                    75,
                    75,
                    85);

            btnCancelar.Click +=
                (s, e) =>
                {
                    DialogResult =
                        DialogResult.Cancel;

                    Close();
                };

            Controls.Add(
                btnCancelar);

            btnCadastrar =
                new Button
                {
                    Text =
                        "Cadastrar Cliente",

                    Size =
                        new Size(
                            150,
                            45),

                    Location =
                        new Point(
                            640,
                            720),

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

            btnCadastrar.Click +=
                async (s, e) =>
                {
                    await CadastrarClienteAsync();
                };

            Controls.Add(
                btnCadastrar);

            AcceptButton =
                btnCadastrar;

            CancelButton =
                btnCancelar;
        }

        // =========================================================
        // BUSCAR CEP
        // =========================================================

        private async Task BuscarCepAsync()
        {
            string cep =
                LimparNumero(
                    txtCep.Text);

            // Ainda não terminou de digitar.
            if (cep.Length == 0)
            {
                return;
            }

            if (cep.Length != 8)
            {
                MessageBox.Show(
                    "O CEP deve possuir 8 números.",
                    "CEP inválido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtCep.Focus();

                return;
            }

            try
            {
                txtCep.Enabled =
                    false;

                Cursor =
                    Cursors.WaitCursor;

                using HttpClient httpClient =
                    new HttpClient();

                httpClient.Timeout =
                    TimeSpan.FromSeconds(10);

                ViaCepResponse? resultado =
                    await httpClient
                        .GetFromJsonAsync<ViaCepResponse>(
                            $"https://viacep.com.br/ws/{cep}/json/");

                if (resultado == null)
                {
                    MessageBox.Show(
                        "Não foi possível localizar o CEP informado.",
                        "CEP",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                if (resultado.Erro)
                {
                    MessageBox.Show(
                        "CEP não encontrado.",
                        "CEP",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtCep.Focus();

                    return;
                }

                // =============================================
                // FORMATA O CEP
                // =============================================

                txtCep.Text =
                    FormatarCep(
                        cep);

                // =============================================
                // PREENCHE O ENDEREÇO
                // =============================================

                txtEndereco.Text =
                    resultado.Logradouro;

                txtBairro.Text =
                    resultado.Bairro;

                txtCidade.Text =
                    resultado.Cidade;

                if (!string.IsNullOrWhiteSpace(
                        resultado.Estado))
                {
                    cmbEstado.SelectedItem =
                        resultado.Estado;
                }

                // =============================================
                // FOCO NO NÚMERO
                // =============================================

                txtNumero.Focus();
            }
            catch (TaskCanceledException)
            {
                MessageBox.Show(
                    "A consulta do CEP demorou demais.\n" +
                    "Tente novamente.",
                    "GoCar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (HttpRequestException)
            {
                MessageBox.Show(
                    "Não foi possível consultar o CEP.\n\n" +
                    "Verifique sua conexão com a internet.",
                    "GoCar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ocorreu um erro ao consultar o CEP.\n\n" +
                    ex.Message,
                    "GoCar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                txtCep.Enabled =
                    true;

                Cursor =
                    Cursors.Default;
            }
        }

        // =========================================================
        // CADASTRAR CLIENTE
        // =========================================================

        private async Task CadastrarClienteAsync()
        {
            if (!ValidarCampos())
            {
                return;
            }

            btnCadastrar.Enabled =
                false;

            btnCancelar.Enabled =
                false;

            btnCadastrar.Text =
                "Cadastrando...";

            try
            {
                var request =
                    new
                    {
                        nome =
                            txtNome.Text.Trim(),

                        email =
                            txtEmail.Text.Trim(),

                        senha =
                            txtSenha.Text,

                        cpf =
                            LimparNumero(
                                txtCpf.Text),

                        telefone =
                            LimparNumero(
                                txtTelefone.Text),

                        cnh =
                            txtCnh.Text.Trim(),

                        categoriaCNH =
                            cmbCategoriaCnh
                                .SelectedItem?
                                .ToString()
                            ?? string.Empty,

                        dataNascimento =
                            dtpNascimento
                                .Value
                                .Date,

                        dataValidadeCNH =
                            dtpValidadeCnh
                                .Value
                                .Date,

                        endereco =
                            txtEndereco.Text
                                .Trim(),

                        numero =
                            txtNumero.Text
                                .Trim(),

                        bairro =
                            txtBairro.Text
                                .Trim(),

                        cidade =
                            txtCidade.Text
                                .Trim(),

                        estado =
                            cmbEstado
                                .SelectedItem?
                                .ToString()
                            ?? string.Empty,

                        cep =
                            LimparNumero(
                                txtCep.Text)
                    };

                HttpResponseMessage response =
                    await ApiClient.PostAsync(
                        "api/Auth/cadastro",
                        request);

                if (response.IsSuccessStatusCode)
                {
                    ClienteCadastrado =
                        true;

                    MessageBox.Show(
                        "Cliente cadastrado com sucesso!",
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

                string mensagem =
                    ObterMensagemErro(
                        conteudo);

                MessageBox.Show(
                    mensagem,
                    "Não foi possível cadastrar",
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
                    "Ocorreu um erro ao cadastrar o cliente.\n\n" +
                    ex.Message,
                    "Erro",
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

                    btnCadastrar.Text =
                        "Cadastrar Cliente";
                }
            }
        }

        // =========================================================
        // VALIDAÇÕES
        // =========================================================

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(
                    txtNome.Text))
            {
                Avisar(
                    "Informe o nome do cliente.",
                    txtNome);

                return false;
            }

            string cpf =
                LimparNumero(
                    txtCpf.Text);

            if (string.IsNullOrWhiteSpace(
                    cpf))
            {
                Avisar(
                    "Informe o CPF.",
                    txtCpf);

                return false;
            }

            if (cpf.Length != 11)
            {
                Avisar(
                    "O CPF deve possuir 11 números.",
                    txtCpf);

                return false;
            }

            if (string.IsNullOrWhiteSpace(
                    txtEmail.Text))
            {
                Avisar(
                    "Informe o e-mail.",
                    txtEmail);

                return false;
            }

            if (!txtEmail.Text.Contains("@") ||
                !txtEmail.Text.Contains("."))
            {
                Avisar(
                    "Informe um e-mail válido.",
                    txtEmail);

                return false;
            }

            string telefone =
                LimparNumero(
                    txtTelefone.Text);

            if (string.IsNullOrWhiteSpace(
                    telefone))
            {
                Avisar(
                    "Informe o telefone.",
                    txtTelefone);

                return false;
            }

            if (telefone.Length < 10 ||
                telefone.Length > 13)
            {
                Avisar(
                    "Informe um telefone válido.",
                    txtTelefone);

                return false;
            }

            if (string.IsNullOrWhiteSpace(
                    txtSenha.Text))
            {
                Avisar(
                    "Informe uma senha.",
                    txtSenha);

                return false;
            }

            if (string.IsNullOrWhiteSpace(
                    txtCnh.Text))
            {
                Avisar(
                    "Informe a CNH.",
                    txtCnh);

                return false;
            }

            if (cmbCategoriaCnh.SelectedItem ==
                null)
            {
                MessageBox.Show(
                    "Selecione a categoria da CNH.",
                    "Atenção",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                cmbCategoriaCnh.Focus();

                return false;
            }

            if (dtpValidadeCnh.Value.Date <
                DateTime.Today)
            {
                MessageBox.Show(
                    "A CNH informada está vencida.",
                    "Atenção",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                dtpValidadeCnh.Focus();

                return false;
            }

            string cep =
                LimparNumero(
                    txtCep.Text);

            if (string.IsNullOrWhiteSpace(
                    cep))
            {
                Avisar(
                    "Informe o CEP.",
                    txtCep);

                return false;
            }

            if (cep.Length != 8)
            {
                Avisar(
                    "O CEP deve possuir 8 números.",
                    txtCep);

                return false;
            }

            if (string.IsNullOrWhiteSpace(
                    txtEndereco.Text))
            {
                Avisar(
                    "Informe o endereço.",
                    txtEndereco);

                return false;
            }

            if (string.IsNullOrWhiteSpace(
                    txtNumero.Text))
            {
                Avisar(
                    "Informe o número.",
                    txtNumero);

                return false;
            }

            if (string.IsNullOrWhiteSpace(
                    txtBairro.Text))
            {
                Avisar(
                    "Informe o bairro.",
                    txtBairro);

                return false;
            }

            if (string.IsNullOrWhiteSpace(
                    txtCidade.Text))
            {
                Avisar(
                    "Informe a cidade.",
                    txtCidade);

                return false;
            }

            if (cmbEstado.SelectedItem ==
                null)
            {
                MessageBox.Show(
                    "Selecione o estado.",
                    "Atenção",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                cmbEstado.Focus();

                return false;
            }

            return true;
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

        // =========================================================
        // COMPONENTES
        // =========================================================

        private TextBox CriarCampo(
            string titulo,
            int x,
            int y,
            int largura)
        {
            CriarLabelCampo(
                titulo,
                x,
                y);

            TextBox campo =
                new TextBox
                {
                    Location =
                        new Point(
                            x,
                            y + 25),

                    Size =
                        new Size(
                            largura,
                            32),

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
                campo);

            return campo;
        }

        private void CriarLabelCampo(
            string texto,
            int x,
            int y)
        {
            Label label =
                new Label
                {
                    Text =
                        texto,

                    Location =
                        new Point(
                            x,
                            y),

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

            Controls.Add(
                label);
        }

        private void CriarTituloSecao(
            string texto,
            int y)
        {
            Label label =
                new Label
                {
                    Text =
                        texto,

                    Location =
                        new Point(
                            40,
                            y),

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
                            12,
                            FontStyle.Bold)
                };

            Controls.Add(
                label);
        }

        private DateTimePicker CriarDatePicker(
            int x,
            int y,
            int largura)
        {
            DateTimePicker picker =
                new DateTimePicker
                {
                    Location =
                        new Point(
                            x,
                            y),

                    Size =
                        new Size(
                            largura,
                            32),

                    Format =
                        DateTimePickerFormat.Short,

                    CalendarMonthBackground =
                        Color.FromArgb(
                            32,
                            32,
                            42),

                    Font =
                        new Font(
                            "Segoe UI",
                            10)
                };

            Controls.Add(
                picker);

            return picker;
        }

        // =========================================================
        // AUXILIARES
        // =========================================================

        private static string LimparNumero(
            string valor)
        {
            if (string.IsNullOrWhiteSpace(
                    valor))
            {
                return string.Empty;
            }

            return string.Concat(
                valor.Where(
                    char.IsDigit));
        }

        private static string FormatarCep(
            string cep)
        {
            string numeros =
                LimparNumero(
                    cep);

            if (numeros.Length != 8)
            {
                return cep;
            }

            return
                numeros.Substring(
                    0,
                    5)
                +
                "-"
                +
                numeros.Substring(
                    5,
                    3);
        }

        private static string ObterMensagemErro(
            string conteudo)
        {
            if (string.IsNullOrWhiteSpace(
                    conteudo))
            {
                return
                    "Não foi possível cadastrar o cliente.";
            }

            try
            {
                using JsonDocument json =
                    JsonDocument.Parse(
                        conteudo);

                if (json.RootElement
                    .TryGetProperty(
                        "mensagem",
                        out JsonElement propriedade))
                {
                    return
                        propriedade.GetString()
                        ??
                        "Não foi possível cadastrar o cliente.";
                }

                if (json.RootElement
                    .TryGetProperty(
                        "errors",
                        out JsonElement erros))
                {
                    foreach (
                        JsonProperty erro
                        in erros.EnumerateObject())
                    {
                        if (erro.Value.ValueKind ==
                            JsonValueKind.Array)
                        {
                            foreach (
                                JsonElement mensagem
                                in erro.Value.EnumerateArray())
                            {
                                string? texto =
                                    mensagem.GetString();

                                if (!string.IsNullOrWhiteSpace(
                                        texto))
                                {
                                    return texto;
                                }
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