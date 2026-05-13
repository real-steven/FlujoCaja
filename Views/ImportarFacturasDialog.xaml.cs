using FlujoCajaWpf.Data;
using FlujoCajaWpf.Models;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace FlujoCajaWpf.Views
{
    /// <summary>
    /// Dialog para importar facturas mediante IA (drag & drop o selector de archivos).
    /// Sube a bucket privado "facturas", llama al webhook de n8n y guarda en movimientos_ia_temporales.
    /// </summary>
    public partial class ImportarFacturasDialog : Window
    {
        private readonly int _casaId;
        private readonly int _mes;
        private readonly int _anio;
        private readonly int? _hojaMensualId;
        private readonly string _usuarioEmail;

        private readonly ObservableCollection<ArchivoProcesado> _archivos = new();
        private static readonly HttpClient _http = new();
        private int _enProceso = 0;

        /// <summary>
        /// Si se asigna, se llama cada vez que un archivo termina de procesarse.
        /// Permite al padre actualizar la tabla de Movimientos IA en tiempo real.
        /// </summary>
        public Func<Task>? ArchivoCompletado { get; set; }

        public ImportarFacturasDialog(int casaId, int mes, int anio, int? hojaMensualId, string usuarioEmail)
        {
            InitializeComponent();
            _casaId       = casaId;
            _mes          = mes;
            _anio         = anio;
            _hojaMensualId = hojaMensualId;
            _usuarioEmail  = usuarioEmail;

            listaArchivos.ItemsSource = _archivos;
        }

        // ─── Drag & drop — Ingresos ───────────────────────────────────────────

        private void BorderIngresos_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                borderDropIngresos.BorderBrush = new SolidColorBrush(Color.FromRgb(16, 185, 129));
                borderDropIngresos.Opacity = 0.75;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void BorderIngresos_DragLeave(object sender, DragEventArgs e)
        {
            borderDropIngresos.BorderBrush = new SolidColorBrush(Color.FromRgb(16, 185, 129));
            borderDropIngresos.Opacity = 1;
        }

        private async void BorderIngresos_Drop(object sender, DragEventArgs e)
        {
            borderDropIngresos.Opacity = 1;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                await ProcesarArchivosAsync(paths, "Ingreso");
            }
        }

        // ─── Drag & drop — Egresos ────────────────────────────────────────────

        private void BorderEgresos_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                borderDropEgresos.BorderBrush = new SolidColorBrush(Color.FromRgb(220, 38, 38));
                borderDropEgresos.Opacity = 0.75;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void BorderEgresos_DragLeave(object sender, DragEventArgs e)
        {
            borderDropEgresos.BorderBrush = new SolidColorBrush(Color.FromRgb(220, 38, 38));
            borderDropEgresos.Opacity = 1;
        }

        private async void BorderEgresos_Drop(object sender, DragEventArgs e)
        {
            borderDropEgresos.Opacity = 1;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                await ProcesarArchivosAsync(paths, "Gasto");
            }
        }

        // ─── Botones seleccionar archivos ────────────────────────────────────

        private async void BuscarIngresos_Click(object sender, RoutedEventArgs e)
        {
            var paths = AbrirSelectorArchivos();
            if (paths.Length > 0)
                await ProcesarArchivosAsync(paths, "Ingreso");
        }

        private async void BuscarEgresos_Click(object sender, RoutedEventArgs e)
        {
            var paths = AbrirSelectorArchivos();
            if (paths.Length > 0)
                await ProcesarArchivosAsync(paths, "Gasto");
        }

        private static string[] AbrirSelectorArchivos()
        {
            var dlg = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Imágenes y PDFs|*.jpg;*.jpeg;*.png;*.pdf;*.webp|Todos los archivos|*.*",
                Title  = "Seleccionar facturas"
            };
            return dlg.ShowDialog() == true ? dlg.FileNames : Array.Empty<string>();
        }

        // ─── Procesamiento principal ──────────────────────────────────────────

        private async Task ProcesarArchivosAsync(string[] paths, string tipo)
        {
            if (paths.Length == 0) return;

            _enProceso += paths.Length;
            ActualizarUI();

            var n8nConfig = await SupabaseHelper.ObtenerConfigN8nAsync();

            foreach (var path in paths)
            {
                var nombreArchivo = System.IO.Path.GetFileName(path);
                var item = new ArchivoProcesado
                {
                    NombreArchivo = nombreArchivo,
                    TipoMovimiento = tipo,
                    Icono   = "⏳",
                    Mensaje = "Subiendo archivo…"
                };
                _archivos.Insert(0, item);
                ActualizarContador();

                try
                {
                    // 1. Leer bytes
                    var bytes = await System.IO.File.ReadAllBytesAsync(path);
                    var ext   = System.IO.Path.GetExtension(path).ToLowerInvariant();
                    var fileName = $"{Guid.NewGuid()}{ext}";

                    // 2. Subir al bucket privado "facturas"
                    item.Mensaje = "Subiendo al almacenamiento…";
                    var (ok, storagePath, uploadErr) = await SupabaseStorageHelper.SubirImagenMovimientoAsync(
                        bytes, fileName, _casaId);

                    if (!ok || string.IsNullOrEmpty(storagePath))
                    {
                        item.MarcarError($"Error al subir: {uploadErr}");
                        continue;
                    }

                    item.StoragePath = storagePath;

                    // 3. Generar URL firmada (24h) para que n8n acceda
                    item.Mensaje = "Generando URL segura…";
                    var signedUrl = await SupabaseStorageHelper.ObtenerUrlFirmadaMovimientoAsync(
                        storagePath, expiresInSeconds: 86400);

                    if (string.IsNullOrEmpty(signedUrl))
                    {
                        item.MarcarError("No se pudo generar URL firmada");
                        continue;
                    }

                    // 4. Insertar registro provisional en movimientos_ia_temporales
                    item.Mensaje = "Guardando en base de datos…";
                    var moviaNew = new MovimientoIASupabase
                    {
                        CasaId          = _casaId,
                        HojaMensualId   = _hojaMensualId,
                        Mes             = _mes,
                        Anio            = _anio,
                        TipoMovimiento  = tipo,
                        FacturaUrl      = storagePath,
                        Estado          = "pendiente",
                        UsuarioCreador  = _usuarioEmail,
                        FechaCreacion   = DateTime.UtcNow
                    };
                    var (insertOk, insertedMov, insertErr) = await SupabaseMovimientoIAHelper.InsertarAsync(moviaNew);

                    if (!insertOk || insertedMov == null)
                    {
                        item.MarcarError($"Error al guardar: {insertErr}");
                        continue;
                    }

                    item.MovimientoIAId = insertedMov.Id;

                    // 5. Preparar base64 para enviar a n8n (evita que n8n descargue la imagen)
                    var imagenBase64   = Convert.ToBase64String(bytes);
                    var extTrimmed     = ext.TrimStart('.');
                    var imagenMimeType = extTrimmed switch
                    {
                        "jpg" or "jpeg" => "image/jpeg",
                        "png"           => "image/png",
                        "pdf"           => "application/pdf",
                        "webp"          => "image/webp",
                        _               => "image/jpeg"
                    };

                    // 6. Llamar a n8n (si está configurado)
                    if (n8nConfig != null
                        && !string.IsNullOrWhiteSpace(n8nConfig.WebhookUrl)
                        && n8nConfig.WebhookUrl != "TU_WEBHOOK_N8N_AQUI")
                    {
                        item.Mensaje = "Analizando con IA…";
                        await EnviarAN8nAsync(n8nConfig, insertedMov.Id, signedUrl, nombreArchivo, tipo, imagenBase64, imagenMimeType, item);
                    }
                    else
                    {
                        item.MarcarExito("Guardado. Configura n8n para análisis automático.");
                    }
                }
                catch (Exception ex)
                {
                    item.MarcarError($"Error inesperado: {ex.Message}");
                }
                finally
                {
                    _enProceso--;
                    ActualizarUI();
                    if (ArchivoCompletado != null)
                        await ArchivoCompletado.Invoke();
                }
            }
        }

        // ─── Llamada al webhook n8n ───────────────────────────────────────────

        private async Task EnviarAN8nAsync(
            N8nConfig config,
            int movIAId,
            string signedUrl,
            string nombreArchivo,
            string tipo,
            string imagenBase64,
            string imagenMimeType,
            ArchivoProcesado item)
        {
            try
            {
                var payload = new
                {
                    movimientoIAId = movIAId,
                    casaId         = _casaId,
                    mes            = _mes,
                    anio           = _anio,
                    tipo,
                    archivoUrl     = signedUrl,
                    nombreArchivo,
                    usuario        = _usuarioEmail,
                    imagenBase64,
                    imagenMimeType
                };

                var json    = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                if (!string.IsNullOrWhiteSpace(config.Token) && config.Token != "TU_TOKEN_SECRETO_AQUI")
                    _http.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", config.Token);

                var response = await _http.PostAsync(config.WebhookUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var respBody = await response.Content.ReadAsStringAsync();
                    await ProcesarRespuestaN8n(movIAId, respBody, item);
                }
                else
                {
                    // Marcar como parcial; el usuario puede reintentar
                    await SupabaseMovimientoIAHelper.ActualizarConDatosIAAsync(
                        movIAId, null, null, null, null, tipo, "parcial", null);
                    item.MarcarParcial($"n8n respondió {(int)response.StatusCode}. Reintenta desde la pestaña IA.");
                }
            }
            catch (Exception ex)
            {
                await SupabaseMovimientoIAHelper.ActualizarConDatosIAAsync(
                    movIAId, null, null, null, null, tipo, "parcial", null);
                item.MarcarParcial($"Error al contactar n8n: {ex.Message}");
            }
        }

        /// <summary>
        /// Parsea la respuesta de n8n y actualiza el registro IA con los datos extraídos.
        /// Soporta objeto directo { fecha, monto, ... } o array n8n [{ fecha, monto, ... }]
        /// También soporta el formato n8n { json: { fecha, monto, ... } }
        /// </summary>
        private async Task ProcesarRespuestaN8n(int movIAId, string respBody, ArchivoProcesado item)
        {
            Console.WriteLine($"[IA] Respuesta n8n raw: {respBody}");
            try
            {
                using var doc = JsonDocument.Parse(respBody);
                var root = doc.RootElement;

                // n8n a veces devuelve array: [{...}] — tomar el primer elemento
                if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                    root = root[0];

                // n8n a veces envuelve en { "json": {...} }
                if (root.TryGetProperty("json", out var jsonEl) && jsonEl.ValueKind == JsonValueKind.Object)
                    root = jsonEl;

                // Groq raw response: { choices: [{ message: { content: "{...}" } }] }
                if (root.TryGetProperty("choices", out var choicesEl)
                    && choicesEl.ValueKind == JsonValueKind.Array
                    && choicesEl.GetArrayLength() > 0)
                {
                    var contentStr = choicesEl[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString() ?? "{}";
                    // Limpiar posibles bloques markdown ```json ... ```
                    contentStr = contentStr.Trim();
                    if (contentStr.StartsWith("```"))
                    {
                        var start = contentStr.IndexOf('{');
                        var end   = contentStr.LastIndexOf('}');
                        if (start >= 0 && end > start)
                            contentStr = contentStr.Substring(start, end - start + 1);
                    }
                    using var innerDoc = JsonDocument.Parse(contentStr);
                    root = innerDoc.RootElement.Clone();
                }

                DateTime? fecha       = null;
                decimal?  monto       = null;
                string?   descripcion = null;
                string?   categoria   = null;
                string?   estado      = "parcial";

                if (root.TryGetProperty("fecha", out var fEl) && fEl.ValueKind != JsonValueKind.Null)
                    fecha = DateTime.TryParse(fEl.GetString(), out var fd) ? fd : null;

                if (root.TryGetProperty("monto", out var mEl) && mEl.ValueKind != JsonValueKind.Null)
                    monto = mEl.TryGetDecimal(out var mv) ? mv : null;

                if (root.TryGetProperty("descripcion", out var dEl))
                    descripcion = dEl.GetString();

                if (root.TryGetProperty("categoria", out var cEl))
                    categoria = cEl.GetString();

                if (root.TryGetProperty("estado", out var eEl))
                    estado = eEl.GetString() ?? "parcial";

                // n8n/Groq devuelve "ok" cuando todo está completo, pero la BD no acepta ese valor.
                // "ok" significa que la IA leyó bien → guardar como "pendiente" (datos listos, esperando aprobación del usuario)
                if (estado == "ok") estado = "pendiente";

                Console.WriteLine($"[IA] Parseado → fecha={fecha}, monto={monto}, descripcion={descripcion}, estado={estado}");

                var (updOk, updErr) = await SupabaseMovimientoIAHelper.ActualizarConDatosIAAsync(
                    movIAId, fecha, monto, descripcion, categoria,
                    item.TipoMovimiento, estado ?? "parcial", respBody);

                if (!updOk)
                {
                    Console.WriteLine($"[IA] ERROR al actualizar en Supabase: {updErr}");
                    item.MarcarParcial($"Error al guardar en BD: {updErr}");
                    return;
                }

                Console.WriteLine($"[IA] Supabase actualizado correctamente (id={movIAId})");

                if (estado == "parcial" || estado == "error")
                    item.MarcarParcial($"Error de lectura. Verifique si '{item.NombreArchivo}' es un archivo de factura válido.");
                else
                    item.MarcarExito($"{descripcion ?? "Factura"} — {(monto.HasValue ? monto.Value.ToString("C0") : "sin monto")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[IA] Error parseando respuesta: {ex.Message}");
                await SupabaseMovimientoIAHelper.ActualizarConDatosIAAsync(
                    movIAId, null, null, null, null, item.TipoMovimiento, "parcial", respBody);
                item.MarcarParcial($"Error de lectura. Verifique si '{item.NombreArchivo}' es un archivo de factura válido.");
            }
        }

        // ─── UI helpers ───────────────────────────────────────────────────────

        private void ActualizarUI()
        {
            bool procesando = _enProceso > 0;
            progressBar.Visibility = procesando ? Visibility.Visible : Visibility.Collapsed;
            txtEstado.Text = procesando ? $"Procesando {_enProceso} archivo(s)…" : "";
            ActualizarContador();
        }

        private void ActualizarContador()
        {
            var total = _archivos.Count;
            txtContadorArchivos.Text = total == 1 ? "1 archivo" : $"{total} archivos";
            txtSinArchivos.Visibility = total == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e) => Close();
    }

    // ─── Modelo auxiliar para la lista de archivos procesados ─────────────────

    public class ArchivoProcesado : INotifyPropertyChanged
    {
        private string _icono   = "⏳";
        private string _mensaje = "";

        public string  NombreArchivo   { get; set; } = "";
        public string  TipoMovimiento  { get; set; } = "";
        public string  StoragePath     { get; set; } = "";
        public int     MovimientoIAId  { get; set; }

        public string Icono
        {
            get => _icono;
            set { _icono = value; OnPropertyChanged(); }
        }

        public string Mensaje
        {
            get => _mensaje;
            set { _mensaje = value; OnPropertyChanged(); }
        }

        // Estados para el DataTemplate
        private bool _esError;
        private bool _esExito;
        private bool _esParcial;

        public bool EsError
        {
            get => _esError;
            set { _esError = value; OnPropertyChanged(); }
        }

        public bool EsExito
        {
            get => _esExito;
            set { _esExito = value; OnPropertyChanged(); }
        }

        public bool EsParcial
        {
            get => _esParcial;
            set { _esParcial = value; OnPropertyChanged(); }
        }

        public string TipoLabel => TipoMovimiento == "Ingreso" ? "INGRESO" : "GASTO";

        public string ColorTipo => TipoMovimiento == "Ingreso" ? "#059669" : "#DC2626";

        public void MarcarExito(string mensaje)
        {
            Icono    = "✅";
            Mensaje  = mensaje;
            EsExito  = true;
            EsError  = false;
            EsParcial = false;
        }

        public void MarcarError(string mensaje)
        {
            Icono    = "❌";
            Mensaje  = mensaje;
            EsError  = true;
            EsExito  = false;
            EsParcial = false;
        }

        public void MarcarParcial(string mensaje)
        {
            Icono     = "⚠️";
            Mensaje   = mensaje;
            EsParcial = true;
            EsError   = false;
            EsExito   = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
