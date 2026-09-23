using GoCar.Desktop.Services;
using GoCar.Desktop.Session;
using System;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoCar.Desktop.Forms
{
    public class NovaFilialForm : Form
    {
        private TextBox txtNome = null!;
        private TextBox txtCnpj = null!;
        private TextBox txtTelefone = null!;
        private TextBox txtEmail = null!;

        private TextBox txtCep = null!;
        private TextBox txtEndereco = null!;
        private TextBox txtNumero = null!;
        private TextBox txtBairro = null!;
        private TextBox txtCidade = null!;
        private TextBox txtEstado = null!;

        private Button btnCadastrar = null!;
        private Button btnCancelar = null!;

        private readonly HttpClient _viaCepClient = new();

        public bool FilialCadastrada { get; private set; }

        public NovaFilialForm()
        {
            ConfigurarFormulario();
            CriarInterface();
        }

        // =====================================================
        // FORMULÁRIO
        // =====================================================

        private void ConfigurarFormulario()
        {
            Text = "Nova Filial - GoCar";

            StartPosition = FormStartPosition.CenterParent;

            ClientSize = new Size(900, 760);

            BackColor = Color.FromArgb(18, 18, 24);

            ForeColor = Color.White;

            Font = new Font("Segoe UI", 10);

            FormBorderStyle = FormBorderStyle.FixedDialog;

            MaximizeBox = false;
            MinimizeBox = false;
        }

        // =====================================================
        // INTERFACE
        // =====================================================

        private void CriarInterface()
        {
            Label lblTitulo = new Label
            {
                Text = "Nova Filial",

                Font = new Font(
                    "Segoe UI",
                    22,
                    FontStyle.Bold),

                ForeColor = Color.White,

                AutoSize = true,

                Location = new Point(40, 25)
            };

            Label lblDescricao = new Label
            {
                Text = "Cadastre uma nova unidade da locadora.",

                ForeColor = Color.FromArgb(
                    170,
                    170,
                    180),

                AutoSize = true,

                Location = new Point(42, 70)
            };

            Controls.Add(lblTitulo);
            Controls.Add(lblDescricao);

            // =================================================
            // DADOS DA FILIAL
            // =================================================

            CriarTituloSecao(
                "Dados da filial",
                115);

            CriarLabelCampo(
                "Nome da filial *",
                40,
                160);

            txtNome = CriarTextBox(
                40,
                185,
                390);

            CriarLabelCampo(
                "CNPJ *",
                470,
                160);

            txtCnpj = CriarTextBox(
                470,
                185,
                390);

            CriarLabelCampo(
                "Telefone *",
                40,
                245);

            txtTelefone = CriarTextBox(
                40,
                270,
                390);

            CriarLabelCampo(
                "E-mail *",
                470,
                245);

            txtEmail = CriarTextBox(
                470,
                270,
                390);

            // =================================================
            // ENDEREÇO
            // =================================================

            CriarTituloSecao(
                "Endereço",
                335);

            CriarLabelCampo(
                "CEP *",
                40,
                380);

            txtCep = CriarTextBox(
                40,
                405,
                250);

            txtCep.Leave +=
                async (s, e) =>
                {
                    await BuscarCepAsync();
                };

            CriarLabelCampo(
                "Endereço *",
                330,
                380);

            txtEndereco = CriarTextBox(
                330,
                405,
                530);

            CriarLabelCampo(
                "Número *",
                40,
                465);

            txtNumero = CriarTextBox(
                40,
                490,
                180);

            CriarLabelCampo(
                "Bairro *",
                260,
                465);

            txtBairro = CriarTextBox(
                260,
                490,
                280);

            CriarLabelCampo(
                "Cidade *",
                580,
                465);

            txtCidade = CriarTextBox(
                580,
                490,
                280);

            CriarLabelCampo(
                "Estado (UF) *",
                40,
                550);

            txtEstado = CriarTextBox(
                40,
                575,
                180);

            txtEstado.MaxLength = 2;

            // =================================================
            // BOTÕES
            // =================================================

            btnCancelar = new Button
            {
                Text = "Cancelar",

                Location = new Point(
                    565,
                    670),

                Size = new Size(
                    140,
                    45),

                FlatStyle = FlatStyle.Flat,

                BackColor = Color.FromArgb(
                    45,
                    45,
                    55),

                ForeColor = Color.White,

                Cursor = Cursors.Hand
            };

            btnCancelar.FlatAppearance.BorderColor =
                Color.FromArgb(
                    75,
                    75,
                    85);

            btnCadastrar = new Button
            {
                Text = "Cadastrar Filial",

                Location = new Point(
                    720,
                    670),

                Size = new Size(
                    140,
                    45),

                FlatStyle = FlatStyle.Flat,

                BackColor = Color.FromArgb(
                    111,
                    38,
                    201),

                ForeColor = Color.White,

                Cursor = Cursors.Hand,

                Font = new Font(
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
                    await CadastrarFilialAsync();
                };

            Controls.Add(btnCancelar);
            Controls.Add(btnCadastrar);

            AcceptButton = btnCadastrar;
            CancelButton = btnCancelar;
        }

        // =====================================================
        // BUSCAR CEP
        // =====================================================

        private async Task BuscarCepAsync()
        {
            string cep =
                SomenteNumeros(
                    txtCep.Text);

            if (string.IsNullOrWhiteSpace(cep))
                return;

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
                Cursor = Cursors.WaitCursor;

                string url =
                    $"https://viacep.com.br/ws/{cep}/json/";

                using HttpResponseMessage response =
                    await _viaCepClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return;
                }

                string json =
                    await response.Content
                        .ReadAsStringAsync();

                using JsonDocument documento =
                    JsonDocument.Parse(json);

                JsonElement root =
                    documento.RootElement;

                // ViaCEP pode retornar erro como booleano
                // ou como texto dependendo da resposta.

                if (root.TryGetProperty(
                        "erro",
                        out JsonElement erro))
                {
                    bool cepInexistente =
                        erro.ValueKind == JsonValueKind.True ||
                        (
                            erro.ValueKind ==
                            JsonValueKind.String &&
                            string.Equals(
                                erro.GetString(),
                                "true",
                                StringComparison.OrdinalIgnoreCase)
                        );

                    if (cepInexistente)
                    {
                        MessageBox.Show(
                            "CEP não encontrado.",
                            "GoCar",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        return;
                    }
                }

                txtEndereco.Text =
                    ObterTextoJson(
                        root,
                        "logradouro");

                txtBairro.Text =
                    ObterTextoJson(
                        root,
                        "bairro");

                txtCidade.Text =
                    ObterTextoJson(
                        root,
                        "localidade");

                txtEstado.Text =
                    ObterTextoJson(
                        root,
                        "uf")
                    .ToUpper();

                txtCep.Text =
                    FormatarCep(cep);

                txtNumero.Focus();
            }
            catch (HttpRequestException)
            {
                // Não bloqueia o cadastro caso
                // o ViaCEP esteja indisponível.
            }
            catch (JsonException)
            {
                // Permite preenchimento manual.
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // =====================================================
        // CADASTRAR
        // =====================================================

        private async Task CadastrarFilialAsync()
        {
            if (!ValidarCampos())
                return;

            string nome =
                txtNome.Text.Trim();

            string cnpj =
                SomenteNumeros(
                    txtCnpj.Text);

            string telefone =
                SomenteNumeros(
                    txtTelefone.Text);

            string email =
                txtEmail.Text.Trim();

            string cep =
                SomenteNumeros(
                    txtCep.Text);

            string endereco =
                txtEndereco.Text.Trim();

            string numero =
                txtNumero.Text.Trim();

            string bairro =
                txtBairro.Text.Trim();

            string cidade =
                txtCidade.Text.Trim();

            string estado =
                txtEstado.Text
                    .Trim()
                    .ToUpper();

            DialogResult confirmacao =
                MessageBox.Show(
                    "Deseja cadastrar esta filial?\n\n" +
                    $"Nome: {nome}\n" +
                    $"CNPJ: {cnpj}\n" +
                    $"Cidade: {cidade} - {estado}",

                    "Confirmar cadastro",

                    MessageBoxButtons.YesNo,

                    MessageBoxIcon.Question);

            if (confirmacao !=
                DialogResult.Yes)
            {
                return;
            }

            var request = new
            {
                nome,
                cnpj,
                telefone,
                email,
                endereco,
                numero,
                bairro,
                cidade,
                estado,
                cep
            };

            try
            {
                btnCadastrar.Enabled = false;
                btnCancelar.Enabled = false;

                btnCadastrar.Text =
                    "Cadastrando...";

                Cursor =
                    Cursors.WaitCursor;

                ConfigurarToken();

                HttpResponseMessage response =
                    await ApiClient.PostAsync(
                        "api/Filiais",
                        request);

                if (response.IsSuccessStatusCode)
                {
                    FilialCadastrada = true;

                    MessageBox.Show(
                        "Filial cadastrada com sucesso!",
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

                    "Não foi possível cadastrar a filial",

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
                    "Ocorreu um erro ao cadastrar a filial.\n\n" +
                    ex.Message,

                    "GoCar",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Error);
            }
            finally
            {
                if (!IsDisposed)
                {
                    btnCadastrar.Enabled = true;
                    btnCancelar.Enabled = true;

                    btnCadastrar.Text =
                        "Cadastrar Filial";

                    Cursor =
                        Cursors.Default;
                }
            }
        }

        // =====================================================
        // VALIDAÇÃO
        // =====================================================

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(
                    txtNome.Text))
            {
                MostrarCampoObrigatorio(
                    "Informe o nome da filial.",
                    txtNome);

                return false;
            }

            string cnpj =
                SomenteNumeros(
                    txtCnpj.Text);

            if (cnpj.Length != 14)
            {
                MostrarCampoObrigatorio(
                    "Informe um CNPJ com 14 números.",
                    txtCnpj);

                return false;
            }

            string telefone =
                SomenteNumeros(
                    txtTelefone.Text);

            if (telefone.Length < 10)
            {
                MostrarCampoObrigatorio(
                    "Informe um telefone válido.",
                    txtTelefone);

                return false;
            }

            if (string.IsNullOrWhiteSpace(
                    txtEmail.Text) ||
                !txtEmail.Text.Contains("@"))
            {
                MostrarCampoObrigatorio(
                    "Informe um e-mail válido.",
                    txtEmail);

                return false;
            }

            string cep =
                SomenteNumeros(
                    txtCep.Text);

            if (cep.Length != 8)
            {
                MostrarCampoObrigatorio(
                    "Informe um CEP com 8 números.",
                    txtCep);

                return false;
            }

            if (string.IsNullOrWhiteSpace(
                    txtEndereco.Text))
            {
                MostrarCampoObrigatorio(
                    "Informe o endereço.",
                    txtEndereco);

                return false;
            }

            if (string.IsNullOrWhiteSpace(
                    txtNumero.Text))
            {
                MostrarCampoObrigatorio(
                    "Informe o número.",
                    txtNumero);

                return false;
            }

            if (string.IsNullOrWhiteSpace(
                    txtBairro.Text))
            {
                MostrarCampoObrigatorio(
                    "Informe o bairro.",
                    txtBairro);

                return false;
            }

            if (string.IsNullOrWhiteSpace(
                    txtCidade.Text))
            {
                MostrarCampoObrigatorio(
                    "Informe a cidade.",
                    txtCidade);

                return false;
            }

            string estado =
                txtEstado.Text
                    .Trim();

            if (estado.Length != 2)
            {
                MostrarCampoObrigatorio(
                    "Informe a UF com 2 letras.",
                    txtEstado);

                return false;
            }

            return true;
        }

        private static void MostrarCampoObrigatorio(
            string mensagem,
            Control controle)
        {
            MessageBox.Show(
                mensagem,
                "GoCar",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            controle.Focus();
        }

        // =====================================================
        // COMPONENTES
        // =====================================================

        private Label CriarLabelCampo(
            string texto,
            int x,
            int y)
        {
            Label label = new Label
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

        private TextBox CriarTextBox(
            int x,
            int y,
            int largura)
        {
            TextBox textBox =
                new TextBox
                {
                    Location =
                        new Point(
                            x,
                            y),

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

            Controls.Add(textBox);

            return textBox;
        }

        private void CriarTituloSecao(
            string texto,
            int y)
        {
            Label label = new Label
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
        // UTILITÁRIOS
        // =====================================================

        private static string SomenteNumeros(
            string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            char[] numeros =
                Array.FindAll(
                    texto.ToCharArray(),
                    char.IsDigit);

            return new string(numeros);
        }

        private static string FormatarCep(
            string cep)
        {
            if (cep.Length != 8)
                return cep;

            return
                $"{cep.Substring(0, 5)}-{cep.Substring(5, 3)}";
        }

        private static string ObterTextoJson(
            JsonElement elemento,
            string propriedade)
        {
            if (elemento.TryGetProperty(
                    propriedade,
                    out JsonElement valor))
            {
                return valor.GetString()
                    ?? string.Empty;
            }

            return string.Empty;
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
        // MENSAGEM DE ERRO
        // =====================================================

        private static string ObterMensagemErro(
            string conteudo)
        {
            if (string.IsNullOrWhiteSpace(
                    conteudo))
            {
                return
                    "Não foi possível cadastrar a filial.";
            }

            try
            {
                using JsonDocument json =
                    JsonDocument.Parse(conteudo);

                if (json.RootElement
                    .TryGetProperty(
                        "mensagem",
                        out JsonElement mensagem))
                {
                    return mensagem.GetString()
                        ?? "Não foi possível cadastrar a filial.";
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
                // Se a API não retornar JSON,
                // mostramos a resposta original.
            }

            return conteudo;
        }
    }
}