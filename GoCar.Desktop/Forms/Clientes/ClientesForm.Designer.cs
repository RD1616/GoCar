namespace GoCar.Desktop.Forms.Clientes
{
    partial class ClientesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblTitulo = new Label();
            lblPesquisa = new Label();
            txtPesquisa = new TextBox();
            btnBuscar = new Button();
            btnNovo = new Button();
            btnEditar = new Button();
            btnDesativar = new Button();
            btnAtualizar = new Button();
            dgvClientes = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dgvClientes).BeginInit();
            SuspendLayout();
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Location = new Point(33, 24);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(49, 15);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Clientes";
            // 
            // lblPesquisa
            // 
            lblPesquisa.AutoSize = true;
            lblPesquisa.Location = new Point(34, 60);
            lblPesquisa.Name = "lblPesquisa";
            lblPesquisa.Size = new Size(60, 15);
            lblPesquisa.TabIndex = 1;
            lblPesquisa.Text = "Pesquisar:";
            // 
            // txtPesquisa
            // 
            txtPesquisa.Location = new Point(100, 57);
            txtPesquisa.Name = "txtPesquisa";
            txtPesquisa.Size = new Size(221, 23);
            txtPesquisa.TabIndex = 2;
            // 
            // btnBuscar
            // 
            btnBuscar.Location = new Point(327, 57);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(75, 23);
            btnBuscar.TabIndex = 3;
            btnBuscar.Text = "Buscar";
            btnBuscar.UseVisualStyleBackColor = true;
            // 
            // btnNovo
            // 
            btnNovo.Location = new Point(408, 57);
            btnNovo.Name = "btnNovo";
            btnNovo.Size = new Size(121, 23);
            btnNovo.TabIndex = 4;
            btnNovo.Text = "+ Novo Cliente";
            btnNovo.UseVisualStyleBackColor = true;
            // 
            // btnEditar
            // 
            btnEditar.Location = new Point(34, 356);
            btnEditar.Name = "btnEditar";
            btnEditar.Size = new Size(75, 23);
            btnEditar.TabIndex = 5;
            btnEditar.Text = "Editar";
            btnEditar.UseVisualStyleBackColor = true;
            btnEditar.Click += button3_Click;
            // 
            // btnDesativar
            // 
            btnDesativar.Location = new Point(361, 356);
            btnDesativar.Name = "btnDesativar";
            btnDesativar.Size = new Size(75, 23);
            btnDesativar.TabIndex = 6;
            btnDesativar.Text = "Desativar";
            btnDesativar.UseVisualStyleBackColor = true;
            // 
            // btnAtualizar
            // 
            btnAtualizar.Location = new Point(454, 356);
            btnAtualizar.Name = "btnAtualizar";
            btnAtualizar.Size = new Size(75, 23);
            btnAtualizar.TabIndex = 7;
            btnAtualizar.Text = "Atualizar";
            btnAtualizar.UseVisualStyleBackColor = true;
            btnAtualizar.Click += button5_Click;
            // 
            // dgvClientes
            // 
            dgvClientes.AllowUserToAddRows = false;
            dgvClientes.AllowUserToDeleteRows = false;
            dgvClientes.AllowUserToResizeColumns = false;
            dgvClientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvClientes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvClientes.Location = new Point(34, 96);
            dgvClientes.MultiSelect = false;
            dgvClientes.Name = "dgvClientes";
            dgvClientes.ReadOnly = true;
            dgvClientes.RowHeadersVisible = false;
            dgvClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClientes.Size = new Size(495, 254);
            dgvClientes.TabIndex = 8;
            // 
            // ClientesForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 611);
            Controls.Add(dgvClientes);
            Controls.Add(btnAtualizar);
            Controls.Add(btnDesativar);
            Controls.Add(btnEditar);
            Controls.Add(btnNovo);
            Controls.Add(btnBuscar);
            Controls.Add(txtPesquisa);
            Controls.Add(lblPesquisa);
            Controls.Add(lblTitulo);
            MinimumSize = new Size(900, 550);
            Name = "ClientesForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "GoCar - Clientes";
            Load += ClientesForm_Load;
            ((System.ComponentModel.ISupportInitialize)dgvClientes).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitulo;
        private Label lblPesquisa;
        private TextBox txtPesquisa;
        private Button btnBuscar;
        private Button btnNovo;
        private Button btnEditar;
        private Button btnDesativar;
        private Button btnAtualizar;
        private DataGridView dgvClientes;
    }
}