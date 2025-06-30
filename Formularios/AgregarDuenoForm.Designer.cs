namespace FlujoDeCajaApp.Formularios
{
    partial class AgregarDuenoForm
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
            this.panelPrincipal = new Panel();
            this.panelEncabezado = new Panel();
            this.lblTitulo = new Label();
            this.pictureBoxLogo = new PictureBox();
            this.btnRegresar = new Button();
            this.panelFormulario = new Panel();
            this.lblTipoEntidad = new Label();
            this.cmbTipoEntidad = new ComboBox();
            this.lblNombreCompleto = new Label();
            this.txtNombreCompleto = new TextBox();
            this.lblIdentificacion = new Label();
            this.txtIdentificacion = new TextBox();
            this.lblCorreoElectronico = new Label();
            this.txtCorreoElectronico = new TextBox();
            this.lblNumeroTelefonico = new Label();
            this.txtNumeroTelefonico = new TextBox();
            this.btnGuardar = new Button();
            this.lblMensajeExito = new Label();
            
            this.panelPrincipal.SuspendLayout();
            this.panelEncabezado.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.panelFormulario.SuspendLayout();
            this.SuspendLayout();
            
            // 
            // panelPrincipal
            // 
            this.panelPrincipal.Controls.Add(this.panelFormulario);
            this.panelPrincipal.Controls.Add(this.panelEncabezado);
            this.panelPrincipal.Dock = DockStyle.Fill;
            this.panelPrincipal.Location = new Point(0, 0);
            this.panelPrincipal.Name = "panelPrincipal";
            this.panelPrincipal.Size = new Size(700, 600);
            this.panelPrincipal.TabIndex = 0;
            this.panelPrincipal.BackColor = Color.FromArgb(245, 245, 245);
            
            // 
            // panelEncabezado
            // 
            this.panelEncabezado.Controls.Add(this.btnRegresar);
            this.panelEncabezado.Controls.Add(this.pictureBoxLogo);
            this.panelEncabezado.Controls.Add(this.lblTitulo);
            this.panelEncabezado.Dock = DockStyle.Top;
            this.panelEncabezado.Location = new Point(0, 0);
            this.panelEncabezado.Name = "panelEncabezado";
            this.panelEncabezado.Size = new Size(700, 80);
            this.panelEncabezado.TabIndex = 0;
            this.panelEncabezado.BackColor = Color.FromArgb(41, 128, 185);
            
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Location = new Point(20, 15);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new Size(50, 50);
            this.pictureBoxLogo.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBoxLogo.TabIndex = 0;
            this.pictureBoxLogo.TabStop = false;
            
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.lblTitulo.ForeColor = Color.White;
            this.lblTitulo.Location = new Point(85, 25);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new Size(220, 32);
            this.lblTitulo.TabIndex = 1;
            this.lblTitulo.Text = "Apartado de Agregación";
            
            // 
            // btnRegresar
            // 
            this.btnRegresar.BackColor = Color.FromArgb(231, 76, 60);
            this.btnRegresar.FlatStyle = FlatStyle.Flat;
            this.btnRegresar.FlatAppearance.BorderSize = 0;
            this.btnRegresar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnRegresar.ForeColor = Color.White;
            this.btnRegresar.Location = new Point(620, 25);
            this.btnRegresar.Name = "btnRegresar";
            this.btnRegresar.Size = new Size(60, 30);
            this.btnRegresar.TabIndex = 2;
            this.btnRegresar.Text = "← Regresar";
            this.btnRegresar.UseVisualStyleBackColor = false;
            
            // 
            // panelFormulario
            // 
            this.panelFormulario.Controls.Add(this.lblTipoEntidad);
            this.panelFormulario.Controls.Add(this.cmbTipoEntidad);
            this.panelFormulario.Controls.Add(this.lblNombreCompleto);
            this.panelFormulario.Controls.Add(this.txtNombreCompleto);
            this.panelFormulario.Controls.Add(this.lblIdentificacion);
            this.panelFormulario.Controls.Add(this.txtIdentificacion);
            this.panelFormulario.Controls.Add(this.lblCorreoElectronico);
            this.panelFormulario.Controls.Add(this.txtCorreoElectronico);
            this.panelFormulario.Controls.Add(this.lblNumeroTelefonico);
            this.panelFormulario.Controls.Add(this.txtNumeroTelefonico);
            this.panelFormulario.Controls.Add(this.btnGuardar);
            this.panelFormulario.Controls.Add(this.lblMensajeExito);
            this.panelFormulario.Dock = DockStyle.Fill;
            this.panelFormulario.Location = new Point(0, 80);
            this.panelFormulario.Name = "panelFormulario";
            this.panelFormulario.Size = new Size(700, 520);
            this.panelFormulario.TabIndex = 1;
            this.panelFormulario.Padding = new Padding(50);
            
            // 
            // lblTipoEntidad
            // 
            this.lblTipoEntidad.AutoSize = true;
            this.lblTipoEntidad.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.lblTipoEntidad.ForeColor = Color.FromArgb(52, 73, 94);
            this.lblTipoEntidad.Location = new Point(50, 30);
            this.lblTipoEntidad.Name = "lblTipoEntidad";
            this.lblTipoEntidad.Size = new Size(135, 20);
            this.lblTipoEntidad.TabIndex = 0;
            this.lblTipoEntidad.Text = "Tipo de Entidad:";
            
            // 
            // cmbTipoEntidad
            // 
            this.cmbTipoEntidad.Font = new Font("Segoe UI", 11F);
            this.cmbTipoEntidad.Location = new Point(50, 55);
            this.cmbTipoEntidad.Name = "cmbTipoEntidad";
            this.cmbTipoEntidad.Size = new Size(600, 28);
            this.cmbTipoEntidad.TabIndex = 1;
            this.cmbTipoEntidad.DropDownStyle = ComboBoxStyle.DropDownList;
            
            // 
            // lblNombreCompleto
            // 
            this.lblNombreCompleto.AutoSize = true;
            this.lblNombreCompleto.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.lblNombreCompleto.ForeColor = Color.FromArgb(52, 73, 94);
            this.lblNombreCompleto.Location = new Point(50, 110);
            this.lblNombreCompleto.Name = "lblNombreCompleto";
            this.lblNombreCompleto.Size = new Size(140, 20);
            this.lblNombreCompleto.TabIndex = 2;
            this.lblNombreCompleto.Text = "Nombre Completo:";
            
            // 
            // txtNombreCompleto
            // 
            this.txtNombreCompleto.Font = new Font("Segoe UI", 11F);
            this.txtNombreCompleto.Location = new Point(50, 135);
            this.txtNombreCompleto.Name = "txtNombreCompleto";
            this.txtNombreCompleto.Size = new Size(600, 27);
            this.txtNombreCompleto.TabIndex = 3;
            this.txtNombreCompleto.BorderStyle = BorderStyle.FixedSingle;
            
            // 
            // lblIdentificacion
            // 
            this.lblIdentificacion.AutoSize = true;
            this.lblIdentificacion.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.lblIdentificacion.ForeColor = Color.FromArgb(52, 73, 94);
            this.lblIdentificacion.Location = new Point(50, 190);
            this.lblIdentificacion.Name = "lblIdentificacion";
            this.lblIdentificacion.Size = new Size(105, 20);
            this.lblIdentificacion.TabIndex = 4;
            this.lblIdentificacion.Text = "Identificación:";
            
            // 
            // txtIdentificacion
            // 
            this.txtIdentificacion.Font = new Font("Segoe UI", 11F);
            this.txtIdentificacion.Location = new Point(50, 215);
            this.txtIdentificacion.Name = "txtIdentificacion";
            this.txtIdentificacion.Size = new Size(600, 27);
            this.txtIdentificacion.TabIndex = 5;
            this.txtIdentificacion.BorderStyle = BorderStyle.FixedSingle;
            
            // 
            // lblCorreoElectronico
            // 
            this.lblCorreoElectronico.AutoSize = true;
            this.lblCorreoElectronico.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.lblCorreoElectronico.ForeColor = Color.FromArgb(52, 73, 94);
            this.lblCorreoElectronico.Location = new Point(50, 270);
            this.lblCorreoElectronico.Name = "lblCorreoElectronico";
            this.lblCorreoElectronico.Size = new Size(143, 20);
            this.lblCorreoElectronico.TabIndex = 6;
            this.lblCorreoElectronico.Text = "Correo Electrónico:";
            
            // 
            // txtCorreoElectronico
            // 
            this.txtCorreoElectronico.Font = new Font("Segoe UI", 11F);
            this.txtCorreoElectronico.Location = new Point(50, 295);
            this.txtCorreoElectronico.Name = "txtCorreoElectronico";
            this.txtCorreoElectronico.Size = new Size(600, 27);
            this.txtCorreoElectronico.TabIndex = 7;
            this.txtCorreoElectronico.BorderStyle = BorderStyle.FixedSingle;
            
            // 
            // lblNumeroTelefonico
            // 
            this.lblNumeroTelefonico.AutoSize = true;
            this.lblNumeroTelefonico.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.lblNumeroTelefonico.ForeColor = Color.FromArgb(52, 73, 94);
            this.lblNumeroTelefonico.Location = new Point(50, 350);
            this.lblNumeroTelefonico.Name = "lblNumeroTelefonico";
            this.lblNumeroTelefonico.Size = new Size(141, 20);
            this.lblNumeroTelefonico.TabIndex = 8;
            this.lblNumeroTelefonico.Text = "Número Telefónico:";
            
            // 
            // txtNumeroTelefonico
            // 
            this.txtNumeroTelefonico.Font = new Font("Segoe UI", 11F);
            this.txtNumeroTelefonico.Location = new Point(50, 375);
            this.txtNumeroTelefonico.Name = "txtNumeroTelefonico";
            this.txtNumeroTelefonico.Size = new Size(600, 27);
            this.txtNumeroTelefonico.TabIndex = 9;
            this.txtNumeroTelefonico.BorderStyle = BorderStyle.FixedSingle;
            
            // 
            // btnGuardar
            // 
            this.btnGuardar.BackColor = Color.FromArgb(46, 204, 113);
            this.btnGuardar.FlatStyle = FlatStyle.Flat;
            this.btnGuardar.FlatAppearance.BorderSize = 0;
            this.btnGuardar.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.btnGuardar.ForeColor = Color.White;
            this.btnGuardar.Location = new Point(300, 430);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new Size(120, 40);
            this.btnGuardar.TabIndex = 10;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = false;
            
            // 
            // lblMensajeExito
            // 
            this.lblMensajeExito.AutoSize = true;
            this.lblMensajeExito.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.lblMensajeExito.ForeColor = Color.FromArgb(46, 204, 113);
            this.lblMensajeExito.Location = new Point(50, 485);
            this.lblMensajeExito.Name = "lblMensajeExito";
            this.lblMensajeExito.Size = new Size(0, 20);
            this.lblMensajeExito.TabIndex = 11;
            this.lblMensajeExito.TextAlign = ContentAlignment.MiddleCenter;
            this.lblMensajeExito.Visible = false;
            
            // 
            // AgregarDuenoForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(700, 600);
            this.Controls.Add(this.panelPrincipal);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AgregarDuenoForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Samara Rentals - Agregar Dueño";
            
            this.panelPrincipal.ResumeLayout(false);
            this.panelEncabezado.ResumeLayout(false);
            this.panelEncabezado.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.panelFormulario.ResumeLayout(false);
            this.panelFormulario.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private Panel panelPrincipal;
        private Panel panelEncabezado;
        private Label lblTitulo;
        private PictureBox pictureBoxLogo;
        private Button btnRegresar;
        private Panel panelFormulario;
        private Label lblTipoEntidad;
        private ComboBox cmbTipoEntidad;
        private Label lblNombreCompleto;
        private TextBox txtNombreCompleto;
        private Label lblIdentificacion;
        private TextBox txtIdentificacion;
        private Label lblCorreoElectronico;
        private TextBox txtCorreoElectronico;
        private Label lblNumeroTelefonico;
        private TextBox txtNumeroTelefonico;
        private Button btnGuardar;
        private Label lblMensajeExito;
    }
}
