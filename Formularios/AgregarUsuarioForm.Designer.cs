using System.Drawing;
using System.Windows.Forms;

namespace FlujoDeCajaApp.Formularios
{
    partial class AgregarUsuarioForm
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelPrincipal = new Panel();
            this.panelFormulario = new Panel();
            this.panelEncabezado = new Panel();
            this.pictureBoxLogo = new PictureBox();
            this.lblTitulo = new Label();
            this.btnRegresar = new Button();
            this.lblDropdownTipo = new Label();
            this.cmbTipoEntidad = new ComboBox();
            this.lblNombreUsuario = new Label();
            this.txtNombreUsuario = new TextBox();
            this.lblContrasena = new Label();
            this.txtContrasena = new TextBox();
            this.lblCorreo = new Label();
            this.txtCorreo = new TextBox();
            this.btnGuardar = new Button();
            this.lblMensajeExito = new Label();
            
            this.panelPrincipal.SuspendLayout();
            this.panelFormulario.SuspendLayout();
            this.panelEncabezado.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            
            // 
            // panelPrincipal
            // 
            this.panelPrincipal.Controls.Add(this.panelFormulario);
            this.panelPrincipal.Controls.Add(this.panelEncabezado);
            this.panelPrincipal.Dock = DockStyle.Fill;
            this.panelPrincipal.Location = new Point(0, 0);
            this.panelPrincipal.Name = "panelPrincipal";
            this.panelPrincipal.Size = new Size(600, 550);
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
            this.lblTitulo.Size = new Size(220, 32);
            this.lblTitulo.TabIndex = 1;
            this.lblTitulo.Text = "Apartado de Agregación";
            
            // 
            // btnRegresar
            // 
            this.btnRegresar.BackColor = Color.FromArgb(231, 76, 60);
            this.btnRegresar.FlatAppearance.BorderSize = 0;
            this.btnRegresar.FlatStyle = FlatStyle.Flat;
            this.btnRegresar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnRegresar.ForeColor = Color.White;
            this.btnRegresar.Location = new Point(520, 20);
            this.btnRegresar.Name = "btnRegresar";
            this.btnRegresar.Size = new Size(70, 35);
            this.btnRegresar.TabIndex = 2;
            this.btnRegresar.Text = "← Regresar";
            this.btnRegresar.UseVisualStyleBackColor = false;
            this.btnRegresar.Cursor = Cursors.Hand;
            
            // 
            // panelFormulario
            // 
            this.panelFormulario.Controls.Add(this.lblDropdownTipo);
            this.panelFormulario.Controls.Add(this.cmbTipoEntidad);
            this.panelFormulario.Controls.Add(this.lblNombreUsuario);
            this.panelFormulario.Controls.Add(this.txtNombreUsuario);
            this.panelFormulario.Controls.Add(this.lblContrasena);
            this.panelFormulario.Controls.Add(this.txtContrasena);
            this.panelFormulario.Controls.Add(this.lblCorreo);
            this.panelFormulario.Controls.Add(this.txtCorreo);
            this.panelFormulario.Controls.Add(this.btnGuardar);
            this.panelFormulario.Controls.Add(this.lblMensajeExito);
            this.panelFormulario.Dock = DockStyle.Fill;
            this.panelFormulario.Location = new Point(0, 80);
            this.panelFormulario.Name = "panelFormulario";
            this.panelFormulario.Size = new Size(600, 470);
            this.panelFormulario.TabIndex = 1;
            this.panelFormulario.Padding = new Padding(40, 30, 40, 30);
            
            // 
            // lblDropdownTipo
            // 
            this.lblDropdownTipo.AutoSize = true;
            this.lblDropdownTipo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblDropdownTipo.ForeColor = Color.FromArgb(44, 62, 80);
            this.lblDropdownTipo.Location = new Point(40, 30);
            this.lblDropdownTipo.Name = "lblDropdownTipo";
            this.lblDropdownTipo.Size = new Size(132, 21);
            this.lblDropdownTipo.TabIndex = 0;
            this.lblDropdownTipo.Text = "Tipo de Entidad:";
            
            // 
            // cmbTipoEntidad
            // 
            this.cmbTipoEntidad.Font = new Font("Segoe UI", 11F);
            this.cmbTipoEntidad.Location = new Point(40, 55);
            this.cmbTipoEntidad.Name = "cmbTipoEntidad";
            this.cmbTipoEntidad.Size = new Size(520, 28);
            this.cmbTipoEntidad.TabIndex = 1;
            this.cmbTipoEntidad.BackColor = Color.White;
            this.cmbTipoEntidad.ForeColor = Color.FromArgb(44, 62, 80);
            
            // 
            // lblNombreUsuario
            // 
            this.lblNombreUsuario.AutoSize = true;
            this.lblNombreUsuario.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblNombreUsuario.ForeColor = Color.FromArgb(44, 62, 80);
            this.lblNombreUsuario.Location = new Point(40, 110);
            this.lblNombreUsuario.Name = "lblNombreUsuario";
            this.lblNombreUsuario.Size = new Size(145, 21);
            this.lblNombreUsuario.TabIndex = 2;
            this.lblNombreUsuario.Text = "Nombre de Usuario:";
            
            // 
            // txtNombreUsuario
            // 
            this.txtNombreUsuario.Font = new Font("Segoe UI", 11F);
            this.txtNombreUsuario.Location = new Point(40, 135);
            this.txtNombreUsuario.Name = "txtNombreUsuario";
            this.txtNombreUsuario.Size = new Size(520, 27);
            this.txtNombreUsuario.TabIndex = 3;
            this.txtNombreUsuario.BorderStyle = BorderStyle.FixedSingle;
            
            // 
            // lblContrasena
            // 
            this.lblContrasena.AutoSize = true;
            this.lblContrasena.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblContrasena.ForeColor = Color.FromArgb(44, 62, 80);
            this.lblContrasena.Location = new Point(40, 180);
            this.lblContrasena.Name = "lblContrasena";
            this.lblContrasena.Size = new Size(92, 21);
            this.lblContrasena.TabIndex = 4;
            this.lblContrasena.Text = "Contraseña:";
            
            // 
            // txtContrasena
            // 
            this.txtContrasena.Font = new Font("Segoe UI", 11F);
            this.txtContrasena.Location = new Point(40, 205);
            this.txtContrasena.Name = "txtContrasena";
            this.txtContrasena.Size = new Size(520, 27);
            this.txtContrasena.TabIndex = 5;
            this.txtContrasena.BorderStyle = BorderStyle.FixedSingle;
            // UseSystemPasswordChar se configurará dinámicamente en el código
            
            // 
            // lblCorreo
            // 
            this.lblCorreo.AutoSize = true;
            this.lblCorreo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblCorreo.ForeColor = Color.FromArgb(44, 62, 80);
            this.lblCorreo.Location = new Point(40, 250);
            this.lblCorreo.Name = "lblCorreo";
            this.lblCorreo.Size = new Size(136, 21);
            this.lblCorreo.TabIndex = 6;
            this.lblCorreo.Text = "Correo Electrónico:";
            
            // 
            // txtCorreo
            // 
            this.txtCorreo.Font = new Font("Segoe UI", 11F);
            this.txtCorreo.Location = new Point(40, 275);
            this.txtCorreo.Name = "txtCorreo";
            this.txtCorreo.Size = new Size(520, 27);
            this.txtCorreo.TabIndex = 7;
            this.txtCorreo.BorderStyle = BorderStyle.FixedSingle;
            
            // 
            // btnGuardar
            // 
            this.btnGuardar.BackColor = Color.FromArgb(39, 174, 96);
            this.btnGuardar.FlatAppearance.BorderSize = 0;
            this.btnGuardar.FlatStyle = FlatStyle.Flat;
            this.btnGuardar.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.btnGuardar.ForeColor = Color.White;
            this.btnGuardar.Location = new Point(250, 330);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new Size(100, 40);
            this.btnGuardar.TabIndex = 8;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Cursor = Cursors.Hand;
            
            // 
            // lblMensajeExito
            // 
            this.lblMensajeExito.AutoSize = true;
            this.lblMensajeExito.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.lblMensajeExito.ForeColor = Color.FromArgb(39, 174, 96);
            this.lblMensajeExito.Location = new Point(200, 390);
            this.lblMensajeExito.Name = "lblMensajeExito";
            this.lblMensajeExito.Size = new Size(200, 20);
            this.lblMensajeExito.TabIndex = 9;
            this.lblMensajeExito.Text = "Usuario agregado correctamente";
            this.lblMensajeExito.Visible = false;
            
            // 
            // AgregarUsuarioForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(600, 550);
            this.Controls.Add(this.panelPrincipal);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AgregarUsuarioForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Samara Rentals - Agregar Usuario";
            this.BackColor = Color.FromArgb(245, 245, 245);
            
            this.panelPrincipal.ResumeLayout(false);
            this.panelFormulario.ResumeLayout(false);
            this.panelFormulario.PerformLayout();
            this.panelEncabezado.ResumeLayout(false);
            this.panelEncabezado.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private Panel panelPrincipal;
        private Panel panelFormulario;
        private Panel panelEncabezado;
        private PictureBox pictureBoxLogo;
        private Label lblTitulo;
        private Button btnRegresar;
        private Label lblDropdownTipo;
        private ComboBox cmbTipoEntidad;
        private Label lblNombreUsuario;
        private TextBox txtNombreUsuario;
        private Label lblContrasena;
        private TextBox txtContrasena;
        private Label lblCorreo;
        private TextBox txtCorreo;
        private Button btnGuardar;
        private Label lblMensajeExito;
    }
}
