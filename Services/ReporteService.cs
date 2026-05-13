using FlujoCajaWpf.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using System.IO;

namespace FlujoCajaWpf.Services
{
    public static class ReporteService
    {
        static ReporteService()
        {
            // Licencia Community (gratuita para proyectos no comerciales)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        // ==================== MODELO DE DATOS REPORTE ====================

        public record DatosReporte(
            string CasaNombre,
            string DuenoNombre,
            string Moneda,
            string MesAnio,
            List<Movimiento> Movimientos
        );

        // ==================== PDF TABLA (movimientos) ====================

        public static async Task GenerarPdfAsync(DatosReporte datos, string rutaArchivo)
        {
            var culture = datos.Moneda == "CRC" ? new CultureInfo("es-CR") : new CultureInfo("en-US");

            var ingresos = datos.Movimientos.Where(m => m.Tipo == "Ingreso").Sum(m => m.Monto);
            var gastos = datos.Movimientos.Where(m => m.Tipo != "Ingreso").Sum(m => Math.Abs(m.Monto));
            var balance = datos.Movimientos.Sum(m => m.Monto);

            // Ordenar: ingresos primero, luego gastos, cada grupo por fecha
            var movOrdenados = datos.Movimientos
                .Where(m => m.Tipo == "Ingreso").OrderBy(m => m.Fecha)
                .Concat(datos.Movimientos.Where(m => m.Tipo != "Ingreso").OrderBy(m => m.Fecha))
                .ToList();

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1.5f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    // ── Header ──
                    page.Header().Column(col =>
                    {
                        col.Item().Background("#1E3A8A").Padding(12).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text($"Reporte Mensual — {datos.CasaNombre}")
                                    .FontSize(18).Bold().FontColor("#FFFFFF");
                                c.Item().Text($"Dueño: {datos.DuenoNombre}  |  Período: {datos.MesAnio}")
                                    .FontSize(10).FontColor("#E0E7FF");
                            });
                            row.AutoItem().AlignMiddle().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}")
                                .FontSize(9).FontColor("#E0E7FF");
                        });
                        col.Item().Height(8);
                    });

                    // ── Contenido ──
                    page.Content().Column(col =>
                    {
                        // Tabla de movimientos
                        col.Item().Table(table =>
                        {
                            // Definir columnas
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(70);    // Fecha
                                c.ConstantColumn(65);    // Tipo
                                c.RelativeColumn(1.5f);  // Categoría
                                c.RelativeColumn(2.5f);  // Descripción
                                c.ConstantColumn(120);   // Monto
                            });

                            // Cabecera
                            static IContainer HeaderCell(IContainer container) =>
                                container.Background("#1E3A8A").Padding(6).AlignMiddle();

                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderCell).Text("Fecha").Bold().FontColor("#FFFFFF");
                                header.Cell().Element(HeaderCell).Text("Tipo").Bold().FontColor("#FFFFFF");
                                header.Cell().Element(HeaderCell).Text("Categoría").Bold().FontColor("#FFFFFF");
                                header.Cell().Element(HeaderCell).Text("Descripción").Bold().FontColor("#FFFFFF");
                                header.Cell().Element(HeaderCell).Text("Monto").Bold().FontColor("#FFFFFF").AlignRight();
                            });

                            // Filas — ingresos primero, luego gastos con separador visual
                            bool separadorGastos = false;
                            int rowIdx = 0;
                            for (int i = 0; i < movOrdenados.Count; i++)
                            {
                                var m = movOrdenados[i];

                                // Separador antes del primer gasto
                                if (m.Tipo != "Ingreso" && !separadorGastos)
                                {
                                    separadorGastos = true;
                                    table.Cell().ColumnSpan(5)
                                        .Background("#FEF3C7").BorderBottom(1).BorderColor("#F59E0B")
                                        .PaddingVertical(5).PaddingLeft(12)
                                        .Text("Gastos").Bold().FontSize(9).FontColor("#92400E");
                                    rowIdx = 0;
                                }

                                var bgHex = rowIdx % 2 == 0 ? "#F8FAFF" : "#FFFFFF";
                                var tipoColor = m.Tipo == "Ingreso" ? "#059669" : "#DC2626";
                                var montoColor = m.Monto >= 0 ? "#059669" : "#DC2626";

                                IContainer DataCell(IContainer c) =>
                                    c.Background(bgHex).BorderBottom(0.5f).BorderColor("#E5E7EB").Padding(5);

                                bool tieneImagen = !string.IsNullOrEmpty(m.ImagenUrl);

                                // Celdas de datos
                                table.Cell().Element(DataCell).AlignTop().Text(m.Fecha.ToString("dd/MM/yyyy")).FontSize(9);
                                table.Cell().Element(DataCell).AlignTop().Text(m.Tipo == "Ingreso" ? "Ingreso" : "Gasto")
                                    .FontSize(9).FontColor(tipoColor).Bold();
                                table.Cell().Element(DataCell).AlignTop().Text(m.CategoriaNombre).FontSize(9);
                                table.Cell().Element(DataCell).AlignTop().Text(m.Descripcion ?? "—").FontSize(9);
                                table.Cell().Element(DataCell).AlignRight().AlignTop().PaddingRight(8)
                                    .Text(t => t.Span(m.Monto.ToString("C", culture)).FontSize(9).FontColor(montoColor).Bold());

                                // Fila extra indicando comprobante
                                if (tieneImagen)
                                {
                                    table.Cell().ColumnSpan(5).Background(bgHex)
                                        .BorderBottom(1).BorderColor("#CBD5E1")
                                        .PaddingLeft(20).PaddingBottom(6).PaddingTop(2)
                                        .Text("📎 Comprobante adjunto (ver PDF de imágenes facturas)")
                                        .FontSize(8).FontColor("#6B7280").Italic();
                                }
                                else
                                {
                                    table.Cell().ColumnSpan(5).Background(bgHex)
                                        .BorderBottom(1).BorderColor("#CBD5E1")
                                        .PaddingLeft(20).PaddingBottom(6).PaddingTop(2)
                                        .Text("🖼 Imagen no adjuntada")
                                        .FontSize(8).FontColor("#9CA3AF").Italic();
                                }

                                rowIdx++;
                            }
                        });

                        // ── Resumen ── (ShowEntire evita que se parta entre páginas)
                        col.Item().PaddingTop(16).ShowEntire().Row(row =>
                        {
                            row.RelativeItem();

                            row.AutoItem().Table(t =>
                            {
                                t.ColumnsDefinition(c =>
                                {
                                    c.ConstantColumn(160); // etiqueta
                                    c.ConstantColumn(90);  // valor
                                });

                                void ResumenFila(string etiqueta, decimal valor, string colorHex)
                                {
                                    t.Cell().Background("#F0F4FF").PaddingVertical(7).PaddingLeft(12)
                                        .Text(etiqueta).Bold().FontSize(10);
                                    t.Cell().Background("#F0F4FF").PaddingVertical(7).PaddingLeft(10).PaddingRight(16)
                                        .Text(t2 => t2.Span(valor.ToString("C", culture)).Bold().FontColor(colorHex).FontSize(10));
                                }

                                ResumenFila("Total Ingresos", ingresos, "#059669");
                                ResumenFila("Total Gastos", gastos, "#DC2626");
                                t.Cell().ColumnSpan(2).Height(1).Background("#CBD5E1");
                                ResumenFila("Balance Neto", balance, balance >= 0 ? "#059669" : "#DC2626");
                            });
                        });
                    });

                    // ── Footer ──
                    page.Footer().AlignCenter()
                        .Text(x =>
                        {
                            x.Span("FlujoCaja — ").FontSize(8).FontColor("#9CA3AF");
                            x.Span(datos.CasaNombre).FontSize(8).FontColor("#9CA3AF");
                            x.Span("  |  Página ").FontSize(8).FontColor("#9CA3AF");
                            x.CurrentPageNumber().FontSize(8).FontColor("#9CA3AF");
                            x.Span(" de ").FontSize(8).FontColor("#9CA3AF");
                            x.TotalPages().FontSize(8).FontColor("#9CA3AF");
                        });
                });
            }).GeneratePdf(rutaArchivo);

            await Task.CompletedTask;
        }

        // ==================== PDF FACTURAS (tabla + imágenes privadas) ====================

        /// <summary>
        /// Genera un PDF con la tabla de movimientos (numerados, ordenados por fecha) y,
        /// debajo de cada movimiento, su imagen de comprobante (si la tiene).
        /// obtenerImagen: función que recibe un Movimiento y devuelve los bytes de la imagen (o null).
        /// </summary>
        public static async Task GenerarPdfFacturasAsync(
            DatosReporte datos,
            string rutaArchivo,
            Func<Movimiento, Task<byte[]?>> obtenerImagen)
        {
            var culture = datos.Moneda == "CRC" ? new CultureInfo("es-CR") : new CultureInfo("en-US");

            // Ordenar: ingresos primero, luego gastos, cada grupo por fecha
            var movimientos = datos.Movimientos
                .Where(m => m.Tipo == "Ingreso").OrderBy(m => m.Fecha)
                .Concat(datos.Movimientos.Where(m => m.Tipo != "Ingreso").OrderBy(m => m.Fecha))
                .ToList();

            // Pre-cargar todas las imágenes antes de generar el PDF
            var imagenes = new Dictionary<int, byte[]>();
            for (int i = 0; i < movimientos.Count; i++)
            {
                var m = movimientos[i];
                if (!string.IsNullOrEmpty(m.ImagenUrl))
                {
                    var bytes = await obtenerImagen(m);
                    if (bytes != null)
                        imagenes[i] = bytes;
                }
            }

            var ingresos = movimientos.Where(m => m.Tipo == "Ingreso").Sum(m => m.Monto);
            var gastos = movimientos.Where(m => m.Tipo != "Ingreso").Sum(m => Math.Abs(m.Monto));
            var balance = movimientos.Sum(m => m.Monto);

            Document.Create(container =>
            {
                // ── Página de resumen con tabla de movimientos ──
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                    page.Header().Column(col =>
                    {
                        col.Item().Background("#1E3A8A").Padding(12).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text($"Reporte de Facturas — {datos.CasaNombre}")
                                    .FontSize(16).Bold().FontColor("#FFFFFF");
                                c.Item().Text($"Dueño: {datos.DuenoNombre}  |  Período: {datos.MesAnio}")
                                    .FontSize(9).FontColor("#E0E7FF");
                            });
                            row.AutoItem().AlignMiddle()
                                .Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}")
                                .FontSize(8).FontColor("#E0E7FF");
                        });
                        col.Item().Height(6);
                    });

                    page.Content().Column(col =>
                    {
                        col.Item().PaddingBottom(10).Text("Índice de movimientos")
                            .FontSize(11).Bold().FontColor("#1E3A8A");

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(0.4f);  // #
                                c.RelativeColumn(1.5f);  // Fecha
                                c.RelativeColumn(1.1f);  // Tipo
                                c.RelativeColumn(3.2f);  // Descripción
                                c.RelativeColumn(1.8f);  // Monto
                            });

                            IContainer Hdr(IContainer c) =>
                                c.Background("#1E3A8A").Padding(6);

                            table.Header(header =>
                            {
                                header.Cell().Element(Hdr).Text("#").Bold().FontColor("#FFFFFF").AlignCenter();
                                header.Cell().Element(Hdr).Text("Fecha").Bold().FontColor("#FFFFFF").AlignCenter();
                                header.Cell().Element(Hdr).Text("Tipo").Bold().FontColor("#FFFFFF").AlignCenter();
                                header.Cell().Element(Hdr).PaddingLeft(10).Text("Descripción").Bold().FontColor("#FFFFFF");
                                header.Cell().Element(Hdr).Text("Monto").Bold().FontColor("#FFFFFF").AlignRight();
                            });

                            bool separadorGastos = false;
                            int rowIdx = 0;
                            for (int i = 0; i < movimientos.Count; i++)
                            {
                                var m = movimientos[i];

                                // Separador antes del primer gasto
                                if (m.Tipo != "Ingreso" && !separadorGastos)
                                {
                                    separadorGastos = true;
                                    table.Cell().ColumnSpan(5)
                                        .Background("#FEF3C7").BorderBottom(1).BorderColor("#F59E0B")
                                        .PaddingVertical(5).PaddingLeft(12)
                                        .Text("Gastos").Bold().FontSize(9).FontColor("#92400E");
                                    rowIdx = 0;
                                }

                                var bgHex = rowIdx % 2 == 0 ? "#F8FAFF" : "#FFFFFF";
                                var tipoColor = m.Tipo == "Ingreso" ? "#059669" : "#DC2626";
                                var montoColor = m.Monto >= 0 ? "#059669" : "#DC2626";

                                IContainer DataCell(IContainer c) =>
                                    c.Background(bgHex).BorderBottom(0.5f).BorderColor("#E5E7EB").Padding(5);

                                table.Cell().Element(DataCell).Text($"{i + 1}").FontSize(9).AlignCenter();
                                table.Cell().Element(DataCell).Text(m.Fecha.ToString("dd/MM/yyyy")).FontSize(9).AlignCenter();
                                table.Cell().Element(DataCell).Text(m.Tipo == "Ingreso" ? "Ingreso" : "Gasto").FontSize(9).FontColor(tipoColor).Bold().AlignCenter();
                                table.Cell().Element(DataCell).PaddingLeft(10).Text(m.Descripcion ?? "—").FontSize(9);
                                table.Cell().Element(DataCell).PaddingRight(4)
                                    .Text(m.Monto.ToString("C", culture)).FontSize(9).FontColor(montoColor).Bold().AlignRight();

                                rowIdx++;
                            }
                        });

                        // Resumen
                        col.Item().PaddingTop(16).Row(row =>
                        {
                            row.RelativeItem();
                            row.AutoItem().Table(t =>
                            {
                                t.ColumnsDefinition(c =>
                                {
                                    c.ConstantColumn(160);
                                    c.ConstantColumn(90);
                                });

                                void ResumenFila(string etiqueta, decimal valor, string colorHex)
                                {
                                    t.Cell().Background("#F0F4FF").PaddingVertical(7).PaddingLeft(12)
                                        .Text(etiqueta).Bold().FontSize(10);
                                    t.Cell().Background("#F0F4FF").PaddingVertical(7).PaddingLeft(10).PaddingRight(16)
                                        .Text(t2 => t2.Span(valor.ToString("C", culture)).Bold().FontColor(colorHex).FontSize(10));
                                }

                                ResumenFila("Total Ingresos", ingresos, "#059669");
                                ResumenFila("Total Gastos", gastos, "#DC2626");
                                t.Cell().ColumnSpan(2).Height(1).Background("#CBD5E1");
                                ResumenFila("Balance Neto", balance, balance >= 0 ? "#059669" : "#DC2626");
                            });
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("FlujoCaja — ").FontSize(8).FontColor("#9CA3AF");
                        x.Span(datos.CasaNombre).FontSize(8).FontColor("#9CA3AF");
                        x.Span("  |  Página ").FontSize(8).FontColor("#9CA3AF");
                        x.CurrentPageNumber().FontSize(8).FontColor("#9CA3AF");
                        x.Span(" de ").FontSize(8).FontColor("#9CA3AF");
                        x.TotalPages().FontSize(8).FontColor("#9CA3AF");
                    });
                });

                // ── Páginas de imágenes: 2 por hoja ──
                var movConImagen = movimientos
                    .Select((m, i) => (m, i))
                    .Where(x => imagenes.ContainsKey(x.i))
                    .ToList();

                for (int p = 0; p < movConImagen.Count; p += 2)
                {
                    var pares = movConImagen.Skip(p).Take(2).ToList();

                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(1.5f, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                        page.Header().Background("#1E3A8A").Padding(10).Row(row =>
                        {
                            row.RelativeItem().Text($"Comprobantes — {datos.CasaNombre}  |  {datos.MesAnio}")
                                .FontSize(12).Bold().FontColor("#FFFFFF");
                            row.AutoItem().AlignMiddle()
                                .Text($"Página de imágenes {p / 2 + 1}")
                                .FontSize(8).FontColor("#E0E7FF");
                        });

                        page.Content().Column(col =>
                        {
                            foreach (var (m, idx) in pares)
                            {
                                var tipoColor = m.Tipo == "Ingreso" ? "#059669" : "#DC2626";
                                col.Item().PaddingTop(12).Column(bloque =>
                                {
                                    // Encabezado del movimiento
                                    bloque.Item().Background("#F0F4FF").Padding(8).Row(row =>
                                    {
                                        row.AutoItem().Background("#1E3A8A").Padding(5).AlignCenter()
                                            .Text($"#{idx + 1}").Bold().FontColor("#FFFFFF").FontSize(10);
                                        row.RelativeItem().PaddingLeft(10).Column(c =>
                                        {
                                            c.Item().Text($"{m.Fecha:dd/MM/yyyy}  —  {(m.Tipo == "Ingreso" ? "Ingreso" : "Gasto")}")
                                                .FontSize(10).Bold().FontColor(tipoColor);
                                            c.Item().Text($"{m.CategoriaNombre}  |  {m.Descripcion ?? "—"}")
                                                .FontSize(9).FontColor("#374151");
                                        });
                                        row.AutoItem().AlignMiddle()
                                            .Text($"Monto: {m.Monto.ToString("C", culture)}")
                                            .Bold().FontColor(tipoColor).FontSize(11);
                                    });

                                    // Imagen centrada con altura máxima para que quepan 2
                                    bloque.Item().PaddingTop(6).AlignCenter()
                                        .MaxHeight(300)
                                        .Image(imagenes[idx], ImageScaling.FitArea);
                                });
                            }
                        });

                        page.Footer().AlignCenter().Text(x =>
                        {
                            x.Span("FlujoCaja Facturas — ").FontSize(8).FontColor("#9CA3AF");
                            x.Span(datos.CasaNombre).FontSize(8).FontColor("#9CA3AF");
                            x.Span("  |  Página ").FontSize(8).FontColor("#9CA3AF");
                            x.CurrentPageNumber().FontSize(8).FontColor("#9CA3AF");
                            x.Span(" de ").FontSize(8).FontColor("#9CA3AF");
                            x.TotalPages().FontSize(8).FontColor("#9CA3AF");
                        });
                    });
                }
            }).GeneratePdf(rutaArchivo);

            await Task.CompletedTask;
        }
    }
}
