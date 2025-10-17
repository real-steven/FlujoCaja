namespace FlujoDeCajaApp.Formularios
{
    partial class AgregarCasaForm
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
            this.panelFormulario = new Panel();
            this.lblNombre = new Label();
            this.txtNombre = new TextBox();
            this.lblDueno = new Label();
            this.cmbDueno = new ComboBox();
            this.lblCategoria = new Label();
            this.cmbCategoria = new ComboBox();
            this.lblMoneda = new Label();
            this.cmbMoneda = new ComboBox();
            this.lblFoto = new Label();
            this.panelFoto = new Panel();
            this.pictureBoxFoto = new PictureBox();
            this.btnSeleccionarFoto = new Button();
            this.btnEliminarFoto = new Button();
            this.panelBotones = new Panel();
            this.btnGuardar = new Button();
            this.btnCancelar = new Button();
            this.openFileDialog = new OpenFileDialog();
            
            this.panelPrincipal.SuspendLayout();
            this.panelEncabezado.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.panelFormulario.SuspendLayout();
            this.panelFoto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFoto)).BeginInit();
            this.panelBotones.SuspendLayout();
            this.SuspendLayout();
            
            // 
            // panelPrincipal
            // 
            this.panelPrincipal.Controls.Add(this.panelFormulario);
            this.panelPrincipal.Controls.Add(this.panelBotones);
            this.panelPrincipal.Controls.Add(this.panelEncabezado);
            this.panelPrincipal.Dock = DockStyle.Fill;
            this.panelPrincipal.Location = new Point(0, 0);
            this.panelPrincipal.Name = "panelPrincipal";
            this.panelPrincipal.Size = new Size(600, 770);
            this.panelPrincipal.TabIndex = 0;
            this.panelPrincipal.BackColor = Color.FromArgb(245, 245, 245);
            
            // 
            // panelEncabezado
            // 
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
            this.lblTitulo.Size = new Size(155, 32);
            this.lblTitulo.TabIndex = 1;
            this.lblTitulo.Text = "Agregar Casa";
            
            // 
            // panelFormulario
            // 
            this.panelFormulario.Controls.Add(this.lblNombre);
            this.panelFormulario.Controls.Add(this.txtNombre);
            this.panelFormulario.Controls.Add(this.lblDueno);
            this.panelFormulario.Controls.Add(this.cmbDueno);
            this.panelFormulario.Controls.Add(this.lblCategoria);
            this.panelFormulario.Controls.Add(this.cmbCategoria);
            this.panelFormulario.Controls.Add(this.lblMoneda);
            this.panelFormulario.Controls.Add(this.cmbMoneda);
            this.panelFormulario.Controls.Add(this.lblFoto);
            this.panelFormulario.Controls.Add(this.panelFoto);
            this.panelFormulario.Dock = DockStyle.Fill;
            this.panelFormulario.Location = new Point(0, 80);
            this.panelFormulario.Name = "panelFormulario";
            this.panelFormulario.Size = new Size(600, 620);
            this.panelFormulario.TabIndex = 1;
            this.panelFormulario.Padding = new Padding(30);
            
            // 
            // lblNombre
            // 
            this.lblNombre.AutoSize = true;
            this.lblNombre.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblNombre.ForeColor = Color.FromArgb(52, 73, 94);
            this.lblNombre.Location = new Point(30, 30);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new Size(116, 19);
            this.lblNombre.TabIndex = 0;
            this.lblNombre.Text = "Nombre de Casa:";
            
            // 
            // txtNombre
            // 
            this.txtNombre.Font = new Font("Segoe UI", 10F);
            this.txtNombre.Location = new Point(30, 55);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new Size(540, 25);
            this.txtNombre.TabIndex = 1;
            this.txtNombre.BorderStyle = BorderStyle.FixedSingle;
            
            // 
            // lblDueno
            // 
            this.lblDueno.AutoSize = true;
            this.lblDueno.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblDueno.ForeColor = Color.FromArgb(52, 73, 94);
            this.lblDueno.Location = new Point(30, 100);
            this.lblDueno.Name = "lblDueno";
            this.lblDueno.Size = new Size(52, 19);
            this.lblDueno.TabIndex = 2;
            this.lblDueno.Text = "Dueño:";
            
            // 
            // cmbDueno
            // 
            this.cmbDueno.Font = new Font("Segoe UI", 10F);
            this.cmbDueno.Location = new Point(30, 125);
            this.cmbDueno.Name = "cmbDueno";
            this.cmbDueno.Size = new Size(540, 25);
            this.cmbDueno.TabIndex = 3;
            this.cmbDueno.DropDownStyle = ComboBoxStyle.DropDown;
            this.cmbDueno.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            this.cmbDueno.AutoCompleteSource = AutoCompleteSource.ListItems;
            
            // 
            // lblCategoria
            // 
            this.lblCategoria.AutoSize = true;
            this.lblCategoria.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblCategoria.ForeColor = Color.FromArgb(52, 73, 94);
            this.lblCategoria.Location = new Point(30, 170);
            this.lblCategoria.Name = "lblCategoria";
            this.lblCategoria.Size = new Size(74, 19);
            this.lblCategoria.TabIndex = 4;
            this.lblCategoria.Text = "Categoría:";
            
            // 
            // cmbCategoria
            // 
            this.cmbCategoria.Font = new Font("Segoe UI", 10F);
            this.cmbCategoria.Location = new Point(30, 195);
            this.cmbCategoria.Name = "cmbCategoria";
            this.cmbCategoria.Size = new Size(540, 25);
            this.cmbCategoria.TabIndex = 5;
            this.cmbCategoria.DropDownStyle = ComboBoxStyle.DropDown;
            this.cmbCategoria.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            this.cmbCategoria.AutoCompleteSource = AutoCompleteSource.ListItems;
            
            // 
            // lblMoneda
            // 
            this.lblMoneda.AutoSize = true;
            this.lblMoneda.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblMoneda.ForeColor = Color.FromArgb(52, 73, 94);
            this.lblMoneda.Location = new Point(30, 240);
            this.lblMoneda.Name = "lblMoneda";
            this.lblMoneda.Size = new Size(65, 19);
            this.lblMoneda.TabIndex = 6;
            this.lblMoneda.Text = "Moneda:";
            
            // 
            // cmbMoneda
            // 
            this.cmbMoneda.Font = new Font("Segoe UI", 10F);
            this.cmbMoneda.Location = new Point(30, 265);
            this.cmbMoneda.Name = "cmbMoneda";
            this.cmbMoneda.Size = new Size(540, 25);
            this.cmbMoneda.TabIndex = 7;
            this.cmbMoneda.DropDownStyle = ComboBoxStyle.DropDownList;
            
            // 
            // lblFoto
            // 
            this.lblFoto.AutoSize = true;
            this.lblFoto.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblFoto.ForeColor = Color.FromArgb(52, 73, 94);
            this.lblFoto.Location = new Point(30, 310);
            this.lblFoto.Name = "lblFoto";
            this.lblFoto.Size = new Size(40, 19);
            this.lblFoto.TabIndex = 8;
            this.lblFoto.Text = "Foto:";
            
            // 
            // panelFoto
            // 
            this.panelFoto.Controls.Add(this.pictureBoxFoto);
            this.panelFoto.Controls.Add(this.btnSeleccionarFoto);
            this.panelFoto.Controls.Add(this.btnEliminarFoto);
            this.panelFoto.Location = new Point(30, 335);
            this.panelFoto.Name = "panelFoto";
            this.panelFoto.Size = new Size(540, 250);
            this.panelFoto.TabIndex = 9;
            this.panelFoto.BorderStyle = BorderStyle.FixedSingle;
            this.panelFoto.BackColor = Color.White;
            
            // 
            // pictureBoxFoto
            // 
            this.pictureBoxFoto.Location = new Point(10, 10);
            this.pictureBoxFoto.Name = "pictureBoxFoto";
            this.pictureBoxFoto.Size = new Size(520, 190);
            this.pictureBoxFoto.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBoxFoto.TabIndex = 0;
            this.pictureBoxFoto.TabStop = false;
            this.pictureBoxFoto.BackColor = Color.FromArgb(240, 240, 240);
            this.pictureBoxFoto.BorderStyle = BorderStyle.FixedSingle;
            
            // 
            // btnSeleccionarFoto
            // 
            this.btnSeleccionarFoto.BackColor = Color.FromArgb(52, 152, 219);
            this.btnSeleccionarFoto.FlatStyle = FlatStyle.Flat;
            this.btnSeleccionarFoto.FlatAppearance.BorderSize = 0;
            this.btnSeleccionarFoto.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnSeleccionarFoto.ForeColor = Color.White;
            this.btnSeleccionarFoto.Location = new Point(10, 210);
            this.btnSeleccionarFoto.Name = "btnSeleccionarFoto";
            this.btnSeleccionarFoto.Size = new Size(130, 30);
            this.btnSeleccionarFoto.TabIndex = 1;
            this.btnSeleccionarFoto.Text = "Seleccionar Foto";
            this.btnSeleccionarFoto.UseVisualStyleBackColor = false;
            
            // 
            // btnEliminarFoto
            // 
            this.btnEliminarFoto.BackColor = Color.FromArgb(231, 76, 60);
            this.btnEliminarFoto.FlatStyle = FlatStyle.Flat;
            this.btnEliminarFoto.FlatAppearance.BorderSize = 0;
            this.btnEliminarFoto.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnEliminarFoto.ForeColor = Color.White;
            this.btnEliminarFoto.Location = new Point(150, 210);
            this.btnEliminarFoto.Name = "btnEliminarFoto";
            this.btnEliminarFoto.Size = new Size(110, 30);
            this.btnEliminarFoto.TabIndex = 2;
            this.btnEliminarFoto.Text = "Eliminar Foto";
            this.btnEliminarFoto.UseVisualStyleBackColor = false;
            this.btnEliminarFoto.Enabled = false;
            
            // 
            // panelBotones
            // 
            this.panelBotones.Controls.Add(this.btnCancelar);
            this.panelBotones.Controls.Add(this.btnGuardar);
            this.panelBotones.Dock = DockStyle.Bottom;
            this.panelBotones.Location = new Point(0, 700);
            this.panelBotones.Name = "panelBotones";
            this.panelBotones.Size = new Size(600, 70);
            this.panelBotones.TabIndex = 2;
            this.panelBotones.BackColor = Color.FromArgb(236, 240, 241);
            this.panelBotones.Padding = new Padding(20);
            
            // 
            // btnGuardar
            // 
            this.btnGuardar.BackColor = Color.FromArgb(46, 204, 113);
            this.btnGuardar.FlatStyle = FlatStyle.Flat;
            this.btnGuardar.FlatAppearance.BorderSize = 0;
            this.btnGuardar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnGuardar.ForeColor = Color.White;
            this.btnGuardar.Location = new Point(350, 20);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new Size(100, 35);
            this.btnGuardar.TabIndex = 0;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = false;
            
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = Color.FromArgb(149, 165, 166);
            this.btnCancelar.FlatStyle = FlatStyle.Flat;
            this.btnCancelar.FlatAppearance.BorderSize = 0;
            this.btnCancelar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnCancelar.ForeColor = Color.White;
            this.btnCancelar.Location = new Point(470, 20);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new Size(100, 35);
            this.btnCancelar.TabIndex = 1;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Todos los archivos|*.*";
            this.openFileDialog.Title = "Seleccionar foto de la casa";
            
            // 
            // AgregarCasaForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(600, 770);
            this.Controls.Add(this.panelPrincipal);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AgregarCasaForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Samara Rentals - Agregar Casa";
            
            this.panelPrincipal.ResumeLayout(false);
            this.panelEncabezado.ResumeLayout(false);
            this.panelEncabezado.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.panelFormulario.ResumeLayout(false);
            this.panelFormulario.PerformLayout();
            this.panelFoto.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFoto)).EndInit();
            this.panelBotones.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Panel panelPrincipal;
        private Panel panelEncabezado;
        private Label lblTitulo;
        private PictureBox pictureBoxLogo;
        private Panel panelFormulario;
        private Label lblNombre;
        private TextBox txtNombre;
        private Label lblDueno;
        private ComboBox cmbDueno;
        private Label lblCategoria;
        private ComboBox cmbCategoria;
        private Label lblMoneda;
        private ComboBox cmbMoneda;
        private Label lblFoto;
        private Panel panelFoto;
        private PictureBox pictureBoxFoto;
        private Button btnSeleccionarFoto;
        private Button btnEliminarFoto;
        private Panel panelBotones;
        private Button btnGuardar;
        private Button btnCancelar;
        private OpenFileDialog openFileDialog;
    }
}
