using GoCar.Desktop.Services;
using System.Globalization;

namespace GoCar.Desktop
{
    public class NovoVeiculoForm : Form
    {
        private readonly Color CorFundo = Color.FromArgb(8, 0, 20);
        private readonly Color CorCard = Color.FromArgb(25, 10, 43);
        private readonly Color CorCampo = Color.FromArgb(18, 7, 31);
        private readonly Color CorRoxo = Color.FromArgb(123, 44, 191);
        private readonly Color CorTextoSecundario = Color.FromArgb(170, 160, 180);

        private TextBox txtPlaca = null!;
        private TextBox txtChassi = null!;
        private TextBox txtRenavam = null!;
        private TextBox txtModelo = null!;
        private TextBox txtMarca = null!;
        private TextBox txtAnoFabricacao = null!;
        private TextBox txtAnoModelo = null!;
        private TextBox txtCor = null!;
        private TextBox txtKmAtual = null!;
        private TextBox txtValorDiaria = null!;

        private ComboBox cmbCombustivel = null!;
        private ComboBox cmbCambio = null!;
        private ComboBox cmbStatus = null!;
        private ComboBox cmbCategoria = null!;
        private ComboBox cmbFilial = null!;

        private Button btnSalvar = null!;
        private Button btnCancelar = null!;

        public bool VeiculoCadastrado { get; private set; }

        public NovoVeiculoForm()
        {
            ConfigurarFormulario();
            CriarInterface();

            Shown += async (_, _) =>
            {
                await CarregarCategorias();
                await CarregarFiliais();
            };
        }

        private void ConfigurarFormulario()
        {
            Text = "GoCar - Novo Veículo";

            Size = new Size(760, 760);
            MinimumSize = new Size(760, 760);

            StartPosition = FormStartPosition.CenterParent;

            BackColor = CorFundo;
            ForeColor = Color.White;

            Font = new Font("Segoe UI", 10F);

            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
        }

        private void CriarInterface()
        {
            var titulo = new Label
            {
                Text = "Novo Veículo",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(35, 25)
            };

            var subtitulo = new Label
            {
                Text = "Cadastre um novo veículo na locadora.",
                ForeColor = CorTextoSecundario,
                AutoSize = true,
                Location = new Point(38, 70)
            };

            Controls.Add(titulo);
            Controls.Add(subtitulo);

            int esquerda1 = 40;
            int esquerda2 = 385;
            int largura = 300;

            int y = 115;

            txtPlaca = CriarTextBox("Placa", esquerda1, y, largura);
            txtChassi = CriarTextBox("Chassi", esquerda2, y, largura);

            y += 75;

            txtRenavam = CriarTextBox("Renavam", esquerda1, y, largura);
            txtModelo = CriarTextBox("Modelo", esquerda2, y, largura);

            y += 75;

            txtMarca = CriarTextBox("Marca", esquerda1, y, largura);
            txtCor = CriarTextBox("Cor", esquerda2, y, largura);

            y += 75;

            txtAnoFabricacao =
                CriarTextBox("Ano de fabricação", esquerda1, y, largura);

            txtAnoModelo =
                CriarTextBox("Ano do modelo", esquerda2, y, largura);

            y += 75;

            txtKmAtual =
                CriarTextBox("Km atual", esquerda1, y, largura);

            txtValorDiaria =
                CriarTextBox("Valor da diária", esquerda2, y, largura);

            y += 75;

            cmbCombustivel =
                CriarComboBox("Combustível", esquerda1, y, largura);

            cmbCambio =
                CriarComboBox("Câmbio", esquerda2, y, largura);

            cmbCombustivel.Items.AddRange(new object[]
            {
                new OpcaoEnum(1, "Gasolina"),
                new OpcaoEnum(2, "Etanol"),
                new OpcaoEnum(3, "Flex"),
                new OpcaoEnum(4, "Diesel"),
                new OpcaoEnum(5, "Elétrico"),
                new OpcaoEnum(6, "Híbrido")
            });

            cmbCambio.Items.AddRange(new object[]
            {
                new OpcaoEnum(1, "Manual"),
                new OpcaoEnum(2, "Automático")
            });

            y += 75;

            cmbStatus =
                CriarComboBox("Status", esquerda1, y, largura);

            cmbCategoria =
                CriarComboBox("Categoria", esquerda2, y, largura);

            cmbStatus.Items.AddRange(new object[]
            {
                new OpcaoEnum(1, "Disponível"),
                new OpcaoEnum(2, "Alugado"),
                new OpcaoEnum(3, "Manutenção"),
                new OpcaoEnum(4, "Indisponível")
            });

            y += 75;

            cmbFilial =
                CriarComboBox("Filial", esquerda1, y, largura);

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(385, 650),
                Size = new Size(140, 45),
                FlatStyle = FlatStyle.Flat,
                BackColor = CorCampo,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };

            btnCancelar.FlatAppearance.BorderColor =
                Color.FromArgb(90, 70, 110);

            btnCancelar.Click += (_, _) => Close();

            btnSalvar = new Button
            {
                Text = "Salvar Veículo",
                Location = new Point(545, 650),
                Size = new Size(140, 45),
                FlatStyle = FlatStyle.Flat,
                BackColor = CorRoxo,
                ForeColor = Color.White,
                Font = new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btnSalvar.FlatAppearance.BorderSize = 0;
            btnSalvar.Click += BtnSalvar_Click;

            Controls.Add(btnCancelar);
            Controls.Add(btnSalvar);
        }

        private TextBox CriarTextBox(
            string titulo,
            int x,
            int y,
            int largura)
        {
            var lbl = new Label
            {
                Text = titulo,
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(x, y)
            };

            var txt = new TextBox
            {
                Location = new Point(x, y + 25),
                Size = new Size(largura, 30),
                BackColor = CorCampo,
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Controls.Add(lbl);
            Controls.Add(txt);

            return txt;
        }

        private ComboBox CriarComboBox(
            string titulo,
            int x,
            int y,
            int largura)
        {
            var lbl = new Label
            {
                Text = titulo,
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(x, y)
            };

            var combo = new ComboBox
            {
                Location = new Point(x, y + 25),
                Size = new Size(largura, 30),

                DropDownStyle =
                    ComboBoxStyle.DropDownList,

                BackColor = CorCampo,
                ForeColor = Color.White,

                FlatStyle = FlatStyle.Flat
            };

            Controls.Add(lbl);
            Controls.Add(combo);

            return combo;
        }

        private async Task CarregarCategorias()
        {
            try
            {
                var categorias =
                    await ApiClient.GetAsync<List<CategoriaResponse>>(
                        "api/Categorias");

                cmbCategoria.Items.Clear();

                if (categorias == null)
                    return;

                foreach (var categoria in categorias)
                    cmbCategoria.Items.Add(categoria);

                if (cmbCategoria.Items.Count > 0)
                    cmbCategoria.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Não foi possível carregar as categorias.\n\n" +
                    ex.Message,
                    "GoCar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async Task CarregarFiliais()
        {
            try
            {
                var filiais =
                    await ApiClient.GetAsync<List<FilialResponse>>(
                        "api/Filiais");

                cmbFilial.Items.Clear();

                if (filiais == null)
                    return;

                foreach (var filial in filiais)
                    cmbFilial.Items.Add(filial);

                if (cmbFilial.Items.Count > 0)
                    cmbFilial.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Não foi possível carregar as filiais.\n\n" +
                    ex.Message,
                    "GoCar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async void BtnSalvar_Click(
            object? sender,
            EventArgs e)
        {
            if (!ValidarCampos())
                return;

            try
            {
                btnSalvar.Enabled = false;
                btnSalvar.Text = "Salvando...";

                var categoria =
                    (CategoriaResponse)cmbCategoria.SelectedItem!;

                var filial =
                    (FilialResponse)cmbFilial.SelectedItem!;

                var combustivel =
                    (OpcaoEnum)cmbCombustivel.SelectedItem!;

                var cambio =
                    (OpcaoEnum)cmbCambio.SelectedItem!;

                var status =
                    (OpcaoEnum)cmbStatus.SelectedItem!;

                decimal valorDiaria;

                var textoValor =
                    txtValorDiaria.Text
                        .Trim()
                        .Replace("R$", "")
                        .Trim();

                if (!decimal.TryParse(
                    textoValor,
                    NumberStyles.Number,
                    CultureInfo.GetCultureInfo("pt-BR"),
                    out valorDiaria))
                {
                    if (!decimal.TryParse(
                        textoValor.Replace(",", "."),
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out valorDiaria))
                    {
                        MessageBox.Show(
                            "Informe um valor de diária válido.",
                            "GoCar",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        return;
                    }
                }

                var novoVeiculo = new
                {
                    placa = txtPlaca.Text.Trim(),
                    chassi = txtChassi.Text.Trim(),
                    renavam = txtRenavam.Text.Trim(),
                    modelo = txtModelo.Text.Trim(),
                    marca = txtMarca.Text.Trim(),

                    anoFabricacao =
                        int.Parse(txtAnoFabricacao.Text),

                    anoModelo =
                        int.Parse(txtAnoModelo.Text),

                    cor = txtCor.Text.Trim(),

                    combustivel = combustivel.Id,
                    cambio = cambio.Id,
                    status = status.Id,

                    kmAtual =
                        int.Parse(txtKmAtual.Text),

                    valorDiaria,

                    isAtivo = true,

                    categoriaId = categoria.Id,
                    filialId = filial.Id
                };

                var response =
                    await ApiClient.PostAsync(
                        "api/Veiculos",
                        novoVeiculo);

                if (response.IsSuccessStatusCode)
                {
                    VeiculoCadastrado = true;

                    MessageBox.Show(
                        "Veículo cadastrado com sucesso!",
                        "GoCar",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    DialogResult = DialogResult.OK;
                    Close();

                    return;
                }

                var erro =
                    await response.Content.ReadAsStringAsync();

                MessageBox.Show(
                    "Não foi possível cadastrar o veículo.\n\n" +
                    $"Código: {(int)response.StatusCode}\n\n" +
                    erro,
                    "GoCar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao cadastrar veículo.\n\n" +
                    ex.Message,
                    "GoCar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnSalvar.Enabled = true;
                btnSalvar.Text = "Salvar Veículo";
            }
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtPlaca.Text) ||
                string.IsNullOrWhiteSpace(txtChassi.Text) ||
                string.IsNullOrWhiteSpace(txtRenavam.Text) ||
                string.IsNullOrWhiteSpace(txtModelo.Text) ||
                string.IsNullOrWhiteSpace(txtMarca.Text))
            {
                MessageBox.Show(
                    "Preencha os dados principais do veículo.",
                    "GoCar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            if (!int.TryParse(
                    txtAnoFabricacao.Text,
                    out _))
            {
                MessageBox.Show(
                    "Informe um ano de fabricação válido.",
                    "GoCar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            if (!int.TryParse(
                    txtAnoModelo.Text,
                    out _))
            {
                MessageBox.Show(
                    "Informe um ano de modelo válido.",
                    "GoCar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            if (!int.TryParse(
                    txtKmAtual.Text,
                    out _))
            {
                MessageBox.Show(
                    "Informe uma quilometragem válida.",
                    "GoCar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            if (cmbCombustivel.SelectedItem == null ||
                cmbCambio.SelectedItem == null ||
                cmbStatus.SelectedItem == null)
            {
                MessageBox.Show(
                    "Selecione combustível, câmbio e status.",
                    "GoCar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            if (cmbCategoria.SelectedItem == null)
            {
                MessageBox.Show(
                    "Selecione uma categoria.",
                    "GoCar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            if (cmbFilial.SelectedItem == null)
            {
                MessageBox.Show(
                    "Selecione uma filial.",
                    "GoCar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            return true;
        }

        private class OpcaoEnum
        {
            public int Id { get; }
            public string Nome { get; }

            public OpcaoEnum(int id, string nome)
            {
                Id = id;
                Nome = nome;
            }

            public override string ToString()
            {
                return Nome;
            }
        }

        private class CategoriaResponse
        {
            public int Id { get; set; }
            public string Nome { get; set; } = "";

            public override string ToString()
            {
                return Nome;
            }
        }

        private class FilialResponse
        {
            public int Id { get; set; }
            public string Nome { get; set; } = "";

            public override string ToString()
            {
                return Nome;
            }
        }
    }
}