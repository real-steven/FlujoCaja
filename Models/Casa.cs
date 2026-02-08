namespace FlujoCajaWpf.Models
{
    /// <summary>
    /// Modelo de casa para uso en la UI
    /// </summary>
    public class Casa
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
        public int DuenoId { get; set; }
        public string? DuenoNombre { get; set; }
        public int CategoriaId { get; set; }
        public string? CategoriaNombre { get; set; }
        public string? RutaImagen { get; set; }
        public string Moneda { get; set; } = "USD";
        public string? Notas { get; set; }
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Convierte CasaSupabase a Casa
        /// </summary>
        public static Casa FromSupabase(CasaSupabase casaDb)
        {
            return new Casa
            {
                Id = casaDb.Id,
                Nombre = casaDb.Nombre,
                Activo = casaDb.Activo,
                DuenoId = casaDb.DuenoId,
                CategoriaId = casaDb.CategoriaId,
                RutaImagen = casaDb.RutaImagen,
                Moneda = casaDb.Moneda,
                Notas = casaDb.Notas,
                FechaCreacion = casaDb.FechaCreacion
            };
        }

        /// <summary>
        /// Convierte Casa a CasaSupabase
        /// </summary>
        public CasaSupabase ToSupabase()
        {
            return new CasaSupabase
            {
                Id = this.Id,
                Nombre = this.Nombre,
                Activo = this.Activo,
                DuenoId = this.DuenoId,
                CategoriaId = this.CategoriaId,
                RutaImagen = this.RutaImagen,
                Moneda = this.Moneda,
                Notas = this.Notas,
                FechaCreacion = this.FechaCreacion
            };
        }
    }
}
