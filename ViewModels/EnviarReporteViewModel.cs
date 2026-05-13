using FlujoCajaWpf.Commands;
using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using FlujoCajaWpf.Services;
using FlujoCajaWpf.ViewModels.Base;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace FlujoCajaWpf.ViewModels
{
    /// <summary>
    /// Ítem seleccionable de correo en la lista de destinatarios.
    /// </summary>
    public class CorreoItem : ViewModelBase
    {
        private bool _seleccionado;
        public string Email { get; }
        public string Etiqueta { get; }
        public long? CorreoCasaId { get; }   // null = email principal de la casa

        public bool Seleccionado
        {
            get => _seleccionado;
            set => SetProperty(ref _seleccionado, value);
        }

        public CorreoItem(string email, string etiqueta, bool seleccionado = false, long? correoCasaId = null)
        {
            Email = email;
            Etiqueta = etiqueta;
            Seleccionado = seleccionado;
            CorreoCasaId = correoCasaId;
        }

        public override string ToString() => string.IsNullOrWhiteSpace(Etiqueta) ? Email : $"{Email} ({Etiqueta})";
    }

    public class EnviarReporteViewModel : ViewModelBase
    {
        // ── Datos de contexto ───────────────────────────────────────────────
        private readonly Casa _casa;
        private readonly string? _rutaPdfTabla;
        private readonly string? _rutaPdfImagenes;
        private readonly Action _cerrarDialogo;

        // ── Estado de archivos seleccionados ────────────────────────────────
        private bool _enviarPdfTabla;
        private bool _enviarPdfImagenes;

        public bool EnviarPdfTabla
        {
            get => _enviarPdfTabla;
            set
            {
                SetProperty(ref _enviarPdfTabla, value);
                ActualizarPreview();
                ActualizarComandos();
            }
        }

        public bool EnviarPdfImagenes
        {
            get => _enviarPdfImagenes;
            set
            {
                SetProperty(ref _enviarPdfImagenes, value);
                ActualizarPreview();
                ActualizarComandos();
            }
        }

        // Compatibilidad con XAML legacy (no usado)
        public bool EnviarPdf => EnviarPdfTabla || EnviarPdfImagenes;

        public bool HayPdfTabla => !string.IsNullOrEmpty(_rutaPdfTabla) && File.Exists(_rutaPdfTabla);
        public bool HayPdfImagenes => !string.IsNullOrEmpty(_rutaPdfImagenes) && File.Exists(_rutaPdfImagenes);
        public bool HayPdf => HayPdfTabla || HayPdfImagenes;

        // ── Correos ─────────────────────────────────────────────────────────
        public ObservableCollection<CorreoItem> Correos { get; } = new();

        private string _nuevoCorreo = string.Empty;
        public string NuevoCorreo
        {
            get => _nuevoCorreo;
            set
            {
                SetProperty(ref _nuevoCorreo, value);
                ActualizarComandos();
            }
        }

        private string _nuevoCorreoError = string.Empty;
        public string NuevoCorreoError
        {
            get => _nuevoCorreoError;
            set => SetProperty(ref _nuevoCorreoError, value);
        }

        private bool _guardarNuevoCorreo;
        public bool GuardarNuevoCorreo
        {
            get => _guardarNuevoCorreo;
            set => SetProperty(ref _guardarNuevoCorreo, value);
        }

        // ── Vista previa del correo ──────────────────────────────────────────
        private string _previewDestinatarios = string.Empty;
        public string PreviewDestinatarios
        {
            get => _previewDestinatarios;
            set => SetProperty(ref _previewDestinatarios, value);
        }

        private string _previewAsunto = string.Empty;
        public string PreviewAsunto
        {
            get => _previewAsunto;
            set => SetProperty(ref _previewAsunto, value);
        }

        private string _previewMensaje = string.Empty;
        public string PreviewMensaje
        {
            get => _previewMensaje;
            set => SetProperty(ref _previewMensaje, value);
        }

        // ── Estado general ───────────────────────────────────────────────────
        private bool _estaCargando;
        public bool EstaCargando
        {
            get => _estaCargando;
            set
            {
                SetProperty(ref _estaCargando, value);
                ActualizarComandos();
            }
        }

        private string _mensajeEstado = string.Empty;
        public string MensajeEstado
        {
            get => _mensajeEstado;
            set => SetProperty(ref _mensajeEstado, value);
        }

        // ── Resultado del envío (leído por el diálogo al cerrarse) ──────────
        public bool EnvioFueExitoso { get; private set; }
        public string DestinatariosEnviados { get; private set; } = string.Empty;

        // ── Delegado de confirmación (inyectado desde code-behind) ───────────
        /// <summary>
        /// Función que muestra un diálogo de confirmación. Retorna true si el usuario confirma.
        /// Si es null, se omite la confirmación (útil para tests).
        /// </summary>
        public Func<string, bool>? ConfirmarEnvio { get; set; }

        // ── Comandos ─────────────────────────────────────────────────────────
        public ICommand AgregarCorreoCommand { get; }
        public ICommand EnviarCommand { get; }
        public ICommand CancelarCommand { get; }
        public ICommand TogglePdfTablaCommand { get; }
        public ICommand TogglePdfImagenesCommand { get; }

        // ──────────────────────────────────────────────────────────────────────
        public EnviarReporteViewModel(Casa casa, string? rutaPdfTabla, string? rutaPdfImagenes, Action cerrarDialogo)
        {
            _casa = casa;
            _rutaPdfTabla = rutaPdfTabla;
            _rutaPdfImagenes = rutaPdfImagenes;
            _cerrarDialogo = cerrarDialogo;

            _enviarPdfTabla = false;
            _enviarPdfImagenes = false;

            AgregarCorreoCommand = new RelayCommand(
                async () => await AgregarCorreoAsync(),
                () => !string.IsNullOrWhiteSpace(NuevoCorreo) && !EstaCargando);

            EnviarCommand = new RelayCommand(
                async () => await EnviarAsync(),
                () => HayDestinatariosSeleccionados && HayArchivoSeleccionado && !EstaCargando);

            CancelarCommand = new RelayCommand(() => _cerrarDialogo());

            TogglePdfTablaCommand = new RelayCommand(
                () => { if (HayPdfTabla) EnviarPdfTabla = !EnviarPdfTabla; },
                () => HayPdfTabla);

            TogglePdfImagenesCommand = new RelayCommand(
                () => { if (HayPdfImagenes) EnviarPdfImagenes = !EnviarPdfImagenes; },
                () => HayPdfImagenes);

            _ = CargarCorreosAsync();
        }

        // ── Propiedades de validación ────────────────────────────────────────
        private bool HayDestinatariosSeleccionados => Correos.Any(c => c.Seleccionado);
        private bool HayArchivoSeleccionado => (EnviarPdfTabla && HayPdfTabla) || (EnviarPdfImagenes && HayPdfImagenes);

        // ── Carga inicial ────────────────────────────────────────────────────
        private async Task CargarCorreosAsync()
        {
            EstaCargando = true;

            // 1. Email del dueño de la casa (tabla duenos)
            if (_casa.DuenoId > 0)
            {
                var dueno = await SupabaseDuenoHelper.ObtenerDuenoPorIdAsync(_casa.DuenoId);
                if (dueno != null && !string.IsNullOrWhiteSpace(dueno.Email))
                {
                    Correos.Add(new CorreoItem(
                        email: dueno.Email,
                        etiqueta: $"Dueño — {dueno.NombreCompleto}",
                        seleccionado: true));
                }
            }

            // 2. Email principal de la casa (columna email_principal en casas)
            if (!string.IsNullOrWhiteSpace(_casa.EmailPrincipal))
            {
                if (!Correos.Any(c => string.Equals(c.Email, _casa.EmailPrincipal, StringComparison.OrdinalIgnoreCase)))
                {
                    Correos.Add(new CorreoItem(_casa.EmailPrincipal, "Principal", seleccionado: true));
                }
            }

            // 3. Correos adicionales guardados en tabla correos_casa
            var adicionales = await SupabaseCorreoCasaHelper.ObtenerCorreosPorCasaAsync(_casa.Id);
            foreach (var c in adicionales)
            {
                if (!Correos.Any(x => string.Equals(x.Email, c.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    Correos.Add(new CorreoItem(
                        email: c.Email,
                        etiqueta: c.Etiqueta ?? string.Empty,
                        seleccionado: false,
                        correoCasaId: c.Id));
                }
            }

            // Suscribir a cambios de selección para actualizar preview
            foreach (var item in Correos)
                item.PropertyChanged += (_, _) => { ActualizarPreview(); ActualizarComandos(); };

            ActualizarPreview();
            ActualizarComandos();
            EstaCargando = false;
        }

        // ── Agregar correo manual ─────────────────────────────────────────────
        private async Task AgregarCorreoAsync()
        {
            NuevoCorreoError = string.Empty;
            var email = NuevoCorreo.Trim();

            if (!EmailService.EsEmailValido(email))
            {
                NuevoCorreoError = "El correo no tiene un formato válido.";
                return;
            }

            if (Correos.Any(c => string.Equals(c.Email, email, StringComparison.OrdinalIgnoreCase)))
            {
                NuevoCorreoError = "Este correo ya está en la lista.";
                return;
            }

            long? correoId = null;

            if (GuardarNuevoCorreo)
            {
                EstaCargando = true;
                var (ok, guardado, error) = await SupabaseCorreoCasaHelper.InsertarCorreoAsync(_casa.Id, email);
                EstaCargando = false;

                if (!ok)
                {
                    NuevoCorreoError = error ?? "No se pudo guardar el correo.";
                    return;
                }
                correoId = guardado?.Id;
            }

            var item = new CorreoItem(email, GuardarNuevoCorreo ? "Guardado" : "Temporal",
                seleccionado: true, correoCasaId: correoId);
            item.PropertyChanged += (_, _) => { ActualizarPreview(); ActualizarComandos(); };
            Correos.Add(item);

            NuevoCorreo = string.Empty;
            GuardarNuevoCorreo = false;
            ActualizarPreview();
            ActualizarComandos();
        }

        // ── Actualizar vista previa ──────────────────────────────────────────
        private void ActualizarPreview()
        {
            var seleccionados = Correos.Where(c => c.Seleccionado).Select(c => c.Email).ToList();
            PreviewDestinatarios = seleccionados.Count > 0
                ? string.Join(", ", seleccionados)
                : "(sin destinatarios)";

            var archivos = new List<string>();
            if (EnviarPdfTabla && HayPdfTabla) archivos.Add("Tabla Movimientos");
            if (EnviarPdfImagenes && HayPdfImagenes) archivos.Add("Imágenes Facturas");
            var archivosStr = archivos.Count > 0 ? string.Join(" y ", archivos) : "ningún archivo";

            PreviewAsunto = $"Reporte mensual — {_casa.Nombre}";

            PreviewMensaje =
                $"Estimado/a,\n\n" +
                $"Se adjunta el reporte mensual de la propiedad \"{_casa.Nombre}\" en formato {archivosStr}.\n\n" +
                $"Si tiene preguntas, no dude en contactarnos.\n\n" +
                $"Saludos,\nSamara Rentals";
        }

        private void ActualizarComandos()
        {
            (AgregarCorreoCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (EnviarCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        // ── Envío ────────────────────────────────────────────────────────────
        private async Task EnviarAsync()
        {
            // Destinatarios seleccionados
            var destinatarios = Correos.Where(c => c.Seleccionado).Select(c => c.Email).ToList();

            // Mostrar confirmación si hay delegado
            if (ConfirmarEnvio != null)
            {
                var confirmar = ConfirmarEnvio(string.Join("\n", destinatarios));
                if (!confirmar) return;
            }

            EstaCargando = true;
            MensajeEstado = "Preparando envío...";

            // Leer config de Resend
            var resendCfg = await SupabaseHelper.ObtenerConfigResendAsync();
            if (resendCfg == null || string.IsNullOrWhiteSpace(resendCfg.ApiKey)
                || resendCfg.ApiKey == "RESEND_API_KEY_AQUI")
            {
                EstaCargando = false;
                MensajeEstado = "Error: Configura la API key de Resend en appsettings.json.";
                return;
            }

            // Armar adjuntos
            var attachments = new List<EmailAttachment>();
            if (EnviarPdfTabla && HayPdfTabla)
            {
                var att = EmailService.CrearAttachment(_rutaPdfTabla!);
                if (att != null) attachments.Add(att);
            }
            if (EnviarPdfImagenes && HayPdfImagenes)
            {
                var att = EmailService.CrearAttachment(_rutaPdfImagenes!);
                if (att != null) attachments.Add(att);
            }

            if (attachments.Count == 0)
            {
                EstaCargando = false;
                MensajeEstado = "Error: No se pudieron leer los archivos adjuntos.";
                return;
            }

            // Cuerpo HTML — usa el PreviewMensaje que el usuario pudo editar
            var htmlBody = BuildHtmlBody(_casa.Nombre, attachments.Select(a => Path.GetFileName(a.FilePath)).ToList(), PreviewMensaje);

            MensajeEstado = "Enviando correo...";
            var result = await EmailService.EnviarAsync(
                apiKey: resendCfg.ApiKey,
                from: resendCfg.From,
                to: destinatarios,
                subject: PreviewAsunto,
                htmlBody: htmlBody,
                attachments: attachments);

            EstaCargando = false;

            // Determinar adjuntos
            var partes = new List<string>();
            if (EnviarPdfTabla && HayPdfTabla) partes.Add("tabla");
            if (EnviarPdfImagenes && HayPdfImagenes) partes.Add("imagenes");
            var adjuntosStr = partes.Count > 0 ? string.Join("+", partes) : "ninguno";

            // Subir PDFs a Storage si el envío fue exitoso
            string? urlPdf = null;

            if (result.Success)
            {
                if (EnviarPdfTabla && HayPdfTabla && _rutaPdfTabla != null)
                {
                    var subida = await SupabaseStorageHelper.SubirReporteAsync(
                        _rutaPdfTabla, _casa.Nombre, Path.GetFileName(_rutaPdfTabla));
                    if (subida.Success) urlPdf = subida.Url;
                }
                if (EnviarPdfImagenes && HayPdfImagenes && _rutaPdfImagenes != null)
                {
                    var subida = await SupabaseStorageHelper.SubirReporteAsync(
                        _rutaPdfImagenes, _casa.Nombre, Path.GetFileName(_rutaPdfImagenes));
                    if (subida.Success && urlPdf == null) urlPdf = subida.Url;
                }
            }

            // 📋 REGISTRAR EN LOG DE CORREOS
            var user = SupabaseAuthHelper.GetCurrentUser();
            await SupabaseLogCorreoHelper.RegistrarEnvioAsync(
                usuarioEmail: user?.Email ?? "desconocido",
                casaNombre: _casa.Nombre,
                destinatarios: destinatarios,
                asunto: PreviewAsunto,
                adjuntos: adjuntosStr,
                estado: result.Success ? "enviado" : "error",
                mensajeId: result.Success ? result.MessageId : null,
                errorDetalle: result.Success ? null : result.Error,
                urlPdf: urlPdf,
                urlExcel: null
            );

            if (result.Success)
            {
                EnvioFueExitoso = true;
                DestinatariosEnviados = string.Join(", ", destinatarios);
                MensajeEstado = $"✅ Correo enviado correctamente. ID: {result.MessageId}";
                _cerrarDialogo();
            }
            else
            {
                MensajeEstado = $"❌ Error al enviar: {result.Error}";
            }
        }

        private static string BuildHtmlBody(string casaNombre, List<string> archivos, string? mensajePersonalizado)
        {
            var archivosHtml = string.Join("", archivos.Select(a => $"<li>{System.Net.WebUtility.HtmlEncode(a)}</li>"));
            var mensajeHtml = !string.IsNullOrWhiteSpace(mensajePersonalizado)
                ? string.Join("<br/>", mensajePersonalizado.Split('\n').Select(l => System.Net.WebUtility.HtmlEncode(l)))
                : $"Se adjunta el reporte mensual de la propiedad <strong>{System.Net.WebUtility.HtmlEncode(casaNombre)}</strong>.";

            return $@"
<!DOCTYPE html>
<html lang='es'>
<body style='font-family:Segoe UI,Arial,sans-serif;color:#1E293B;max-width:600px;margin:auto;padding:24px'>
  <div style='background:linear-gradient(135deg,#0F172A,#1E3A8A);padding:24px;border-radius:12px 12px 0 0'>
    <h2 style='color:white;margin:0'>📊 Reporte Mensual</h2>
    <p style='color:#E0E7FF;margin:6px 0 0'>{System.Net.WebUtility.HtmlEncode(casaNombre)}</p>
  </div>
  <div style='background:#F8FAFC;padding:24px;border:1px solid #E2E8F0;border-top:none;border-radius:0 0 12px 12px'>
    <p>Estimado/a,</p>
    <p>{mensajeHtml}</p>
    <p><strong>Archivos adjuntos:</strong></p>
    <ul>{archivosHtml}</ul>
    <p style='margin-top:24px;color:#64748B;font-size:13px'>
      Si tiene preguntas, no dude en contactarnos.<br/>
      <em>Samara Rentals</em>
    </p>
  </div>
</body>
</html>";
        }
    }
}
