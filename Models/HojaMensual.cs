namespace FlujoCajaWpf.Models
{
    public class HojaMensual
    {
        public int Id { get; set; }
        public int CasaId { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Cerrada { get; set; }

        // Propiedades calculadas
        public string NombreMes
        {
            get
            {
                return Mes switch
                {
                    1 => "Enero",
                    2 => "Febrero",
                    3 => "Marzo",
                    4 => "Abril",
                    5 => "Mayo",
                    6 => "Junio",
                    7 => "Julio",
                    8 => "Agosto",
                    9 => "Septiembre",
                    10 => "Octubre",
                    11 => "Noviembre",
                    12 => "Diciembre",
                    _ => $"Mes {Mes}"
                };
            }
        }

        public string Periodo => $"{NombreMes} {Anio}";

        public static HojaMensual FromSupabase(HojaMensualSupabase hoja)
        {
            return new HojaMensual
            {
                Id = hoja.Id,
                CasaId = hoja.CasaId,
                Mes = hoja.Mes,
                Anio = hoja.Anio,
                FechaCreacion = hoja.FechaCreacion,
                Cerrada = hoja.Cerrada
            };
        }

        public HojaMensualSupabase ToSupabase()
        {
            return new HojaMensualSupabase
            {
                Id = this.Id,
                CasaId = this.CasaId,
                Mes = this.Mes,
                Anio = this.Anio,
                FechaCreacion = this.FechaCreacion,
                Cerrada = this.Cerrada
            };
        }
    }
}
