using System.Drawing;
using System.Windows.Forms;

namespace FlujoDeCajaApp.Formularios
{
    partial class MovimientosCasaControl
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

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelPrincipal = new Panel();
            this.panelEncabezado = new Panel();
            this.lblTituloCasa = new Label();
            this.btnVolver = new Button();
            this.panelBotonesSuperior = new Panel();
            this.btnGenerar = new Button();
            this.btnExportar = new Button();
            this.btnExtraer = new Button();
            this.panelContenido = new Panel();
            this.panelTabla = new Panel();
            this.panelEncabezadoTabla = new Panel();
            this.lblTituloTabla = new Label();
            this.btnAgregarReglones = new Button();
            this.nudCantidadReglones = new NumericUpDown();
            this.lblAgregarReglones = new Label();
            this.dgvMovimientos = new DataGridView();
            this.panelLateral = new Panel();
            this.panelFiltros = new Panel();
            this.lblFiltros = new Label();
            this.lblMes = new Label();
            this.cmbMes = new ComboBox();
            this.lblAño = new Label();
            this.nudAño = new NumericUpDown();
            this.panelCalculos = new Panel();
            this.lblTituloCalculos = new Label();
            this.lblBalanceAnteriorTexto = new Label();
            this.lblBalanceAnterior = new Label();
            this.lblTotalGastosTexto = new Label();
            this.lblTotalGastos = new Label();
            this.lblTotalIngresosTexto = new Label();
            this.lblTotalIngresos = new Label();
            this.lblBalanceActualTexto = new Label();
            this.lblBalanceActual = new Label();

            this.panelPrincipal.SuspendLayout();
            this.panelEncabezado.SuspendLayout();
            this.panelBotonesSuperior.SuspendLayout();
            this.panelContenido.SuspendLayout();
            this.panelTabla.SuspendLayout();
            this.panelEncabezadoTabla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidadReglones)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMovimientos)).BeginInit();
            this.panelLateral.SuspendLayout();
            this.panelFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAño)).BeginInit();
            this.panelCalculos.SuspendLayout();
            this.SuspendLayout();

            // 
            // panelPrincipal
            // 
            this.panelPrincipal.Controls.Add(this.panelContenido);
            this.panelPrincipal.Controls.Add(this.panelBotonesSuperior);
            this.panelPrincipal.Controls.Add(this.panelEncabezado);
            this.panelPrincipal.Dock = DockStyle.Fill;
            this.panelPrincipal.Location = new Point(0, 0);
            this.panelPrincipal.Name = "panelPrincipal";
            this.panelPrincipal.Size = new Size(1200, 800);
            this.panelPrincipal.TabIndex = 0;
            this.panelPrincipal.BackColor = Color.FromArgb(44, 62, 80);

            // 
            // panelEncabezado
            // 
            this.panelEncabezado.Controls.Add(this.btnVolver);
            this.panelEncabezado.Controls.Add(this.lblTituloCasa);
            this.panelEncabezado.Dock = DockStyle.Top;
            this.panelEncabezado.Location = new Point(0, 0);
            this.panelEncabezado.Name = "panelEncabezado";
            this.panelEncabezado.Size = new Size(1200, 60);
            this.panelEncabezado.TabIndex = 0;
            this.panelEncabezado.BackColor = Color.FromArgb(41, 128, 185);

            // 
            // lblTituloCasa
            // 
            this.lblTituloCasa.AutoSize = true;
            this.lblTituloCasa.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.lblTituloCasa.ForeColor = Color.White;
            this.lblTituloCasa.Location = new Point(20, 15);
            this.lblTituloCasa.Name = "lblTituloCasa";
            this.lblTituloCasa.Size = new Size(200, 32);
            this.lblTituloCasa.TabIndex = 0;
            this.lblTituloCasa.Text = "Movimientos - Casa";

            // 
            // btnVolver
            // 
            this.btnVolver.BackColor = Color.FromArgb(231, 76, 60);
            this.btnVolver.FlatAppearance.BorderSize = 0;
            this.btnVolver.FlatStyle = FlatStyle.Flat;
            this.btnVolver.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnVolver.ForeColor = Color.White;
            this.btnVolver.Location = new Point(1100, 12);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new Size(80, 35);
            this.btnVolver.TabIndex = 1;
            this.btnVolver.Text = "← Volver";
            this.btnVolver.UseVisualStyleBackColor = false;
            this.btnVolver.Cursor = Cursors.Hand;

            // 
            // panelBotonesSuperior
            // 
            this.panelBotonesSuperior.Controls.Add(this.btnExtraer);
            this.panelBotonesSuperior.Controls.Add(this.btnExportar);
            this.panelBotonesSuperior.Controls.Add(this.btnGenerar);
            this.panelBotonesSuperior.Dock = DockStyle.Top;
            this.panelBotonesSuperior.Location = new Point(0, 60);
            this.panelBotonesSuperior.Name = "panelBotonesSuperior";
            this.panelBotonesSuperior.Size = new Size(1200, 50);
            this.panelBotonesSuperior.TabIndex = 1;
            this.panelBotonesSuperior.BackColor = Color.FromArgb(52, 73, 94);
            this.panelBotonesSuperior.Padding = new Padding(20, 10, 20, 10);

            // 
            // btnGenerar
            // 
            this.btnGenerar.BackColor = Color.FromArgb(52, 152, 219);
            this.btnGenerar.FlatAppearance.BorderSize = 0;
            this.btnGenerar.FlatStyle = FlatStyle.Flat;
            this.btnGenerar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnGenerar.ForeColor = Color.White;
            this.btnGenerar.Location = new Point(880, 10);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new Size(100, 30);
            this.btnGenerar.TabIndex = 0;
            this.btnGenerar.Text = "Datos Prueba";
            this.btnGenerar.UseVisualStyleBackColor = false;
            this.btnGenerar.Cursor = Cursors.Hand;

            // 
            // btnExportar
            // 
            this.btnExportar.BackColor = Color.FromArgb(39, 174, 96);
            this.btnExportar.FlatAppearance.BorderSize = 0;
            this.btnExportar.FlatStyle = FlatStyle.Flat;
            this.btnExportar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnExportar.ForeColor = Color.White;
            this.btnExportar.Location = new Point(990, 10);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new Size(80, 30);
            this.btnExportar.TabIndex = 1;
            this.btnExportar.Text = "Exportar";
            this.btnExportar.UseVisualStyleBackColor = false;
            this.btnExportar.Cursor = Cursors.Hand;

            // 
            // btnExtraer
            // 
            this.btnExtraer.BackColor = Color.FromArgb(155, 89, 182);
            this.btnExtraer.FlatAppearance.BorderSize = 0;
            this.btnExtraer.FlatStyle = FlatStyle.Flat;
            this.btnExtraer.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnExtraer.ForeColor = Color.White;
            this.btnExtraer.Location = new Point(1080, 10);
            this.btnExtraer.Name = "btnExtraer";
            this.btnExtraer.Size = new Size(80, 30);
            this.btnExtraer.TabIndex = 2;
            this.btnExtraer.Text = "Extraer";
            this.btnExtraer.UseVisualStyleBackColor = false;
            this.btnExtraer.Cursor = Cursors.Hand;

            // 
            // panelContenido
            // 
            this.panelContenido.Controls.Add(this.panelTabla);
            this.panelContenido.Controls.Add(this.panelLateral);
            this.panelContenido.Dock = DockStyle.Fill;
            this.panelContenido.Location = new Point(0, 110);
            this.panelContenido.Name = "panelContenido";
            this.panelContenido.Size = new Size(1200, 690);
            this.panelContenido.TabIndex = 2;
            this.panelContenido.BackColor = Color.FromArgb(44, 62, 80);
            this.panelContenido.Padding = new Padding(20);

            // 
            // panelTabla
            // 
            this.panelTabla.Controls.Add(this.dgvMovimientos);
            this.panelTabla.Controls.Add(this.panelEncabezadoTabla);
            this.panelTabla.Dock = DockStyle.Fill;
            this.panelTabla.Location = new Point(20, 20);
            this.panelTabla.Name = "panelTabla";
            this.panelTabla.Size = new Size(860, 650);
            this.panelTabla.TabIndex = 0;
            this.panelTabla.BackColor = Color.White;
            this.panelTabla.BorderStyle = BorderStyle.FixedSingle;

            // 
            // panelEncabezadoTabla
            // 
            this.panelEncabezadoTabla.Controls.Add(this.lblAgregarReglones);
            this.panelEncabezadoTabla.Controls.Add(this.nudCantidadReglones);
            this.panelEncabezadoTabla.Controls.Add(this.btnAgregarReglones);
            this.panelEncabezadoTabla.Controls.Add(this.lblTituloTabla);
            this.panelEncabezadoTabla.Dock = DockStyle.Top;
            this.panelEncabezadoTabla.Location = new Point(0, 0);
            this.panelEncabezadoTabla.Name = "panelEncabezadoTabla";
            this.panelEncabezadoTabla.Size = new Size(858, 50);
            this.panelEncabezadoTabla.TabIndex = 0;
            this.panelEncabezadoTabla.BackColor = Color.FromArgb(52, 73, 94);
            this.panelEncabezadoTabla.BorderStyle = BorderStyle.FixedSingle;

            // 
            // lblTituloTabla
            // 
            this.lblTituloTabla.AutoSize = true;
            this.lblTituloTabla.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblTituloTabla.ForeColor = Color.White;
            this.lblTituloTabla.Location = new Point(15, 15);
            this.lblTituloTabla.Name = "lblTituloTabla";
            this.lblTituloTabla.Size = new Size(98, 21);
            this.lblTituloTabla.TabIndex = 0;
            this.lblTituloTabla.Text = "Movimientos";

            // 
            // btnAgregarReglones
            // 
            this.btnAgregarReglones.BackColor = Color.FromArgb(52, 152, 219);
            this.btnAgregarReglones.FlatAppearance.BorderSize = 0;
            this.btnAgregarReglones.FlatStyle = FlatStyle.Flat;
            this.btnAgregarReglones.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnAgregarReglones.ForeColor = Color.White;
            this.btnAgregarReglones.Location = new Point(750, 10);
            this.btnAgregarReglones.Name = "btnAgregarReglones";
            this.btnAgregarReglones.Size = new Size(30, 30);
            this.btnAgregarReglones.TabIndex = 2;
            this.btnAgregarReglones.Text = "↓";
            this.btnAgregarReglones.UseVisualStyleBackColor = false;
            this.btnAgregarReglones.Cursor = Cursors.Hand;

            // 
            // nudCantidadReglones
            // 
            this.nudCantidadReglones.Font = new Font("Segoe UI", 9F);
            this.nudCantidadReglones.Location = new Point(680, 15);
            this.nudCantidadReglones.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            this.nudCantidadReglones.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudCantidadReglones.Name = "nudCantidadReglones";
            this.nudCantidadReglones.Size = new Size(60, 23);
            this.nudCantidadReglones.TabIndex = 1;
            this.nudCantidadReglones.Value = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudCantidadReglones.TextAlign = HorizontalAlignment.Center;

            // 
            // lblAgregarReglones
            // 
            this.lblAgregarReglones.AutoSize = true;
            this.lblAgregarReglones.Font = new Font("Segoe UI", 9F);
            this.lblAgregarReglones.ForeColor = Color.White;
            this.lblAgregarReglones.Location = new Point(550, 17);
            this.lblAgregarReglones.Name = "lblAgregarReglones";
            this.lblAgregarReglones.Size = new Size(125, 15);
            this.lblAgregarReglones.TabIndex = 3;
            this.lblAgregarReglones.Text = "Agregar reglones:";

            // 
            // dgvMovimientos
            // 
            this.dgvMovimientos.BackgroundColor = Color.White;
            this.dgvMovimientos.BorderStyle = BorderStyle.None;
            this.dgvMovimientos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMovimientos.Dock = DockStyle.Fill;
            this.dgvMovimientos.Location = new Point(0, 50);
            this.dgvMovimientos.Name = "dgvMovimientos";
            this.dgvMovimientos.RowTemplate.Height = 25;
            this.dgvMovimientos.Size = new Size(858, 598);
            this.dgvMovimientos.TabIndex = 1;
            this.dgvMovimientos.GridColor = Color.FromArgb(220, 220, 220);

            // 
            // panelLateral
            // 
            this.panelLateral.Controls.Add(this.panelCalculos);
            this.panelLateral.Controls.Add(this.panelFiltros);
            this.panelLateral.Dock = DockStyle.Right;
            this.panelLateral.Location = new Point(880, 20);
            this.panelLateral.Name = "panelLateral";
            this.panelLateral.Size = new Size(300, 650);
            this.panelLateral.TabIndex = 1;

            // 
            // panelFiltros
            // 
            this.panelFiltros.Controls.Add(this.nudAño);
            this.panelFiltros.Controls.Add(this.lblAño);
            this.panelFiltros.Controls.Add(this.cmbMes);
            this.panelFiltros.Controls.Add(this.lblMes);
            this.panelFiltros.Controls.Add(this.lblFiltros);
            this.panelFiltros.Dock = DockStyle.Top;
            this.panelFiltros.Location = new Point(0, 0);
            this.panelFiltros.Name = "panelFiltros";
            this.panelFiltros.Size = new Size(300, 180);
            this.panelFiltros.TabIndex = 0;
            this.panelFiltros.BackColor = Color.FromArgb(52, 73, 94);
            this.panelFiltros.BorderStyle = BorderStyle.FixedSingle;
            this.panelFiltros.Padding = new Padding(15);

            // 
            // lblFiltros
            // 
            this.lblFiltros.AutoSize = true;
            this.lblFiltros.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblFiltros.ForeColor = Color.White;
            this.lblFiltros.Location = new Point(15, 15);
            this.lblFiltros.Name = "lblFiltros";
            this.lblFiltros.Size = new Size(58, 21);
            this.lblFiltros.TabIndex = 0;
            this.lblFiltros.Text = "Filtros";

            // 
            // lblMes
            // 
            this.lblMes.AutoSize = true;
            this.lblMes.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblMes.ForeColor = Color.White;
            this.lblMes.Location = new Point(15, 50);
            this.lblMes.Name = "lblMes";
            this.lblMes.Size = new Size(36, 19);
            this.lblMes.TabIndex = 1;
            this.lblMes.Text = "Mes:";

            // 
            // cmbMes
            // 
            this.cmbMes.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbMes.Font = new Font("Segoe UI", 10F);
            this.cmbMes.FormattingEnabled = true;
            this.cmbMes.Location = new Point(15, 75);
            this.cmbMes.Name = "cmbMes";
            this.cmbMes.Size = new Size(250, 25);
            this.cmbMes.TabIndex = 2;

            // 
            // lblAño
            // 
            this.lblAño.AutoSize = true;
            this.lblAño.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblAño.ForeColor = Color.White;
            this.lblAño.Location = new Point(15, 115);
            this.lblAño.Name = "lblAño";
            this.lblAño.Size = new Size(36, 19);
            this.lblAño.TabIndex = 3;
            this.lblAño.Text = "Año:";

            // 
            // nudAño
            // 
            this.nudAño.Font = new Font("Segoe UI", 10F);
            this.nudAño.Location = new Point(15, 140);
            this.nudAño.Maximum = new decimal(new int[] { 2030, 0, 0, 0 });
            this.nudAño.Minimum = new decimal(new int[] { 2020, 0, 0, 0 });
            this.nudAño.Name = "nudAño";
            this.nudAño.Size = new Size(250, 25);
            this.nudAño.TabIndex = 4;
            this.nudAño.Value = new decimal(new int[] { 2025, 0, 0, 0 });

            // 
            // panelCalculos
            // 
            this.panelCalculos.Controls.Add(this.lblBalanceActual);
            this.panelCalculos.Controls.Add(this.lblBalanceActualTexto);
            this.panelCalculos.Controls.Add(this.lblTotalIngresos);
            this.panelCalculos.Controls.Add(this.lblTotalIngresosTexto);
            this.panelCalculos.Controls.Add(this.lblTotalGastos);
            this.panelCalculos.Controls.Add(this.lblTotalGastosTexto);
            this.panelCalculos.Controls.Add(this.lblBalanceAnterior);
            this.panelCalculos.Controls.Add(this.lblBalanceAnteriorTexto);
            this.panelCalculos.Controls.Add(this.lblTituloCalculos);
            this.panelCalculos.Dock = DockStyle.Fill;
            this.panelCalculos.Location = new Point(0, 180);
            this.panelCalculos.Name = "panelCalculos";
            this.panelCalculos.Size = new Size(300, 470);
            this.panelCalculos.TabIndex = 1;
            this.panelCalculos.BackColor = Color.FromArgb(52, 73, 94);
            this.panelCalculos.BorderStyle = BorderStyle.FixedSingle;
            this.panelCalculos.Padding = new Padding(15);

            // 
            // lblTituloCalculos
            // 
            this.lblTituloCalculos.AutoSize = true;
            this.lblTituloCalculos.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblTituloCalculos.ForeColor = Color.White;
            this.lblTituloCalculos.Location = new Point(15, 15);
            this.lblTituloCalculos.Name = "lblTituloCalculos";
            this.lblTituloCalculos.Size = new Size(77, 21);
            this.lblTituloCalculos.TabIndex = 0;
            this.lblTituloCalculos.Text = "Resumen";

            // 
            // lblBalanceAnteriorTexto
            // 
            this.lblBalanceAnteriorTexto.AutoSize = true;
            this.lblBalanceAnteriorTexto.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblBalanceAnteriorTexto.ForeColor = Color.White;
            this.lblBalanceAnteriorTexto.Location = new Point(15, 60);
            this.lblBalanceAnteriorTexto.Name = "lblBalanceAnteriorTexto";
            this.lblBalanceAnteriorTexto.Size = new Size(121, 19);
            this.lblBalanceAnteriorTexto.TabIndex = 1;
            this.lblBalanceAnteriorTexto.Text = "Balance Anterior:";

            // 
            // lblBalanceAnterior
            // 
            this.lblBalanceAnterior.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.lblBalanceAnterior.ForeColor = Color.White;
            this.lblBalanceAnterior.Location = new Point(15, 85);
            this.lblBalanceAnterior.Name = "lblBalanceAnterior";
            this.lblBalanceAnterior.Size = new Size(250, 20);
            this.lblBalanceAnterior.TabIndex = 2;
            this.lblBalanceAnterior.Text = "₡0.00";
            this.lblBalanceAnterior.TextAlign = ContentAlignment.MiddleRight;

            // 
            // lblTotalGastosTexto
            // 
            this.lblTotalGastosTexto.AutoSize = true;
            this.lblTotalGastosTexto.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblTotalGastosTexto.ForeColor = Color.White;
            this.lblTotalGastosTexto.Location = new Point(15, 125);
            this.lblTotalGastosTexto.Name = "lblTotalGastosTexto";
            this.lblTotalGastosTexto.Size = new Size(96, 19);
            this.lblTotalGastosTexto.TabIndex = 3;
            this.lblTotalGastosTexto.Text = "Total Gastos:";

            // 
            // lblTotalGastos
            // 
            this.lblTotalGastos.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.lblTotalGastos.ForeColor = Color.FromArgb(231, 76, 60);
            this.lblTotalGastos.Location = new Point(15, 150);
            this.lblTotalGastos.Name = "lblTotalGastos";
            this.lblTotalGastos.Size = new Size(250, 20);
            this.lblTotalGastos.TabIndex = 4;
            this.lblTotalGastos.Text = "₡0.00";
            this.lblTotalGastos.TextAlign = ContentAlignment.MiddleRight;

            // 
            // lblTotalIngresosTexto
            // 
            this.lblTotalIngresosTexto.AutoSize = true;
            this.lblTotalIngresosTexto.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblTotalIngresosTexto.ForeColor = Color.White;
            this.lblTotalIngresosTexto.Location = new Point(15, 190);
            this.lblTotalIngresosTexto.Name = "lblTotalIngresosTexto";
            this.lblTotalIngresosTexto.Size = new Size(106, 19);
            this.lblTotalIngresosTexto.TabIndex = 5;
            this.lblTotalIngresosTexto.Text = "Total Ingresos:";

            // 
            // lblTotalIngresos
            // 
            this.lblTotalIngresos.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.lblTotalIngresos.ForeColor = Color.FromArgb(39, 174, 96);
            this.lblTotalIngresos.Location = new Point(15, 215);
            this.lblTotalIngresos.Name = "lblTotalIngresos";
            this.lblTotalIngresos.Size = new Size(250, 20);
            this.lblTotalIngresos.TabIndex = 6;
            this.lblTotalIngresos.Text = "₡0.00";
            this.lblTotalIngresos.TextAlign = ContentAlignment.MiddleRight;

            // 
            // lblBalanceActualTexto
            // 
            this.lblBalanceActualTexto.AutoSize = true;
            this.lblBalanceActualTexto.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.lblBalanceActualTexto.ForeColor = Color.White;
            this.lblBalanceActualTexto.Location = new Point(15, 260);
            this.lblBalanceActualTexto.Name = "lblBalanceActualTexto";
            this.lblBalanceActualTexto.Size = new Size(118, 20);
            this.lblBalanceActualTexto.TabIndex = 7;
            this.lblBalanceActualTexto.Text = "Balance Actual:";

            // 
            // lblBalanceActual
            // 
            this.lblBalanceActual.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblBalanceActual.ForeColor = Color.White;
            this.lblBalanceActual.Location = new Point(15, 285);
            this.lblBalanceActual.Name = "lblBalanceActual";
            this.lblBalanceActual.Size = new Size(250, 22);
            this.lblBalanceActual.TabIndex = 8;
            this.lblBalanceActual.Text = "₡0.00";
            this.lblBalanceActual.TextAlign = ContentAlignment.MiddleRight;

            // 
            // MovimientosCasaControl
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(44, 62, 80);
            this.Controls.Add(this.panelPrincipal);
            this.Name = "MovimientosCasaControl";
            this.Size = new Size(1200, 800);

            this.panelPrincipal.ResumeLayout(false);
            this.panelEncabezado.ResumeLayout(false);
            this.panelEncabezado.PerformLayout();
            this.panelBotonesSuperior.ResumeLayout(false);
            this.panelContenido.ResumeLayout(false);
            this.panelTabla.ResumeLayout(false);
            this.panelEncabezadoTabla.ResumeLayout(false);
            this.panelEncabezadoTabla.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidadReglones)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMovimientos)).EndInit();
            this.panelLateral.ResumeLayout(false);
            this.panelFiltros.ResumeLayout(false);
            this.panelFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAño)).EndInit();
            this.panelCalculos.ResumeLayout(false);
            this.panelCalculos.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private Panel panelPrincipal;
        private Panel panelEncabezado;
        private Label lblTituloCasa;
        private Button btnVolver;
        private Panel panelBotonesSuperior;
        private Button btnGenerar;
        private Button btnExportar;
        private Button btnExtraer;
        private Panel panelContenido;
        private Panel panelTabla;
        private Panel panelEncabezadoTabla;
        private Label lblTituloTabla;
        private Button btnAgregarReglones;
        private NumericUpDown nudCantidadReglones;
        private Label lblAgregarReglones;
        private DataGridView dgvMovimientos;
        private Panel panelLateral;
        private Panel panelFiltros;
        private Label lblFiltros;
        private Label lblMes;
        private ComboBox cmbMes;
        private Label lblAño;
        private NumericUpDown nudAño;
        private Panel panelCalculos;
        private Label lblTituloCalculos;
        private Label lblBalanceAnteriorTexto;
        private Label lblBalanceAnterior;
        private Label lblTotalGastosTexto;
        private Label lblTotalGastos;
        private Label lblTotalIngresosTexto;
        private Label lblTotalIngresos;
        private Label lblBalanceActualTexto;
        private Label lblBalanceActual;
    }
}
