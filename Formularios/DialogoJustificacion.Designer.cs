namespace FlujoDeCajaApp.Formularios
{
    partial class DialogoJustificacion
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
            lblMensaje = new Label();
            txtJustificacion = new TextBox();
            btnAceptar = new Button();
            btnCancelar = new Button();
            panelBotones = new Panel();
            lblTitulo = new Label();
            SuspendLayout();
            // 
            // lblMensaje
            // 
            lblMensaje.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblMensaje.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblMensaje.Location = new Point(20, 50);
            lblMensaje.Name = "lblMensaje";
            lblMensaje.Size = new Size(440, 40);
            lblMensaje.TabIndex = 0;
            lblMensaje.Text = "Ingrese la justificación para esta acción:";
            lblMensaje.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtJustificacion
            // 
            txtJustificacion.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtJustificacion.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            txtJustificacion.Location = new Point(20, 100);
            txtJustificacion.Multiline = true;
            txtJustificacion.Name = "txtJustificacion";
            txtJustificacion.PlaceholderText = "Escriba la justificación aquí...";
            txtJustificacion.ScrollBars = ScrollBars.Vertical;
            txtJustificacion.Size = new Size(440, 80);
            txtJustificacion.TabIndex = 1;
            // 
            // btnAceptar
            // 
            btnAceptar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnAceptar.BackColor = Color.FromArgb(52, 152, 219);
            btnAceptar.FlatStyle = FlatStyle.Flat;
            btnAceptar.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            btnAceptar.ForeColor = Color.White;
            btnAceptar.Location = new Point(290, 200);
            btnAceptar.Name = "btnAceptar";
            btnAceptar.Size = new Size(80, 35);
            btnAceptar.TabIndex = 2;
            btnAceptar.Text = "Aceptar";
            btnAceptar.UseVisualStyleBackColor = false;
            // 
            // btnCancelar
            // 
            btnCancelar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancelar.BackColor = Color.FromArgb(149, 165, 166);
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            btnCancelar.ForeColor = Color.White;
            btnCancelar.Location = new Point(380, 200);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(80, 35);
            btnCancelar.TabIndex = 3;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = false;
            // 
            // panelBotones
            // 
            panelBotones.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelBotones.BackColor = Color.FromArgb(236, 240, 241);
            panelBotones.Location = new Point(0, 190);
            panelBotones.Name = "panelBotones";
            panelBotones.Size = new Size(484, 55);
            panelBotones.TabIndex = 4;
            // 
            // lblTitulo
            // 
            lblTitulo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblTitulo.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitulo.ForeColor = Color.FromArgb(52, 73, 94);
            lblTitulo.Location = new Point(20, 15);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(440, 25);
            lblTitulo.TabIndex = 5;
            lblTitulo.Text = "Justificación Requerida";
            lblTitulo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // DialogoJustificacion
            // 
            AcceptButton = btnAceptar;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            CancelButton = btnCancelar;
            ClientSize = new Size(484, 245);
            Controls.Add(lblTitulo);
            Controls.Add(btnCancelar);
            Controls.Add(btnAceptar);
            Controls.Add(txtJustificacion);
            Controls.Add(lblMensaje);
            Controls.Add(panelBotones);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DialogoJustificacion";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Justificación";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblMensaje;
        private TextBox txtJustificacion;
        private Button btnAceptar;
        private Button btnCancelar;
        private Panel panelBotones;
        private Label lblTitulo;
    }
}