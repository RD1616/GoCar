namespace GoCar.Desktop.Forms
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            picBackground = new PictureBox();
            pnlLogin = new Panel();
            pnlLinhaDireita = new Panel();
            pnlLinhaEsquerda = new Panel();
            lblRestrito = new Label();
            lblStatus = new Label();
            btnEntrar = new Button();
            pnlSenha = new Panel();
            picOlho = new PictureBox();
            pictureBox2 = new PictureBox();
            txtSenha = new TextBox();
            pnlEmail = new Panel();
            pictureBox1 = new PictureBox();
            txtEmail = new TextBox();
            lblDescricao = new Label();
            lblTitulo = new Label();
            picLogo = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)picBackground).BeginInit();
            pnlLogin.SuspendLayout();
            pnlSenha.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picOlho).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            pnlEmail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            SuspendLayout();
            // 
            // picBackground
            // 
            picBackground.Dock = DockStyle.Fill;
            picBackground.Location = new Point(0, 0);
            picBackground.Name = "picBackground";
            picBackground.Size = new Size(800, 749);
            picBackground.SizeMode = PictureBoxSizeMode.StretchImage;
            picBackground.TabIndex = 0;
            picBackground.TabStop = false;
            // 
            // pnlLogin
            // 
            pnlLogin.BackgroundImage = (Image)resources.GetObject("pnlLogin.BackgroundImage");
            pnlLogin.BackgroundImageLayout = ImageLayout.Stretch;
            pnlLogin.Controls.Add(lblRestrito);
            pnlLogin.Controls.Add(btnEntrar);
            pnlLogin.Controls.Add(pnlEmail);
            pnlLogin.Controls.Add(pnlSenha);
            pnlLogin.Controls.Add(lblDescricao);
            pnlLogin.Controls.Add(lblTitulo);
            pnlLogin.Controls.Add(picLogo);
            pnlLogin.Controls.Add(pnlLinhaDireita);
            pnlLogin.Controls.Add(pnlLinhaEsquerda);
            pnlLogin.Controls.Add(lblStatus);
            pnlLogin.Location = new Point(56, 100);
            pnlLogin.Name = "pnlLogin";
            pnlLogin.Size = new Size(421, 640);
            pnlLogin.TabIndex = 1;
            pnlLogin.Paint += pnlLogin_Paint;
            // 
            // pnlLinhaDireita
            // 
            pnlLinhaDireita.BackColor = Color.Gray;
            pnlLinhaDireita.Location = new Point(304, 575);
            pnlLinhaDireita.Name = "pnlLinhaDireita";
            pnlLinhaDireita.Size = new Size(95, 1);
            pnlLinhaDireita.TabIndex = 9;
            // 
            // pnlLinhaEsquerda
            // 
            pnlLinhaEsquerda.BackColor = Color.Gray;
            pnlLinhaEsquerda.Location = new Point(22, 575);
            pnlLinhaEsquerda.Name = "pnlLinhaEsquerda";
            pnlLinhaEsquerda.Size = new Size(95, 1);
            pnlLinhaEsquerda.TabIndex = 8;
            // 
            // lblRestrito
            // 
            lblRestrito.BackColor = Color.Transparent;
            lblRestrito.Font = new Font("Segoe UI Semibold", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblRestrito.ForeColor = Color.White;
            lblRestrito.Location = new Point(114, 565);
            lblRestrito.Name = "lblRestrito";
            lblRestrito.Size = new Size(190, 20);
            lblRestrito.TabIndex = 7;
            lblRestrito.Text = "Acesso restrito a administradores";
            lblRestrito.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblStatus
            // 
            lblStatus.BackColor = Color.Transparent;
            lblStatus.ForeColor = Color.IndianRed;
            lblStatus.Location = new Point(16, 482);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(389, 25);
            lblStatus.TabIndex = 6;
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnEntrar
            // 
            btnEntrar.BackColor = Color.FromArgb(111, 38, 201);
            btnEntrar.Cursor = Cursors.Hand;
            btnEntrar.FlatAppearance.BorderSize = 0;
            btnEntrar.FlatStyle = FlatStyle.Flat;
            btnEntrar.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnEntrar.ForeColor = Color.White;
            btnEntrar.Location = new Point(16, 430);
            btnEntrar.Name = "btnEntrar";
            btnEntrar.Size = new Size(389, 42);
            btnEntrar.TabIndex = 5;
            btnEntrar.Text = "Entrar →";
            btnEntrar.UseVisualStyleBackColor = false;
            btnEntrar.Click += btnEntrar_Click;
            // 
            // pnlSenha
            // 
            pnlSenha.BackColor = Color.Black;
            pnlSenha.Controls.Add(picOlho);
            pnlSenha.Controls.Add(pictureBox2);
            pnlSenha.Controls.Add(txtSenha);
            pnlSenha.Location = new Point(16, 320);
            pnlSenha.Name = "pnlSenha";
            pnlSenha.Size = new Size(389, 42);
            pnlSenha.TabIndex = 4;
            // 
            // picOlho
            // 
            picOlho.BackColor = Color.Transparent;
            picOlho.Cursor = Cursors.Hand;
            picOlho.Image = (Image)resources.GetObject("picOlho.Image");
            picOlho.Location = new Point(350, 9);
            picOlho.Name = "picOlho";
            picOlho.Size = new Size(24, 24);
            picOlho.SizeMode = PictureBoxSizeMode.Zoom;
            picOlho.TabIndex = 2;
            picOlho.TabStop = false;
            picOlho.Click += picOlho_Click;
            // 
            // pictureBox2
            // 
            pictureBox2.BackColor = Color.Transparent;
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(12, 9);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(24, 24);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 1;
            pictureBox2.TabStop = false;
            // 
            // txtSenha
            // 
            txtSenha.BackColor = Color.Black;
            txtSenha.BorderStyle = BorderStyle.None;
            txtSenha.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtSenha.ForeColor = Color.White;
            txtSenha.Location = new Point(45, 10);
            txtSenha.Name = "txtSenha";
            txtSenha.PlaceholderText = "Senha";
            txtSenha.Size = new Size(290, 18);
            txtSenha.TabIndex = 0;
            txtSenha.UseSystemPasswordChar = true;
            // 
            // pnlEmail
            // 
            pnlEmail.BackColor = Color.Black;
            pnlEmail.Controls.Add(pictureBox1);
            pnlEmail.Controls.Add(txtEmail);
            pnlEmail.Location = new Point(16, 235);
            pnlEmail.Name = "pnlEmail";
            pnlEmail.Size = new Size(389, 42);
            pnlEmail.TabIndex = 3;
            pnlEmail.Paint += pnlEmail_Paint;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(12, 9);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(24, 24);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // txtEmail
            // 
            txtEmail.BackColor = Color.Black;
            txtEmail.BorderStyle = BorderStyle.None;
            txtEmail.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtEmail.ForeColor = Color.White;
            txtEmail.Location = new Point(45, 10);
            txtEmail.Name = "txtEmail";
            txtEmail.PlaceholderText = "Usuário ou e-mail";
            txtEmail.Size = new Size(325, 18);
            txtEmail.TabIndex = 0;
            // 
            // lblDescricao
            // 
            lblDescricao.AutoSize = true;
            lblDescricao.BackColor = Color.Transparent;
            lblDescricao.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblDescricao.ForeColor = Color.Gainsboro;
            lblDescricao.Location = new Point(16, 182);
            lblDescricao.Name = "lblDescricao";
            lblDescricao.Size = new Size(271, 17);
            lblDescricao.TabIndex = 2;
            lblDescricao.Text = "Faça login para acessar o painel de controle.";
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.BackColor = Color.Transparent;
            lblTitulo.Font = new Font("Microsoft Sans Serif", 8.25F);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(16, 158);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(123, 13);
            lblTitulo.TabIndex = 1;
            lblTitulo.Text = "Acesso do Administrador";
            // 
            // picLogo
            // 
            picLogo.BackColor = Color.Transparent;
            picLogo.Image = (Image)resources.GetObject("picLogo.Image");
            picLogo.Location = new Point(65, 45);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(290, 90);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.TabIndex = 0;
            picLogo.TabStop = false;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(8, 0, 20);
            ClientSize = new Size(800, 749);
            Controls.Add(pnlLogin);
            Controls.Add(picBackground);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "GoCar - Acesso Administrativo";
            ((System.ComponentModel.ISupportInitialize)picBackground).EndInit();
            pnlLogin.ResumeLayout(false);
            pnlLogin.PerformLayout();
            pnlSenha.ResumeLayout(false);
            pnlSenha.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picOlho).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            pnlEmail.ResumeLayout(false);
            pnlEmail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox picBackground;
        private Panel pnlLogin;
        private Label lblTitulo;
        private PictureBox picLogo;
        private Panel pnlEmail;
        private Label lblDescricao;
        private Panel pnlSenha;
        private TextBox txtEmail;
        private Button btnEntrar;
        private TextBox txtSenha;
        private Label lblStatus;
        private Panel pnlLinhaEsquerda;
        private Label lblRestrito;
        private Panel pnlLinhaDireita;
        private PictureBox picOlho;
        private PictureBox pictureBox2;
        private PictureBox pictureBox1;
    }
}