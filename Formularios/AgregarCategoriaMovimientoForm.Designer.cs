namespace FlujoDeCajaApp.Formularios
{
    partial class AgregarCategoriaMovimientoForm
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
            this.lblNombreCategoria = new Label();
            this.txtNombreCategoria = new TextBox();
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
            this.panelPrincipal.Size = new Size(600, 450);
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
            this.panelEncabezado.Size = new Size(600, 80);
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
            this.lblTitulo.Size = new Size(280, 32);
            this.lblTitulo.TabIndex = 1;
            this.lblTitulo.Text = "Categoría de Movimiento";
            
            // 
            // btnRegresar
            // 
            this.btnRegresar.BackColor = Color.FromArgb(231, 76, 60);
            this.btnRegresar.FlatStyle = FlatStyle.Flat;
            this.btnRegresar.FlatAppearance.BorderSize = 0;
            this.btnRegresar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnRegresar.ForeColor = Color.White;
            this.btnRegresar.Location = new Point(520, 25);
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
            this.panelFormulario.Controls.Add(this.lblNombreCategoria);
            this.panelFormulario.Controls.Add(this.txtNombreCategoria);
            this.panelFormulario.Controls.Add(this.btnGuardar);
            this.panelFormulario.Controls.Add(this.lblMensajeExito);
            this.panelFormulario.Dock = DockStyle.Fill;
            this.panelFormulario.Location = new Point(0, 80);
            this.panelFormulario.Name = "panelFormulario";
            this.panelFormulario.Size = new Size(600, 370);
            this.panelFormulario.TabIndex = 1;
            this.panelFormulario.Padding = new Padding(50);
            
            // 
            // lblTipoEntidad
            // 
            this.lblTipoEntidad.AutoSize = true;
            this.lblTipoEntidad.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblTipoEntidad.ForeColor = Color.FromArgb(52, 73, 94);
            this.lblTipoEntidad.Location = new Point(50, 30);
            this.lblTipoEntidad.Name = "lblTipoEntidad";
            this.lblTipoEntidad.Size = new Size(142, 21);
            this.lblTipoEntidad.TabIndex = 0;
            this.lblTipoEntidad.Text = "Tipo de Entidad:";
            
            // 
            // cmbTipoEntidad
            // 
            this.cmbTipoEntidad.Font = new Font("Segoe UI", 12F);
            this.cmbTipoEntidad.Location = new Point(50, 55);
            this.cmbTipoEntidad.Name = "cmbTipoEntidad";
            this.cmbTipoEntidad.Size = new Size(500, 29);
            this.cmbTipoEntidad.TabIndex = 1;
            this.cmbTipoEntidad.DropDownStyle = ComboBoxStyle.DropDownList;
            
            // 
            // lblNombreCategoria
            // 
            this.lblNombreCategoria.AutoSize = true;
            this.lblNombreCategoria.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblNombreCategoria.ForeColor = Color.FromArgb(52, 73, 94);
            this.lblNombreCategoria.Location = new Point(50, 120);
            this.lblNombreCategoria.Name = "lblNombreCategoria";
            this.lblNombreCategoria.Size = new Size(240, 21);
            this.lblNombreCategoria.TabIndex = 2;
            this.lblNombreCategoria.Text = "Nombre de la Categoría de Movimiento:";
            
            // 
            // txtNombreCategoria
            // 
            this.txtNombreCategoria.Font = new Font("Segoe UI", 12F);
            this.txtNombreCategoria.Location = new Point(50, 145);
            this.txtNombreCategoria.Name = "txtNombreCategoria";
            this.txtNombreCategoria.Size = new Size(500, 29);
            this.txtNombreCategoria.TabIndex = 3;
            this.txtNombreCategoria.BorderStyle = BorderStyle.FixedSingle;
            
            // 
            // btnGuardar
            // 
            this.btnGuardar.BackColor = Color.FromArgb(46, 204, 113);
            this.btnGuardar.FlatStyle = FlatStyle.Flat;
            this.btnGuardar.FlatAppearance.BorderSize = 0;
            this.btnGuardar.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.btnGuardar.ForeColor = Color.White;
            this.btnGuardar.Location = new Point(240, 220);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new Size(120, 45);
            this.btnGuardar.TabIndex = 4;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = false;
            
            // 
            // lblMensajeExito
            // 
            this.lblMensajeExito.AutoSize = true;
            this.lblMensajeExito.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblMensajeExito.ForeColor = Color.FromArgb(46, 204, 113);
            this.lblMensajeExito.Location = new Point(50, 300);
            this.lblMensajeExito.Name = "lblMensajeExito";
            this.lblMensajeExito.Size = new Size(0, 21);
            this.lblMensajeExito.TabIndex = 5;
            this.lblMensajeExito.TextAlign = ContentAlignment.MiddleCenter;
            this.lblMensajeExito.Visible = false;
            
            // 
            // AgregarCategoriaMovimientoForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(600, 450);
            this.Controls.Add(this.panelPrincipal);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AgregarCategoriaMovimientoForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Samara Rentals - Agregar Categoría de Movimiento";
            
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
        private Label lblNombreCategoria;
        private TextBox txtNombreCategoria;
        private Button btnGuardar;
        private Label lblMensajeExito;
    }
}